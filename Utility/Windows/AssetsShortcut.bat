:: batch命令

@echo off

set curPath=%~dp0

FolderShortcut.bat curPath\..\Assets curPath\..\AssetsBackup

pause 
