::Visual Studio
::c:\powerbidiffer\powerbidiffer.exe difftool %1 %2 -d "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe" -a "/diff ""{lp}"" ""{rp}"" ""{ln}"" ""{rn}"""

::VSCode
::c:\powerbidiffer\powerbidiffer.exe difftool %1 %2 -d "C:\Program Files\Microsoft VS Code\Code.exe"  -a -- "--diff ""{lp}"" ""{rp}"""

::WinMerge
c:\powerbidiffer\powerbidiffer.exe difftool %1 %2 -d "C:\Program Files (x86)\WinMerge\WinMergeU.exe" -a "/xq /e /s /dl ""{ln}"" /dr ""{rn}"" ""{lp}"" ""{rp}"""


::code for global config
::cmd=\"C:\\PowerBiDiffer\\powerbidiffer.cmd\" \"$LOCAL\" \"$REMOTE\"
