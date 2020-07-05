![.NET Core](https://github.com/rajrao/PowerBiDiffer/workflows/.NET%20Core/badge.svg)

# ReadMe #

## Introduction ##

This tool can be used to extract and display the power query mashup formulas in use by your PBIX file. Its primary purpose is to work with #GIT, but can be used standalone to. There are 2 ways in which it can be used with GIT: using GIT's differencing engine using **textconv** driver and using an external tool (such as visual studio or WinMerge). In addition, the tool also can be used with JSON files. This allows you to difference dataflow JSON files that you might have exported out of PowerBi online.

**This tool does not allow you to perform merges and using it for merges will likely cause corruption of your files.** Its intended usage it to perform comparison between different versions to make sure the changes you are checking into source control are what you intended to apply. It currently does not have the ability to compare measures and columns added to the datamodel via DAX.

## PBIX ##

When used with PBIX, the tool extracts the mashup formulas in your PBIX file. If used with the textconv driver, the tool extracts the mashup and outputs it to standard output (console). This is done, because GIT uses the std-output and extracts the text and performs its differencing using its enternal diff engine. This allows you to use the [*git diff*](https://git-scm.com/docs/git-diff) command with its various options. When used with an external tool like VisualStudio, the tool extracts the mashup from the 2 input files and saves them to temp files, the temp files are provided to the external tool to perform its differencing.

# Instructions #

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
		
	*to exit out of the diff mode hit q*

		or 
		
	#### Uses the custom tool for displaying difference ####

		git difftool *.pbix

		git difftool *.json

1. Other useful GIT commands 

	Display the difference between 2 commits (all of these commands can be used with just *git diff* too).
	
		git difftool 3208 fc4b
		
	Display the difference between commit 3208 and HEAD
	
		git difftool 3208 HEAD
		
	Display the difference between current commit and the previous commit (use ^ to signify how far back)
	
		git difftool HEAD^ HEAD
		
	Display the difference for only files with extension \*.json
	
		git difftool *.json

## Standalone Usage ##

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
