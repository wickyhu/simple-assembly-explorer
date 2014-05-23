// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/****************************************************************************************
 * File:
 *  ProfilerCallback.h
 *
 * Description:
 *  
 *
 *
 ***************************************************************************************/
#ifndef __PROFILER_CALLBACK_H__
#define __PROFILER_CALLBACK_H__

#include "mscoree.h"
#include "ProfilerInfo.h"


/////////////////////////////////////////////////////////////////////////////////////////
// Each test should provide the following blob (with a new GUID)
//
	// {C9326B03-E51D-43a3-9394-9B8ECCDBAD9B}
    extern const GUID __declspec( selectany ) CLSID_PROFILER = 
	{ 0xc9326b03, 0xe51d, 0x43a3, { 0x93, 0x94, 0x9b, 0x8e, 0xcc, 0xdb, 0xad, 0x9b } };

    #define THREADING_MODEL "Both"
    #define PROGID_PREFIX "SimpleProfiler"
	#define COCLASS_DESCRIPTION "Simple Profiler by WiCKY Hu, http://simpledotnet.googlepages.com"
    #define PROFILER_GUID "{C9326B03-E51D-43a3-9394-9B8ECCDBAD9B}"
//
/////////////////////////////////////////////////////////////////////////////////////////


/***************************************************************************************
 ********************                                               ********************
 ********************       ProfilerCallback Declaration            ********************
 ********************                                               ********************
 ***************************************************************************************/

class ProfilerCallback : 
    public PrfInfo,
    public ICorProfilerCallback2 
{
    public:
    
        ProfilerCallback();
        ~ProfilerCallback();

		//tables
		stdext::hash_map<FunctionID, FunctionInfo*> m_pFunctionTable;
		stdext::hash_map<ClassID, ClassInfo*> m_pClassTable;
		stdext::hash_map<long, ClassInfo*> m_pClassNameTable;
		stdext::hash_map<ModuleID, ModuleInfo*> m_pModuleTable;

	protected:
		HRESULT _GetFunctionInfo(FunctionInfo **ppFunctionInfo );
		HRESULT GetClassName(IMetaDataImport *pMDImport, mdToken classToken, WCHAR className[], ClassID *classTypeArgs, ULONG *totalGenericArgCount);
		void AppendTypeArgName(ULONG argIndex, ClassID *actualClassTypeArgs, ClassID *actualMethodTypeArgs, BOOL methodFormalArg, __out_ecount(cchBuffer) char *buffer, size_t cchBuffer);
		long Hash(PWCHAR str);

	public:
        void Failure( const char *message = NULL );

        void AddModule(ModuleID moduleID ); 
        void RemoveModule( ModuleID moduleID ); 

        void AddFunction( FunctionID functionID ); 
        void RemoveFunction( FunctionID functionID ); 

        void AddClass( ClassID classID ); 
        void RemoveClass( ClassID classID ); 

		HRESULT GetNameFromClassID( ClassID classID, WCHAR className[] );
		HRESULT ProfilerCallback::GetNameFromClassIDEx( ClassID classID, WCHAR className[], ModuleID &moduleID, mdTypeDef &classToken );

        //
        // IUnknown 
        //
        COM_METHOD( ULONG ) AddRef(); 
        COM_METHOD( ULONG ) Release();
        COM_METHOD( HRESULT ) QueryInterface( REFIID riid, void **ppInterface );


        //
        // STARTUP/SHUTDOWN EVENTS
        //
        virtual COM_METHOD( HRESULT ) Initialize( IUnknown *pICorProfilerInfoUnk );
               
        HRESULT DllDetachShutdown();                           
        COM_METHOD( HRESULT ) Shutdown();
                                         

        //
        // APPLICATION DOMAIN EVENTS
        //
        COM_METHOD( HRESULT ) AppDomainCreationStarted( AppDomainID appDomainID );
        
        COM_METHOD( HRESULT ) AppDomainCreationFinished( AppDomainID appDomainID,
                                                         HRESULT hrStatus );
    
        COM_METHOD( HRESULT ) AppDomainShutdownStarted( AppDomainID appDomainID );

        COM_METHOD( HRESULT ) AppDomainShutdownFinished( AppDomainID appDomainID, 
                                                         HRESULT hrStatus );


        //
        // ASSEMBLY EVENTS
        //
        COM_METHOD( HRESULT ) AssemblyLoadStarted( AssemblyID assemblyID );
        
        COM_METHOD( HRESULT ) AssemblyLoadFinished( AssemblyID assemblyID,
                                                    HRESULT hrStatus );
    
        COM_METHOD( HRESULT ) AssemblyUnloadStarted( AssemblyID assemblyID );

        COM_METHOD( HRESULT ) AssemblyUnloadFinished( AssemblyID assemblyID, 
                                                      HRESULT hrStatus );
        
        
        //
        // MODULE EVENTS
        //
        COM_METHOD( HRESULT ) ModuleLoadStarted( ModuleID moduleID );
        
        COM_METHOD( HRESULT ) ModuleLoadFinished( ModuleID moduleID,
                                                  HRESULT hrStatus );
    
        COM_METHOD( HRESULT ) ModuleUnloadStarted( ModuleID moduleID );

        COM_METHOD( HRESULT ) ModuleUnloadFinished( ModuleID moduleID, 
                                                    HRESULT hrStatus );

        COM_METHOD( HRESULT ) ModuleAttachedToAssembly( ModuleID moduleID,
                                                        AssemblyID assemblyID );
                
        
        //
        // CLASS EVENTS
        //
        COM_METHOD( HRESULT ) ClassLoadStarted( ClassID classID );
        
        COM_METHOD( HRESULT ) ClassLoadFinished( ClassID classID,
                                                 HRESULT hrStatus );
    
        COM_METHOD( HRESULT ) ClassUnloadStarted( ClassID classID );

        COM_METHOD( HRESULT ) ClassUnloadFinished( ClassID classID, 
                                                   HRESULT hrStatus );

        COM_METHOD( HRESULT ) FunctionUnloadStarted( FunctionID functionID );
        
        
        //
        // JIT EVENTS
        //              
        COM_METHOD( HRESULT ) JITCompilationStarted( FunctionID functionID,
                                                     BOOL fIsSafeToBlock );
                                        
        COM_METHOD( HRESULT ) JITCompilationFinished( FunctionID functionID,
                                                      HRESULT hrStatus,
                                                      BOOL fIsSafeToBlock );
    
        COM_METHOD( HRESULT ) JITCachedFunctionSearchStarted( FunctionID functionID,
                                                              BOOL *pbUseCachedFunction );
        
        COM_METHOD( HRESULT ) JITCachedFunctionSearchFinished( FunctionID functionID,
                                                               COR_PRF_JIT_CACHE result );
                                                                     
        COM_METHOD( HRESULT ) JITFunctionPitched( FunctionID functionID );
        
        COM_METHOD( HRESULT ) JITInlining( FunctionID callerID,
                                           FunctionID calleeID,
                                           BOOL *pfShouldInline );

        
        //
        // THREAD EVENTS
        //
        COM_METHOD( HRESULT ) ThreadCreated( ThreadID threadID );
    
        COM_METHOD( HRESULT ) ThreadDestroyed( ThreadID threadID );

        COM_METHOD( HRESULT ) ThreadAssignedToOSThread( ThreadID managedThreadID,
                                                        DWORD osThreadID );
        //
        // REMOTING EVENTS
        //                                                      

        //
        // Client-side events
        //
        COM_METHOD( HRESULT ) RemotingClientInvocationStarted();

        COM_METHOD( HRESULT ) RemotingClientSendingMessage( GUID *pCookie,
                                                            BOOL fIsAsync );

        COM_METHOD( HRESULT ) RemotingClientReceivingReply( GUID *pCookie,
                                                            BOOL fIsAsync );

        COM_METHOD( HRESULT ) RemotingClientInvocationFinished();

        //
        // Server-side events
        //
        COM_METHOD( HRESULT ) RemotingServerReceivingMessage( GUID *pCookie,
                                                              BOOL fIsAsync );

        COM_METHOD( HRESULT ) RemotingServerInvocationStarted();

        COM_METHOD( HRESULT ) RemotingServerInvocationReturned();

        COM_METHOD( HRESULT ) RemotingServerSendingReply( GUID *pCookie,
                                                          BOOL fIsAsync );


        //
        // CONTEXT EVENTS
        //                                                      
        COM_METHOD( HRESULT ) UnmanagedToManagedTransition( FunctionID functionID,
                                                            COR_PRF_TRANSITION_REASON reason );
    
        COM_METHOD( HRESULT ) ManagedToUnmanagedTransition( FunctionID functionID,
                                                            COR_PRF_TRANSITION_REASON reason );
                                                                  
                                                                        
        //
        // SUSPENSION EVENTS
        //    
        COM_METHOD( HRESULT ) RuntimeSuspendStarted( COR_PRF_SUSPEND_REASON suspendReason );

        COM_METHOD( HRESULT ) RuntimeSuspendFinished();

        COM_METHOD( HRESULT ) RuntimeSuspendAborted();

        COM_METHOD( HRESULT ) RuntimeResumeStarted();

        COM_METHOD( HRESULT ) RuntimeResumeFinished();

        COM_METHOD( HRESULT ) RuntimeThreadSuspended( ThreadID threadid );

        COM_METHOD( HRESULT ) RuntimeThreadResumed( ThreadID threadid );


        //
        // GC EVENTS
        //    
        COM_METHOD( HRESULT ) MovedReferences( ULONG cmovedObjectIDRanges,
                                               ObjectID oldObjectIDRangeStart[],
                                               ObjectID newObjectIDRangeStart[],
                                               ULONG cObjectIDRangeLength[] );
    
        COM_METHOD( HRESULT ) SurvivingReferences( ULONG cmovedObjectIDRanges,
                                                   ObjectID objectIDRangeStart[],
                                                   ULONG cObjectIDRangeLength[] );

        COM_METHOD( HRESULT ) ObjectAllocated( ObjectID objectID,
                                               ClassID classID );
    
        COM_METHOD( HRESULT ) ObjectsAllocatedByClass( ULONG classCount,
                                                       ClassID classIDs[],
                                                       ULONG objects[] );
    
        COM_METHOD( HRESULT ) ObjectReferences( ObjectID objectID,
                                                ClassID classID,
                                                ULONG cObjectRefs,
                                                ObjectID objectRefIDs[] );
    
        COM_METHOD( HRESULT ) RootReferences( ULONG cRootRefs,
                                              ObjectID rootRefIDs[] );
    
        
        //
        // EXCEPTION EVENTS
        //                                                         

        // Exception creation
        COM_METHOD( HRESULT ) ExceptionThrown( ObjectID thrownObjectID );

        // Search phase
        COM_METHOD( HRESULT ) ExceptionSearchFunctionEnter( FunctionID functionID );
    
        COM_METHOD( HRESULT ) ExceptionSearchFunctionLeave();
    
        COM_METHOD( HRESULT ) ExceptionSearchFilterEnter( FunctionID functionID );
    
        COM_METHOD( HRESULT ) ExceptionSearchFilterLeave();
    
        COM_METHOD( HRESULT ) ExceptionSearchCatcherFound( FunctionID functionID );
        
        COM_METHOD( HRESULT ) ExceptionCLRCatcherFound();

        COM_METHOD( HRESULT ) ExceptionCLRCatcherExecute();

        COM_METHOD( HRESULT ) ExceptionOSHandlerEnter( FunctionID functionID );
            
        COM_METHOD( HRESULT ) ExceptionOSHandlerLeave( FunctionID functionID );
    
        // Unwind phase
        COM_METHOD( HRESULT ) ExceptionUnwindFunctionEnter( FunctionID functionID );
    
        COM_METHOD( HRESULT ) ExceptionUnwindFunctionLeave();
        
        COM_METHOD( HRESULT ) ExceptionUnwindFinallyEnter( FunctionID functionID );
    
        COM_METHOD( HRESULT ) ExceptionUnwindFinallyLeave();
        
        COM_METHOD( HRESULT ) ExceptionCatcherEnter( FunctionID functionID,
                                                     ObjectID objectID );
    
        COM_METHOD( HRESULT ) ExceptionCatcherLeave();

        
        //
        // COM CLASSIC WRAPPER
        //
        COM_METHOD( HRESULT )  COMClassicVTableCreated( ClassID wrappedClassID,
                                                        REFGUID implementedIID,
                                                        void *pVTable,
                                                        ULONG cSlots );

        COM_METHOD( HRESULT )  COMClassicVTableDestroyed( ClassID wrappedClassID,
                                                          REFGUID implementedIID,
                                                          void *pVTable );

        COM_METHOD( HRESULT ) STDMETHODCALLTYPE ThreadNameChanged( 
            /* [in] */ ThreadID threadId,
            /* [in] */ ULONG cchName,
            /* [in] */ WCHAR name[  ]);
    
        /*
         * The CLR calls GarbageCollectionStarted before beginning a 
         * garbage collection. All GC callbacks pertaining to this
         * collection will occur between the GarbageCollectionStarted
         * callback and the corresponding GarbageCollectionFinished
         * callback, which will occur on the same thread.
         *
         *          cGenerations indicates the total number of entries in
         *                the generationCollected array
         *          generationCollected is an array of booleans, indexed
         *                by COR_PRF_GC_GENERATIONS, indicating which
         *                generations are being collected in this collection
         *          wasInduced indicates whether this GC was induced
         *                by the application calling GC.Collect().
         */
        COM_METHOD( HRESULT )  GarbageCollectionStarted(
            /* [in] */int cGenerations,
            /*[in, size_is(cGenerations), length_is(cGenerations)]*/ BOOL generationCollected[],
            /*[in]*/ COR_PRF_GC_REASON reason);

        /*
         * The CLR calls GarbageCollectionFinished after a garbage
         * collection has completed and all GC callbacks have been
         * issued for it.
         */
        COM_METHOD( HRESULT )  GarbageCollectionFinished();

        /*
         * The CLR calls FinalizeableObjectQueued to notify the code profiler
         * that an object with a finalizer (destructor in C# parlance) has
         * just been queued to the finalizer thread for execution of its
         * Finalize method.
         *
         * finalizerFlags describes aspects of the finalizer, and takes its
         *     value from COR_PRF_FINALIZER_FLAGS.
         *
         */

        COM_METHOD( HRESULT ) STDMETHODCALLTYPE FinalizeableObjectQueued(
            /* [in] */DWORD finalizerFlags,
            /* [in] */ObjectID objectID);

        COM_METHOD( HRESULT ) STDMETHODCALLTYPE RootReferences2( 
            /* [in] */ ULONG cRootRefs,
            /* [size_is][in] */ ObjectID rootRefIds[  ],
            /* [size_is][in] */ COR_PRF_GC_ROOT_KIND rootKinds[  ],
            /* [size_is][in] */ COR_PRF_GC_ROOT_FLAGS rootFlags[  ],
            /* [size_is][in] */ UINT_PTR rootIds[  ]);

        /*
         * The CLR calls HandleCreated when a gc handle has been created.
         *
         */
        COM_METHOD( HRESULT ) STDMETHODCALLTYPE HandleCreated(
            /* [in] */ UINT_PTR handleId,
            /* [in] */ ObjectID initialObjectId);

        /*
         * The CLR calls HandleDestroyed when a gc handle has been destroyed.
         *
         */
        COM_METHOD( HRESULT ) STDMETHODCALLTYPE HandleDestroyed(
            /* [in] */ UINT_PTR handleId);

        //
        // instantiate an instance of the callback interface
        //
        static COM_METHOD( HRESULT) CreateObject( REFIID riid, void **ppInterface );            
        
                                                                                                     
        // used by function hooks, they have to be static
        static void  Enter( FunctionID functionID, 
                                    UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO frameInfo, 
                                     COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo );
        static void  Leave( FunctionID functionID, COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange );
        static void  Tailcall( FunctionID functionID );

        //
        // wrapper for the threads
        //
        void _ThreadStubWrapper( );

    private:

        void _ProcessEnvVariables();

		bool LogEnabled();
		void LogToAny( const char *format, ... );		
		void LogToAnyInternal( const char *format, ... );		

		int ExceptionFilter(unsigned int code, struct _EXCEPTION_POINTERS *ep);

        HRESULT _GetNameFromElementType( CorElementType elementType, __out_ecount(buflen) WCHAR *buffer, size_t buflen );
		CorElementType _GetElementTypeFromClassName(WCHAR *className);

		bool ProfilerCallback::_IsNeedToLog(WCHAR* pFilter);

		HRESULT TraceParameterList(
			FunctionInfo *pFunctionInfo,   
		  COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo,
		  ICorProfilerInfo2 *cpi,
		  IMetaDataImport *mdi);

		HRESULT TraceParameter(COR_PRF_FUNCTION_ARGUMENT_RANGE *range,
			ParameterInfo *pParameterInfo,
			ICorProfilerInfo2 *cpi,
			IMetaDataImport *mdi);

		HRESULT TraceValue(
		  ObjectID oid,
		  ICorProfilerInfo2 *cpi,
		  IMetaDataImport *mdiRef,
		  ParameterInfo *pParameterInfo);

		HRESULT TraceULong(
		  UINT_PTR startAddress);

		HRESULT TraceLong(
		  UINT_PTR startAddress);

		HRESULT TraceUInt(
		  UINT_PTR startAddress);

		HRESULT TraceInt(
		  UINT_PTR startAddress);

		HRESULT TraceUShort(
		  UINT_PTR startAddress);

		HRESULT TraceShort(
		  UINT_PTR startAddress);

		HRESULT TraceSByte(
		  UINT_PTR startAddress);

		HRESULT TraceByte(
		  UINT_PTR startAddress);

		HRESULT TraceChar(
		  UINT_PTR startAddress);

		HRESULT TraceBoolean(
		  UINT_PTR startAddress);

		HRESULT TraceFloat(
		  UINT_PTR startAddress);

		HRESULT TraceDouble(
		  UINT_PTR startAddress);

		HRESULT TraceString(
		  ObjectID oid, 
		  ICorProfilerInfo2 *cpi);

		HRESULT GetStructParamInfo(ICorProfilerInfo2 *cpi,  
		  IMetaDataImport **mdiRef,
		  ParameterInfo *pParameterInfo,
		  ClassID &classId,
		  mdTypeDef &typeDef);

		ULONG GetElementTypeSize( CorElementType elementType );

		HRESULT TraceStruct(
		  UINT_PTR startAddress,
		  ICorProfilerInfo2 *cpi,
		  IMetaDataImport *mdi,
		  ParameterInfo *pParameterInfo,
		  ULONG *pcSize,
		  ClassID classIdIn,
		  mdTypeDef typeDefIn );

		HRESULT TraceClass(
		  ObjectID oid,
		  ICorProfilerInfo2 *cpi,
		  IMetaDataImport *mdi,
		  ParameterInfo *pParameterInfo);

		HRESULT TraceObject(
		  UINT_PTR startAddress,
		  ICorProfilerInfo2 *cpi,
		  IMetaDataImport *mdi,
		  ParameterInfo *pParameterInfo);

		HRESULT TraceArray(
		  ObjectID oid,
		  ICorProfilerInfo2 *cpi,
		  IMetaDataImport *mdi,
		  ParameterInfo *pParameterInfo);

		CorElementType GetType(
		  PCCOR_SIGNATURE& sigBlob, 
		  bool &isByRef, 
		  mdTypeDef &typeDef,
		  bool &isArray);

		HRESULT GetTypeName(
		  PTCHAR name, 
		  ULONG size, 
		  CorElementType type, 
		  mdTypeDef typeDef,
		  IMetaDataImport *mdi);

    public:
    
    private:

        // various counters
        long m_refCount;                        
        DWORD m_dwShutdown;

        // operation indicators
        char *m_path;
		char *m_status_path;
        BOOL m_bShutdown;
        DWORD m_dwProcessId;
        WCHAR *m_filter;
		int m_filterLen;
        CRITICAL_SECTION m_criticalSection;

		BOOL m_includeSystem;
		BOOL m_traceParameter;
		BOOL m_traceEvent;
        
        // file stuff
        FILE *m_stream;

        HANDLE m_hArray[(DWORD)SENTINEL_HANDLE];

        // names for the events and the callbacks
        char m_logFileName[MAX_LENGTH+1];

        // IGCHost callback
        IGCHost *m_pGCHost;
		//BOOL m_bDumpGCInfo;
        //bool m_SuspendForGC;
                
}; // ProfilerCallback

extern ProfilerCallback *g_pCallbackObject;     // global reference to callback object
CRITICAL_SECTION g_criticalSection;

#endif //  __PROFILER_CALLBACK_H__

// End of File
        
        
