#include "Bytes.h"

EXPORT_API int32_t/*bool*/ IsLittleEndian() {
	union U16Bytes u16;
	u16.value = 0x0102;
	return u16.st.v0 == 0x02;
}

EXPORT_API int32_t/*bool*/ Split16(int16_t target, int8_t* array, int32_t length, int32_t placeLittleEndian /*bool*/) {
	if (array != NULL && length >= sizeof(int16_t)) {
		union U16Bytes u;
		u.value = target;
		if (placeLittleEndian) {
			array[0] = u.st.v1;
			array[1] = u.st.v0;
		}
		else {
			array[0] = u.st.v0;
			array[1] = u.st.v1;
		}
		return 1;
	}
	return 0;
}

EXPORT_API int32_t/*bool*/ Split32(int32_t target, int8_t* array, int32_t length, int32_t placeLittleEndian /*bool*/) {
	if (array != NULL && length >= sizeof(int32_t)) {
		union U32Bytes u;
		u.value = target;
		if (placeLittleEndian) {
			array[0] = u.st.v3;
			array[1] = u.st.v2;
			array[2] = u.st.v1;
			array[3] = u.st.v0;
		}
		else {
			array[0] = u.st.v0;
			array[1] = u.st.v1;
			array[2] = u.st.v2;
			array[3] = u.st.v3;
		}
		return 1;
	}
	return 0;
}

EXPORT_API int32_t/*bool*/ Split64(int64_t target, int8_t* array, int32_t length, int32_t placeLittleEndian /*bool*/) {
	if (array != NULL && length >= sizeof(int64_t)) {
		union U64Bytes u;
		u.value = target;
		if (placeLittleEndian) {
			array[0] = u.st.v7;
			array[1] = u.st.v6;
			array[2] = u.st.v5;
			array[3] = u.st.v4;
			array[4] = u.st.v3;
			array[5] = u.st.v2;
			array[6] = u.st.v1;
			array[7] = u.st.v0;
		}
		else {
			array[0] = u.st.v0;
			array[1] = u.st.v1;
			array[2] = u.st.v2;
			array[3] = u.st.v3;
			array[4] = u.st.v4;
			array[5] = u.st.v5;
			array[6] = u.st.v6;
			array[7] = u.st.v7;
		}
		return 1;
	}
	return 0;
}

EXPORT_API int16_t Swap16(int16_t target) {
	/* // 方案1
		int8_t v0 = (int8_t)(target >> 0);
		int8_t v1 = (int8_t)(target >> 8);

		int16_t ret = 0;
		ret = (int16_t)(((int16_t)v0 << 8) | (int16_t)v1);

		return ret;
	*/

	union U16Bytes u;
	u.value = target;
	SWAP(u.st.v0, u.st.v1);
	
	return u.value;
}

EXPORT_API int32_t Swap32(int32_t target) {
	/* // 方案1
		int8_t v0 = (int8_t)(target >> 0);
		int8_t v1 = (int8_t)(target >> 8);
		int8_t v2 = (int8_t)(target >> 16);
		int8_t v3 = (int8_t)(target >> 24);

		int32_t ret = 0;
		ret |= (int32_t)(((int32_t)v0 << 24) | (int32_t)v1 << 16);
		ret |= (int32_t)(((int32_t)v2 << 8) | (int32_t)v3 << 0);
		return ret;
	*/

	union U32Bytes u;
	u.value = target;
	SWAP(u.st.v0, u.st.v3);
	SWAP(u.st.v1, u.st.v2);

	return u.value;
}

EXPORT_API int64_t Swap64(int64_t target) {
	/* // 方案1
		int8_t v0 = (int8_t)(target >> 0);
		int8_t v1 = (int8_t)(target >> 8);
		int8_t v2 = (int8_t)(target >> 16);
		int8_t v3 = (int8_t)(target >> 24);
		int8_t v4 = (int8_t)(target >> 32);
		int8_t v5 = (int8_t)(target >> 40);
		int8_t v6 = (int8_t)(target >> 48);
		int8_t v7 = (int8_t)(target >> 56);

		int64_t ret = 0;
		ret |= (int64_t)(((int64_t)v0 << 56) | (int64_t)v1 << 48);
		ret |= (int64_t)(((int64_t)v2 << 40) | (int64_t)v3 << 32);
		ret |= (int64_t)(((int64_t)v4 << 24) | (int64_t)v5 << 16);
		ret |= (int64_t)(((int64_t)v6 << 8) | (int64_t)v7 << 0);
		return ret;
	*/

	union U64Bytes u;
	u.value = target;
	SWAP(u.st.v0, u.st.v7);
	SWAP(u.st.v1, u.st.v6);
	SWAP(u.st.v2, u.st.v5);
	SWAP(u.st.v3, u.st.v4);

	return u.value;
}

EXPORT_API int16_t Combine16(int8_t* array, int32_t startIndex, int32_t endIndex, int32_t placeLittleEndian /*bool*/) {
	if (array == NULL || startIndex > endIndex || startIndex < 0) {
		return 0;
	}

	int8_t tArray[sizeof(int16_t)] = { 0, 0 };
	int32_t i = 0;
	while (startIndex < endIndex && i < sizeof(int16_t)) {
		tArray[i++] = array[startIndex++];
	}

	/*
		int16_t ret = 0;
		if (placeLittleEndian) {
			ret |= (int16_t)(((int16_t)tArray[0] << 0) | (int16_t)tArray[1] << 8);
		}
		else {
			ret |= (int16_t)(((int16_t)tArray[0] << 8) | (int16_t)tArray[1] << 0);
		}
		return ret;
	*/

	union U16Bytes u;
	memcpy(u.bytes, tArray, sizeof(int16_t));
	if (placeLittleEndian == IsLittleEndian()) {
		return u.value;
	}
	else {
		return Swap16(u.value);
	}
}


EXPORT_API int32_t Combine32(int8_t* array, int32_t startIndex, int32_t endIndex, int32_t placeLittleEndian /*bool*/) {
	if (array == NULL || startIndex > endIndex || startIndex < 0) {
		return 0;
	}

	int8_t tArray[sizeof(int32_t)] = { 0, 0, 0, 0 };
	int32_t i = 0;
	while (startIndex < endIndex && i < sizeof(int32_t)) {
		tArray[i++] = array[startIndex++];
	}

	/*
		int32_t ret = 0;
		if (placeLittleEndian) {
			ret |= (int32_t)(((int32_t)tArray[0] << 0) | (int32_t)tArray[1] << 8);
			ret |= (int32_t)(((int32_t)tArray[2] << 16) | (int32_t)tArray[3] << 24);
		}
		else {
			ret |= (int32_t)(((int32_t)tArray[0] << 24) | (int32_t)tArray[1] << 16);
			ret |= (int32_t)(((int32_t)tArray[2] << 8) | (int32_t)tArray[3] << 0);
		}
		return ret;
	*/

	union U32Bytes u;
	memcpy(u.bytes, tArray, sizeof(int32_t));
	if (placeLittleEndian == IsLittleEndian()) {
		return u.value;
	}
	else {
		return Swap32(u.value);
	}
}


EXPORT_API int64_t Combine64(int8_t* array, int32_t startIndex, int32_t endIndex, int32_t placeLittleEndian /*bool*/) {
	if (array == NULL || startIndex > endIndex || startIndex < 0) {
		return 0;
	}

	int8_t tArray[sizeof(int64_t)] = { 0, 0, 0, 0, 0, 0, 0, 0 };
	int32_t i = 0;
	while (startIndex < endIndex && i < sizeof(int64_t)) {
		tArray[i++] = array[startIndex++];
	}
	
	/* 
		int64_t ret = 0;
		if (placeLittleEndian) {
			ret |= (int64_t)(((int64_t)tArray[0] << 0) | (int64_t)tArray[1] << 8);
			ret |= (int64_t)(((int64_t)tArray[2] << 16) | (int64_t)tArray[3] << 24);
			ret |= (int64_t)(((int64_t)tArray[4] << 32) | (int64_t)tArray[5] << 40);
			ret |= (int64_t)(((int64_t)tArray[6] << 48) | (int64_t)tArray[7] << 56);
		}
		else {
			ret |= (int64_t)(((int64_t)tArray[0] << 56) | (int64_t)tArray[1] << 48);
			ret |= (int64_t)(((int64_t)tArray[2] << 40) | (int64_t)tArray[3] << 32);
			ret |= (int64_t)(((int64_t)tArray[4] << 24) | (int64_t)tArray[5] << 16);
			ret |= (int64_t)(((int64_t)tArray[6] << 8) | (int64_t)tArray[7] << 0);
		}
		return ret;
	*/
	
	union U64Bytes u;
	memcpy(u.bytes, tArray, sizeof(int64_t));
	if (placeLittleEndian == IsLittleEndian()) {
		return u.value;
	}
	else {
		return Swap64(u.value);
	}
}

