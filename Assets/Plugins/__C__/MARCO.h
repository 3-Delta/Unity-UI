// MARCO.h: 标准系统包含文件的包含文件
// 或项目特定的包含文件。

#pragma once

// https://www.zhihu.com/question/37692782/answer/2929878510
// #define fun(x) do{fun1(x);fun2(x);}while(0)

#define TO_BOOL(e) return !!(e)

// #define ON_ZERO(e) (sizeof(struct { int: -!!(e); }))

// https://www.zhihu.com/question/377057346