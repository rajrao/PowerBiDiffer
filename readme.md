![.NET Core](https://github.com/rajrao/PowerBiDiffer/workflows/.NET%20Core/badge.svg)

# ReadMe #

## Introduction ##
1. The tool can be run on its own and has 2 modes: textconv and difftool.

1. Textconv takes a single file as input and spits out to std output the text.

1. The difftool takes 2 files as input and converts them to text and invokes the diff tool specified on the command line to run.

## Examples ##

1. spit out the json unminified and \r\n replaced with newlines

		textconv "testA.json"

2. Use a tool to display the differences

	### Visual Studio ###

		difftool "testA.json" "testb.json" -d "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe" -a "/diff \"{lp}\" \"{rp}\" \"{ln}\" \"{rn}\""

	### WinMerge ###

		difftool "testA.json" "testb.json" -d "C:\Program Files (x86)\WinMerge\winmergeu.exe" -a "/xq /e /s /dl \"{ln}\" /dr \"{rn}\" \"{lp}\" \"{rp}\"" -v


## GIT ##

1. Add a .gitattributes file if you dont already have one (this is located at the root of your repo)

1. Add the following text to the .gitattributes file

		*.PBIX   diff=pbix
		*.pbix   diff=pbix
		*.json	 diff=json
		*.JSON	 diff=json

1. Open the config file in the .git folder of your repo

1. Add the following code: *(Make sure you update the location to where you have put PowerBiDiffer. Also update path to Visual Studio. if you instead want to use WinMerge, replace commands appropriates)*

		[diff]
			tool = PowerBiDiffer
			guitool = PowerBiDiffer
		[difftool]
			prompt = false
		[difftool "PowerBiDiffer"]
			cmd = \"c:\\PowerBiDiffer\\PowerBiDiffer.exe\" difftool "$LOCAL" "$REMOTE" -d \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\devenv.exe\" -a '/diff \"{lp}\" \"{rp}\" \"{ln}\" \"{rn}\"'
			keepBackup = false
		[diff "json"]
			textconv = \"c:\\PowerBiDiffer\\PowerBiDiffer.exe\" textconv
		[diff "pbix"]
			textconv = \"c:\\PowerBiDiffer\\PowerBiDiffer.exe\" textconv

1. Save the file

1. Test it using the following commands:

	#### Uses the command line and Gits diff tool ####
	
		git diff *.pbix
		
		or

		git diff *.json


		or 
	#### Uses the custom tool for displaying difference ####

		git difftool *.pbix

		git difftool *.json
