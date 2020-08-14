@echo off

SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
SET me=%~n0
SET parent=%~dp0

cls
echo uinstalling PowerBiDiffer from global git config

git config --global --unset diff.tool 
git config --global --unset diff.guitool
git config --global --unset difftool.prompt false
git config --global --unset difftool.PowerBiDiffer
git config --global --unset diff.json.textconv
git config --global --unset diff.pbix.textconv
git config --global --unset difftool.PowerBiDiffer.cmd
git config --global --unset difftool.PowerBiDiffer.keepbackup

echo Completed uninstalling a difftool successfully!

:END
echo on
