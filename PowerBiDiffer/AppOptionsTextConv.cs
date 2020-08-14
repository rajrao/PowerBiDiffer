using CommandLine;

namespace PowerBiDiffer
{
    [Verb("textconv", HelpText = "Mode: Text Conversion")]
    public class AppOptionsTextConv : AppOptions
    {
        private string _localFile;

        [Value(0, Required = true, HelpText = "Local file")]
        public string LocalFile
        {
            get => _localFile;
            set => _localFile = value?.Replace("/", "\\");
        }
    }
}