#Commands#

textconv "..\..\..\..\TestPBix\testA.json"

#visualStudio 2019
	difftool "..\..\..\..\TestPBix\testA.json" "..\..\..\..\TestPBix\testb.json" -d "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe" -a "/diff \"{lp}\" \"{rp}\" \"{ln}\" \"{rn}\""

	GIT:
		\"c:\\PowerBiDiffer\\PowerBiDiffer.exe\" difftool "$LOCAL" "$REMOTE" -d \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\devenv.exe\" -a '/diff \"{lp}\" \"{rp}\" \"{ln}\" \"{rn}\"' -v
		
#WinMerge#
	difftool "..\..\..\..\TestPBix\testA.json" "..\..\..\..\TestPBix\testb.json" -d "C:\Program Files (x86)\WinMerge\winmergeu.exe" -a "/xq /e /s /dl \"{ln}\" /dr \"{rn}\" \"{lp}\" \"{rp}\"" -v



#Post Deploy#
xcopy "$(ProjectDir)$(OutDir)" "c:\PowerBiDiffer\" /i/d/y