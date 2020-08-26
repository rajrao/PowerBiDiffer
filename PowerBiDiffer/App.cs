using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PowerBiDiffer
{
    class App
    {
        public static void ExecuteComparison(AppOptionsDiffTool appOptionsDiffTool)
        {
            var localFileName = Path.GetFileNameWithoutExtension(appOptionsDiffTool.LocalFile);
            var remoteFileName = Path.GetFileNameWithoutExtension(appOptionsDiffTool.RemoteFile);
            var extensionLocalFile = Path.GetExtension(appOptionsDiffTool.LocalFile);
            var extensionRemoteFile = Path.GetExtension(appOptionsDiffTool.RemoteFile);

            var localFileIsNull = string.Equals(appOptionsDiffTool.LocalFile, "nul", StringComparison.OrdinalIgnoreCase);
            var remoteFileIsNull = string.Equals(appOptionsDiffTool.RemoteFile, "nul", StringComparison.OrdinalIgnoreCase);

            string sanitizedLocalFilePath = appOptionsDiffTool.LocalFile;
            string sanitizedRemoteFilePath = appOptionsDiffTool.RemoteFile;
            
            var localIsJson = FileIsJson(extensionLocalFile);
            var remoteIsJson = FileIsJson(extensionRemoteFile);
            var localIsPbix = string.Equals(
                extensionLocalFile,
                ".pbix", StringComparison.OrdinalIgnoreCase);
            var remoteIsPbix = string.Equals(
                extensionRemoteFile,
                ".pbix", StringComparison.OrdinalIgnoreCase);

            IExtractText pbixProcessor = new PbixProcessor();
            IExtractText jsonProcessor = new JsonProcessor();
            if (localIsPbix)
            {
                var sanitizedTextLocal = pbixProcessor.ExtractTextFromFile(appOptionsDiffTool.LocalFile, new ExtractTextOptions { IncludeMetaData = true });
                sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".txt");
            }
            else if (localIsJson)
            {
                var sanitizedTextLocal = jsonProcessor.ExtractTextFromFile(appOptionsDiffTool.LocalFile);
                sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, appOptionsDiffTool.TreatJsonAsJson ? ".json": ".txt");
            }
            else if (localFileIsNull)
            {
                sanitizedLocalFilePath = WriteToTemp(string.Empty, ".txt");
            }
            else if (appOptionsDiffTool.LocalFile.Contains(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
            {
                sanitizedLocalFilePath = Path.GetTempFileName();
                File.Copy(appOptionsDiffTool.LocalFile, sanitizedLocalFilePath, true);
            }

            if (remoteIsPbix)
            {
                var sanitizedTextRemote = pbixProcessor.ExtractTextFromFile(appOptionsDiffTool.RemoteFile,
                    new ExtractTextOptions { IncludeMetaData = true });
                sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".txt");
            }
            else if (remoteIsJson)
            {
                var sanitizedTextRemote = jsonProcessor.ExtractTextFromFile(appOptionsDiffTool.RemoteFile);
                sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, appOptionsDiffTool.TreatJsonAsJson ? ".json" : ".txt");
            }
            else if (remoteFileIsNull)
            {
                sanitizedRemoteFilePath = WriteToTemp(string.Empty, ".txt");
            }
            else if (appOptionsDiffTool.RemoteFile.Contains(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
            {
                sanitizedRemoteFilePath = Path.GetTempFileName();
                File.Copy(appOptionsDiffTool.RemoteFile, sanitizedRemoteFilePath, true);
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

        private static bool FileIsJson(string extensionLocalFile)
        {
            return string.Equals(
                extensionLocalFile,
                ".json", StringComparison.OrdinalIgnoreCase) || string.Equals(
                extensionLocalFile,
                ".ipynb", StringComparison.OrdinalIgnoreCase);
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