#include "Encrypt.h"

void XOR(char* array, int len) {
	for (int i = 0; i < len; ++i) {
		if (array[i] == 0xff || array[i] == 0x00) {
			continue;
		}
		array[i] ^= (char)0xff;
	}
}

EXPORT_API void EXPORT_API Encrypt(char* array, int len) {
	XOR(array, len);
}

EXPORT_API void EXPORT_API Decrypt(char* array, int len) {
	XOR(array, len);
}
