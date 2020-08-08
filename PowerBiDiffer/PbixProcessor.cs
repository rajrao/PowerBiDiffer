using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Packaging;
using System.Text;

namespace PowerBiDiffer
{
    public class PbixProcessor : IExtractText
    {
        public string ExtractTextFromFile(string filePath, ExtractTextOptions extractTextOptions)
        {
            var fileIsNull = string.Equals(filePath, "nul", StringComparison.OrdinalIgnoreCase);
            if (fileIsNull)
                return string.Empty;

            //https://www.fourmoo.com/2017/05/02/what-makes-up-a-power-bi-desktop-pbix-file/
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.packaging.package.open?view=dotnet-plat-ext-3.1

            string sanitizedText = string.Empty;
            using (Package package =
                Package.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var dataMashupPart = package.GetPart(new Uri("/DataMashup", UriKind.Relative));
                using var dataMashupStream = dataMashupPart.GetStream();
                var dataMashupParts = GetPackagePartContents(dataMashupPart, new List<string> { "/Formulas/Section1.m"});

                if (extractTextOptions != null && extractTextOptions.IncludeMetaData)
                {
                    sanitizedText = dataMashupParts["/Formulas/Section1.m"]
                                    + Environment.NewLine + Environment.NewLine
                                    + "-------------- Mashup Metadata ------------------"
                                    + Environment.NewLine + Environment.NewLine
                                    + dataMashupParts["DataMashup.Metadata"];

                }
                else
                {
                    sanitizedText = dataMashupParts["/Formulas/Section1.m"];
                }
            }

            return sanitizedText;
        }

        private StringDictionary GetPackagePartContents(PackagePart package, List<string> packagePartPaths)
        {
            StringDictionary packagePartContents = new StringDictionary();
            using var packageStream = package.GetStream();
            using BinaryReader packageStreamReader = new BinaryReader(packageStream, Encoding.Default, true);
            {
                //ms-qdeff - query definition file format
                //https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-qdeff/27b1dd1e-7de8-45d9-9c84-dfcc7a802e37
                //4 bytes = version
                //4 bytes = length of PackageParts
                //length of PackageParts bytes = PackageParts
                //4 bytes = length of permissions
                //length of permissions bytes  = permissions
                //4 bytes = length of metadata
                //length of metadata = metadata
                //4 bytes = length of permissions bindings
                //length of permissions bindings = permissions bindings

                int version = packageStreamReader.ReadInt32();
                int packagePartsLength = packageStreamReader.ReadInt32();
                var packagePartsBytes = packageStreamReader.ReadBytes(packagePartsLength);
                var permissionsLength = packageStreamReader.ReadInt32();
                packageStreamReader.ReadBytes(permissionsLength);
                var metaDataLength = packageStreamReader.ReadInt32();
                var metaDataBytes = packageStreamReader.ReadBytes(metaDataLength);

                using var packagePartsStream = packagePartsBytes.ToMemoryStream();
                using (var internalPackage = Package.Open(packagePartsStream, FileMode.Open, FileAccess.Read))
                {
                    foreach (var packagePartPath in packagePartPaths)
                    {
                        using var partStream = internalPackage.GetPart(new Uri(packagePartPath, UriKind.Relative))?.GetStream();
                        var partStreamAsString = partStream?.ReadString();
                        packagePartContents.Add(packagePartPath, partStreamAsString);
                    }
                }

                //metadata
                //4 bytes = version
                //4 bytes = metadata length
                //metadata length bytes = metadata XML
                //4 bytes = content length
                //content length bytes = content (OPC package)

                using var metaDataStream = metaDataBytes.ToMemoryStream();
                using var metaDataBinaryStream = new BinaryReader(metaDataStream);
                var metadataVersion = metaDataBinaryStream.ReadInt32();
                var metaDataXmlLength = metaDataBinaryStream.ReadInt32();
                var metaDataXmlBytes = metaDataBinaryStream.ReadBytes(metaDataXmlLength);
                var metaData = metaDataXmlBytes.ToMemoryStream().ReadString();
                var metaDataXml = HelperExtensions.PrettyPrintXml(metaData);

                packagePartContents.Add("DataMashup.Metadata", metaDataXml);
            }

            return packagePartContents;
        }
    }
}