using System;
using System.Collections.Generic;
using System.IO;

namespace PowerBiDiffer
{
    class App
    {
        public static void ExecuteComparison(AppOptionsDiffTool appOptionsDiffTool)
        {   
            if (string.Equals(appOptionsDiffTool.LocalFile, "nul", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(appOptionsDiffTool.RemoteFile, "nul", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var localFileName = Path.GetFileNameWithoutExtension(appOptionsDiffTool.LocalFile);
            var remoteFileName = Path.GetFileNameWithoutExtension(appOptionsDiffTool.RemoteFile);
            var extensionLocalFile = Path.GetExtension(appOptionsDiffTool.LocalFile);
            var extensionRemoteFile = Path.GetExtension(appOptionsDiffTool.RemoteFile);

            string sanitizedLocalFilePath;
            string sanitizedRemoteFilePath;
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

                    var sanitizedTextLocal = processor.ExtractTextFromFile(appOptionsDiffTool.LocalFile, new ExtractTextOptions{IncludeMetaData = true});
                    sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".txt");

                    var sanitizedTextRemote = processor.ExtractTextFromFile(appOptionsDiffTool.RemoteFile, new ExtractTextOptions { IncludeMetaData = true });
                    sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".txt");
                }
                else if (isJson)
                {
                    IExtractText processor = new JsonProcessor();
                    var sanitizedTextLocal = processor.ExtractTextFromFile(appOptionsDiffTool.LocalFile);
                    sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".json");

                    var sanitizedTextRemote = processor.ExtractTextFromFile(appOptionsDiffTool.RemoteFile);
                    sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".json");
                }
                else
                {
                    sanitizedLocalFilePath = appOptionsDiffTool.LocalFile;
                    sanitizedRemoteFilePath = appOptionsDiffTool.LocalFile;
                }
            }
            else
            {
                sanitizedLocalFilePath = appOptionsDiffTool.LocalFile;
                sanitizedRemoteFilePath = appOptionsDiffTool.RemoteFile;
            }

            
            var diffTool = appOptionsDiffTool.DiffTool;
            Dictionary<string, string> templateData = new Dictionary<string, string>(){
                {"{localFilePath}", sanitizedLocalFilePath},
                {"{remoteFilePath}", sanitizedRemoteFilePath},
                {"{localFileName}", localFileName},
                {"{remoteFileName}", remoteFileName},
                {"{lp}", sanitizedLocalFilePath},
                {"{rp}", sanitizedRemoteFilePath},
                {"{ln}", localFileName},
                {"{rn}", remoteFileName},
            };
            var diffArguments = appOptionsDiffTool.DiffToolArguments.InstantiateTemplate(templateData);
            
            if (appOptionsDiffTool.Verbose)
            {
                Console.WriteLine($"DiffTool: {diffTool}");
                Console.WriteLine($"DiffTool Args: {diffArguments}");
                Console.WriteLine($"CommandLine: {diffTool} {diffArguments}");
            }
            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
            {
                pProcess.StartInfo.FileName = diffTool;
                pProcess.StartInfo.Arguments = diffArguments;
                var processStarted = pProcess.Start();
                if (!processStarted && appOptionsDiffTool.Verbose)
                {
                    Console.WriteLine("Process was not started!");
                }
            }
        }

        public static void ConvertToText(string filePath)
        {
            var isJson = string.Compare(
                Path.GetExtension(filePath),
                ".json", StringComparison.OrdinalIgnoreCase) == 0;
            var isPbix = string.Compare(
                Path.GetExtension(filePath),
                ".pbix", StringComparison.OrdinalIgnoreCase) == 0;
            if (isPbix)
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