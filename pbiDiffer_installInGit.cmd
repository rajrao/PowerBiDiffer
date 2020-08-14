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

:END
echo on
::pause
:: @exit /b !errorlevel!