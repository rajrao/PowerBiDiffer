using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerBiDiffer
{
    public class JsonProcessor : IExtractText
    {
        public string ExtractTextFromFile(string filePath, ExtractTextOptions extractTextOptions = null)
        {
            var fileIsNull = string.Equals(filePath, "nul", StringComparison.OrdinalIgnoreCase);
            if (fileIsNull)
                return string.Empty;

            using var textStream = File.OpenText(filePath);
            using var jsonTextReader = new JsonTextReader(textStream) { DateParseHandling = DateParseHandling.None };
            var jToken = JToken.ReadFrom(jsonTextReader);
            var indentedJson = jToken.ToString(Newtonsoft.Json.Formatting.Indented);
            var sanitizedText = indentedJson.Replace("\\r\\n", Environment.NewLine).Replace("\\\"", "\"");
            return sanitizedText;
        }
    }
}