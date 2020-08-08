using CommandLine;

namespace PowerBiDiffer
{
    public abstract class AppOptions
    {
        [Option('v', Default = false, HelpText = "Verbose")]
        public bool Verbose { get; set; }

        [Option('l', Default = false, HelpText = "Break into debugger")]
        public bool LaunchDebugger { get; set; }


        [Option('j', Default = true, HelpText = "Treat json files as JSON and not as text")]
        public bool TreatJsonAsJson { get; set; }
    }
}