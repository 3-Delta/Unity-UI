// Encrypt.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

#include "EXPORT.h"	
#include "MARCO.h"

void XOR(char* array, int len);
EXTREN_C_BEGIN
	extern EXPORT_API void Encrypt(char* array, int len);
	extern EXPORT_API void Decrypt(char* array, int len);
EXTREN_C_END