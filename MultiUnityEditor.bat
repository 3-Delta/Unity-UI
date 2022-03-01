::https://zhuanlan.zhihu.com/p/94197105
::Create Dir link

set LINK_DIR=..\__MultiUnityEditor__

if exist %LINK_DIR% (
	rd /s/q %LINK_DIR%
)

md %LINK_DIR%

::rmdir %LINK_DIR%
::mkdir %LINK_DIR%

mklink /J %LINK_DIR%\Assets Assets
mklink /J %LINK_DIR%\ProjectSettings ProjectSettings
mklink /J %LINK_DIR%\Library Library

pause