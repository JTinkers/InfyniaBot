#include "stdafx.h"

#include <windows.h>

#pragma once

#pragma once

#pragma managed

using namespace System;
using namespace System::Reflection;
//using namespace Ciderbit::Component;

DWORD WINAPI MainThread(LPVOID param)
{
	//Component::Initialize();

	FreeLibraryAndExitThread((HMODULE)param, 0);

	return 0;
}

#pragma unmanaged

HMODULE hModule;
BOOL APIENTRY DllMain(HINSTANCE hInstance, DWORD reason, LPVOID reserved)
{
	switch (reason)
	{
		case DLL_PROCESS_ATTACH:
			CreateThread(0, 0, MainThread, hModule, 0, 0);
			break;
		case DLL_PROCESS_DETACH:
			FreeLibrary(hModule);
			break;
	}
	return true;
}