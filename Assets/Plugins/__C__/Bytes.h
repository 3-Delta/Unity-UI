// Bytes.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

#include "EXPORT.h"
#include "MARCO.h"

#include <stdint.h>
#include <stddef.h>

union U16Bytes {
	int16_t value;

	struct ST {
		int8_t v0;
		int8_t v1;
	}st;

	int8_t bytes[sizeof(int16_t)];
};

union U32Bytes {
	int32_t value;

	struct ST {
		int8_t v0;
		int8_t v1;
		int8_t v2;
		int8_t v3;
	}st;

	int8_t bytes[sizeof(int32_t)];
};

union U64Bytes {
	int64_t value;

	struct ST {
		int8_t v0;
		int8_t v1;
		int8_t v2;
		int8_t v3;
		int8_t v4;
		int8_t v5;
		int8_t v6;
		int8_t v7;
	}st;

	int8_t bytes[sizeof(int64_t)];
};

EXTREN_C_BEGIN
	extern EXPORT_API int32_t/*bool*/ IsLittleEndian();

	extern EXPORT_API int32_t/*bool*/ Split16(int16_t target, int8_t* array, int32_t length, int32_t placeLittleEndian /*bool*/);
	extern EXPORT_API int32_t/*bool*/ Split32(int32_t target, int8_t* array, int32_t length, int32_t placeLittleEndian /*bool*/);
	extern EXPORT_API int32_t/*bool*/ Split64(int64_t target, int8_t* array, int32_t length, int32_t placeLittleEndian /*bool*/);

	extern EXPORT_API int16_t Combine16(int8_t* array, int32_t startIndex, int32_t endIndex, int32_t placeLittleEndian /*bool*/);
	extern EXPORT_API int32_t Combine32(int8_t* array, int32_t startIndex, int32_t endIndex, int32_t placeLittleEndian /*bool*/);
	extern EXPORT_API int64_t Combine64(int8_t* array, int32_t startIndex, int32_t endIndex, int32_t placeLittleEndian /*bool*/);

	extern EXPORT_API int16_t Swap16(int16_t target);
	extern EXPORT_API int32_t Swap32(int32_t target);
	extern EXPORT_API int64_t Swap64(int64_t target);
EXTREN_C_END
