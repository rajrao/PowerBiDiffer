using CommandLine;

namespace PowerBiDiffer
{
    [Verb("difftool", HelpText = "Mode: Diff Tool")]
    public class AppOptionsDiffTool: AppOptions
    {
        [Value(0, Required = true, HelpText = "Local file")]
        public string LocalFile { get; set; }
        
        [Value(1, Required = false, HelpText = "Remote file")]
        public string RemoteFile { get; set; }

        [Option('d', longName: "difftool", Required = false, HelpText = "Diff Tool Path")]
        public string DiffTool { get; set; }

        [Option('a', longName: "args", Required = false, HelpText = "Diff Tool arguments. " +
                                                                    "Supported templates: " +
                                                                    "{localFilePath} or {lp}: Local file path" +
                                                                    "{remoteFilePath} or {rp}: Remote file path" +
                                                                    "{localFileName} or {ln}: local file name (for description)" +
                                                                    "{remoteFileName} or {rn}: remote file name (for description)")]
        public string DiffToolArguments { get; set; }
    }
}