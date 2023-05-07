// MsgBitField.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

#include <stdint.h>
#include <stddef.h>

#include "EXPORT.h"

EXTREN_C_BEGIN
	extern uint16_t EXPORT_API GetValue(uint16_t first, uint16_t second);
	extern void EXPORT_API GetFirstSecond(uint16_t value, uint16_t* first, uint16_t* second);
EXTREN_C_END