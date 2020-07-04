using CommandLine;

namespace PowerBiDiffer
{
    [Verb("textconv", HelpText = "Mode: Text Conversion")]
    public class AppOptionsTextConv : AppOptions
    {
        [Value(0,Required = true, HelpText = "Local file")]
        public string LocalFile { get; set; }
    }
}