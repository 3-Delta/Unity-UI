// EXPORT.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

#if _MSC_VER // this is defined when compiling with Visual Studio
	#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this
#else
	#define EXPORT_API // XCode does not need annotating exported functions, so define is empty
#endif

// Link following functions C-style (required for plugins)
#if __cplusplus // 修改后缀名 c --> cpp 会发现这个宏在生效
	#define EXTREN_C_BEGIN extern "C" {
	#define EXTREN_C_END }
#else
	#define EXTREN_C_BEGIN 
	#define EXTREN_C_END 
#endif