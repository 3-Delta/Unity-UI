::https://zhuanlan.zhihu.com/p/94197105
::Create Dir link

set LINK_DIR=..\__MultiUnityEditor__

rmdir %LINK_DIR%
mkdir %LINK_DIR%

rmdir %LINK_DIR%\Assets
rmdir %LINK_DIR%\ProjectSettings
rmdir %LINK_DIR%\Library

mklink /J %LINK_DIR%\Assets Assets
mklink /J %LINK_DIR%\ProjectSettings ProjectSettings
mklink /J %LINK_DIR%\Library Library

pause