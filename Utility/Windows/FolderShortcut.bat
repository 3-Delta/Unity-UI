:: 作用：创建文件的软连接
:: 举例：MakeShortcut.bat $(TargetDir)$(ProjectName).exe $(SolutionDir)$(ProjectName).exe

:: rem 和 :: 是注释
:: echo表示输出到控制台，off表示命令本身不输出到控制台
:: @是不让本行命令输出到控制台
:: set 设置变量， 使用变量需要%%包起来, =前后不能有空格
:: 外部参数传递到bat内部，bat内部使用%1 ~ %9接收，最多9个参数, %0则是批处理文件本身
:: %cd% 表示执行bat文件时所在路径， 比如E盘下执行F盘下的一个bat文件， cd显示为E
:: %~dp0 当前执行的bat所在的路径，不包括bat名,也就是F盘
:: 推荐 %~dp0，%cd%经常在管理员权限执行下出现问题

:: 某些情况下执行bat文件需要管理员权限，但是我们不可能每次都右键管理员运行，这里解决方案：https://www.jb51.net/article/193692.htm

:: https://blog.csdn.net/albertsh/article/details/52807345
:: https://www.cnblogs.com/xpwi/p/9626959.html
:: 上一级目录就直接后接\..\即可，是字符串连接的一部分，例如 set parentPath=%cd%%\..\

@echo off

:: 获取管理员权限，但是引起了很多的参数传递的问题
:: %1 mshta vbscript:CreateObject(“Shell.Application”).ShellExecute(“cmd.exe”,"/c %~s0 ::","",“runas”,1)(window.close)&&exit

set src=%1
set dest=%2

echo source: %src%
echo destination：%dest%

:: 如果dest文件已经存在，则先删除老的
if exist %dest% (
	del %dest%
)

:: 软连接 和 快捷方式 不是一个东西， 比如一个依赖其他dll的exe创建的软连接就不能独立执行
:: mklink 默认产生文件的软连接， mklink /?帮助
:: mklink %dest% %src%

:: 快捷方式 https://superuser.com/questions/455364/how-to-create-a-shortcut-using-a-batch-script
set SCRIPT="%TEMP%\%RANDOM%-%RANDOM%-%RANDOM%-%RANDOM%.vbs"

echo Set oWS = WScript.CreateObject("WScript.Shell") >> %SCRIPT%
echo sLinkFile = "%dest%.lnk" >> %SCRIPT%
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%
echo oLink.TargetPath = "%src%" >> %SCRIPT%
echo oLink.Description = "快捷方式"
echo oLink.Save >> %SCRIPT%

cscript /nologo %SCRIPT%
del %SCRIPT%

echo Press any key to exit
pause
