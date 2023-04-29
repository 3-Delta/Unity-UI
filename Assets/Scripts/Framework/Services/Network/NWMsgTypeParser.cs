using System;

// 利用位域bitfield处理协议号，其实就是将协议号用两部分组装起来，一部分是first，也就是主模块号，另一部分是second也就是该模块内具体的协议号
// 可以将该代码分装成C++的dll，这样客户端可以使用，服务端也可以使用
// https://www.zhihu.com/pin/1633497757422940160
public class NWMsgTypeParser
{
    /* c格式代码
    #include <stdint.h>

	#define DLLExport __declspec(dllexport)

	extern "C" {
		union U {
			uint16_t value;
			struct ST {
				uint16_t first : 10;
				uint16_t second : 6;
			}st;
		};

		DLLExport uint16_t GetValue(uint16_t first, uint16_t second) {
			union U u;
			u.st.first = first;
			u.st.second = second;
			return u.value;
		}

		DLLExport void GetFirstSecond(uint16_t value, uint16_t* first, uint16_t* second) {
			union U u;
			u.value = value;
			if (first != NULL) {
				*first = u.st.first;
			}
			if (second != NULL) {
				*second = u.st.second;
			}
		}
	}
    */
}
