:: https://www.jianshu.com/p/48c71e7359d3
:: https://www.cnblogs.com/DswCnblog/p/5432326.html

@echo off
set curPath=%~dp0
echo curPath = %curPath%s

set sourcePath=%curPath%..\Resource\Protos
echo sourcePath = %sourcePath%

set destPath=%curPath%..\Server\Server\Protos
echo destPath = %destPath%

for %%i in (%sourcePath%\*.proto) do (
	%curPath%Network\protoc_original\bin\protoc.exe -I=%sourcePath% --csharp_out=%destPath% %%i
)

pause
