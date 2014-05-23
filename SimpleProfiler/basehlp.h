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
#ifndef __BASEHLP_H__
#define __BASEHLP_H__

#include "basehdr.h"

//
// exception macro
//
#define _THROW_EXCEPTION( message )  { throw new BaseException( message ); }
#define ARRAY_LEN(a) (sizeof(a) / sizeof((a)[0]))


/***************************************************************************************
 ********************                                               ********************
 ********************           BaseException Declaration           ********************
 ********************                                               ********************
 ***************************************************************************************/
class DECLSPEC BaseException
{
    public:
        
        BaseException( const char *reason );
        virtual ~BaseException();
        virtual void ReportFailure();

    private:
        char *m_reason;
        
}; // BaseException

/***************************************************************************************
 ********************                                               ********************
 ********************            Synchronize Declaration            ********************
 ********************                                               ********************
 ***************************************************************************************/
class DECLSPEC Synchronize 
{
    public:
    
        Synchronize( CRITICAL_SECTION &criticalSection );
        ~Synchronize();
        
        
    private:
    
        CRITICAL_SECTION &m_block;
        
}; // Synchronize

#endif // __BASEHLP_H__

// End of File
