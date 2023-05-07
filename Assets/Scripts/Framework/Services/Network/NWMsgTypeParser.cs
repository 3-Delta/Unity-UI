using System;
using System.Runtime.InteropServices;

// 利用位域bitfield处理协议号，其实就是将协议号用两部分组装起来，一部分是first，也就是主模块号，另一部分是second也就是该模块内具体的协议号
// 可以将该代码分装成C++的dll，这样客户端可以使用，服务端也可以使用
// https://www.zhihu.com/pin/1633497757422940160
// https://zhuanlan.zhihu.com/p/146320103 item:66
public class NWMsgTypeParser {
    /*
     * il层面：call void Logic.Layout::GetFirstSecond(uint16, uint16&, uint16&)
     */

    /*
     C#层面： 

     在 C# 中使用 DllImport 属性可以引入 C++ DLL 文件中的函数，并在 C# 中调用这些函数。这是因为 DllImport 属性可以告诉编译器在编译时搜索和使用指定的 DLL 文件，
     并且根据 DLL 文件中的函数签名调用相应的函数。实际上，DllImport 属性使用了 Windows API 中的一些函数，如 LoadLibrary 和 GetProcAddress，
	 来查找和加载指定的 DLL 文件，并定位其中的函数。然后，它通过将传递的参数打包成一组相应的数据类型，将数据传递到 C++ 函数中，并处理函数返回的结果。
	 */

    [DllImport("MsgBitField")]
    public static extern ushort GetValue(ushort first, ushort second);

    [DllImport("MsgBitField")]
    public static extern void GetFirstSecond(ushort value, out ushort first, out ushort second);
}
