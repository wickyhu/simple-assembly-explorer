// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
#include "ProfilerInfo.h"

/***************************************************************************************
 ********************                                               ********************
 ********************             BaseInfo Implementation           ********************
 ********************                                               ********************
 ***************************************************************************************/

/* public */
BaseInfo::BaseInfo( SIZE_T id) : 
    m_id( id )
{   
    wcscpy_s( m_name, ARRAY_LEN(m_name), L"UNKNOWN" );
} // ctor         


/* public virtual */
BaseInfo::~BaseInfo()
{       
} // dtor

/* public */
template<typename T>
BOOL BaseInfo::Compare( T in_key )
{
    SIZE_T key = (SIZE_T)in_key;

    return (BOOL)(m_id == key);
    
} // BaseInfo::Compare

/* public */
template<typename T>
Comparison BaseInfo::CompareEx( T in_key )
{
    Comparison res = EQUAL_TO;
    SIZE_T key = (SIZE_T)in_key;

    if ( key > m_id )
        res =  GREATER_THAN;
    
    else if ( key < m_id )
        res = LESS_THAN;


    return res;

} // BaseInfo::CompareEx

/***************************************************************************************
 ********************                                               ********************
 ********************          FunctionInfo Implementation          ********************
 ********************                                               ********************
 ***************************************************************************************/

/* public */
FunctionInfo::FunctionInfo( FunctionID functionID) : 
    BaseInfo( functionID )    
{
   
} // ctor         

/* public virtual */
FunctionInfo::~FunctionInfo()
{  
	if(m_pReturnInfo != NULL) {
		delete m_pReturnInfo;
	}
	if(m_ppParamInfo != NULL) {
		delete [] m_ppParamInfo;
	}
} // dtor

/***************************************************************************************
 ********************                                               ********************
 ********************          ParameterInfo Implementation          ********************
 ********************                                               ********************
 ***************************************************************************************/

/* public */
ParameterInfo::ParameterInfo( mdParamDef paramDef ) 
{
    m_def = paramDef;	
} // ctor         

/* public virtual */
ParameterInfo::~ParameterInfo()
{  
} // dtor

/***************************************************************************************
 ********************                                               ********************
 ********************            ClassInfo Implementation           ********************
 ********************                                               ********************
 ***************************************************************************************/

/* public */
ClassInfo::ClassInfo( ClassID classID ) : 
    BaseInfo( classID ),
	m_numFields(0),
	m_fieldDefs(NULL)
{
} // ctor         

/* public virtual */
ClassInfo::~ClassInfo()
{
	if(m_fieldDefs != NULL) 
		delete [] m_fieldDefs;
} // dtor

/* public */
bool ClassInfo::IsValidField(IMetaDataImport *mdi, mdFieldDef fieldDef) {
	if(m_fieldDefs == NULL) {
		//enum fields	
		HCORENUM fieldEnum = NULL;
  
		HRESULT hr = mdi->EnumFields(
		  &fieldEnum, 
		  m_classToken, 
		  NULL, 
		  0, &m_numFields);
		hr = mdi->CountEnum(fieldEnum, &m_numFields);
		m_fieldDefs = new mdFieldDef[m_numFields];
		hr = mdi->EnumFields(
		  &fieldEnum, 
		  m_classToken, 
		  m_fieldDefs, 
		  m_numFields, &m_numFields);

		if(fieldEnum)
		  mdi->CloseEnum(fieldEnum);
	}
	if(m_fieldDefs == NULL) return false;

	bool found = false;
	for(ULONG i=0; i<m_numFields; i++) {
		if(m_fieldDefs[i] == fieldDef) {
			found = true;
			break;
		}
	}
	return found;
}

/***************************************************************************************
 ********************                                               ********************
 ********************            ModuleInfo Implementation           ********************
 ********************                                               ********************
 ***************************************************************************************/

/* public */
ModuleInfo::ModuleInfo( ICorProfilerInfo2 *cpi, ModuleID moduleID ) : 
    BaseInfo( moduleID ),
	m_mdi(NULL),
	m_cpi(cpi),
	m_loadAddress(0)
{
} // ctor         

/* public virtual */
ModuleInfo::~ModuleInfo()
{
	if(m_mdi != NULL) 
		delete m_mdi;
} // dtor

/* public */
IMetaDataImport * ModuleInfo::GetMDI() 
{
	if(m_mdi == NULL) {
		HRESULT hr = m_cpi->GetModuleMetaData(m_id, ofRead, IID_IMetaDataImport, (IUnknown **)&m_mdi);
		if(!SUCCEEDED(hr)) {
			m_mdi = NULL;
		}
	}
	return m_mdi;
}

/***************************************************************************************
 ********************                                               ********************
 ********************              PrfInfo Implementation           ********************
 ********************                                               ********************
 ***************************************************************************************/

/* public */
PrfInfo::PrfInfo() :
    m_dwEventMask( 0 ),
    m_pProfilerInfo( NULL ),
	m_pProfilerInfo2( NULL )
{

} // ctor


/* virtual public */
PrfInfo::~PrfInfo()
{
    if ( m_pProfilerInfo != NULL )
        m_pProfilerInfo->Release();        
       
    if ( m_pProfilerInfo2 != NULL )
        m_pProfilerInfo2->Release();     
       
} // dtor 


void PrfInfo::Failure( const char *message )
{
    if ( message == NULL )     
        message = "\n\n**** SEVERE FAILURE: TURNING OFF APPLICABLE PROFILING EVENTS ****\n\n";  
    //
    // Display the error message and discontinue monitoring CLR events, except the 
    // IMMUTABLE ones. Turning off the IMMUTABLE events can cause crashes. The only
    // place that we can safely enable or disable immutable events is the Initialize
    // callback.
    //
    TEXT_OUTLN( message );
    m_pProfilerInfo->SetEventMask( (m_dwEventMask & (DWORD)COR_PRF_MONITOR_IMMUTABLE) );    
                        
} // PrfInfo::Failure

// end of file
