// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/***************************************************************************************
 * File:
 *  basehlp.h
 *
 * Description:
 *  
 *
 *
 ***************************************************************************************/
#ifndef __REGUTIL_H__
#define __REGUTIL_H__


/***************************************************************************************
 ********************                                               ********************
 ********************           BaseException Declaration           ********************
 ********************                                               ********************
 ***************************************************************************************/
#define NumItems( s ) (sizeof( s ) / sizeof( s[0] ))


class REGUTIL
{
    public:

        static BOOL SetKeyAndValue( __in_z const char *szKey,     
                                    const char *szSubkey,  
                                    const char *szValue ); 

        static BOOL DeleteKey( __in_z const char *szKey,       
                               const char *szSubkey );  

        static BOOL SetRegValue( const char *szKeyName, 
                                 const char *szKeyword, 
                                 __in_z __in_opt const char *szValue ); 

        static HRESULT RegisterCOMClass( REFCLSID rclsid,               
                                         __in_z __in_opt const char *szDesc,            
                                         const char *szProgIDPrefix,
                                         int iVersion,  
                                         const char *szClassProgID,     
                                         const char *szThreadingModel, 
                                         const char *szModule );       

        static HRESULT UnregisterCOMClass( REFCLSID rclsid,            
                                           const char *szProgIDPrefix, 
                                           int iVersion,               
                                           const char *szClassProgID );

        static HRESULT FakeCoCreateInstance( REFCLSID rclsid, 
                                             REFIID riid, 
                                             void** ppv );
        

    private:

        static HRESULT _RegisterClassBase( REFCLSID rclsid,          
                                           __in_z __in_opt const char *szDesc,       
                                           const char *szProgID,     
                                           const char *szIndepProgID,
                                           __out_ecount(cchOutCLSID) char *szOutCLSID,
                                           size_t cchOutCLSID );       

        static HRESULT _UnregisterClassBase( REFCLSID rclsid,            
                                             const char *szProgID,       
                                             __in_z const char *szIndepProgID,  
                                             __out_ecount(cchOutCLSID) char *szOutCLSID,
                                             size_t cchOutCLSID );          

}; // REGUTIL

#endif // __REGUTIL_H__

// End of File
