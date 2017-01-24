/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

/*
 * main.cpp
 *
 * Loads a splash screen bitmap from the resources built into the
 * executable. Writes the current XenCenter version numbers onto
 * the bitmap, and displays it on screen. Waits for a message
 * from XenCenter, and then quits. Also monitors the XenCenter
 * process, and quits if it dies (in case XenCenter crashes
 * before sending the message).
 *
 * Parts of code taken from msdn.
 */

// Disable deprecation warnings in the CRT
#define _CRT_SECURE_NO_DEPRECATE
#include <windows.h>
#include <iostream>
#include <sstream>
#include <Tchar.h>
#include <strsafe.h>
#include <vector>
#include <Lmcons.h>
#include <fstream>
#include <ctime>
#include "resource.h"
#include "util.h"

using namespace std;

// Our own made-up IDs for timers
const int ShortTimerId = 1;
const int LongTimerId = 2;

const int ShortTimerInterval = 100;
// How long to wait for XenCenterMain to start
const int LongTimerInterval = 30000;

const int PipeTimeout = 60 * 1000;

// How long the splash should try to acquire the splashscreen lock for
const int SplashLockMaxWait = 60000;
// How many ms the splash screen should wait between attempts
const int SplashLockSleepInterval = 250;    

// Size of the splash bitmap
const int ImageSizeX = 415;
const int ImageSizeY = 217;

const TCHAR SplashClassName[] = TEXT("XenCenterSplash0001");
const TCHAR PipeStub[] = TEXT("\\\\.\\pipe\\XenCenter-");
const TCHAR SplashPipeStub[] = TEXT("\\\\.\\pipe\\XenCenterSplash-");
// The path to the main C# XenCenter exe, relative to the location of the splash exe.
const TCHAR XenCenterPath[] = TEXT("XenCenterMain.exe");
const size_t PathLen = 17;

#ifdef _DEBUG
const TCHAR ProductVersion[] = TEXT("0.0");
const TCHAR ProductBuild[] = TEXT("0000");
#else
const TCHAR ProductVersion[] = TEXT("[BRANDING_PRODUCT_VERSION]");
const TCHAR ProductBuild[] = TEXT("@BUILD_NUMBER@");
#endif

// The in-memory Device Context
HDC memdc;

PROCESS_INFORMATION pi;

static wstring PipeName(const TCHAR * stub, wstring mainExePath)
{
    // Replace '\' with '-' in mainExePath
    wstring sanitizedMainExePath(mainExePath);
    {
        size_t index = sanitizedMainExePath.npos;
        while ((index = sanitizedMainExePath.find('\\', 0)) != sanitizedMainExePath.npos)
        {
            sanitizedMainExePath.replace(index, 1, 1, '-');
        }
    }

    DWORD tmp = UNLEN + 1;
    TCHAR UserName[UNLEN + 1];
    GetUserName(UserName, &tmp);

    DWORD pid = GetCurrentProcessId();
    DWORD sid = 0;
    if (0 == ProcessIdToSessionId(pid, &sid))
    {
        // Ignore error and force sid to 0.
        sid = 0;
    }
    
    wostringstream SplashPipePath;
    SplashPipePath << stub << sid << '-' << UserName << '-' << sanitizedMainExePath;

    wstring s = SplashPipePath.str();
    // Max length of named pipe name string is 256 chars
    if (s.length() > 256)
    {
        s.resize(256);
    }

    return s;
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch(msg)
    {
        case WM_CLOSE:
            DestroyWindow(hwnd);
        break;
        case WM_DESTROY:
            // Hide the window - will happen anyway if we exit, but not until
            // after XenCenterMain exits if --wait was specified.
            ShowWindow(hwnd, SW_HIDE);
            // Line below sends a WM_QUIT message to this thread
            PostQuitMessage(0);
        break;
        case WM_LBUTTONDOWN:
            ShowWindow(hwnd, SW_HIDE);
        break;
        case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC screendc = BeginPaint(hwnd, &ps);
            BitBlt(screendc, ps.rcPaint.left, ps.rcPaint.top, ps.rcPaint.right, ps.rcPaint.bottom,
                memdc, ps.rcPaint.left, ps.rcPaint.top, SRCCOPY);
            EndPaint(hwnd, &ps);
        }
        break;
        case WM_TIMER:
        {
            switch (wParam)
            {
            case ShortTimerId:
                if (WaitForSingleObject(pi.hProcess, 0) != WAIT_TIMEOUT)
                {
                    // XenCenter has closed (e.g. crashed) without killing the splash screen: exit.
                    KillTimer(hwnd, ShortTimerId);
                    DestroyWindow(hwnd);
                }
                else
                {
                    // Poll again later
                    SetTimer(hwnd, ShortTimerId, ShortTimerInterval, NULL);
                }
                break;
            case LongTimerId:
                // We've been open too long: close even though we haven't heard from XenCenter
                DestroyWindow(hwnd);
                break;
            default:
                return DefWindowProc(hwnd, msg, wParam, lParam);
            }
        }
        default:
            return DefWindowProc(hwnd, msg, wParam, lParam);
    }
    return 0;
}

int WINAPI _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine, int nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);

    // Record if an error that doesn't prevent us launching XenCenterMain has occurred.
    // If set to true, we write the splash log to file and launch XenCenterMain, even
    // if the splash screen locking or named pipe argument passing have failed.
    bool nonCriticalError = false;

    // Open logging output stream. The accumulated contents of this stream are written
    // to a log file only in the event of an ErrorExit.
    wostringstream logStream;
    time_t rawtime = time(NULL);
    if (rawtime > -1)
    {
        logStream << TEXT("splash .exe started at ") << ctime(&rawtime) << endl;
    }
    else
    {
        logStream << TEXT("WARNING: time() returned -1") << endl;
    }

    // Get the full path to the splash exe
    const size_t pathBufSize = 128 * 1024;
    wchar_t splashExePath[pathBufSize];
    DWORD pathLen = GetModuleFileName(NULL, splashExePath, pathBufSize);
    if (pathLen == 0 || pathLen == pathBufSize)
    {
        ErrorExit(logStream, TEXT("GetModuleFileName"), true);
    }
    logStream << TEXT("splashExePath: ") << splashExePath << endl;

    // Now work out the path to the main exe
    wstring mainExePath(splashExePath);
    {
        size_t index = mainExePath.find_last_of('\\', pathLen);
        if (index != mainExePath.npos)
        {
            mainExePath.resize(index);
            mainExePath.push_back('\\');
        }
        mainExePath.append(XenCenterPath);
    }
    logStream << TEXT("mainExePath: ") << mainExePath << endl;

    // Acquire splash screen lock
    HANDLE splashPipeHandle = INVALID_HANDLE_VALUE;
    {
        wstring s = PipeName(SplashPipeStub, mainExePath);
        logStream << "Attempting to acquire splash screen lock: " << s.c_str() << endl;

        for (int numTries = 0; numTries * SplashLockSleepInterval < SplashLockMaxWait; numTries++)
        {
            splashPipeHandle = CreateNamedPipe(s.c_str(),
                PIPE_ACCESS_DUPLEX | FILE_FLAG_FIRST_PIPE_INSTANCE,
                PIPE_WAIT, PIPE_UNLIMITED_INSTANCES, 0, 0, 0, NULL);

            if (splashPipeHandle == INVALID_HANDLE_VALUE)
            {
                DWORD lastError = GetLastError();
                if (lastError == ERROR_ACCESS_DENIED)
                {
                    // Pipe in use. Sleep and retry.
                    Sleep(SplashLockSleepInterval);
                }
                else
                {
                    // Unexpected error code
                    logStream << "WARNING: CreateNamedPipe failed with unexpected error. Error code: " << GetLastError() << endl;
                    nonCriticalError = true;
                }
            }
            else
            {
                // We have acquired the lock
                logStream << "Acquired splash screen lock" << endl;
                break;
            }
        }

        if (splashPipeHandle == INVALID_HANDLE_VALUE)
        {
            // Maximum attempts reached without success. Exit.
            logStream << "WARNING: Couldn't acquire splash screen lock before timeout." << endl;
            nonCriticalError = true;
        }
    }
 
    // First try to pass the cmd line arguments into the named pipe
    {
        wstring s = PipeName(PipeStub, mainExePath);
        logStream << TEXT("Pipe path 's': ") << s << endl;

        // Allocate a buffer for data sent to us through the pipe.
        // (Should never actually be any, but the command needs a buffer param anyway)
        const int dataOutLength = 64 * 1024;
        LPVOID dataOut = malloc(dataOutLength);
        if (dataOut == NULL)
        {
            logStream << TEXT("WARNING: malloc dataOut failed. Error code: ") << GetLastError() << endl;
            nonCriticalError = true;
        }

        DWORD bytesRead;
        if (!CallNamedPipe(s.c_str(), lpCmdLine, (DWORD)_tcslen(lpCmdLine) * sizeof(TCHAR), dataOut, dataOutLength, &bytesRead, PipeTimeout))
        {
            DWORD lastError = GetLastError();
            if (lastError == ERROR_FILE_NOT_FOUND)
            {
                logStream << TEXT("CallNamedPipe gave ERROR_FILE_NOT_FOUND: proceeding to launch XenCenter") << endl;
            }
            else if (lastError == ERROR_BROKEN_PIPE)
            {
                logStream << TEXT("CallNamedPipe gave ERROR_BROKEN_PIPE: proceeding to launch XenCenter") << endl;
            }
            else
            {
                logStream << "WARNING: CallNamedPipe failed with unexpected error. Error code: " << GetLastError() << endl;
                nonCriticalError = true;
            }
        }
        else
        {
            // Success: we passed the args into the pipe. Exit.
            logStream << "Success: command line arguments were passed into pipe. Exiting." << endl;
            exit(0);
        }
        free(dataOut);
    }

    // If we get here, sending into the pipe failed. Start XenCenter.
    STARTUPINFO si;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);

    logStream << TEXT("Running CreateProcess with GetCommandLine(): ") << GetCommandLine() << endl;
    if (!CreateProcess(mainExePath.c_str(), // module name
        GetCommandLine(), // Command line
        NULL,             // Process handle not inheritable
        NULL,             // Thread handle not inheritable
        FALSE,            // Set handle inheritance to FALSE
        NORMAL_PRIORITY_CLASS,
        NULL,             // Use parent's environment block
        NULL,             // Use parent's starting directory 
        &si,              // Pointer to STARTUPINFO structure
        &pi)              // Pointer to PROCESS_INFORMATION structure
    )
    {
        ErrorExit(logStream, TEXT("CreateProcess"), true);
    }
    CloseHandle(pi.hThread);

    if (nonCriticalError)
    {
        // At least we managed to launch XenCenterMain.
        // Probably not appropriate to show the splash screen. Exit now.
        ErrorExit(logStream, TEXT("nonCriticalError"), false);
    }

    // Show the splash screen. First register the window class.
    WNDCLASSEX wc;
    wc.cbSize        = sizeof(WNDCLASSEX);
    wc.style         = 0;
    wc.lpfnWndProc   = WndProc;
    wc.cbClsExtra    = 0;
    wc.cbWndExtra    = 0;
    wc.hInstance     = hInstance;
    wc.hIcon         = NULL;
    wc.hCursor       = LoadCursor(NULL, IDC_ARROW);
    wc.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wc.lpszMenuName  = NULL;
    wc.lpszClassName = SplashClassName;
    wc.hIconSm       = NULL;

    if (RegisterClassEx(&wc) == NULL)
    {
        ErrorExit(logStream, TEXT("RegisterClassEx"), false);
    }

    // Get the screen (desktop) DC
    HDC screendc = CreateIC(TEXT("DISPLAY"), NULL, NULL, NULL);
    if (screendc == NULL)
    {
        ErrorExit(logStream, TEXT("CreateIC"), false);
    }

    // Get the primary monitor desktop size
    const int screenwidth  = GetSystemMetrics(SM_CXSCREEN);
    const int screenheight = GetSystemMetrics(SM_CYSCREEN);

    logStream << TEXT("Creating splash window") << endl;
    // Create the splash window
    int x = (screenwidth-ImageSizeX)/2;
    int y = (screenheight-ImageSizeY)/2;
    HWND hwnd = CreateWindowEx(WS_EX_TOPMOST | WS_EX_TOOLWINDOW, SplashClassName, NULL, WS_POPUP, x, y, ImageSizeX, ImageSizeY, NULL, NULL, hInstance, NULL);
    if (hwnd == NULL)
    {
        ErrorExit(logStream, TEXT("CreateWindowEx"), false);
    }

    // Load the splash bitmap from the embedded resource file
    HBITMAP image = (HBITMAP)LoadImage(hInstance, MAKEINTRESOURCE(IDB_BITMAP1), IMAGE_BITMAP, 0, 0, LR_DEFAULTCOLOR);
    if (image == NULL)
    {
        ErrorExit(logStream, TEXT("LoadImage"), false);
    }

    // Create the in-memory Device Context
    memdc = CreateCompatibleDC(screendc);
    if (memdc == NULL)
    {
        ErrorExit(logStream, TEXT("CreateCompatibleDC(screendc)"), false);
    }

    // Create a DC for the splash image
    HDC filedc = CreateCompatibleDC(screendc);
    if (filedc == NULL)
    {
        ErrorExit(logStream, TEXT("CreateCompatibleDC(filedc)"), false);
    }

    // Blit the splash image into the memory DC
    SelectObject(filedc, image);
    HBITMAP bmp = CreateCompatibleBitmap(screendc, ImageSizeX, ImageSizeY);
    ReleaseDC(hwnd, screendc);
    SelectObject(memdc, bmp);
    BitBlt(memdc, 0, 0, ImageSizeX, ImageSizeY, filedc, 0, 0, SRCCOPY);
    DeleteObject(image);
    DeleteDC(filedc);

    logStream << TEXT("Showing splash window") << endl;
    // Show the splash window
    ShowWindow(hwnd, nCmdShow);
    UpdateWindow(hwnd);

    // Start timer to poll for XenCenter having started
    SetTimer(hwnd, ShortTimerId, ShortTimerInterval, NULL);

    // Start timeout timer after which splash window closes anyway
    SetTimer(hwnd, LongTimerId, LongTimerInterval, NULL);

    MSG msg;
    // Start message loop
    while(GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    if (!DisconnectNamedPipe(splashPipeHandle))
    {
        // Irritating but non-fatal
        logStream << "DisconnectNamedPipe failed with error " << GetLastError() << endl;
    }
    if (!CloseHandle(splashPipeHandle))
    {
        // Likewise
        logStream << "CloseHandle failed with error " << GetLastError() << endl;
    }

    // Check to see if args contain '--wait': if so, wait for XenCenterMain process to exit
    {
        wstring args(lpCmdLine);
        logStream << TEXT("args: ") << args.c_str() << endl;
        if (args.find(TEXT("--wait"), 0) != args.npos)
        {
            logStream << TEXT("--wait detected: waiting for main XenCenter process to exit") << endl;
            WaitForSingleObject(pi.hProcess, INFINITE);
            logStream << TEXT("XenCenter process exited") << endl;
        }
        else
        {
            CloseHandle(pi.hProcess);
        }
    }

    logStream << TEXT("Exiting normally") << endl;

    return (int)msg.wParam;
}
