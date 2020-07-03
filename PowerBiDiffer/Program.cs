using System;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerBiDiffer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                throw new ArgumentException("Args cannot be empty");
            }

            var type = args[0];

            if (type.EndsWith("-textconv", StringComparison.OrdinalIgnoreCase))
            {
                var filePath = args[1];
                var isJson = string.Compare(
                    Path.GetExtension(filePath),
                    ".json", StringComparison.OrdinalIgnoreCase) == 0;
                var isPBIX = string.Compare(
                    Path.GetExtension(filePath),
                    ".pbix", StringComparison.OrdinalIgnoreCase) == 0;
                if (isPBIX)
                {
                    var sanitizedText = ExtractTextFromPbix(filePath);
                    Console.WriteLine(sanitizedText);
                }
                else if (isJson)
                {
                    var sanitizedText = ExtractTextFromJson(filePath);
                    Console.WriteLine(sanitizedText);
                }
                else
                {
                    var fileText = File.ReadAllText(filePath);
                    Console.WriteLine(fileText);
                }
            }
            else if (type.EndsWith("-diff", StringComparison.OrdinalIgnoreCase))
            {
                var diffToolVsPath =
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe";
                var argumentsVs = "/diff";
                var diffToolPbix = @"C:\Program Files (x86)\WinMerge\WinMergeU.exe";
                var argumentsPbix = "/e /s";
                //var diffToolPath =
                //    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\vsdiffmerge.exe";
                //var arguments = "/t";

                var localFilePath = args[1];
                if (string.Equals(localFilePath, "nul", StringComparison.OrdinalIgnoreCase))
                {
                    localFilePath = string.Empty;
                }
                var localFileName = Path.GetFileNameWithoutExtension(localFilePath);
                var remoteFilePath = args[2];
                if (string.Equals(remoteFilePath, "nul", StringComparison.OrdinalIgnoreCase))
                {
                    remoteFilePath = string.Empty;
                }

                if (string.IsNullOrEmpty(localFilePath) || string.IsNullOrEmpty(remoteFilePath))
                {
                    return;
                }
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
                    var isPBIX = string.Compare(
                        extensionLocalFile,
                        ".pbix", StringComparison.OrdinalIgnoreCase) == 0;
                    Console.WriteLine($"Json:{isJson} Pbix:{isPBIX}");
                    Console.WriteLine($"localFilePath:{localFilePath} remoteFilePath:{remoteFilePath}");
                    if (isPBIX)
                    {
                        var sanitizedTextLocal = ExtractTextFromPbix(localFilePath);
                        sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".txt");

                        var sanitizedTextRemote = ExtractTextFromPbix(remoteFilePath);
                        sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".txt");

                        diffTool = diffToolPbix;
                        diffArguments = $@"{argumentsPbix} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" /dl ""{localFileName}"" /dr ""{remoteFileName}""";
                    }
                    else if (isJson)
                    {
                        var sanitizedTextLocal = ExtractTextFromJson(localFilePath);
                        sanitizedLocalFilePath = WriteToTemp(sanitizedTextLocal, ".json");

                        var sanitizedTextRemote = ExtractTextFromJson(remoteFilePath);
                        sanitizedRemoteFilePath = WriteToTemp(sanitizedTextRemote, ".json");

                        //diffTool = diffToolVsPath;
                        //diffArguments =
                        //    $@"{argumentsVs} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" ""{localFileName}"" ""{remoteFileName}""";

                        diffTool = diffToolPbix;
                        diffArguments = $@"{argumentsPbix} ""{sanitizedLocalFilePath}"" ""{sanitizedRemoteFilePath}"" /dl ""{localFileName}"" /dr ""{remoteFileName}""";
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

                Console.WriteLine(diffTool);
                Console.WriteLine(diffArguments);
                using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                {
                    pProcess.StartInfo.FileName = diffTool;
                    pProcess.StartInfo.Arguments = diffArguments;
                    //pProcess.StartInfo.UseShellExecute = false;
                    //pProcess.StartInfo.RedirectStandardOutput = true;
                    //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                    Console.WriteLine("Starting process");
                    bool processStarted = pProcess.Start();
                    Console.WriteLine($"Process Started Status: {processStarted}");
                }

                //throw new Exception($"this is working {args[1]} {args[2]} {Directory.GetCurrentDirectory()}");
            }
            else
            {
                throw new Exception($"this is an error");
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

        private static string ExtractTextFromJson(string filePath)
        {
            using var textStream = File.OpenText(filePath);
            using var jsonTextReader = new JsonTextReader(textStream) { DateParseHandling = DateParseHandling.None };
            JsonLoadSettings loadSettings = new JsonLoadSettings
            {
                DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
            };
            var jToken = JToken.ReadFrom(jsonTextReader, loadSettings);
            var indentedJson = jToken.ToString(Newtonsoft.Json.Formatting.Indented);
            var sanitizedText = indentedJson.Replace("\\r\\n", Environment.NewLine).Replace("\\\"", "\"");
            return sanitizedText;
        }

        private static string ExtractTextFromPbix(string filePath)
        {
            string sanitizedText = string.Empty;
            using (Package package =
                Package.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var dataMashupPart = package.GetPart(new Uri("/DataMashup", UriKind.Relative));
                using var dataMashupStream = dataMashupPart.GetStream();
                using BinaryReader reader1 = new BinaryReader(dataMashupStream, Encoding.Default, true);
                {
                    //ms-qdeff - query definition file format
                    //https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-qdeff/27b1dd1e-7de8-45d9-9c84-dfcc7a802e37
                    //first 4 bytes = version
                    //next 4 bytes = length of parts
                    //use length of parts to ready bytes for next stream

                    int version = (int)reader1.ReadUInt32();
                    int packagePartsLength = (int)reader1.ReadUInt32();
                    var packageParts = reader1.ReadBytes(packagePartsLength);
                    var packagePartsStream = ToMemoryStream(packageParts);
                    using (var internalPackage = Package.Open(packagePartsStream, FileMode.Open, FileAccess.Read))
                    {
                        using var formulaStream = internalPackage.GetPart(new Uri("/Formulas/Section1.m", UriKind.Relative)).GetStream();
                        sanitizedText = ReadString(formulaStream);
                    }
                }
            }

            return sanitizedText;
        }

        static MemoryStream ToMemoryStream(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes.Length);
            stream.Write((ReadOnlySpan<byte>)bytes);
            stream.Position = 0L;
            return stream;
        }

        static string ReadString(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream, true))
                return streamReader.ReadToEnd();
        }
    }
}