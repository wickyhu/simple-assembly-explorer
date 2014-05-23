// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/****************************************************************************************
 * File:
 *  ProfilerInfo.h
 *
 * Description:
 *
 *
 *
 ***************************************************************************************/
#ifndef __PROFILER_INFO_H__
#define __PROFILER_INFO_H__

#include "basehlp.h"

#define FILENAME "profiler.log"
#define STATUS_FILENAME "profiler.log.enabled"

#define BYTESIZE 8

//
// env variables
//
#define SP_LOG_PATH "SP_LOG_PATH"
#define SP_FILTER "SP_FILTER"
#define SP_INCLUDE_SYSTEM "SP_INCLUDE_SYSTEM"
#define SP_TRACE_EVENT "SP_TRACE_EVENT"
#define SP_TRACE_PARAMETER "SP_TRACE_PARAMETER"

enum ObjHandles
{ 
    GC_HANDLE = 0, 
    OBJ_HANDLE = 1,
    CALL_HANDLE = 2,
    TRIGGER_GC_HANDLE = 3, 
    SENTINEL_HANDLE = 4, 
};

enum Comparison { LESS_THAN, EQUAL_TO, GREATER_THAN };

/***************************************************************************************
 ********************                                               ********************
 ********************             BaseInfo Declaration              ********************
 ********************                                               ********************
 ***************************************************************************************/
class BaseInfo
{
    public:    
        BaseInfo( SIZE_T id );
        virtual ~BaseInfo();
        
    public:            
        template<typename T> BOOL Compare( T key );
        template<typename T> Comparison CompareEx( T key );

    public:            
        SIZE_T m_id;
        WCHAR m_name[MAX_LENGTH];
    
}; // BaseInfo

/***************************************************************************************
 ********************                                               ********************
 ********************          ParameterInfo Declaration             ********************
 ********************                                               ********************
 ***************************************************************************************/
class ParameterInfo 
{
    public:
		ParameterInfo(mdParamDef paramDef);
        virtual ~ParameterInfo();      

    public:
        mdParamDef m_def;
		WCHAR m_name[MAX_LENGTH];
		DWORD m_attributes;
		CorElementType m_type;
		bool m_isByRef;
		mdTypeDef m_typeDef;
		bool m_isArray;
		WCHAR m_typeName[MAX_LENGTH];

		ModuleID m_moduleId;
};

/***************************************************************************************
 ********************                                               ********************
 ********************          FunctionInfo Declaration             ********************
 ********************                                               ********************
 ***************************************************************************************/
class FunctionInfo :
    public BaseInfo
{
    public:

        FunctionInfo( FunctionID functionID );
        virtual ~FunctionInfo();      

    public:    
        mdMethodDef m_methodDef;
		ModuleID m_moduleId;
  
		ParameterInfo *m_pReturnInfo;
		ULONG m_argCount;
		ParameterInfo **m_ppParamInfo;
}; // FunctionInfo

/***************************************************************************************
 ********************                                               ********************
 ********************             ClassInfo Declaration             ********************
 ********************                                               ********************
 ***************************************************************************************/
class ClassInfo :
    public BaseInfo
{
    public:

        ClassInfo( ClassID classID);
        virtual ~ClassInfo();
    
	public:    
		ModuleID m_moduleID;
        mdTypeDef m_classToken;
		
		mdFieldDef *m_fieldDefs;
		ULONG m_numFields;

	public:
		bool IsValidField(IMetaDataImport *mdi, mdFieldDef fieldDef);

}; // ClassInfo

/***************************************************************************************
 ********************                                               ********************
 ********************             ModuleInfo Declaration             ********************
 ********************                                               ********************
 ***************************************************************************************/
class ModuleInfo :
    public BaseInfo
{
    public:

        ModuleInfo( ICorProfilerInfo2 *cpi, ModuleID moduleID);
        virtual ~ModuleInfo();
    
	public:    
		IMetaDataImport *m_mdi;
		ICorProfilerInfo2 *m_cpi;
		LPCBYTE m_loadAddress;

	public:
		IMetaDataImport *GetMDI();

}; // ClassInfo

/***************************************************************************************
 ********************                                               ********************
 ********************              PrfInfo Declaration              ********************
 ********************                                               ********************
 ***************************************************************************************/
class PrfInfo
{             
    public:    
        PrfInfo();                     
        virtual ~PrfInfo();
                   
	protected:
		DWORD m_dwEventMask;

        ICorProfilerInfo *m_pProfilerInfo;
        ICorProfilerInfo2 *m_pProfilerInfo2;
        
	public:
        virtual void Failure( const char *message = NULL );

}; // PrfInfo


#endif // __PROFILER_INFO_H___

// End of File
