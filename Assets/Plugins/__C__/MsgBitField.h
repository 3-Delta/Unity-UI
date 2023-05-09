// MsgBitField.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

#include <stdint.h>
#include <stddef.h>

#include "EXPORT.h"
#include "MARCO.h"

EXTREN_C_BEGIN
	extern EXPORT_API uint16_t GetValue(uint16_t first, uint16_t second);
	extern EXPORT_API void GetFirstSecond(uint16_t value, uint16_t* first, uint16_t* second);
EXTREN_C_END