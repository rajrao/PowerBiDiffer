using CommandLine;

namespace PowerBiDiffer
{
    public abstract class AppOptions
    {
        [Option('v', Default = false, HelpText = "Verbose")]
        public bool Verbose { get; set; }
    }
}