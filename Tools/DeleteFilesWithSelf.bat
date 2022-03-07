echo off

rem cd .\ccc

set curPath=%~dp0
echo curPath = %curPath%

for %%i in (*.*) do (
	del %%i
)

pause
