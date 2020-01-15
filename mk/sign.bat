@echo off
rem Copyright (c) Citrix Systems, Inc.
rem All rights reserved.
rem
rem Redistribution and use in source and binary forms,
rem with or without modification, are permitted provided
rem that the following conditions are met:
rem
rem *   Redistributions of source code must retain the above
rem     copyright notice, this list of conditions and the
rem     following disclaimer.
rem *   Redistributions in binary form must reproduce the above
rem     copyright notice, this list of conditions and the
rem     following disclaimer in the documentation and/or other
rem     materials provided with the distribution.
rem
rem THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
rem CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
rem INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
rem MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
rem DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
rem CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
rem SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
rem BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
rem SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
rem INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
rem WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
rem NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
rem OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
rem SUCH DAMAGE.

rem Script parameters:
rem 1 Signing node name
rem 2 Sign in SBE
rem 3 Self-signing certificate sha1 thumbprint
rem 4 Self-signing certificate sha256 thumbprint
rem 5 File to be signed
rem 6 Description

set timestamp_sha1=http://timestamp.verisign.com/scripts/timestamp.dll
set timestamp_sha2=http://sha256timestamp.ws.symantec.com/sha256/timestamp

set thumb_sha1=%3
set thumb_sha256=%4

if /I %~x5 == .msi (
  set cross_sign=no
) else (
  set cross_sign=yes
)

set descr=%~6

if "%~2"=="true" (
  signing in SBE
  set CTXSIGN=C:\ctxsign2\ctxsign.exe

  %CTXSIGN%  --authorise --workerID %~1 --orchID %~1 --jobID XenServerWindowsLegacyPVTools_signing --task XenServerDotnetPackages-%GLOBAL_BUILD_NUMBER% --debug > out.txt
  echo OUTPUT FROM CTXSIGN --AUTHORISE:
  type out.txt
  echo OUTPUT ENDS

  if %errorlevel% neq 0 exit /b %errorlevel%

  echo
  set /p CCSS_TICKET= < out.txt

  %CTXSIGN% --sign --key XenServer.NET_KEY --cross-sign --pagehashes yes --type Authenticode --description "%descr%" %5
  if %errorlevel% neq 0 exit /b %errorlevel%

  if "%cross_sign%"=="yes" (
    %CTXSIGN% --sign --authenticode-append --authenticode-SHA256 --key XenServerSHA256.NET_KEY --cross-sign --pagehashes yes %5
    if %errorlevel% neq 0 exit /b %errorlevel%
  )
  %CTXSIGN% --end

) else (
  echo self signing

  if /I "%cross_sign%" == "yes" (
    signtool sign -v -sm -sha1 %thumb_sha1% -d "%descr%" -t %timestamp_sha1% %5
    if %errorlevel% neq 0 exit /b %errorlevel%
    signtool sign -v -sm -as -sha1 %thumb_sha256% -d "%descr%" -tr %timestamp_sha2% -td sha256 %5
  ) else (
    signtool sign -v -sm -sha1 %thumb_sha1% -d "%descr%" -tr %timestamp_sha2% -td sha256 %5
  )

  if %errorlevel% neq 0 exit /b %errorlevel%
)
