using System;
using System.IO;

namespace PowerBiDiffer
{
    class App
    {
        public static void ExecuteComparison(string localFilePath, string remoteFilePath)
        {
            var diffToolVsPath =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe";
            var argumentsVs = "/diff";
            var diffToolPbix = @"C:\Program Files (x86)\WinMerge\WinMergeU.exe";
            var argumentsPbix = "/e /s";
            //var diffToolPath =
            //    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\vsdiffmerge.exe";
            //var arguments = "/t";

            if (string.Equals(localFilePath, "nul", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(remoteFilePath, "nul", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var localFileName = Path.GetFileNameWithoutExtension(localFilePath);
            var remoteFileName = Path.GetFileNameWithoutExtension(remoteFilePath);
            var extensionLocalFile = Path.GetExtension(localFilePath);
            var extensionRemoteFile = Path.GetExtension(remoteFilePath);

            string sanitizedLocalFilePath;
            string sanitizedRemoteFilePath;
            string diffTool;
            string diffArguments;
            if (string.Equals(extensionLocalFile, extensionRemoteFile, StringComparison.OrdinalIgnoreCase))
            {
                var isJson = string.Compare(
                    extensionLocalFile,
                    ".json", StringComparison.OrdinalIgnoreCase) == 0;
                var isPbix = string.Compare(
                    extensionLocalFile,
                    ".pbix", StringComparison.OrdinalIgnoreCase) == 0;
                
                if (isPbix)
                {
                    IExtractText processor = new PbixProcessor();

                    var sanitizedTextLocal = processor.ExtractTextFromFile(localFilePath);
                    sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".txt");

                    var sanitizedTextRemote = processor.ExtractTextFromFile(remoteFilePath);
                    sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".txt");

                    diffTool = diffToolPbix;
                    diffArguments =
                        $@"{argumentsPbix} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" /dl ""{localFileName}"" /dr ""{remoteFileName}""";
                }
                else if (isJson)
                {
                    IExtractText processor = new JsonProcessor();
                    var sanitizedTextLocal = processor.ExtractTextFromFile(localFilePath);
                    sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".json");

                    var sanitizedTextRemote = processor.ExtractTextFromFile(remoteFilePath);
                    sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".json");

                    //diffTool = diffToolVsPath;
                    //diffArguments =
                    //    $@"{argumentsVs} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" ""{localFileName}"" ""{remoteFileName}""";

                    diffTool = diffToolPbix;
                    diffArguments =
                        $@"{argumentsPbix} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" /dl ""{localFileName}"" /dr ""{remoteFileName}""";
                }
                else
                {
                    sanitizedLocalFilePath = localFilePath;
                    sanitizedRemoteFilePath = remoteFilePath;

                    diffTool = diffToolVsPath;
                    diffArguments =
                        $@"{argumentsVs} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" ""{localFileName}"" ""{remoteFileName}""";
                }
            }
            else
            {
                sanitizedLocalFilePath = localFilePath;
                sanitizedRemoteFilePath = remoteFilePath;

                diffTool = diffToolVsPath;
                diffArguments =
                    $@"{argumentsVs} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" ""{localFileName}"" ""{remoteFileName}""";
            }

            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
            {
                pProcess.StartInfo.FileName = diffTool;
                pProcess.StartInfo.Arguments = diffArguments;
                pProcess.Start();
            }
        }

        public static void ConvertToText(string filePath)
        {
            var isJson = string.Compare(
                Path.GetExtension(filePath),
                ".json", StringComparison.OrdinalIgnoreCase) == 0;
            var isPBIX = string.Compare(
                Path.GetExtension(filePath),
                ".pbix", StringComparison.OrdinalIgnoreCase) == 0;
            if (isPBIX)
            {
                IExtractText processor = new PbixProcessor();
                var sanitizedText = processor.ExtractTextFromFile(filePath);
                Console.WriteLine(sanitizedText);
            }
            else if (isJson)
            {
                IExtractText processor = new JsonProcessor();
                var sanitizedText = processor.ExtractTextFromFile(filePath);
                Console.WriteLine(sanitizedText);
            }
            else
            {
                var fileText = File.ReadAllText(filePath);
                Console.WriteLine(fileText);
            }
        }

        private static string WriteToTemp(string sanitizedText, string extension = null)
        {
            var tempFile = Path.GetTempFileName();
            if (!string.IsNullOrEmpty(extension))
            {
                tempFile = Path.ChangeExtension(tempFile, extension);
            }
            File.WriteAllText(tempFile, sanitizedText);
            return tempFile;
        }
    }
}