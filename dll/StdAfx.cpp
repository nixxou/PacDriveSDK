// stdafx.cpp : source file that includes just the standard includes
//	PacDrive.pch will be the pre-compiled header
//	stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

// TODO: reference any additional headers you need in STDAFX.H
// and not in this file

void Debug(LPCTSTR lpszFormat, ...)
{
    va_list args;
    va_start(args, lpszFormat);
    int nBuf;
    TCHAR szBuffer[512];
    nBuf = _vsntprintf(szBuffer, 511, lpszFormat, args);
    OutputDebugString(szBuffer);
    va_end(args);
}
