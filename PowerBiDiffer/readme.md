

config

[diff]
	tool = vsdiffmerge
	guitool = vsdiffmerge
[difftool]
	prompt = false
[difftool "vsdiffmerge"]
	#cmd = \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\vsdiffmerge.exe\" \"$LOCAL\" \"$REMOTE\" //t
	cmd = \"C:\\Users\\rrao\\source\\repos\\GitHub\\PowerBiDiffer\\PowerBiDiffer\\bin\\Debug\\netcoreapp3.1\\PowerBiDiffer.exe\" -diff \"$LOCAL\" \"$REMOTE\"
	keepBackup = false
[diff "json"]
	textconv = \"C:\\Users\\rrao\\source\\repos\\GitHub\\PowerBiDiffer\\PowerBiDiffer\\bin\\Debug\\netcoreapp3.1\\PowerBiDiffer.exe\" -textconv
[diff "pbix"]
	textconv = \"C:\\Users\\rrao\\source\\repos\\GitHub\\PowerBiDiffer\\PowerBiDiffer\\bin\\Debug\\netcoreapp3.1\\PowerBiDiffer.exe\" -textconv
	#tool = vsdiffmerge
	#guitool = vsdiffmerge
	#binary = true





.gitarttibutes

*.PBIX   diff=pbix
*.pbix   diff=pbix
*.json	 diff=json
*.JSON	 diff=json