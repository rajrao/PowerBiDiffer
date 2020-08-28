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

echo creating powerbidiffer.cmd File

(
    Echo ::Visual Studio
    Echo ::@"%cd%\powerbidiffer.exe" difftool %%1 %%2 -d "!InstallDir!" -a "/diff ""{lp}"" ""{rp}"" ""{ln}"" ""{rn}"""
    ECHO.
    ECHO ::VSCode
    ECHO ::@"%cd%\powerbidiffer.exe" difftool  %%1 %%2 -d "C:\Program Files\Microsoft VS Code\Code.exe"  -a -- "--diff ""{lp}"" ""{rp}"""
    ECHO.
    ECHO ::WinMerge
    ECHO @"%cd%\powerbidiffer.exe" difftool  %%1 %%2 -d "C:\Program Files (x86)\WinMerge\WinMergeU.exe" -a "/xq /e /s /dl ""{ln}"" /dr ""{rn}"" ""{lp}"" ""{rp}"""
) > "PowerBiDiffer.cmd"

(
    Echo git difftool -t PowerBiDiffer %1
) > "gitDiff.cmd"

:END
echo on
::pause
:: @exit /b !errorlevel!
