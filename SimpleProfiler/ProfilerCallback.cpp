// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/****************************************************************************************
 * File:
 *  ProfilerCallBack.cpp
 *
 * Description:
 *  Implements ICorProfilerCallback. Logs every event of interest to a file on disc.
 *
 ***************************************************************************************/ 

#undef _WIN32_WINNT
#define _WIN32_WINNT    0x0403

#include <windows.h>
#include <share.h>

#include "basehlp.h"
#include "basehlp.hpp"

#include "ProfilerInfo.h"
#include "ProfilerCallback.h"

ProfilerCallback *g_pCallbackObject;        // global reference to callback object
/***************************************************************************************
 ********************                                               ********************
 ********************   Global Functions Used for Thread Support    ********************
 ********************                                               ********************
 ***************************************************************************************/

/* static __stdcall */
DWORD __stdcall ThreadStub( void *pObject )
{    
    ((ProfilerCallback *)pObject)->_ThreadStubWrapper();   

    return 0;
                       
} // _GCThreadStub



/***************************************************************************************
 ********************                                               ********************
 ********************   Global Functions Used by Function Hooks     ********************
 ********************                                               ********************
 ***************************************************************************************/

//
// The functions EnterStub, LeaveStub and TailcallStub are wrappers. The use of 
// of the extended attribute "__declspec( naked )" does not allow a direct call
// to a profiler callback (e.g., ProfilerCallback::Enter( functionID )).
//
// The enter/leave function hooks must necessarily use the extended attribute
// "__declspec( naked )". Please read the corprof.idl for more details. 
//

EXTERN_C void __stdcall EnterStub(FunctionID functionID, 
                                    UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO frameInfo, 
                                     COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo)
{
    ProfilerCallback::Enter( functionID, clientData, frameInfo, argumentInfo );
    
} // EnterStub


EXTERN_C void __stdcall LeaveStub( FunctionID functionID, COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange )
{
    ProfilerCallback::Leave( functionID, retvalRange );
    
} // LeaveStub


EXTERN_C void __stdcall TailcallStub( FunctionID functionID )
{
    ProfilerCallback::Tailcall( functionID );
    
} // TailcallStub


#ifdef _X86_

void __stdcall EnterNaked2(FunctionID funcId,
                                     UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO frameInfo, 
                                     COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo)
{

  __asm
  {
    PUSH EBP
    MOV EBP, ESP

    PUSH EAX
    PUSH ECX
    PUSH EDX

  }

  EnterStub(funcId, clientData, frameInfo, argumentInfo);

  __asm
  {
    POP  EDX
    POP  ECX
    POP  EAX

    MOV ESP, EBP
    POP EBP

    RET 10H
  }
  
} // EnterNaked

void __stdcall LeaveNaked2(FunctionID funcId,
                                     UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO frameInfo, 
                                     COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange)
{
 __asm
  {
    PUSH EBP
    MOV EBP, ESP

    PUSH EAX
    PUSH ECX
    PUSH EDX
  }

 LeaveStub(funcId, retvalRange);

  __asm
  {
    POP  EDX
    POP  ECX
    POP  EAX

    MOV ESP, EBP
    POP EBP

    RET 10H
  }
} // LeaveNaked


void __stdcall TailcallNaked2(FunctionID funcId,
                                        UINT_PTR clientData, 
                                        COR_PRF_FRAME_INFO frameInfo)
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call TailcallStub
        pop edx
        pop ecx
        pop eax
        ret 12
    }
} // TailcallNaked

#elif defined(_AMD64_)
// these are linked in AMD64 assembly (amd64\asmhelpers.asm)
EXTERN_C void EnterNaked2(FunctionID funcId, 
                          UINT_PTR clientData, 
                          COR_PRF_FRAME_INFO frameInfo, 
                          COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo);
EXTERN_C void LeaveNaked2(FunctionID funcId, 
                          UINT_PTR clientData, 
                          COR_PRF_FRAME_INFO frameInfo, 
                          COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange);
EXTERN_C void TailcallNaked2(FunctionID funcId, 
                             UINT_PTR clientData, 
                             COR_PRF_FRAME_INFO frameInfo);
#endif // _X86_    

/***************************************************************************************
 ********************                                               ********************
 ********************     ProfilerCallBack Implementation           ********************
 ********************                                               ********************
 ***************************************************************************************/


/* public */

ProfilerCallback::ProfilerCallback() :
    PrfInfo(),
    m_path( NULL ),
	m_status_path( NULL),
    m_refCount( 0 ),
    m_stream( NULL ),
    m_dwShutdown( 0 ),
    m_bShutdown( FALSE ),
    m_dwProcessId( NULL ),
    m_filter( NULL ),
	m_filterLen( 0 ),
    m_pGCHost( NULL ),
	m_includeSystem( FALSE ),
	m_traceEvent( FALSE ),
	m_traceParameter( FALSE )
{
    HRESULT hr = S_OK;

    //TEXT_OUTLN( "Turning off profiling for child processes" )    
    SetEnvironmentVariableW(L"Cor_Enable_Profiling", L"0x0");

    //
    // initializations
    //
    if (!InitializeCriticalSectionAndSpinCount( &m_criticalSection, 10000 ))
        hr = E_FAIL;
    if (!InitializeCriticalSectionAndSpinCount( &g_criticalSection, 10000 ))
        hr = E_FAIL;

	g_pCallbackObject = this;
    m_dwProcessId = GetCurrentProcessId();
	
	//
	// define in which mode you will operate
	//
	strcpy_s( m_logFileName, ARRAY_LEN(m_logFileName), FILENAME );
	_ProcessEnvVariables();

	if (SUCCEEDED(hr) )
    {
		//
		// open the correct file stream fo dump the logging information
		//
		m_stream = _fsopen((m_path != NULL) ? m_path : m_logFileName, "a+", _SH_DENYWR);
		hr = ( m_stream == NULL ) ? E_FAIL : S_OK;
		if ( SUCCEEDED( hr ))
		{
			setvbuf(m_stream, NULL, _IOFBF, 32768);
			LogToAnyInternal( "SimpleProfiler\n");
		}
		else {
			TEXT_OUTLN( "Unable to open log file - No log will be produced" );
		}
    }

	if(FAILED(hr))
	{
        m_dwEventMask = COR_PRF_MONITOR_NONE;
	}
        
} // ctor


/* public */
ProfilerCallback::~ProfilerCallback()
{
	LogToAnyInternal( "~SimpleProfiler\n");

    if ( m_path != NULL )
    {
        delete[] m_path;
        m_path = NULL;
    }
    
    if ( m_filter != NULL )
    {
        delete[] m_filter;
        m_filter = NULL;    
    }

    if ( m_stream != NULL )
    {
		fflush( m_stream );
        fclose( m_stream );
        m_stream = NULL;
    }

	m_pFunctionTable.clear();
	m_pClassTable.clear();

    DeleteCriticalSection( &m_criticalSection );
    DeleteCriticalSection( &g_criticalSection );
    
	g_pCallbackObject = NULL;

	m_pFunctionTable.clear();
	m_pClassTable.clear();
	m_pClassNameTable.clear();
	m_pModuleTable.clear();

} // dtor

        
/* public */
ULONG ProfilerCallback::AddRef() 
{
    return InterlockedIncrement( &m_refCount );

} // ProfilerCallback::AddRef


/* public */
ULONG ProfilerCallback::Release() 
{
    long refCount;
    refCount = InterlockedDecrement( &m_refCount );
    if ( refCount == 0 )
        delete this;
    return refCount;

} // ProfilerCallback::Release


/* public */
HRESULT ProfilerCallback::QueryInterface( REFIID riid, void **ppInterface )
{
    if ( riid == IID_IUnknown )
        *ppInterface = static_cast<IUnknown *>( this ); 

    else if ( riid == IID_ICorProfilerCallback )
        *ppInterface = static_cast<ICorProfilerCallback *>( this );

    else if ( riid == IID_ICorProfilerCallback2 )
        *ppInterface = static_cast<ICorProfilerCallback2 *>( this );

	else
    {
        *ppInterface = NULL;

        return E_NOINTERFACE;
    }
    
    reinterpret_cast<IUnknown *>( *ppInterface )->AddRef();

    return S_OK;

} // ProfilerCallback::QueryInterface 


/* public static */
HRESULT ProfilerCallback::CreateObject( REFIID riid, void **ppInterface )
{
    HRESULT hr = E_NOINTERFACE;
    
    *ppInterface = NULL;
    if (   (riid == IID_IUnknown)
        || (riid == IID_ICorProfilerCallback2) 
        || (riid == IID_ICorProfilerCallback) )
    {           
        ProfilerCallback *pProfilerCallback;
        
        pProfilerCallback = new ProfilerCallback();
        if ( pProfilerCallback != NULL )
        {
            hr = S_OK;
            
            pProfilerCallback->AddRef();
            *ppInterface = static_cast<ICorProfilerCallback *>( pProfilerCallback );
        }
        else
            hr = E_OUTOFMEMORY;
    }    

    return hr;

} // ProfilerCallback::CreateObject


IGCHost *GetGCHost()
{
    ICorRuntimeHost *pCorHost = NULL;

    CoInitializeEx(NULL, COINIT_MULTITHREADED);

    HRESULT hr = CoCreateInstance( CLSID_CorRuntimeHost, 
                                   NULL, 
                                   CLSCTX_INPROC_SERVER, 
                                   IID_ICorRuntimeHost,
                                   (void**)&pCorHost );

    if (SUCCEEDED(hr))
    {
        IGCHost *pGCHost = NULL;

        hr = pCorHost->QueryInterface(IID_IGCHost, (void**)&pGCHost);

        if (SUCCEEDED(hr))
            return pGCHost;
        else
            printf("Could not QueryInterface hr = %x\n", hr);
    }
    else
        printf("Could not CoCreateInstanceEx hr = %x\n", hr);

    return NULL;
}

/* public */

HRESULT ProfilerCallback::Initialize( IUnknown *pICorProfilerInfoUnk )
{     
    HRESULT hr;

    hr = pICorProfilerInfoUnk->QueryInterface( IID_ICorProfilerInfo,
                                               (void **)&m_pProfilerInfo );   
    if ( SUCCEEDED( hr ) )
    {
        hr = pICorProfilerInfoUnk->QueryInterface( IID_ICorProfilerInfo2,
                                                   (void **)&m_pProfilerInfo2 );
	}else{
		Failure( "QueryInterface for ICorProfilerInfo FAILED" );
	}

    if ( SUCCEEDED( hr ) )
    {
        //printf("event mask = %x\n", m_dwEventMask);
		hr = m_pProfilerInfo2->SetEventMask( m_dwEventMask );

        if ( SUCCEEDED( hr ) )
        {
#if defined(_X86_)
            hr = m_pProfilerInfo2->SetEnterLeaveFunctionHooks2( EnterNaked2,
                                                                LeaveNaked2,
                                                                TailcallNaked2 );
#elif defined(_AMD64_)
            hr = m_pProfilerInfo2->SetEnterLeaveFunctionHooks2( (FunctionEnter2 *)EnterNaked2,
                                                                (FunctionLeave2 *)LeaveNaked2,
                                                                (FunctionTailcall2 *)TailcallNaked2 );
#else
            hr = m_pProfilerInfo2->SetEnterLeaveFunctionHooks2( (FunctionEnter2 *)&EnterStub,
                                                                (FunctionLeave2 *)&LeaveStub,
                                                                (FunctionTailcall2 *)&TailcallStub );
#endif // defined(_X86_) || defined(_AMD64_)

            if ( SUCCEEDED( hr ) )
            {
                Sleep(100); // Give the threads a chance to read any signals that are already set.
            }
            else
                Failure( "ICorProfilerInfo::SetEnterLeaveFunctionHooks() FAILED" );
        }
        else
            Failure( "SetEventMask for Profiler FAILED" );           
    }       
    else
	{
		Failure( "QueryInterface for ICorProfilerInfo2 FAILED" );
        Failure( "Allocation for Profiler FAILED" );           
	}

	if(m_traceEvent) {
		LogToAny("Initialize\n");
	}
    return S_OK;

} // ProfilerCallback::Initialize


/* public */
HRESULT ProfilerCallback::Shutdown()
{
    m_dwShutdown++;

	if(m_traceEvent) {
		LogToAny("Shutdown\n");
	}
    return S_OK;          

} // ProfilerCallback::Shutdown


/* public */
HRESULT ProfilerCallback::DllDetachShutdown()
{
    //
    // If no shutdown occurs during DLL_DETACH, release the callback
    // interface pointer. This scenario will more than likely occur
    // with any interop related program (e.g., a program that is 
    // comprised of both managed and unmanaged components).
    //
    m_dwShutdown++;
    if ( (m_dwShutdown == 1) && (g_pCallbackObject != NULL) )
    {
        g_pCallbackObject->Release();   
        g_pCallbackObject = NULL;
    }
    
    return S_OK;          

} // ProfilerCallback::DllDetachShutdown

/* public */

__forceinline void ProfilerCallback::Enter( FunctionID functionID, 
                                    UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO frameInfo, 
                                     COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo )
{
    FunctionInfo *pFunctionInfo = NULL;	
    pFunctionInfo = g_pCallbackObject->m_pFunctionTable[functionID];
	
	if(pFunctionInfo != NULL && g_pCallbackObject->_IsNeedToLog(pFunctionInfo->m_name) && 
		(g_pCallbackObject->m_traceEvent || g_pCallbackObject->m_traceParameter)
		)
	{
		g_pCallbackObject->LogToAny("> %S", 
			pFunctionInfo->m_name);		

		if(g_pCallbackObject->m_traceParameter)
		{
			g_pCallbackObject->LogToAny("(");

			HRESULT hr;
			mdMethodDef methodDef;
			IMetaDataImport *mdi = NULL;
			
			hr = g_pCallbackObject->m_pProfilerInfo->GetTokenAndMetaDataFromFunction(
				  functionID, 
				  IID_IMetaDataImport,
				  (IUnknown**) &mdi, 
				  &methodDef);

			// output parameter list
			if(SUCCEEDED(hr) && mdi)
			{
				hr = g_pCallbackObject->TraceParameterList( pFunctionInfo, 
					argumentInfo, g_pCallbackObject->m_pProfilerInfo2, mdi);
			}
			
			g_pCallbackObject->LogToAny(") %S", pFunctionInfo->m_pReturnInfo->m_typeName);

			if(mdi) mdi->Release();
		}

		g_pCallbackObject->LogToAny("\n");
	}	

} // ProfilerCallback::Enter

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceParameterList(
	FunctionInfo* pFunctionInfo, 
  COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo,
  ICorProfilerInfo2 *cpi,
  IMetaDataImport *mdi)
{
  HRESULT hr = S_OK;
  if(pFunctionInfo->m_argCount == 0) return hr;  

  // output parameters
  int offset = argumentInfo->numRanges - pFunctionInfo->m_argCount;
  for(ULONG i = 0; 
      i < pFunctionInfo->m_argCount;
      i++)
  {
    hr = TraceParameter(		
		&(argumentInfo->ranges[i + offset]), pFunctionInfo->m_ppParamInfo[i], cpi, mdi);

	if(i < pFunctionInfo->m_argCount - 1) LogToAny(", ");

  }

  return hr;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceParameter(
  COR_PRF_FUNCTION_ARGUMENT_RANGE *range,
  ParameterInfo *pParameterInfo,
  ICorProfilerInfo2 *cpi,
  IMetaDataImport *mdi)
{
	HRESULT hr = S_OK;
  // get argument direction
  PWCHAR direction;	
  if(!pParameterInfo->m_isByRef)
      direction = L"";
  else if(IsPdOut(pParameterInfo->m_attributes) != 0)
      direction = L"out ";
  else
	  direction = L"ref ";	

  // output
  LogToAny("%S%S %S", direction, pParameterInfo->m_typeName, pParameterInfo->m_name);

	// no out parameters on enter  
  if(IsPdOut(pParameterInfo->m_attributes) == 0) 
	{
	  LogToAny("=");
	  
	  hr = TraceValue(
		pParameterInfo->m_isByRef ? *(PUINT_PTR)(range->startAddress) : range->startAddress, 
		cpi, mdi, 
		pParameterInfo);	  
	}
	
  return hr;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceValue(
  UINT_PTR startAddress,
  ICorProfilerInfo2 *cpi,
  IMetaDataImport *mdi, 
  ParameterInfo *pParameterInfo)
{
	/*if(pParameterInfo->m_def == mdTokenNil || pParameterInfo->m_def == 0xffffffff) 
	{
		LogToAny("?");
		return S_OK;
	}*/

	if(pParameterInfo->m_isArray) {
		return TraceArray(*(PUINT_PTR)(startAddress), cpi, mdi, pParameterInfo);
	}

  switch(pParameterInfo->m_type)
  {
  case ELEMENT_TYPE_STRING:
    return TraceString(*(PUINT_PTR)startAddress, cpi);
  case ELEMENT_TYPE_BOOLEAN:
	  return TraceBoolean(startAddress);
  case ELEMENT_TYPE_CHAR:
	return TraceChar(startAddress);
  case ELEMENT_TYPE_I1:
	return TraceSByte(startAddress);
  case ELEMENT_TYPE_U1:
	return TraceByte(startAddress);
  case ELEMENT_TYPE_I2:
	return TraceShort(startAddress);
  case ELEMENT_TYPE_U2:
	return TraceUShort(startAddress);
  case ELEMENT_TYPE_I4:
	return TraceInt(startAddress);
  case ELEMENT_TYPE_U4:
	return TraceUInt(startAddress);
  case ELEMENT_TYPE_I8:
	return TraceLong(startAddress);
  case ELEMENT_TYPE_U8:
	return TraceULong(startAddress);
  case ELEMENT_TYPE_R4:
    return TraceFloat(startAddress);
  case ELEMENT_TYPE_R8:
    return TraceDouble(startAddress);
  case ELEMENT_TYPE_VALUETYPE:
    return TraceStruct(startAddress, cpi, mdi, pParameterInfo, NULL, NULL, mdTokenNil);
  case ELEMENT_TYPE_CLASS:
	return TraceClass(*(PUINT_PTR)(startAddress), cpi, mdi, pParameterInfo);
  case ELEMENT_TYPE_OBJECT:
	return TraceObject(startAddress, cpi, mdi, pParameterInfo);
	/*
        case ELEMENT_TYPE_PTR:
        case ELEMENT_TYPE_ARRAY:
        case ELEMENT_TYPE_I:
        case ELEMENT_TYPE_U:
        case ELEMENT_TYPE_OBJECT:
        case ELEMENT_TYPE_SZARRAY:
		case ELEMENT_TYPE_FNPTR:
        case ELEMENT_TYPE_MAX:
        case ELEMENT_TYPE_END:
        case ELEMENT_TYPE_BYREF:
        case ELEMENT_TYPE_PINNED:
        case ELEMENT_TYPE_SENTINEL:
        case ELEMENT_TYPE_CMOD_OPT:
        case ELEMENT_TYPE_MODIFIER:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_TYPEDBYREF:
*/
  case ELEMENT_TYPE_VOID:
	  break;
  default:
    LogToAny("?");
    break;
  }

  return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceObject(
	UINT_PTR startAddress,
  ICorProfilerInfo2 *cpi,
  IMetaDataImport *mdiRef,
  ParameterInfo *pParameterInfo)
{
	//__asm { int 3 }
	bool traced = false;
	ObjectID oid = *(ObjectID *)startAddress;
	ClassID classId = NULL;
	HRESULT hr = cpi->GetClassFromObject(oid, &classId);
	if(SUCCEEDED(hr)) {
		ClassInfo *pClassInfo = m_pClassTable[classId];
		if(pClassInfo != NULL) {
			CorElementType type;
			type = _GetElementTypeFromClassName(pClassInfo->m_name);
			if(type <= ELEMENT_TYPE_R8 || type == ELEMENT_TYPE_I || type == ELEMENT_TYPE_U || type == ELEMENT_TYPE_END) {
			  ULONG32 bufferOffset = 0;
			  hr = cpi->GetBoxClassLayout(classId, &bufferOffset);
			  if(SUCCEEDED(hr)) {
				  oid = oid + bufferOffset;
				  if(type == ELEMENT_TYPE_END) {
					  type = ELEMENT_TYPE_VALUETYPE;
				  }
			  }else{
				  if(type == ELEMENT_TYPE_END) {
					  type = ELEMENT_TYPE_CLASS;
					  hr = S_OK;
				  }
			  }
			}
			if(SUCCEEDED(hr)) {
			 switch(type)
			  {
					case ELEMENT_TYPE_STRING:
						hr = TraceString(oid, cpi);traced = true;break;
					case ELEMENT_TYPE_BOOLEAN:
						hr = TraceBoolean(oid);traced = true;break;
					case ELEMENT_TYPE_CHAR:
						hr = TraceChar(oid);traced = true;break;
					case ELEMENT_TYPE_I1:
						hr = TraceSByte(oid);traced = true;break;
					case ELEMENT_TYPE_U1:
						hr = TraceByte(oid);traced = true;break;
					case ELEMENT_TYPE_I2:
						hr = TraceShort(oid);traced = true;break;
					case ELEMENT_TYPE_U2:
						hr = TraceUShort(oid);traced = true;break;
					case ELEMENT_TYPE_I4:
						hr = TraceInt(oid);traced = true;break;
					case ELEMENT_TYPE_U4:
						hr = TraceUInt(oid);traced = true;break;
					case ELEMENT_TYPE_I8:
						hr = TraceLong(oid);traced = true;break;
					case ELEMENT_TYPE_U8:
						hr = TraceULong(oid);traced = true;break;
					case ELEMENT_TYPE_R4:
						hr = TraceFloat(oid);traced = true;break;
					case ELEMENT_TYPE_R8:
						hr = TraceDouble(oid);traced = true;break;
					case ELEMENT_TYPE_VALUETYPE:
						hr = TraceStruct(oid, cpi, mdiRef, pParameterInfo, NULL, classId, pClassInfo->m_classToken);traced = true;break;
					case ELEMENT_TYPE_CLASS:
						hr = TraceClass(oid, cpi, mdiRef, pParameterInfo);traced = true;break;
					default:
						hr = E_FAIL; break;
			 }//end switch
			}
			if(!traced) LogToAny("?");
			LogToAny("(%S)", pClassInfo->m_name);
		}//end pClassInfo!=NULL
	}
	return hr;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceArray(
	ObjectID oid,
  ICorProfilerInfo2 *cpi,
  IMetaDataImport *mdiRef,
  ParameterInfo *pParameterInfo)
{
	ULONG32 dim = 1;
	ULONG32 *dimSizes = new ULONG32[dim];
	int *dimLowerBounds = new int[dim];
	BYTE **ppData = new BYTE*[dim];
	//__asm { int 3 }

	HRESULT hr = cpi->GetArrayObjectInfo(oid, dim, dimSizes, dimLowerBounds, ppData);
	if(SUCCEEDED(hr)) {
		ULONG32 dimSize = dimSizes[0];
		ULONG32 dimLowerBound = dimLowerBounds[0];
		BYTE *pData = ppData[0];
		LogToAny("{");
		for(ULONG32 i=dimLowerBound; i<dimSize; i++) {
			switch(pParameterInfo->m_type) {
				case ELEMENT_TYPE_STRING:
					TraceString(*(ObjectID *)pData, cpi);
					pData += sizeof(ObjectID);
					break;
			  case ELEMENT_TYPE_BOOLEAN:
				  TraceBoolean((UINT_PTR)pData);
				  pData += sizeof(bool);
				  break;
			  case ELEMENT_TYPE_CHAR:
				  TraceChar((UINT_PTR)pData);
				  pData += sizeof(WCHAR);
				  break;
			  case ELEMENT_TYPE_I1:
				  TraceSByte((UINT_PTR)pData);
				  pData += sizeof(char);
				  break;
			  case ELEMENT_TYPE_U1:
				  TraceByte((UINT_PTR)pData);
				  pData += sizeof(unsigned char);
				  break;
				case ELEMENT_TYPE_I2:
				  TraceShort((UINT_PTR)pData);
				  pData += sizeof(short);
				  break;
				case ELEMENT_TYPE_U2:
				  TraceUShort((UINT_PTR)pData);
				  pData += sizeof(unsigned short);
				  break;
				case ELEMENT_TYPE_I4:
					TraceInt((UINT_PTR)pData);
					pData += sizeof(int);
					break;
				case ELEMENT_TYPE_U4:
				  TraceUInt((UINT_PTR)pData);
				  pData += sizeof(unsigned int);
				  break;
				case ELEMENT_TYPE_I8:
				  TraceLong((UINT_PTR)pData);
				  pData += sizeof(long long);
				  break;
				case ELEMENT_TYPE_U8:
				  TraceULong((UINT_PTR)pData);
				  pData += sizeof(unsigned long long);
				  break;
			  case ELEMENT_TYPE_R4:
				  TraceFloat((UINT_PTR)pData);
				  pData += sizeof(float);
				  break;
			  case ELEMENT_TYPE_R8:
				  TraceDouble((UINT_PTR)pData);
				  pData += sizeof(double);
				  break;
			  case ELEMENT_TYPE_VALUETYPE:
				  {
					  ULONG size = 0;
					TraceStruct((UINT_PTR)pData, cpi, mdiRef, pParameterInfo, &size, NULL, mdTokenNil);
					if(size < 0) {
						LogToAny("Cannot determine struct size.");
						break;
					}else{
						pData += size; // how to get size of struct?
					}
				  }
				  break;			  
			  case ELEMENT_TYPE_CLASS:
				  TraceClass(*(ObjectID *)pData, cpi, mdiRef, pParameterInfo);
				  pData += sizeof(ObjectID);
				  break;
			  case ELEMENT_TYPE_OBJECT:
				  TraceObject((UINT_PTR)pData, cpi, mdiRef, pParameterInfo);
				  pData += sizeof(ObjectID);
				  break;
			  default:
				  LogToAny("?");
				  break;
			}
			if(i<dimSize-1) LogToAny(",");
		}
		LogToAny("}");
	}
	return hr;
}
// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceULong(UINT_PTR startAddress)
{
	LogToAny("%u", *(unsigned long long *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceLong(UINT_PTR startAddress)
{
	LogToAny("%d", *(long long *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceUInt(UINT_PTR startAddress)
{
	LogToAny("%u", *(unsigned int *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceInt(UINT_PTR startAddress)
{
	LogToAny("%d", *(int *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceUShort(UINT_PTR startAddress)
{
	LogToAny("%u", *(unsigned short *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceShort(UINT_PTR startAddress)
{
	LogToAny("%d", *(short *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceByte(UINT_PTR startAddress)
{
	LogToAny("%u", *(unsigned char *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceSByte(UINT_PTR startAddress)
{
	LogToAny("%d", *(char *)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceChar(UINT_PTR startAddress)
{
	LogToAny("'%c'", *(char*)startAddress);
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceBoolean(UINT_PTR startAddress)
{
	if((*(bool*)startAddress)) 
	{
		LogToAny("true");
	}else{
		LogToAny("false");
	}
	return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceFloat(UINT_PTR startAddress)
{
  LogToAny("%f", *(float*)startAddress);
  return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceDouble(UINT_PTR startAddress)
{
  LogToAny("%f", *(double*)startAddress);
  return S_OK;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceString(
  ObjectID oid,
  ICorProfilerInfo2 *cpi)
{
	if(oid == mdTokenNil || oid == 0xffffffff) 
	{
		LogToAny("null");
		return S_OK;
	}

	//if(oid <= 0xFF) { // how to check whether ObjectID is valid?
	//	LogToAny("?");
	//	return S_OK;
	//}

  // get string
  ULONG bufferOffset=0;
  ULONG stringLengthOffset=0;
  ULONG bufferLengthOffset=0;
  HRESULT hr = cpi->GetStringLayout(&bufferLengthOffset, &stringLengthOffset, &bufferOffset);
  if(SUCCEEDED(hr))
  {
	  __try {
    LPWSTR buffer = (LPWSTR) (oid + bufferOffset);
	DWORD bufferLength = *((DWORD *)(oid + bufferLengthOffset));
	DWORD stringLength = *((DWORD *)(oid + stringLengthOffset));
	LogToAny("\"%S\"", buffer);
	  }__except(ExceptionFilter(GetExceptionCode(), GetExceptionInformation())){
		  LogToAny("?");
	  }
  }else{
	  LogToAny("?");
  }
  return hr;
}

int ProfilerCallback::ExceptionFilter(unsigned int code, struct _EXCEPTION_POINTERS *ep) {
   if (code == EXCEPTION_ACCESS_VIOLATION) {
      return EXCEPTION_EXECUTE_HANDLER;
   } else {
      return EXCEPTION_CONTINUE_SEARCH;
   };
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::TraceClass(
	ObjectID oid,
  ICorProfilerInfo2 *cpi,
  IMetaDataImport *mdiRef,
  ParameterInfo *pParameterInfo)
{
	//__asm { int 3 }

	HRESULT hr = S_OK;
	int fieldCnt = 0;
	IMetaDataImport *mdi = mdiRef;

	// get class for object
	ClassID classId = NULL;
	hr = cpi->GetClassFromObject(oid, &classId);
	ClassInfo *pClassInfo = NULL;
	if(SUCCEEDED(hr)) { 
		pClassInfo = m_pClassTable[classId];
	}
	if(pClassInfo==NULL) {
		LogToAny("?");
		return E_FAIL;
	}
	
	LogToAny("{");
	
	bool isRef = (pParameterInfo->m_moduleId != pClassInfo->m_moduleID);
	if(isRef) {
		ModuleInfo *pModuleInfo = m_pModuleTable[pClassInfo->m_moduleID];
		if(pModuleInfo != NULL) {
			mdi = pModuleInfo->GetMDI();
		}
	}

	if(SUCCEEDED(hr) && classId != NULL)
	{
		ULONG fieldOffsetCount = 0;
		hr = cpi->GetClassLayout(
			classId,
			NULL, 0, &fieldOffsetCount, NULL);

		if(fieldOffsetCount != 0) {

			COR_FIELD_OFFSET* fieldOffset = new COR_FIELD_OFFSET[fieldOffsetCount];

			hr = cpi->GetClassLayout(
				classId,
				fieldOffset, fieldOffsetCount, &fieldOffsetCount, NULL);

			// output fields
			if(SUCCEEDED(hr))
			{
			  for(ULONG i=0; i < fieldOffsetCount; i++)
			  {
				  ParameterInfo *pFieldParam = new ParameterInfo(fieldOffset[i].ridOfField);
				// get field name and prepare type info from metadata
				PCCOR_SIGNATURE sigBlob;
				ULONG sigBlobSize;		

				hr = mdi->GetFieldProps(
					fieldOffset[i].ridOfField, NULL,
					pFieldParam->m_name, MAX_LENGTH,    // get field name
				  NULL, NULL,             // acutal size of name, not relevant here
				  &sigBlob, &sigBlobSize, // field metadata
				  NULL, NULL, NULL);

				// get type and typename
				if(SUCCEEDED(hr))
				{

				  // make sure metadata describes a field and move forward in metadata
					if(*sigBlob++ == IMAGE_CEE_CS_CALLCONV_FIELD) 
					{}

				  // get kind of type (primitive, class, ...) and type name
					pFieldParam->m_type = GetType(sigBlob, pFieldParam->m_isByRef, pFieldParam->m_typeDef, pFieldParam->m_isArray);
				  hr = GetTypeName(pFieldParam->m_typeName, MAX_LENGTH, pFieldParam->m_type, pFieldParam->m_typeDef, mdi);
				  if(SUCCEEDED(hr) && pFieldParam->m_isArray)
					wcscat_s(pFieldParam->m_name, L"[]");
				}

				// output
				if(SUCCEEDED(hr))
				{
					if(fieldCnt++ > 0) 
						LogToAny(", ");
					LogToAny("%S %S=", pFieldParam->m_typeName, pFieldParam->m_name);

					if(pClassInfo->IsValidField(mdi, fieldOffset[i].ridOfField)) {
						switch(pFieldParam->m_type) {
							case ELEMENT_TYPE_VALUETYPE:
							case ELEMENT_TYPE_CLASS:
								LogToAny("?");
								break;
							default:
								TraceValue( 
									oid + fieldOffset[i].ulOffset, 
									cpi, mdi, pFieldParam);
								break;
						}
					}else{
						LogToAny("?");
					}
				}

				if(pFieldParam != NULL) 
					delete pFieldParam;
			  }
			}else{
				LogToAny("err=0x%p", hr);
			}

			if(fieldOffset)
				delete [] fieldOffset;
		
		} //end of numFields != 0
	
	}//end

	LogToAny("}");
	return hr;
}

HRESULT ProfilerCallback::GetStructParamInfo(ICorProfilerInfo2 *cpi,  
  IMetaDataImport **mdiRef,
  ParameterInfo *pParameterInfo,
  ClassID &classId,
  mdTypeDef &typeDef) 
{
	HRESULT hr = S_OK;
	typeDef = pParameterInfo->m_typeDef;
	bool isRef = (TypeFromToken( typeDef ) == mdtTypeRef);

	classId = NULL;
	// get class id
	if(isRef) {
		ClassInfo *pClassInfo = m_pClassNameTable[Hash(pParameterInfo->m_typeName)];
		if(pClassInfo != NULL) {
			classId = pClassInfo->m_id;
		}

        IMetaDataImport *pMetaDataImportRef = NULL;
		mdTypeDef classToken;
        hr = (*mdiRef)->ResolveTypeRef(typeDef, IID_IMetaDataImport, (IUnknown **)&pMetaDataImportRef, &classToken);
        if (hr == S_OK)
        {
            *mdiRef = pMetaDataImportRef;
            typeDef = classToken;
        }

	}else{
		hr = cpi->GetClassFromToken(pParameterInfo->m_moduleId, typeDef, &classId);
		if(!SUCCEEDED(hr)) 
			classId = NULL;
	}	
	return hr;
}

HRESULT ProfilerCallback::TraceStruct(
  UINT_PTR startAddress,
  ICorProfilerInfo2 *cpi,  
  IMetaDataImport *mdiRef,
  ParameterInfo *pParameterInfo,
  ULONG *pcSize, 
  ClassID classIdIn,
  mdTypeDef typeDefIn 
  )
{
	HRESULT hr = S_OK;
	int fieldCnt = 0;
	mdTypeDef typeDef = pParameterInfo->m_typeDef;
	IMetaDataImport *mdi = mdiRef;	
	ClassID classId = NULL;

	if(classIdIn != NULL && typeDefIn != mdTokenNil) {
		classId = classIdIn;
		typeDef = typeDefIn;
	}else{
		hr = GetStructParamInfo(cpi, &mdi, pParameterInfo, classId, typeDef);
	}

	ClassInfo *pClassInfo = NULL;
	if(classId != NULL) {
		pClassInfo = m_pClassTable[classId];
	}
	if(pClassInfo == NULL) {
		LogToAny("?");
		return E_FAIL;
	}

	ULONG structSize = 0;

	LogToAny("{");

	{
		ULONG fieldOffsetCount = 0;
		if(SUCCEEDED(hr)) {
			hr = cpi->GetClassLayout(
				classId, 
				NULL, 0, &fieldOffsetCount, NULL);
		}

		if(SUCCEEDED(hr) && fieldOffsetCount != 0) {
			
			//__asm { int 3 } 

			COR_FIELD_OFFSET* fieldOffset = new COR_FIELD_OFFSET[fieldOffsetCount];
			hr = cpi->GetClassLayout(
				classId, 
				fieldOffset, fieldOffsetCount, &fieldOffsetCount, NULL);

			// output fields
			if(SUCCEEDED(hr))
			{
			  for(ULONG i=0; i < fieldOffsetCount; i++)
			  {
				  ParameterInfo *pFieldParam = new ParameterInfo(fieldOffset[i].ridOfField);
				// get field name and prepare type info from metadata
				PCCOR_SIGNATURE sigBlob;
				ULONG sigBlobSize;		

				hr = mdi->GetFieldProps(
					fieldOffset[i].ridOfField, NULL,
					pFieldParam->m_name, MAX_LENGTH,    // get field name
				  NULL, NULL,             // acutal size of name, not relevant here
				  &sigBlob, &sigBlobSize, // field metadata
				  NULL, NULL, NULL);

				// get type and typename
				if(SUCCEEDED(hr))
				{
				  // make sure metadata describes a field and move forward in metadata
					if(*sigBlob++ == IMAGE_CEE_CS_CALLCONV_FIELD) 
					{}

				  // get kind of type (primitive, class, ...) and type name
					pFieldParam->m_type = GetType(sigBlob, pFieldParam->m_isByRef, pFieldParam->m_typeDef, pFieldParam->m_isArray);
				  hr = GetTypeName(pFieldParam->m_typeName, MAX_LENGTH, pFieldParam->m_type, pFieldParam->m_typeDef, mdi);
				  if(SUCCEEDED(hr) && pFieldParam->m_isArray)
					wcscat_s(pFieldParam->m_name, L"[]");
				}

				// output
				if(SUCCEEDED(hr))
				{
					ULONG typeSize = GetElementTypeSize(pFieldParam->m_type);
					if(typeSize < 0) 
						structSize = -1;
					if(structSize>=0)
						structSize += typeSize;

					if(fieldCnt++ > 0) 
						LogToAny(", ");
					LogToAny("%S %S=", pFieldParam->m_typeName, pFieldParam->m_name);					
					
					//__asm { int 3 }

					if(pClassInfo->IsValidField(mdi, fieldOffset[i].ridOfField)) {
						switch(pFieldParam->m_type) {
							case ELEMENT_TYPE_VALUETYPE:
							case ELEMENT_TYPE_CLASS:
								LogToAny("?");
								break;
							default:
								TraceValue( 
									startAddress + fieldOffset[i].ulOffset, 
									cpi, mdi, pFieldParam);
								break;
						}
					}else{
						LogToAny("?");
					}
				}
			  }
			}else{
				LogToAny("err=0x%p", hr);
			}
		
		} //end of numFields != 0
		else if(!SUCCEEDED(hr)) {
			LogToAny("err=0x%p", hr);
		}
	}

	LogToAny("}");

	if(pcSize != NULL) *pcSize = structSize;
	return hr;
}

// ----------------------------------------------------------------------------
CorElementType ProfilerCallback::GetType(
  PCCOR_SIGNATURE& sigBlob, 
  bool &isByRef, 
  mdTypeDef &typeDef,
  bool &isArray)
{
  CorElementType type = (CorElementType) *sigBlob++;

  isByRef = (ELEMENT_TYPE_BYREF == type);

  if(isByRef)
    type = (CorElementType) *sigBlob++;

  isArray = (ELEMENT_TYPE_SZARRAY == type);

  if(isArray)
    type = (CorElementType) *sigBlob++;

  typeDef = mdTypeDefNil;

  if(ELEMENT_TYPE_VALUETYPE == type || ELEMENT_TYPE_CLASS == type)
  {
    sigBlob += CorSigUncompressToken(sigBlob, &typeDef);
  }

  return type;
}

// ----------------------------------------------------------------------------
HRESULT ProfilerCallback::GetTypeName(
  PWCHAR name, 
  ULONG size, 
  CorElementType type, 
  mdTypeDef typeDef, 
  IMetaDataImport *mdi)
{
  HRESULT hr = S_OK;
  
  switch(type)
  {
  case ELEMENT_TYPE_VALUETYPE:
  case ELEMENT_TYPE_CLASS:
	if ( TypeFromToken( typeDef ) == mdtTypeRef )
	{
		hr = mdi->GetTypeRefProps(
        typeDef, NULL, 
        name, MAX_LENGTH, NULL);
	}
	else
    {
      hr = mdi->GetTypeDefProps(
        typeDef,
        name, MAX_LENGTH, NULL, NULL, NULL);
    }
	if(!SUCCEEDED(hr)) 
	{
		name[0] = '\0';
	}
    break;
  default:
	  {
		  _GetNameFromElementType(type, name, size);
	  }
    break;
  }

  return hr;
}

/* public */
__forceinline void ProfilerCallback::Leave( FunctionID functionID, COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange )
{
    FunctionInfo *pFunctionInfo = NULL;	
    pFunctionInfo = g_pCallbackObject->m_pFunctionTable[functionID];

	if(pFunctionInfo != NULL && g_pCallbackObject->_IsNeedToLog(pFunctionInfo->m_name) && 
		(g_pCallbackObject->m_traceEvent || g_pCallbackObject->m_traceParameter)
		)
	{
		g_pCallbackObject->LogToAny("< %S", 
			pFunctionInfo->m_name);

		if(g_pCallbackObject->m_traceParameter) 
		{
			g_pCallbackObject->LogToAny(" %S", 
				pFunctionInfo->m_pReturnInfo->m_typeName);

			HRESULT hr;
			mdMethodDef methodDef;		
			IMetaDataImport *mdi = NULL;

			if(pFunctionInfo->m_pReturnInfo->m_type != ELEMENT_TYPE_VOID) 
			{
				hr = g_pCallbackObject->m_pProfilerInfo->GetTokenAndMetaDataFromFunction(
				  functionID, 
				  IID_IMetaDataImport,
				  (IUnknown**) &mdi, 
				  &methodDef);

				if(SUCCEEDED(hr) && mdi != NULL) {
					g_pCallbackObject->LogToAny("=");
					hr = g_pCallbackObject->TraceValue( 
						pFunctionInfo->m_pReturnInfo->m_isByRef ? *(PUINT_PTR)(retvalRange->startAddress) : retvalRange->startAddress, 
						g_pCallbackObject->m_pProfilerInfo2, mdi,
						pFunctionInfo->m_pReturnInfo);	  
				}
			}
		}

		g_pCallbackObject->LogToAny("\n");
	}

} // ProfilerCallback::Leave

/* public */
void ProfilerCallback::Tailcall( FunctionID functionID )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( g_pCallbackObject->m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(g_pCallbackObject->m_traceEvent) {
		g_pCallbackObject->LogToAny("Tailcall: FunctionID=0x%p\n", functionID);
	}

} // ProfilerCallback::Tailcall

/* public */
HRESULT ProfilerCallback::ModuleLoadFinished( ModuleID moduleID,
                                              HRESULT hrStatus )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	
	try
    {
		if(SUCCEEDED(hrStatus)) {
			AddModule(moduleID);
		}
    }
    catch ( BaseException *exception )
    {
        exception->ReportFailure();
        delete exception;       
        Failure();    
    }

	if(m_traceEvent) {
		ModuleInfo *pModuleInfo = m_pModuleTable[moduleID];
		if(pModuleInfo != NULL) 
			LogToAny( "ModuleLoadFinished: ModuleID=0x%p, Name=%S, Address=0x%p\n", 
				moduleID,
				pModuleInfo->m_name,
                pModuleInfo->m_loadAddress
				  );
	}	
    return S_OK;

} // ProfilerCallback::ModuleLoadFinished

/* public */
HRESULT ProfilerCallback::JITCompilationStarted( FunctionID functionID,
                                                 BOOL fIsSafeToBlock )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	
    try
    { 
        AddFunction( functionID );       
    }
    catch ( BaseException *exception )
    {
        exception->ReportFailure();
        delete exception;       
        Failure();    
    }
	
	if(m_traceEvent) {
		FunctionInfo *pFunctionInfo = m_pFunctionTable[functionID];
		if(pFunctionInfo != NULL) 
			LogToAny("JITCompilationStarted: FunctionID=0x%p, Name=%S\n", functionID, pFunctionInfo->m_name);
	}

    return S_OK;
    
} // ProfilerCallback::JITCompilationStarted


/* public */
HRESULT ProfilerCallback::JITCachedFunctionSearchStarted( FunctionID functionID,
                                                          BOOL *pbUseCachedFunction )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

    // use the pre-jitted function
    *pbUseCachedFunction = TRUE;
	
    try
    {
        AddFunction( functionID );       
    }
    catch ( BaseException *exception )
    {
        exception->ReportFailure();
        delete exception;       
        Failure();    
    }

	if(m_traceEvent) {
		FunctionInfo *pFunctionInfo = m_pFunctionTable[functionID];
		if(pFunctionInfo != NULL) 
			LogToAny("JITCachedFunctionSearchStarted: FunctionID=0x%p, Name=%S\n", functionID, pFunctionInfo->m_name);
	}

    return S_OK;
       
} // ProfilerCallback::JITCachedFunctionSearchStarted

/* public */
HRESULT ProfilerCallback::JITCompilationFinished( FunctionID functionID,
                                                  HRESULT hrStatus,
                                                  BOOL fIsSafeToBlock )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("JITCompilationFinished: FunctionID=0x%p\n", functionID);
	}

    return S_OK;
    
} // ProfilerCallback::JITCompilationFinished


/* public */
HRESULT ProfilerCallback::JITCachedFunctionSearchFinished( FunctionID functionID,
                                                           COR_PRF_JIT_CACHE result )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("JITCachedFunctionSearchFinished: FunctionID=0x%p\n", functionID);
	}

    return S_OK;
      
} // ProfilerCallback::JITCachedFunctionSearchFinished


/* public */
HRESULT ProfilerCallback::ExceptionUnwindFunctionEnter( FunctionID functionID )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( g_pCallbackObject->m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("ExceptionUnwindFunctionEnter: FunctionID=0x%p\n", functionID);
	}
    return S_OK;

} // ProfilerCallback::ExceptionUnwindFunctionEnter


/* public */
HRESULT ProfilerCallback::ExceptionUnwindFunctionLeave( )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("ExceptionUnwindFunctionLeave\n");
	}

	return S_OK;

} // ProfilerCallback::ExceptionUnwindFunctionLeave


/* public */ 
HRESULT ProfilerCallback::ThreadCreated( ThreadID threadID )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( g_pCallbackObject->m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		HRESULT hr;
		ThreadID myThreadID;

		hr = m_pProfilerInfo->GetCurrentThreadID( &myThreadID );
		if ( SUCCEEDED( hr ) )
		{        
			if ( threadID == myThreadID )
			{
				DWORD win32ThreadID;
				hr = m_pProfilerInfo->GetThreadInfo( threadID, &win32ThreadID );
				if ( SUCCEEDED( hr ) ) 
				{
					LogToAny("ThreadCreated: ThreadID=%0xp, Win32ThreadID=0x%p\n", threadID, win32ThreadID);
				}					
				else
				{
					_THROW_EXCEPTION( "ICorProfilerInfo::GetThreadInfo() FAILED" )
				}
			}
			else
				_THROW_EXCEPTION( "Thread ID's do not match FAILED" )
		}
		else
			_THROW_EXCEPTION( "ICorProfilerInfo::GetCurrentThreadID() FAILED" )
	}

    return S_OK; 
    
} // ProfilerCallback::ThreadCreated


/* public */
HRESULT ProfilerCallback::ThreadDestroyed( ThreadID threadID )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( g_pCallbackObject->m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
    if(m_traceEvent)
    {
		LogToAny("ThreadDestroyed: ThreadID=%Id\n", threadID);
    }
    return S_OK;
    
} // ProfilerCallback::ThreadDestroyed

/* public */
HRESULT ProfilerCallback::ThreadAssignedToOSThread( ThreadID managedThreadID,
                                                    DWORD osThreadID ) 
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( g_pCallbackObject->m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("ThreadAssignedToOSThread: ThreadID=0x%p, OSThreadID=0x%p\n", managedThreadID, osThreadID);
	}
    return S_OK;
    
} // ProfilerCallback::ThreadAssignedToOSThread

/* public */
HRESULT ProfilerCallback::UnmanagedToManagedTransition( FunctionID functionID,
                                                        COR_PRF_TRANSITION_REASON reason )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

    if ( m_traceEvent && reason == COR_PRF_TRANSITION_RETURN )
    {
		LogToAny("UnmanagedToManagedTransition: FunctionID=0x%p\n", functionID);
    }

    return S_OK;

} // ProfilerCallback::UnmanagedToManagedTransition



/* public */
HRESULT ProfilerCallback::ManagedToUnmanagedTransition( FunctionID functionID,
                                                        COR_PRF_TRANSITION_REASON reason )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

    if ( m_traceEvent && reason == COR_PRF_TRANSITION_CALL )
    {
		LogToAny("ManagedToUnmanagedTransition: FunctiondID=0x%p\n", functionID);
    }
    return S_OK;

} // ProfilerCallback::ManagedToUnmanagedTransition

HRESULT ProfilerCallback::ObjectAllocated( ObjectID objectID,
                                           ClassID classID )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("ObjectAllocated: ObjectID=0x%p, ClassID=0x%p\n", objectID, classID);
	}

    return S_OK;

} // ProfilerCallback::ObjectAllocated



/* public */
HRESULT ProfilerCallback::ObjectReferences( ObjectID objectID,
                                            ClassID classID,
                                            ULONG objectRefs,
                                            ObjectID objectRefIDs[] )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("ObjectReferences: ObjectID=0x%p, ClassID=0x%p", objectID, classID);
	}
    return S_OK;

} // ProfilerCallback::ObjectReferences



/* public */
HRESULT ProfilerCallback::RootReferences( ULONG rootRefs,
                                          ObjectID rootRefIDs[] )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("RootReferences: 0x%p\n", rootRefs);
	}
    return S_OK;

} // ProfilerCallback::RootReferences



/* public */
HRESULT ProfilerCallback::RuntimeSuspendStarted( COR_PRF_SUSPEND_REASON suspendReason )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( g_pCallbackObject->m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

    //if we are shutting down , terminate all the threads
    if ( suspendReason == COR_PRF_SUSPEND_FOR_SHUTDOWN )
    {
		m_bShutdown = TRUE;
    }

	if(m_traceEvent) {
		LogToAny("RuntimeSuspendStarted: %d\n", suspendReason);
	}
    return S_OK;
    
} // ProfilerCallback::RuntimeSuspendStarted



/* public */
HRESULT ProfilerCallback::RuntimeResumeFinished()
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny( "RuntimeResumeFinished\n");
	}
    return S_OK;
    
} // ProfilerCallback::RuntimeResumeFinished


/***************************************************************************************
 ********************                                               ********************
 ********************     Callbacks With Default Implementation     ********************
 ********************                                               ********************
 ***************************************************************************************/



/* public */
HRESULT ProfilerCallback::AppDomainCreationStarted( AppDomainID appDomainID )
{
	if(m_traceEvent) {
		LogToAny("AppDomainCreationStarted: DomainID=0x%p\n", appDomainID);
	}
    return S_OK;

} // ProfilerCallback::AppDomainCreationStarted



/* public */
HRESULT ProfilerCallback::AppDomainCreationFinished( AppDomainID appDomainID,
                                                     HRESULT hrStatus )
{

    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		ULONG size;
		WCHAR name[2048];
		ProcessID processId;
		if(SUCCEEDED(m_pProfilerInfo->GetAppDomainInfo(appDomainID, 2048, &size, name, &processId)))
		{
			LogToAny("AppDomainCreationFinished: DomainID=0x%p, Name=%S, ProcessID=0x%p\n", appDomainID, name, processId);
		}
	}
	return S_OK;

} // ProfilerCallback::AppDomainCreationFinished



/* public */
HRESULT ProfilerCallback::AppDomainShutdownStarted( AppDomainID appDomainID )
{
	if(m_traceEvent) {
		LogToAny("AppDomainShutdownStarted: DomainID=0x%p\n", appDomainID);
	}
    return S_OK;

} // ProfilerCallback::AppDomainShutdownStarted

      


/* public */
HRESULT ProfilerCallback::AppDomainShutdownFinished( AppDomainID appDomainID,
                                                     HRESULT hrStatus )
{
	if(m_traceEvent) {
		LogToAny("AppDomainShutdownFinished: DomainID=0x%p\n", appDomainID);
	}
    return S_OK;

} // ProfilerCallback::AppDomainShutdownFinished



/* public */
HRESULT ProfilerCallback::AssemblyLoadStarted( AssemblyID assemblyId )
{
	if(m_traceEvent) {
		LogToAny("AssemblyLoadStarted: AssemblyID=0x%p\n", assemblyId); 
	}
    return S_OK;
} // ProfilerCallback::AssemblyLoadStarted



/* public */
HRESULT ProfilerCallback::AssemblyLoadFinished( AssemblyID assemblyId,
                                                HRESULT hrStatus )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
   
	if(m_traceEvent) {
		ULONG size;
		WCHAR name[2048];
		ModuleID moduleId;
		AppDomainID appDomainId;
		if(SUCCEEDED(m_pProfilerInfo->GetAssemblyInfo(assemblyId, 2048, &size, name, &appDomainId, &moduleId)))
		{
			LogToAny("AssemblyLoadFinished: AssemblyID=0x%p, Name=%S, AppDomainID=0x%p, ModuleID=0x%p\n", assemblyId, name, appDomainId, moduleId); 
		}
	}
    return S_OK;
} // ProfilerCallback::AssemblyLoadFinished


/* public */
HRESULT ProfilerCallback::AssemblyUnloadStarted( AssemblyID assemblyId )
{
	if(m_traceEvent) {
		LogToAny("AssemblyUnloadStarted: AssemblyID=0x%p\n", assemblyId); 
	}
    return S_OK;

} // ProfilerCallback::AssemblyUnLoadStarted

      
/* public */
HRESULT ProfilerCallback::AssemblyUnloadFinished( AssemblyID assemblyId,
                                                  HRESULT hrStatus )
{
	if(m_traceEvent) {
		LogToAny("AssemblyUnloadFinished: AssemblyID=0x%p\n", assemblyId); 
	}
    return S_OK;

} // ProfilerCallback::AssemblyUnLoadFinished


/* public */
HRESULT ProfilerCallback::ModuleLoadStarted( ModuleID moduleID )
{
	if(m_traceEvent) {
		LogToAny("ModuleLoadStarted: ModuleID=0x%p\n", moduleID);
	}
    return S_OK;

} // ProfilerCallback::ModuleLoadStarted


/* public */
HRESULT ProfilerCallback::ModuleUnloadStarted( ModuleID moduleID )
{
	if(m_traceEvent) {
		LogToAny("ModuleUnloadStarted: ModuleID=0x%p\n", moduleID);
	}
    return S_OK;

} // ProfilerCallback::ModuleUnloadStarted
      

/* public */
HRESULT ProfilerCallback::ModuleUnloadFinished( ModuleID moduleID,
                                                HRESULT hrStatus )
{
	if(m_traceEvent) {
		LogToAny("ModuleUnloadFinished: ModuleID=0x%p\n", moduleID);
	}

	try
    {
		if(SUCCEEDED(hrStatus)) {
			RemoveModule(moduleID);
		}
    }
    catch ( BaseException *exception )
    {
        exception->ReportFailure();
        delete exception;       
        Failure();    
    }

    return S_OK;

} // ProfilerCallback::ModuleUnloadFinished


/* public */
HRESULT ProfilerCallback::ModuleAttachedToAssembly( ModuleID moduleID,
                                                    AssemblyID assemblyID )
{
	if(m_traceEvent) {
		LogToAny("ModuleAttachedToAssembly: ModuleID=0x%p, AssemblyID=0x%p\n", moduleID, assemblyID);
	}
    return S_OK;

} // ProfilerCallback::ModuleAttachedToAssembly


/* public */
HRESULT ProfilerCallback::ClassLoadStarted( ClassID classID )
{
	if(m_traceEvent) {
		LogToAny("ClassLoadStarted: ClassID=0x%p\n", classID);
	}
    return S_OK;

} // ProfilerCallback::ClassLoadStarted


/* public */
HRESULT ProfilerCallback::ClassLoadFinished( ClassID classID, 
                                             HRESULT hrStatus )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
    try
    {
		if(SUCCEEDED(hrStatus)) {
			AddClass(classID);
		}
    }
    catch ( BaseException *exception )
    {
        exception->ReportFailure();
        delete exception;       
        Failure();    
    }
	
	if(m_traceEvent)
	{
		ClassInfo *pClassInfo = m_pClassTable[classID];
		if(pClassInfo != NULL) 
			LogToAny("ClassLoadFinished: ClassID=0x%p, Name=%S, ModuleID=%p, Token=%p\n", classID, pClassInfo->m_name, pClassInfo->m_moduleID, pClassInfo->m_classToken);
	}

    return S_OK;
} // ProfilerCallback::ClassLoadFinished


/* public */
HRESULT ProfilerCallback::ClassUnloadStarted( ClassID classID )
{
	if(m_traceEvent) {
		LogToAny("ClassUnloadStarted: ClassID=0x%p\n", classID);
	}
    return S_OK;

} // ProfilerCallback::ClassUnloadStarted


HRESULT ProfilerCallback::ClassUnloadFinished( ClassID classID, 
                                               HRESULT hrStatus )
{
	if(m_traceEvent) {
		ClassInfo *pClassInfo = m_pClassTable[classID];
		if(pClassInfo != NULL) 
			LogToAny("ClassUnloadFinished: ClassID=0x%p, Name=%S, ModuleID=%p, Token=%p\n", classID, pClassInfo->m_name, pClassInfo->m_moduleID, pClassInfo->m_classToken);
	}

	try
    {
		if(SUCCEEDED(hrStatus)) {
			RemoveClass(classID);
		}
    }
    catch ( BaseException *exception )
    {
        exception->ReportFailure();
        delete exception;       
        Failure();    
    }

    return S_OK;

} // ProfilerCallback::ClassUnloadFinished


/* public */
HRESULT ProfilerCallback::FunctionUnloadStarted( FunctionID functionID )
{
	if(m_traceEvent) {
		LogToAny("FunctionUnloadStarted: FunctionID=0x%p\n", functionID);
	}
    return S_OK;

} // ProfilerCallback::FunctionUnloadStarted


/* public */
HRESULT ProfilerCallback::JITFunctionPitched( FunctionID functionID )
{
    if(m_traceEvent) {
		LogToAny("JITFunctionPitched: FunctionID=0x%p\n", functionID);
	}
    return S_OK;
    
} // ProfilerCallback::JITFunctionPitched


/* public */
HRESULT ProfilerCallback::JITInlining( FunctionID callerID,
                                       FunctionID calleeID,
                                       BOOL *pfShouldInline )
{
	if(m_traceEvent) {
		LogToAny("JITInlining: CallerID=0x%p, CalleeID=0x%p\n", callerID, calleeID);
	}
    return S_OK;

} // ProfilerCallback::JITInlining


/* public */
HRESULT ProfilerCallback::RemotingClientInvocationStarted()
{
	if(m_traceEvent) {
		LogToAny("RemotingClientInvocationStarted\n");
	}
    return S_OK;
    
} // ProfilerCallback::RemotingClientInvocationStarted


/* public */
HRESULT ProfilerCallback::RemotingClientSendingMessage( GUID *pCookie,
                                                        BOOL fIsAsync )
{
	if(m_traceEvent) {
		LogToAny("RemotingClientSendingMessage\n");
	}
    return S_OK;
    
} // ProfilerCallback::RemotingClientSendingMessage


/* public */
HRESULT ProfilerCallback::RemotingClientReceivingReply( GUID *pCookie,
                                                        BOOL fIsAsync )
{
	if(m_traceEvent) {
		LogToAny("RemotingClientReceivingReply\n");
	}
    return S_OK;
    
} // ProfilerCallback::RemotingClientReceivingReply


HRESULT ProfilerCallback::RemotingClientInvocationFinished()
{
	if(m_traceEvent) {
		LogToAny("RemotingClientInvocationFinished\n");
	}
   return S_OK;
    
} // ProfilerCallback::RemotingClientInvocationFinished


/* public */
HRESULT ProfilerCallback::RemotingServerReceivingMessage( GUID *pCookie,
                                                          BOOL fIsAsync )
{
	if(m_traceEvent) {
		LogToAny("RemotingServerReceivingMessage\n");
	}
    return S_OK;
    
} // ProfilerCallback::RemotingServerReceivingMessage


/* public */
HRESULT ProfilerCallback::RemotingServerInvocationStarted()
{
	if(m_traceEvent) {
		LogToAny("RemotingServerInvocationStarted\n");
	}
    return S_OK;
    
} // ProfilerCallback::RemotingServerInvocationStarted


/* public */
HRESULT ProfilerCallback::RemotingServerInvocationReturned()
{
	if(m_traceEvent) {
		LogToAny("RemotingServerInvocationReturned\n");
	}
    return S_OK;
    
} // ProfilerCallback::RemotingServerInvocationReturned


/* public */
HRESULT ProfilerCallback::RemotingServerSendingReply( GUID *pCookie,
                                                      BOOL fIsAsync )
{
	if(m_traceEvent) {
		LogToAny("RemotingServerSendingReply\n");
	}

    return S_OK;

} // ProfilerCallback::RemotingServerSendingReply


/* public */
HRESULT ProfilerCallback::RuntimeSuspendFinished()
{
	if(m_traceEvent) {
		LogToAny("RuntimeSuspendFinished\n");
	}

    return S_OK;
    
} // ProfilerCallback::RuntimeSuspendFinished


/* public */
HRESULT ProfilerCallback::RuntimeSuspendAborted()
{
	if(m_traceEvent) {
		LogToAny("RuntimeSuspendAborted\n");
	}

    return S_OK;
    
} // ProfilerCallback::RuntimeSuspendAborted


/* public */
HRESULT ProfilerCallback::RuntimeResumeStarted()
{
	if(m_traceEvent) {
		LogToAny("RuntimeResumeStarted\n");
	}

    return S_OK;
    
} // ProfilerCallback::RuntimeResumeStarted


/* public */
HRESULT ProfilerCallback::RuntimeThreadSuspended( ThreadID threadID )
{
	if(m_traceEvent) {
		LogToAny("RuntimeThreadSuspended: ThreadID=%Id\n", threadID);
	}
	
    return S_OK;
    
} // ProfilerCallback::RuntimeThreadSuspended


/* public */
HRESULT ProfilerCallback::RuntimeThreadResumed( ThreadID threadID )
{
	if(m_traceEvent) {
		LogToAny("RuntimeThreadResumed: ThreadID=%Id\n", threadID);
	}

    return S_OK;
    
} // ProfilerCallback::RuntimeThreadResumed


/* public */
HRESULT ProfilerCallback::MovedReferences( ULONG cmovedObjectIDRanges,
                                           ObjectID oldObjectIDRangeStart[],
                                           ObjectID newObjectIDRangeStart[],
                                           ULONG cObjectIDRangeLength[] )
{	
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("MovedReferences\n");
	}	
    return S_OK;

} // ProfilerCallback::MovedReferences


HRESULT ProfilerCallback::SurvivingReferences( ULONG cmovedObjectIDRanges,
                                               ObjectID objectIDRangeStart[],
                                               ULONG cObjectIDRangeLength[] )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("SurvivingReferences\n");
	}	

    return S_OK;

} // ProfilerCallback::SurvivingReferences

/* public */
HRESULT ProfilerCallback::ObjectsAllocatedByClass( ULONG classCount,
                                                   ClassID classIDs[],
                                                   ULONG objects[] )
{
	if(m_traceEvent) {
		LogToAny("ObjectsAllocatedByClass: ClassCount=%u\n", classCount);
	}	

    return S_OK;

} // ProfilerCallback::ObjectsAllocatedByClass


/* public */
HRESULT ProfilerCallback::ExceptionThrown( ObjectID thrownObjectID )
{

	LogToAny("\nExceptionThrown: ObjectID=0x%p\n", thrownObjectID);
    return S_OK;

} // ProfilerCallback::ExceptionThrown 


/* public */
HRESULT ProfilerCallback::ExceptionSearchFunctionEnter( FunctionID functionID )
{
	if(m_traceEvent) {
		LogToAny("ExceptionSearchFunctionEnter: FunctionID=0x%p\n", functionID);
	}	

    return S_OK;

} // ProfilerCallback::ExceptionSearchFunctionEnter


/* public */
HRESULT ProfilerCallback::ExceptionSearchFunctionLeave()
{
	if(m_traceEvent) {
		LogToAny("ExceptionSearchFunctionLeave\n");
	}	

    return S_OK;

} // ProfilerCallback::ExceptionSearchFunctionLeave


/* public */
HRESULT ProfilerCallback::ExceptionSearchFilterEnter( FunctionID functionID )
{
	if(m_traceEvent) {
		LogToAny("ExceptionSearchFilterEnter: FunctionID=0x%p\n", functionID);
	}	
    return S_OK;

} // ProfilerCallback::ExceptionSearchFilterEnter


/* public */
HRESULT ProfilerCallback::ExceptionSearchFilterLeave()
{
	if(m_traceEvent) {
		LogToAny("ExceptionSearchFilterLeave\n");
	}	
	
    return S_OK;

} // ProfilerCallback::ExceptionSearchFilterLeave 


/* public */
HRESULT ProfilerCallback::ExceptionSearchCatcherFound( FunctionID functionID )
{

	if(m_traceEvent) {
		LogToAny("ExceptionSearchCatcherFound: FunctionID=0x%p\n", functionID);
	}	
    return S_OK;

} // ProfilerCallback::ExceptionSearchCatcherFound


/* public */
HRESULT ProfilerCallback::ExceptionCLRCatcherFound()
{
	if(m_traceEvent) {
		LogToAny("ExceptionCLRCatcherFound\n");
	}	
	
    return S_OK;
}

/* public */
HRESULT ProfilerCallback::ExceptionCLRCatcherExecute()
{
	if(m_traceEvent) {
		LogToAny("ExceptionCLRCatcherExecute\n");
	}	
	
    return S_OK;
}


/* public */
HRESULT ProfilerCallback::ExceptionOSHandlerEnter( FunctionID functionID )
{
	if(m_traceEvent) {
		LogToAny("ExceptionOSHandlerEnter: FunctionID=0x%p\n", functionID);
	}	
	
    return S_OK;

} // ProfilerCallback::ExceptionOSHandlerEnter

    
/* public */
HRESULT ProfilerCallback::ExceptionOSHandlerLeave( FunctionID functionID )
{
	if(m_traceEvent) {
		LogToAny("ExceptionOSHandlerLeave: FunctionID=0x%p\n", functionID);
	}	
	
    return S_OK;

} // ProfilerCallback::ExceptionOSHandlerLeave


/* public */
HRESULT ProfilerCallback::ExceptionUnwindFinallyEnter( FunctionID functionID )
{

	if(m_traceEvent) {
		LogToAny("ExceptionUnwindFinallyEnter: FunctionID=0x%p\n", functionID);
	}	
	
    return S_OK;

} // ProfilerCallback::ExceptionUnwindFinallyEnter


/* public */
HRESULT ProfilerCallback::ExceptionUnwindFinallyLeave()
{

	if(m_traceEvent) {
		LogToAny("ExceptionUnwindFinallyLeave\n");
	}	
	
    return S_OK;

} // ProfilerCallback::ExceptionUnwindFinallyLeave


/* public */
HRESULT ProfilerCallback::ExceptionCatcherEnter( FunctionID functionID,
                                                 ObjectID objectID )
{
	if(m_traceEvent) {
		LogToAny("ExceptionCatcherEnter: FunctionID=0x%p, ObjectID=0x%p\n", functionID, objectID);
	}
	
    return S_OK;

} // ProfilerCallback::ExceptionCatcherEnter


/* public */
HRESULT ProfilerCallback::ExceptionCatcherLeave()
{
	if(m_traceEvent) {
		LogToAny("ExceptionCatcherLeave\n");
	}
	
    return S_OK;

} // ProfilerCallback::ExceptionCatcherLeave


/* public */
HRESULT ProfilerCallback::COMClassicVTableCreated( ClassID wrappedClassID,
                                                   REFGUID implementedIID,
                                                   void *pVTable,
                                                   ULONG cSlots )
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("COMClassicVTableCreated: WrappedClassID=0x%p\n", wrappedClassID);
	}

    return S_OK;
} // ProfilerCallback::COMClassicWrapperCreated


/* public */
HRESULT ProfilerCallback::COMClassicVTableDestroyed( ClassID wrappedClassID,
                                                     REFGUID implementedIID,
                                                     void *pVTable )
{
	if(m_traceEvent) {
		LogToAny("COMClassicVTableDestroyed: ClassID=0x%p\n", wrappedClassID);
	}
	
	return S_OK;

} // ProfilerCallback::COMClassicWrapperDestroyed


/* public */
HRESULT ProfilerCallback::ThreadNameChanged( 
            /* [in] */ ThreadID threadId,
            /* [in] */ ULONG cchName,
            /* [in] */ WCHAR name[  ])
{
	if(m_traceEvent) {
		LogToAny("ThreadNameChanged: ThreadID=%Id, Name=%S\n", threadId, name);
	}
    return S_OK;
} // ProfilerCallback::ThreadNameChanged


/* public */
HRESULT ProfilerCallback::FinalizeableObjectQueued( 
            /* [in] */ DWORD finalizerFlags,
            /* [in] */ ObjectID objectID)
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("FinalizeableObjectQueued: FinalizerFlags=%u, ObjectID=0x%p\n", finalizerFlags, objectID);
	}
	
	
    return S_OK;
}

/* public */
HRESULT ProfilerCallback::RootReferences2( 
            /* [in] */ ULONG cRootRefs,
            /* [size_is][in] */ ObjectID rootRefIds[  ],
            /* [size_is][in] */ COR_PRF_GC_ROOT_KIND rootKinds[  ],
            /* [size_is][in] */ COR_PRF_GC_ROOT_FLAGS rootFlags[  ],
            /* [size_is][in] */ UINT_PTR rootIds[  ])
{
	
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("RootReferences2: 0x%p\n", cRootRefs);
	}
	
    return S_OK;
}

/* public */
HRESULT ProfilerCallback::HandleCreated(
            /* [in] */ UINT_PTR handleId,
            /* [in] */ ObjectID initialObjectId)
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	if(m_traceEvent) {
		LogToAny("HandleCreated: HandleID=0x%p\n", handleId);
	}
	
    return S_OK;
}

/* public */
HRESULT ProfilerCallback::HandleDestroyed(
            /* [in] */ UINT_PTR handleId)
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  
	if(m_traceEvent) {
		LogToAny("HandleDestroyed: HandleID=0x%p\n", handleId);
	}
    return S_OK;
}

bool ProfilerCallback::_IsNeedToLog(WCHAR *pFilter) 
{
	if(m_includeSystem == FALSE) 
	{
		if(_wcsnicmp(pFilter, L"System.", ARRAY_LEN(L"System.") - 1) == 0) return false;
		if(_wcsnicmp(pFilter, L"Microsoft.", ARRAY_LEN(L"Microsoft.") - 1) == 0) return false;
	}

	//LogToAny("DEBUG: %S %S %d\n", pFilter, m_filter, m_filterLen);
	if(m_filter != NULL && m_filterLen >= 2) 
	{
		//int cmp = _wcsnicmp(pClassName, m_filter, m_filterLen);
		//LogToAny("SP_CLASS: %S %S %d %d\n", pClassName, m_filter, cmp, m_filterLen-1);
		//if(cmp==0) return true;
		//else return false;

		if(wcsstr(pFilter, m_filter) == NULL) return false;
	}

	return true;
}

HRESULT ProfilerCallback::GarbageCollectionStarted(
    /* [in] */int cGenerations,
    /*[in, size_is(cGenerations), length_is(cGenerations)]*/ BOOL generationCollected[],
    /*[in]*/ COR_PRF_GC_REASON reason)
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	LogToAny("GarbageCollectionStarted\n");
    return S_OK;
}


/*
 * The CLR calls GarbageCollectionFinished after a garbage
 * collection has completed and all GC callbacks have been
 * issued for it.
 */
HRESULT  ProfilerCallback::GarbageCollectionFinished()
{
    ///////////////////////////////////////////////////////////////////////////
    Synchronize guard( m_criticalSection );
    ///////////////////////////////////////////////////////////////////////////  

	LogToAny("GarbageCollectionFinished\n");
    return S_OK;
}


/***************************************************************************************
 ********************                                               ********************
 ********************              Private Functions                ********************
 ********************                                               ********************
 ***************************************************************************************/ 

/* public */
void ProfilerCallback::_ProcessEnvVariables()
{
    char buffer[4*MAX_LENGTH];

	//
    // look if the user specified another path to save the output file
    //
    buffer[0] = '\0';
	if ( GetEnvironmentVariableA( SP_LOG_PATH, buffer, MAX_LENGTH ) > 0 )
    {
        // room for buffer chars + '\' + logfilename chars + '\0':
        size_t len = ARRAY_LEN(buffer) + ARRAY_LEN(m_logFileName);
        m_path = new char[len];
        if ( m_path != NULL )
        {
            sprintf_s( m_path, len, "%s\\%s", buffer, m_logFileName ); 
			len = ARRAY_LEN(buffer) + ARRAY_LEN(STATUS_FILENAME);
			m_status_path = new char[len];
		    sprintf_s( m_status_path, len, "%s\\%s", buffer, STATUS_FILENAME ); 
        }
    }

    buffer[0] = '\0';
	m_includeSystem = FALSE;
    if (GetEnvironmentVariableA(SP_INCLUDE_SYSTEM, buffer, MAX_LENGTH) > 0)
    {
        if(_stricmp("1", buffer) == 0)
        {
            m_includeSystem = TRUE;
        }
    }

    buffer[0] = '\0';
	m_traceEvent = FALSE;
    if (GetEnvironmentVariableA(SP_TRACE_EVENT, buffer, MAX_LENGTH) > 0)
    {
        if(_stricmp("1", buffer) == 0)
        {
            m_traceEvent = TRUE;
		}
    }

    buffer[0] = '\0';
	m_traceParameter = FALSE;
    if (GetEnvironmentVariableA(SP_TRACE_PARAMETER, buffer, MAX_LENGTH) > 0)
    {
        if(_stricmp("1", buffer) == 0)
        {
            m_traceParameter = TRUE;
        }
    }

	if(m_traceEvent == TRUE) {
		m_dwEventMask = (DWORD) COR_PRF_ALL;
	}
	else {
		m_dwEventMask = (DWORD) (
			COR_PRF_MONITOR_CLASS_LOADS | 
			COR_PRF_MONITOR_MODULE_LOADS | 
			COR_PRF_MONITOR_ENTERLEAVE | 
			COR_PRF_ENABLE_FUNCTION_ARGS |
			COR_PRF_ENABLE_FUNCTION_RETVAL |
			COR_PRF_MONITOR_JIT_COMPILATION |
			COR_PRF_DISABLE_OPTIMIZATIONS | 
			COR_PRF_DISABLE_INLINING //|
			//COR_PRF_MONITOR_ASSEMBLY_LOADS | 
			//COR_PRF_MONITOR_APPDOMAIN_LOADS | 
			//COR_PRF_MONITOR_THREADS 
			);
	}

    //
    // in which class you are interested in
    //
    buffer[0] = '\0';	
    if( GetEnvironmentVariableA( SP_FILTER, buffer, MAX_LENGTH ) > 0) 
	{
		//
		// if the env variable does not exist copy to it the null
		// string otherwise copy its value
		//
		//__asm { int 3 }
		const size_t len = ARRAY_LEN(buffer) + 1;
		m_filter = new WCHAR[len];
		if ( m_filter != NULL )
		{
			_snwprintf_s( m_filter, len, len-1, L"%S", buffer );
			m_filterLen = (int) wcslen(m_filter);
		}
		else
		{
			//
			// some error has happened, do not monitor anything
			//
			printf( "Memory Allocation Error in ProfilerCallback .ctor\n" );
			printf( "**** No Profiling Will Take place **** \n" );
			m_dwEventMask = (DWORD) COR_PRF_MONITOR_NONE;           
		}
	}

} // ProfilerCallback::_ProcessEnvVariables

/* private */
ULONG ProfilerCallback::GetElementTypeSize( CorElementType elementType )
{
	ULONG size = 0;
    switch ( elementType )
    {
        case ELEMENT_TYPE_BOOLEAN:
			size = sizeof(bool);
             break;
        case ELEMENT_TYPE_CHAR:
        case ELEMENT_TYPE_I1:
        case ELEMENT_TYPE_U1:
			size = 1;
             break;

        case ELEMENT_TYPE_I2:
        case ELEMENT_TYPE_U2:
			size = 2;
			break;

        case ELEMENT_TYPE_I4:
        case ELEMENT_TYPE_U4:
			size = 4;
			break;

        case ELEMENT_TYPE_I8:
        case ELEMENT_TYPE_U8:
			size = 8;
			break;

        case ELEMENT_TYPE_R4:
			size = 4;
			break;

        case ELEMENT_TYPE_R8:
			size = 8;
			break;

        case ELEMENT_TYPE_STRING:
			size = sizeof(ObjectID);
			break;

        case ELEMENT_TYPE_PTR:
			size = sizeof(INT_PTR);
			break;

        case ELEMENT_TYPE_CLASS:
             size = sizeof(ObjectID);
             break;

        case ELEMENT_TYPE_I:
             size = sizeof(INT_PTR);
             break;

        case ELEMENT_TYPE_U:
             size = sizeof(UINT_PTR);
             break;

        case ELEMENT_TYPE_ARRAY:
        case ELEMENT_TYPE_VALUETYPE:
        case ELEMENT_TYPE_OBJECT:
        case ELEMENT_TYPE_SZARRAY:
		case ELEMENT_TYPE_VOID:
		case ELEMENT_TYPE_FNPTR:
        case ELEMENT_TYPE_MAX:
        case ELEMENT_TYPE_END:
        case ELEMENT_TYPE_BYREF:
        case ELEMENT_TYPE_PINNED:
        case ELEMENT_TYPE_SENTINEL:
        case ELEMENT_TYPE_CMOD_OPT:
        case ELEMENT_TYPE_MODIFIER:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_TYPEDBYREF:
        default:
			size = -1;
             break;
    }

    return size;
} // ProfilerCallback::GetElementTypeSize


/* private */
HRESULT ProfilerCallback::_GetNameFromElementType( CorElementType elementType, __out_ecount(buflen) WCHAR *buffer, size_t buflen )
{
    HRESULT hr = S_OK;

    switch ( elementType )
    {
        case ELEMENT_TYPE_BOOLEAN:
             //wcscpy_s( buffer, buflen, L"System.Boolean" );
			 wcscpy_s( buffer, buflen, L"bool" );
             break;

        case ELEMENT_TYPE_CHAR:
             //wcscpy_s( buffer, buflen, L"System.Char" );
			 wcscpy_s( buffer, buflen, L"char" );
             break;

        case ELEMENT_TYPE_I1:
             //wcscpy_s( buffer, buflen, L"System.SByte" );
			 wcscpy_s( buffer, buflen, L"sbyte" );
             break;

        case ELEMENT_TYPE_U1:
             //wcscpy_s( buffer, buflen, L"System.Byte" );
			 wcscpy_s( buffer, buflen, L"byte" );
             break;

        case ELEMENT_TYPE_I2:
             //wcscpy_s( buffer, buflen, L"System.Int16" );
			 wcscpy_s( buffer, buflen, L"short" );
             break;

        case ELEMENT_TYPE_U2:
             //wcscpy_s( buffer, buflen, L"System.UInt16" );
			 wcscpy_s( buffer, buflen, L"ushort" );
             break;

        case ELEMENT_TYPE_I4:
             //wcscpy_s( buffer, buflen, L"System.Int32" );
			 wcscpy_s( buffer, buflen, L"int" );
             break;

        case ELEMENT_TYPE_U4:
             //wcscpy_s( buffer, buflen, L"System.UInt32" );
			 wcscpy_s( buffer, buflen, L"uint" );
             break;

        case ELEMENT_TYPE_I8:
             //wcscpy_s( buffer, buflen, L"System.Int64" );
			 wcscpy_s( buffer, buflen, L"long" );
             break;

        case ELEMENT_TYPE_U8:
             //wcscpy_s( buffer, buflen, L"System.UInt64" );
			 wcscpy_s( buffer, buflen, L"ulong" );
             break;

        case ELEMENT_TYPE_R4:
             //wcscpy_s( buffer, buflen, L"System.Single" );
			 wcscpy_s( buffer, buflen, L"float" );
             break;

        case ELEMENT_TYPE_R8:
             //wcscpy_s( buffer, buflen, L"System.Double" );
			 wcscpy_s( buffer, buflen, L"double" );
             break;

        case ELEMENT_TYPE_STRING:
             //wcscpy_s( buffer, buflen, L"System.String" );
			 wcscpy_s( buffer, buflen, L"string" );
             break;

        case ELEMENT_TYPE_PTR:
             //wcscpy_s( buffer, buflen, L"System.IntPtr" );
			 wcscpy_s( buffer, buflen, L"IntPtr" );
             break;

        case ELEMENT_TYPE_VALUETYPE:
             wcscpy_s( buffer, buflen, L"struct" );
             break;

        case ELEMENT_TYPE_CLASS:
             wcscpy_s( buffer, buflen, L"class" );
             break;

        case ELEMENT_TYPE_ARRAY:
             //wcscpy_s( buffer, buflen, L"System.Array" );
			 wcscpy_s( buffer, buflen, L"Array" );
             break;

        case ELEMENT_TYPE_I:
             wcscpy_s( buffer, buflen, L"INT_PTR" );
             break;

        case ELEMENT_TYPE_U:
             wcscpy_s( buffer, buflen, L"UINT_PTR" );
             break;

        case ELEMENT_TYPE_OBJECT:
             //wcscpy_s( buffer, buflen, L"System.Object" );
			 wcscpy_s( buffer, buflen, L"object" );
             break;

        case ELEMENT_TYPE_SZARRAY:
             //wcscpy_s( buffer, buflen, L"System.Array" );
			 wcscpy_s( buffer, buflen, L"Array" );
             break;

		case ELEMENT_TYPE_VOID:
			 wcscpy_s( buffer, buflen, L"void" );
             break;
			break;

		case ELEMENT_TYPE_FNPTR:
        case ELEMENT_TYPE_MAX:
        case ELEMENT_TYPE_END:
        case ELEMENT_TYPE_BYREF:
        case ELEMENT_TYPE_PINNED:
        case ELEMENT_TYPE_SENTINEL:
        case ELEMENT_TYPE_CMOD_OPT:
        case ELEMENT_TYPE_MODIFIER:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_TYPEDBYREF:
        default:
             wcscpy_s( buffer, buflen, L"?" );
             break;
    }

    return hr;
} // ProfilerCallback::_GetNameFromElementType

CorElementType ProfilerCallback::_GetElementTypeFromClassName(WCHAR *className)
{
	if(_wcsnicmp(className, L"System.Boolean", ARRAY_LEN(L"System.Boolean") - 1) == 0)
		return ELEMENT_TYPE_BOOLEAN;
	if(_wcsnicmp(className, L"System.Char", ARRAY_LEN(L"System.Char") - 1) == 0)
		return ELEMENT_TYPE_CHAR;
	if(_wcsnicmp(className, L"System.SByte", ARRAY_LEN(L"System.SByte") - 1) == 0)
		return ELEMENT_TYPE_I1;
	if(_wcsnicmp(className, L"System.Byte", ARRAY_LEN(L"System.Byte") - 1) == 0)
		return ELEMENT_TYPE_U1;
	if(_wcsnicmp(className, L"System.Int16", ARRAY_LEN(L"System.Int16") - 1) == 0)
		return ELEMENT_TYPE_I2;
	if(_wcsnicmp(className, L"System.UInt16", ARRAY_LEN(L"System.UInt16") - 1) == 0)
		return ELEMENT_TYPE_U2;
	if(_wcsnicmp(className, L"System.Int32", ARRAY_LEN(L"System.Int32") - 1) == 0)
		return ELEMENT_TYPE_I4;
	if(_wcsnicmp(className, L"System.UInt32", ARRAY_LEN(L"System.UInt32") - 1) == 0)
		return ELEMENT_TYPE_U4;
	if(_wcsnicmp(className, L"System.Int64", ARRAY_LEN(L"System.Int64") - 1) == 0)
		return ELEMENT_TYPE_I8;
	if(_wcsnicmp(className, L"System.UInt64", ARRAY_LEN(L"System.UInt64") - 1) == 0)
		return ELEMENT_TYPE_U8;
	if(_wcsnicmp(className, L"System.Single", ARRAY_LEN(L"System.Single") - 1) == 0)
		return ELEMENT_TYPE_R4;
	if(_wcsnicmp(className, L"System.Double", ARRAY_LEN(L"System.Double") - 1) == 0)
		return ELEMENT_TYPE_I8;
	if(_wcsnicmp(className, L"System.String", ARRAY_LEN(L"System.String") - 1) == 0)
		return ELEMENT_TYPE_STRING;
	if(_wcsnicmp(className, L"System.IntPtr", ARRAY_LEN(L"System.IntPtr") - 1) == 0)
		return ELEMENT_TYPE_PTR;
	return ELEMENT_TYPE_END;
	/*
		case ELEMENT_TYPE_ARRAY;
        case ELEMENT_TYPE_VALUETYPE:
        case ELEMENT_TYPE_CLASS:
        case ELEMENT_TYPE_I:
        case ELEMENT_TYPE_U:
        case ELEMENT_TYPE_OBJECT:
        case ELEMENT_TYPE_SZARRAY:
		case ELEMENT_TYPE_VOID:
		case ELEMENT_TYPE_FNPTR:
        case ELEMENT_TYPE_MAX:
        case ELEMENT_TYPE_END:
        case ELEMENT_TYPE_BYREF:
        case ELEMENT_TYPE_PINNED:
        case ELEMENT_TYPE_SENTINEL:
        case ELEMENT_TYPE_CMOD_OPT:
        case ELEMENT_TYPE_MODIFIER:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_TYPEDBYREF:        
	*/
} // ProfilerCallback::_GetElementTypeFromClassName


/* public */
bool ProfilerCallback::LogEnabled()
{
	if(m_status_path == NULL) return false;
	DWORD sz = GetFileAttributesA(m_status_path);
	return  sz != 0xFFFFFFFF; 
}

void ProfilerCallback::LogToAnyInternal(const char *format, ... )
{
	if(m_stream == NULL) return;
	///////////////////////////////////////////////////////////////////////////
	Synchronize guard( m_criticalSection );
	///////////////////////////////////////////////////////////////////////////  
	{
		va_list args;
		va_start( args, format );
		vfprintf( m_stream, format, args );
	}
}

void ProfilerCallback::LogToAny( const char *format, ... )
{
	if(m_stream == NULL) return;
	if(!LogEnabled()) return;

	///////////////////////////////////////////////////////////////////////////
	Synchronize guard( m_criticalSection );
	///////////////////////////////////////////////////////////////////////////  
	{
		va_list args;
		va_start( args, format );
		vfprintf( m_stream, format, args );
	}

} // ProfilerCallback::LogToAny

/* public */
void ProfilerCallback::_ThreadStubWrapper()
{
    m_pGCHost = GetGCHost();
    //
    // loop and listen for a ForceGC event
    //
    while( TRUE )
    {
        DWORD dwResult;
        
        
        //
        // wait until someone signals an event from the GUI or the profiler
        //
        dwResult = WaitForMultipleObjects( SENTINEL_HANDLE, m_hArray, FALSE, INFINITE );
        if ( dwResult >= WAIT_OBJECT_0 && dwResult < WAIT_OBJECT_0 + SENTINEL_HANDLE)
        {
            ///////////////////////////////////////////////////////////////////////////
            Synchronize guard( g_criticalSection );
            ///////////////////////////////////////////////////////////////////////////  

            //
            // reset the event
            //
            ObjHandles type = (ObjHandles)(dwResult - WAIT_OBJECT_0);

            ResetEvent( m_hArray[type] );

            //
            // FALSE: indicates a ForceGC event arriving from the GUI
            // TRUE: indicates that the thread has to terminate
            // in both cases you need to send to the GUI an event to let it know
            // what the deal is
            //
            if ( m_bShutdown == FALSE )
            {
                //
                // what type do you have ?
                //
                switch( type )
                {
                    case GC_HANDLE:
                        //
                        // force the GC and do not worry about the result
                        //
                        if ( m_pProfilerInfo != NULL )
                        {
                            // dump the GC info on the next GC
                            m_pProfilerInfo->ForceGC();
                        }
                        break;
                    
                    case TRIGGER_GC_HANDLE:
                        //
                        // force the GC and do not worry about the result
                        //
                        if ( m_pProfilerInfo != NULL )
                        {
                            m_pProfilerInfo->ForceGC();
                        }
                        break;
                    
                    case OBJ_HANDLE:
                        //
                        // you need to update the set event mask, given the previous state
                        //
                        if ( m_pProfilerInfo != NULL )
                        {
                            // flush the log file
                            fflush(m_stream);
                        }                       
                        break;
                    
                    case CALL_HANDLE:
                        {
                            // flush the log file
                            fflush(m_stream);
                        }
                        break;
                    
                    
                    default:
                        TEXT_OUTLN( !"Valid Option" );
                }
            }
            else
            {
                //
                // Terminate
                //                
                break;
            }

        }
        else
        {
            Failure( " WaitForSingleObject TimedOut " );
            break;
        } 
    }

} // ProfilerCallback::_ThreadStubWrapper

void ProfilerCallback::Failure( const char *message )
{
    if ( message == NULL )     
        message = "\n\n**** SEVERE FAILURE: TURNING OFF APPLICABLE PROFILING EVENTS ****\n\n";     
    
    //
    // Display the error message and discontinue monitoring CLR events, except the 
    // IMMUTABLE ones. Turning off the IMMUTABLE events can cause crashes. The only
    // place that we can safely enable or disable immutable events is the Initialize
    // callback.
    //
    LogToAny( message );
	LogToAny("\n");
    m_pProfilerInfo->SetEventMask( (m_dwEventMask & (DWORD)COR_PRF_MONITOR_IMMUTABLE) );    
                        
} // ProfilerCallback::Failure

/*--------------------------------------------------------------------------------------------------*/

/* public */
/* throws BaseException */
void ProfilerCallback::AddModule( ModuleID moduleID )
{    
    if ( moduleID != NULL )
    {
        ModuleInfo *pModuleInfo;        

        pModuleInfo = m_pModuleTable[moduleID];
        if ( pModuleInfo == NULL )
        {
            pModuleInfo = new ModuleInfo( m_pProfilerInfo2, moduleID );
            if ( pModuleInfo != NULL )
            {
				try
				{
					ULONG dummy;					                
					HRESULT hr = m_pProfilerInfo->GetModuleInfo( moduleID,
														 &pModuleInfo->m_loadAddress,
														 MAX_LENGTH,
														 &dummy, 
														 pModuleInfo->m_name,
														 NULL );

					if(SUCCEEDED(hr))
						m_pModuleTable[moduleID] = pModuleInfo;
					else
						delete pModuleInfo;
                }
                catch(BaseException *exception)
                {
                    delete pModuleInfo;
                    throw;            
                }
            }        
            else
                _THROW_EXCEPTION( "Allocation for ModuleInfo Object FAILED" )
        }         
    }
    else
        _THROW_EXCEPTION( "ModuleID is NULL" )
          
} // ProfilerCallback::AddFunction

/* public */
/* throws BaseException */
void ProfilerCallback::RemoveModule( ModuleID moduleID )
{    
    if ( moduleID != NULL )
    {
        ModuleInfo *pModuleInfo;        

        pModuleInfo = m_pModuleTable[moduleID];
		if ( pModuleInfo != NULL ) {
            m_pModuleTable[moduleID] = NULL; 
			delete pModuleInfo;
		}
        else
            _THROW_EXCEPTION( "Module was not found in the Module Table" )
    }
    else
        _THROW_EXCEPTION( "ModuleID is NULL" )
            
} // ProfilerCallback::RemoveFunction

/* public */
/* throws BaseException */
void ProfilerCallback::AddFunction( FunctionID functionID )
{    
    if ( functionID != NULL )
    {
        FunctionInfo *pFunctionInfo;        

        pFunctionInfo = m_pFunctionTable[functionID];
        if ( pFunctionInfo == NULL )
        {
            pFunctionInfo = new FunctionInfo( functionID );
            if ( pFunctionInfo != NULL )
            {
                try
                {
                    HRESULT hr = _GetFunctionInfo( &pFunctionInfo );
					if(SUCCEEDED(hr))
						m_pFunctionTable[functionID] = pFunctionInfo;
					else
						delete pFunctionInfo;
                }
                catch(BaseException *exception)
                {
                    delete pFunctionInfo;
                    throw;            
                }
            }        
            else
                _THROW_EXCEPTION( "Allocation for FunctionInfo Object FAILED" )
        }         
    }
    else
        _THROW_EXCEPTION( "FunctionID is NULL" )
          
} // ProfilerCallback::AddFunction

/* public */
/* throws BaseException */
void ProfilerCallback::RemoveFunction( FunctionID functionID )
{    
    if ( functionID != NULL )
    {
        FunctionInfo *pFunctionInfo;        

        pFunctionInfo = m_pFunctionTable[functionID];
		if ( pFunctionInfo != NULL ) {
            m_pFunctionTable[functionID] = NULL; 
			delete pFunctionInfo;
		}
        else
            _THROW_EXCEPTION( "Function was not found in the Function Table" )
    }
    else
        _THROW_EXCEPTION( "FunctionID is NULL" )
            
} // ProfilerCallback::RemoveFunction

/* public */
/* throws BaseException */
void ProfilerCallback::AddClass( ClassID classID )
{    
    if ( classID != NULL )
    {
        ClassInfo *pClassInfo;        
		HRESULT hr = S_OK;

        pClassInfo = m_pClassTable[classID];
        if ( pClassInfo == NULL )
        {
            pClassInfo = new ClassInfo( classID );
            if ( pClassInfo != NULL )
            {

            //
            // we have 2 cases
            // case 1: class is an array
            // case 2: class is a real class
            //
            ULONG rank = 0;
            CorElementType elementType;
            ClassID realClassID = NULL;
            WCHAR ranks[MAX_LENGTH];
            bool finalizable = false;

            // case 1 
            hr = m_pProfilerInfo->IsArrayClass( classID, &elementType, &realClassID, &rank );
            if ( hr == S_OK )
            {
                ClassID prevClassID;
                ranks[0] = '\0';
                do
                {
                    prevClassID = realClassID;
                    _snwprintf_s( ranks, ARRAY_LEN(ranks), ARRAY_LEN(ranks)-1, L"%s[]", ranks);
                    hr = m_pProfilerInfo->IsArrayClass( prevClassID, &elementType, &realClassID, &rank );
                    if ( (hr == S_FALSE) || (FAILED(hr)) || (realClassID == NULL) )
                    {
                        //
                        // before you break set the realClassID to the value that it was before the 
                        // last unsuccessful call
                        //
                        realClassID = prevClassID;
                        
                        break;
                    }
                }
                while ( TRUE );
                
                if ( SUCCEEDED( hr ) )
                {
					if(SUCCEEDED(GetNameFromClassIDEx(classID, pClassInfo->m_name, pClassInfo->m_moduleID, pClassInfo->m_classToken)))
					{
						m_pClassTable[classID] = pClassInfo;
						m_pClassNameTable[Hash(pClassInfo->m_name)] = pClassInfo;
						//LogToAny("DEBUG1: %S,%p,%p,%S\n", pClassInfo->m_name, classID, m_pClassNameTable[pClassInfo->m_name]->m_id, m_pClassNameTable[pClassInfo->m_name]->m_name);
					}else{
						delete pClassInfo;
					}				
                }
                else
                    Failure( "ERROR: Looping for Locating the ClassID FAILED" );
            }
            // case 2
            else if ( hr == S_FALSE )
            {
				if(SUCCEEDED(GetNameFromClassIDEx(classID, pClassInfo->m_name, pClassInfo->m_moduleID, pClassInfo->m_classToken)))
				{
					m_pClassTable[classID] = pClassInfo;
					m_pClassNameTable[Hash(pClassInfo->m_name)] = pClassInfo;
					//LogToAny("DEBUG2: %S,%p,%p,%S\n", pClassInfo->m_name, classID, m_pClassNameTable[pClassInfo->m_name]->m_id, m_pClassNameTable[pClassInfo->m_name]->m_name);
				}else{
					delete pClassInfo;
				}
            }
            else
                Failure( "ERROR: ICorProfilerInfo::IsArrayClass() FAILED" );
        }
        else
            Failure( "ERROR: Allocation for ClassInfo FAILED" );    

		}//end of pClassInfo == NULL         
    }
	else
        _THROW_EXCEPTION( "ClassID is NULL" )
          
} // ProfilerCallback::AddFunction

/* public */
/* throws BaseException */
void ProfilerCallback::RemoveClass( ClassID classID )
{    
    if ( classID != NULL )
    {
        ClassInfo *pClassInfo;        

        pClassInfo = m_pClassTable[classID];
		if ( pClassInfo != NULL ) {
            m_pClassTable[classID] = NULL;
			m_pClassNameTable[Hash(pClassInfo->m_name)] = NULL;
			delete pClassInfo;
		}
        else
            _THROW_EXCEPTION( "Class was not found in the Class Table" )
    }
    else
        _THROW_EXCEPTION( "ClassID is NULL" )
            
} // ProfilerCallback::RemoveFunction

DECLSPEC
/* static public */
HRESULT ProfilerCallback::_GetFunctionInfo( FunctionInfo **ppFunctionInfo )
{
    HRESULT hr = E_FAIL; // assume success
	FunctionInfo *pFunctionInfo = (*ppFunctionInfo);
	FunctionID functionID = pFunctionInfo->m_id;
	WCHAR *functionName = pFunctionInfo->m_name;
	
    if ( functionID != NULL )
    {
        mdToken funcToken = mdTypeDefNil;
        IMetaDataImport *pMDImport = NULL;
		WCHAR funName[MAX_LENGTH] = L"UNKNOWN";
		//
        // Get the MetadataImport interface and the metadata token 
        //
        hr = m_pProfilerInfo->GetTokenAndMetaDataFromFunction( functionID, 
                                                               IID_IMetaDataImport, 
                                                               (IUnknown **)&pMDImport,
                                                               &funcToken );
        if ( SUCCEEDED( hr ) )
        {
            mdTypeDef classToken = mdTypeDefNil;
            DWORD methodAttr = 0;
            PCCOR_SIGNATURE sigBlob = NULL;
			ULONG sigBytes;

            hr = pMDImport->GetMethodProps( funcToken,
                                            &classToken,
                                            funName,
                                            MAX_LENGTH,
                                            0,
                                            &methodAttr,
                                            &sigBlob,
                                            &sigBytes,
                                            NULL, 
                                            NULL );

            if ( SUCCEEDED( hr ) )
            {
				// get class id
                WCHAR className[MAX_LENGTH] = L"UNKNOWN";
                ClassID classId = 0;

                if (m_pProfilerInfo2 != NULL)
                {
                    hr = m_pProfilerInfo2->GetFunctionInfo2(functionID,
                                                            0,
                                                            &classId,
															&(pFunctionInfo->m_moduleId),
															&(pFunctionInfo->m_methodDef),
                                                            0,
                                                            NULL,
                                                            NULL);
                    if (!SUCCEEDED(hr))
                        classId = 0;
                }

                if (classId == 0)
                {
                    hr = m_pProfilerInfo->GetFunctionInfo(functionID,
                                                        &classId,
														&(pFunctionInfo->m_moduleId),
														&(pFunctionInfo->m_methodDef));
                }

				//get class name
                if (SUCCEEDED(hr) && classId != 0)
                {
                    hr = GetNameFromClassID(classId, className);
                }
                else if (classToken != mdTypeDefNil)
                {
                    ULONG classGenericArgCount = 0;
                    hr = GetClassName(pMDImport, classToken, className, NULL, &classGenericArgCount);
                }
				
				_snwprintf_s( functionName, MAX_LENGTH, MAX_LENGTH-1, L"%s::%s", className, funName );

				//pFunctionInfo->m_moduleId and m_methodDef init after classID inited
				//so all functions use these fields need to put after classID initialization

				// calling convention, argument count and return type 
				ULONG callConv = IMAGE_CEE_CS_CALLCONV_MAX;

				sigBlob += CorSigUncompressData(sigBlob, &callConv);
				sigBlob += CorSigUncompressData(sigBlob, &(pFunctionInfo->m_argCount));

				ParameterInfo *pReturn = new ParameterInfo(0);
				pReturn->m_moduleId = pFunctionInfo->m_moduleId;

				pReturn->m_type = g_pCallbackObject->GetType(sigBlob, 
					pReturn->m_isByRef, 
					pReturn->m_typeDef, 
					pReturn->m_isArray);
				hr = g_pCallbackObject->GetTypeName(pReturn->m_typeName, MAX_LENGTH, 
					pReturn->m_type, 
					pReturn->m_typeDef, 
					pMDImport);
				if(SUCCEEDED(hr) && pReturn->m_isArray)
				  wcscat_s(pReturn->m_typeName, L"[]");

				pFunctionInfo->m_pReturnInfo = pReturn;

				// get parameters
				if(SUCCEEDED(hr) && m_traceParameter && pFunctionInfo->m_argCount>0) {					
					HCORENUM paramEnum = NULL;
					mdParamDef* paramDefs = new mdParamDef[pFunctionInfo->m_argCount];
					ULONG numParams = 0;
					hr = pMDImport->EnumParams(
					  &paramEnum, 
					  pFunctionInfo->m_methodDef, 
					  paramDefs, 
					  pFunctionInfo->m_argCount, &numParams);

					if(paramEnum)
					  pMDImport->CloseEnum(paramEnum);

					pFunctionInfo->m_ppParamInfo = new ParameterInfo*[pFunctionInfo->m_argCount];

					for(ULONG i = 0; 
						  sigBlob != NULL && i < pFunctionInfo->m_argCount;
						  i++)
					{
						ParameterInfo *pParamInfo = new ParameterInfo(paramDefs[i]);
						pParamInfo->m_moduleId = pFunctionInfo->m_moduleId;

						// get parameter name and attributes (for direction in/out/ref)
						HRESULT hr = pMDImport->GetParamProps(
							paramDefs[i], NULL, NULL, 
							pParamInfo->m_name, MAX_LENGTH, NULL, 
							&(pParamInfo->m_attributes), NULL, NULL, NULL);

						// get parameter type and type name
						if(SUCCEEDED(hr))
						{
							pParamInfo->m_type = GetType(sigBlob, 
								pParamInfo->m_isByRef, 
								pParamInfo->m_typeDef, 
								pParamInfo->m_isArray);
							hr = GetTypeName(pParamInfo->m_typeName, MAX_LENGTH, 
								pParamInfo->m_type, 
								pParamInfo->m_typeDef, 
								pMDImport);
							if(SUCCEEDED(hr) && pParamInfo->m_isArray)
							  wcscat_s(pParamInfo->m_typeName, L"[]");
						}

						pFunctionInfo->m_ppParamInfo[i] = pParamInfo;
					}

					if(paramDefs)
						delete [] paramDefs;
				}

			}
            pMDImport->Release();
        }
    } 
    //
    // This corresponds to an unmanaged frame
    //
    else
    {
        hr = S_OK;
    }
    
    return hr;

} // ProfilerCallback::GetFunctionProperties

static void StrAppend(__out_ecount(cchBuffer) char *buffer, const char *str, size_t cchBuffer)
{
    size_t bufLen = strlen(buffer) + 1;
    if (bufLen <= cchBuffer)
        strncat_s(buffer, cchBuffer, str, cchBuffer-bufLen);
}

/* public */
HRESULT ProfilerCallback::GetNameFromClassID( ClassID classID, WCHAR className[] )

{
	ModuleID moduleID;
	mdTypeDef classToken;
	return GetNameFromClassIDEx(classID, className, moduleID, classToken);
}

/* public */
HRESULT ProfilerCallback::GetNameFromClassIDEx( ClassID classID, WCHAR className[], ModuleID &moduleID, mdTypeDef &classToken )
{
    HRESULT hr = E_FAIL;
    
    if ( m_pProfilerInfo != NULL )
    {
		hr = m_pProfilerInfo->GetClassIDInfo( classID, 
                                              &moduleID,  
                                              &classToken );                                                                                                                                              
        if ( SUCCEEDED( hr ) )
        {             
            IMetaDataImport *pMDImport = NULL;                
            
            hr = m_pProfilerInfo->GetModuleMetaData( moduleID, 
                                                     (ofRead | ofWrite),
                                                     IID_IMetaDataImport, 
                                                     (IUnknown **)&pMDImport );
            if ( SUCCEEDED( hr ) )
            {
                if ( classToken != mdTypeDefNil )
                {
                    ClassID *classTypeArgs = NULL;
                    ULONG32 classTypeArgCount = 0;
#ifdef mdGenericPar
                    if (m_pProfilerInfo2 != NULL)
                    {
                        hr = m_pProfilerInfo2->GetClassIDInfo2(classID,
                                                               NULL,
                                                               NULL,
                                                               NULL,
                                                               0,
                                                               &classTypeArgCount,
                                                               NULL);
                        
                        if (SUCCEEDED(hr) && classTypeArgCount > 0)
                        {
                            classTypeArgs = (ClassID *)_alloca(classTypeArgCount*sizeof(classTypeArgs[0]));

                            hr = m_pProfilerInfo2->GetClassIDInfo2(classID,
                                                                   NULL,
                                                                   NULL,
                                                                   NULL,
                                                                   classTypeArgCount,
                                                                   &classTypeArgCount,
                                                                   classTypeArgs);
                        }
                        if (!SUCCEEDED(hr))
                            classTypeArgs = NULL;
                    }
#endif // mdGenericPar
                    DWORD dwTypeDefFlags = 0;
                    ULONG genericArgCount = 0;
                    hr = GetClassName(pMDImport, classToken, className, classTypeArgs, &genericArgCount);
                    if ( FAILED( hr ) )
                        Failure( "_GetClassNameHelper() FAILED" );
                }
                else
                    Failure( ("The class token is mdTypeDefNil, class does NOT have MetaData info") );

                pMDImport->Release ();
            }
            else
            {
//                Failure( "IProfilerInfo::GetModuleMetaData() => IMetaDataImport FAILED" );
                wcscpy_s(className, MAX_LENGTH, L"???");
                hr = S_OK;
            }
        }
        else    
            Failure( "ICorProfilerInfo::GetClassIDInfo() FAILED" );
    }
    else
        Failure( "ICorProfilerInfo Interface has NOT been Initialized" );

    return hr;

} // ProfilerCallback::GetNameFromClassIDEx

DECLSPEC
/* static public */
HRESULT ProfilerCallback::GetClassName(IMetaDataImport *pMDImport, mdToken classToken, WCHAR className[], ClassID *classTypeArgs, ULONG *totalGenericArgCount)
{
    DWORD dwTypeDefFlags = 0;
    HRESULT hr = S_OK;
    hr = pMDImport->GetTypeDefProps( classToken, 
                                     className, 
                                     MAX_LENGTH,
                                     NULL, 
                                     &dwTypeDefFlags, 
                                     NULL ); 
    if ( FAILED( hr ) )
    {
        return hr;
    }
    *totalGenericArgCount = 0;
    if (IsTdNested(dwTypeDefFlags))
    {
//      printf("%S is a nested class\n", className);
        mdToken enclosingClass = mdTokenNil;
        hr = pMDImport->GetNestedClassProps(classToken, &enclosingClass);
        if ( FAILED( hr ) )
        {
            return hr;
        }
//      printf("Enclosing class for %S is %d\n", className, enclosingClass);
        hr = GetClassName(pMDImport, enclosingClass, className, classTypeArgs, totalGenericArgCount);
//      printf("Enclosing class name %S\n", className);
        if (FAILED(hr))
            return hr;
        size_t length = wcslen(className);
        if (length + 2 < MAX_LENGTH)
        {
            className[length++] = '.';
            hr = pMDImport->GetTypeDefProps( classToken, 
                                            className + length, 
                                            (ULONG)(MAX_LENGTH - length),
                                            NULL, 
                                            NULL, 
                                            NULL );
            if ( FAILED( hr ) )
            {
                return hr;
            }
//          printf("%S is a nested class\n", className);
        }
    }

    WCHAR *backTick = wcschr(className, L'`');
    if (backTick != NULL)
    {
        *backTick = L'\0';
        ULONG genericArgCount = wcstoul(backTick+1, NULL, 10);

        if (genericArgCount >0)
        {
            char typeArgText[MAX_LENGTH];
            typeArgText[0] = '\0';

            StrAppend(typeArgText, "<", MAX_LENGTH);
            for (ULONG i = *totalGenericArgCount; i < *totalGenericArgCount + genericArgCount; i++)
            {
                if (i != *totalGenericArgCount)
                    StrAppend(typeArgText, ",", MAX_LENGTH);
                AppendTypeArgName(i, classTypeArgs, NULL, FALSE, typeArgText, MAX_LENGTH);
            }
            StrAppend(typeArgText, ">", MAX_LENGTH);

            *totalGenericArgCount += genericArgCount;
    
            _snwprintf_s(className, MAX_LENGTH, MAX_LENGTH-1, L"%s%S", className, typeArgText);
        }
    }
    return hr;
}

void ProfilerCallback::AppendTypeArgName(ULONG argIndex, ClassID *actualClassTypeArgs, ClassID *actualMethodTypeArgs, BOOL methodFormalArg, __out_ecount(cchBuffer) char *buffer, size_t cchBuffer)
{
    char argName[MAX_LENGTH];

    argName[0] = '\0';

    ClassID classId = 0;
    if (methodFormalArg && actualMethodTypeArgs != NULL)
        classId = actualMethodTypeArgs[argIndex];
    if (!methodFormalArg && actualClassTypeArgs != NULL)
        classId = actualClassTypeArgs[argIndex];

    if (classId != 0)
    {
        WCHAR className[MAX_LENGTH];

        HRESULT hr = GetNameFromClassID(classId, className);
        if (SUCCEEDED(hr))
            _snprintf_s( argName, ARRAY_LEN(argName), ARRAY_LEN(argName)-1, "%S", className);
    }

    if (argName[0] == '\0')
    {
        char argStart = methodFormalArg ? 'M' : 'T';
        if (argIndex <= 6)
        {
            // the first 7 parameters are printed as M, N, O, P, Q, R, S 
            // or as T, U, V, W, X, Y, Z 
            sprintf_s( argName, ARRAY_LEN(argName), "%c", argIndex + argStart);
        }
        else
        {
            // everything after that as M7, M8, ... or T7, T8, ...
            sprintf_s( argName, ARRAY_LEN(argName), "%c%u", argStart, argIndex);
        }
    }

    StrAppend( buffer, argName, cchBuffer);
}

long ProfilerCallback::Hash(PWCHAR str)
{
	const char *locName = "English_America";
   locale loc ( locName, LC_ALL );   
   size_t len = wcslen(str);
	
   long r = use_facet< collate<wchar_t> > ( loc ).
	   hash (str, &str[len-1 ]);
   return r;
}
/***************************************************************************************
 ********************                                               ********************
 ********************              DllMain/ClassFactory             ********************
 ********************                                               ********************
 ***************************************************************************************/ 
#include "dllmain.hpp"

// End of File

