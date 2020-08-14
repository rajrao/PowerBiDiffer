@echo off

SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
SET me=%~n0
SET parent=%~dp0

cls
set InstallDir=""
for /f "usebackq tokens=*" %%i in (`vswhere.exe -latest -property productPath`) do (
  set InstallDir=%%i
)

echo installing PowerBiDiffer into global git config

git config --global diff.tool PowerBiDiffer
git config --global diff.guitool PowerBiDiffer
git config --global difftool.prompt false
git config --global difftool.PowerBiDiffer.keepbackup false
git config --global diff.json.textconv "\"%cd%\PowerBiDiffer.exe\" textconv"
git config --global diff.pbix.textconv "\"%cd%\PowerBiDiffer.exe\" textconv"

git config --global difftool.PowerBiDiffer.cmd "\"%cd%\PowerBiDiffer.cmd\" \"$LOCAL\" \"$REMOTE\"


if exist "!InstallDir!" (
	echo Using visual studio as diffTool from !InstallDir!

	git config --global difftool.PowerBiDiffer.cmd "\"%cd%\PowerBiDiffer.exe\" difftool \"$LOCAL\" \"$REMOTE\" -d \"!InstallDir!\" -a '/diff \"{lp}\" \"{rp}\" \"{ln}\" \"{rn}\"'"
	
	echo visual studio installed as diffTool

	goto SUCCESS
)

if exist "C:\Program Files (x86)\WinMerge\winmergeu.exe" (

	echo Visual Studio not found, using WinMerge
	set "winMergePath=C:\Program Files (x86)\WinMerge\winmergeu.exe"
		
	git config --global difftool.PowerBiDiffer.cmd "\"%cd%\PowerBiDiffer.exe\" difftool \"$LOCAL\" \"$REMOTE\" -d \"!winMergePath!\" -a '/xq /e /s /dl \"{ln}\" /dr \"{rn}\" \"{lp}\" \"{rp}\"'"
	
	echo WinMerge installed as diffTool
	
	goto SUCCESS
)

:FAILURE
echo DiffTool was not completed successfully, as VisualStudio, nor WinMerge were found

:SUCCESS
echo Completed installing a difftool successfully!

:END
echo on
::pause
:: @exit /b !errorlevel!