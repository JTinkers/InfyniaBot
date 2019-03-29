#include "stdafx.h"

#include <windows.h>

#include <iostream>

#include <string>

#pragma once

#pragma once

#pragma managed

using namespace System;
using namespace System::Reflection;
using namespace Engine;
using namespace std;

DWORD WINAPI MainThread(LPVOID param)
{
	//HMODULE lib = LoadLibraryA("Engine.dll");

	Engine::Core::Initialize();

	//FreeLibrary(lib);

	FreeLibraryAndExitThread(GetModuleHandleA("Initializer.dll"), 0);

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