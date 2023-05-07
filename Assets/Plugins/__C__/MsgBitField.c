#include "MsgBitField.h"

union U {
	uint16_t value;
	struct ST {
		uint16_t first : 10;
		uint16_t second : 6;
	}st;
};

EXPORT_API uint16_t GetValue(uint16_t first, uint16_t second) {
	union U u;
	u.st.first = first;
	u.st.second = second;
	return u.value;
}

EXPORT_API void GetFirstSecond(uint16_t value, uint16_t* first, uint16_t* second) {
	union U u;
	u.value = value;
	if (first != NULL) {
		*first = u.st.first;
	}
	if (second != NULL) {
		*second = u.st.second;
	}
}