config

[diff]
	tool = vsdiffmerge
	guitool = vsdiffmerge
[difftool]
	prompt = false
[difftool "vsdiffmerge"]
	#cmd = \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\vsdiffmerge.exe\" \"$LOCAL\" \"$REMOTE\" //t
	cmd = \"C:\\Junk\\Sources\\Test\\ConsoleApp1\\ConsoleApp1\\bin\\Debug\\netcoreapp3.1\\consoleapp1.exe\" -diff \"$LOCAL\" \"$REMOTE\"
	keepBackup = false
[diff "json"]
	textconv = \"C:\\Junk\\Sources\\Test\\ConsoleApp1\\ConsoleApp1\\bin\\Debug\\netcoreapp3.1\\consoleapp1.exe\" -textconv
[diff "pbix"]
	textconv = \"C:\\Junk\\Sources\\Test\\ConsoleApp1\\ConsoleApp1\\bin\\Debug\\netcoreapp3.1\\consoleapp1.exe\" -textconv
	tool = vsdiffmerge
	guitool = vsdiffmerge
	binary = true





.gitarttibutes

*.PBIX   diff=pbix
*.pbix   diff=pbix
*.json	 diff=json
*.JSON	 diff=json