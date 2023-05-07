// Encrypt.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

#include "EXPORT.h"

void XOR(char* array, int len);
EXTREN_C_BEGIN
	extern void EXPORT_API Encrypt(char* array, int len);
	extern void EXPORT_API Decrypt(char* array, int len);
EXTREN_C_END