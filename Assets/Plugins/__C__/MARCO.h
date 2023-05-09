// MARCO.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

// https://www.zhihu.com/question/37692782/answer/2929878510
// #define fun(x) do{fun1(x);fun2(x);}while(0)

#define TO_BOOL(e) return !!(e)

#define SWAP(l, r) {(l) = (l) ^ (r); (r) = (l) ^ (r); (l) = (l) ^ (r);}

#define SAFE_DELETE(ptr) {do { if((ptr) != NULL) { delete (ptr);  (ptr) = NULL; } }while(0)}

#define SAFE_DELETE_ARRAY(ptr) {do { if((ptr) != NULL) { delete [](ptr);  (ptr) = NULL; } }while(0)}

#define ENSURE(caseValue) { if (!(caseValue)) return; }

#define ENSURE_BOOL(caseValue) { if (!(caseValue)) return false; }

#define ENSURE_INT0(caseValue) { if (!(caseValue)) return 0; }

#define ENSURE_INT1(caseValue) { if (!(caseValue)) return -1; }

#define ENSURE_FLOAT(caseValue) { if (!(caseValue)) return 0.0f; }

#define ENSURE_POINTER(caseValue) { if (!(caseValue)) return NULL; }

#define ENSURE_CONTINUE(caseValue) { if (!(caseValue)) continue; }

#define ENSURE_BREAK(caseValue) { if (!(caseValue)) break; }

// #define INDEX_ITEM(array, index, member) array[index].member
#define ZERO_ARRAY(array, length) { (memset((array), 0, (length)), (void)0) }

// #define ON_ZERO(e) (sizeof(struct { int: -!!(e); }))

// https://www.zhihu.com/question/377057346