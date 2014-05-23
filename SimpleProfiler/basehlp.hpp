// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/****************************************************************************************
 * File:
 *  basehlp.hpp
 *
 * Description:
 *  
 *
 *
 ***************************************************************************************/

/***************************************************************************************
 ********************                                               ********************
 ********************          BaseException Implementation         ********************
 ********************                                               ********************
 ***************************************************************************************/

DECLSPEC
/* public */
BaseException::BaseException( const char *reason ) :
    m_reason( NULL )
{
    SIZE_T length = strlen( reason );    
    
    m_reason = new char[(length + 1)];
    if (m_reason != NULL)
        strcpy_s( m_reason, length + 1, reason );
} // ctor

DECLSPEC
/* virtual public */
BaseException::~BaseException() 
{
    if ( m_reason != NULL )
        delete[] m_reason;

} // dtor

DECLSPEC
/* virtual public */
void BaseException::ReportFailure()
{
    TEXT_OUTLN( m_reason );
    
} // BaseException::ReportFailure

/***************************************************************************************
 ********************                                               ********************
 ********************            Synchronize Implementation         ********************
 ********************                                               ********************
 ***************************************************************************************/

DECLSPEC
/* public */
Synchronize::Synchronize( CRITICAL_SECTION &criticalSection ) : 
    m_block( criticalSection )
{
    EnterCriticalSection( &m_block );
    
} // ctor


DECLSPEC
/* public */
Synchronize::~Synchronize()
{
    LeaveCriticalSection( &m_block );

} // dtor


// End of File
 
