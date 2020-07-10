using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PowerBiDiffer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            if (!ProcessCommandLineArgs(args, services))
            {
                return;
            }


            await using ServiceProvider serviceProvider = services.BuildServiceProvider();

            var appOptions = serviceProvider.GetService<AppOptions>();

            if (appOptions.Verbose)
            {
                Console.WriteLine("Args....");
                foreach (string arg in args)
                {
                    Console.WriteLine(arg);
                }
                Console.WriteLine("Args printed!");
            }

            switch (appOptions)
            {
                case AppOptionsDiffTool appOptionsDiffTool:
                    if (string.IsNullOrWhiteSpace(appOptionsDiffTool.LocalFile) || 
                        (!appOptionsDiffTool.LocalFile.Equals("nul", StringComparison.OrdinalIgnoreCase) && !File.Exists(appOptionsDiffTool.LocalFile)))
                    {
                        throw new ArgumentOutOfRangeException("LocalFile", "localfile cannot be empty and must exist!");
                    }
                    if (string.IsNullOrWhiteSpace(appOptionsDiffTool.RemoteFile) ||
                        (!appOptionsDiffTool.RemoteFile.Equals("nul", StringComparison.OrdinalIgnoreCase) && !File.Exists(appOptionsDiffTool.RemoteFile)))
                    {
                        throw new ArgumentOutOfRangeException("RemoteFile", "RemoteFile cannot be empty and must exist!");
                    }
                    if (string.IsNullOrWhiteSpace(appOptionsDiffTool.DiffTool) || !File.Exists(appOptionsDiffTool.DiffTool))
                    {
                        throw new ArgumentOutOfRangeException("DiffTool", "DiffTool cannot be empty and must exist!");
                    }

                    App.ExecuteComparison(appOptionsDiffTool);
                    break;
                case AppOptionsTextConv appOptionsTextConv:
                    if (string.IsNullOrWhiteSpace(appOptionsTextConv.LocalFile) || !File.Exists(appOptionsTextConv.LocalFile))
                    {
                        throw new ArgumentOutOfRangeException("LocalFile", "localfile cannot be empty and must exist!");
                    }
                    App.ConvertToText(appOptionsTextConv.LocalFile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(appOptions));
            }


        }

        /// <summary>
        /// Return false if errors were encountered
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool ProcessCommandLineArgs(string[] args, ServiceCollection services)
        {
            bool argumentsHasErrors = false;
            var parserResult = new Parser(config =>
                {
                    config.HelpWriter = null;
                    config.CaseSensitive = false;
                    config.AutoHelp = true;
                    config.IgnoreUnknownArguments = false;
                    config.EnableDashDash = true;
                })
                .ParseArguments<AppOptionsTextConv, AppOptionsDiffTool>(args); //list options classes

            parserResult.WithParsed(appOptions =>
                {
                    services.AddSingleton(appOptions as AppOptions);
                    string options = Parser.Default.FormatCommandLine(appOptions, configuration =>
                    {
                        configuration.UseEqualToken = true;
                        configuration.SkipDefault = false;
                    });
                    Console.WriteLine(options);

                })
                .WithNotParsed(errors =>
                {
                    argumentsHasErrors = true;
                    var helpText = HelpText.AutoBuild(parserResult, h =>
                    {
                        h.AdditionalNewLineAfterOption = false;
                        h.AddNewLineBetweenHelpSections = true;
                        h.AddEnumValuesToHelpText = true;
                        return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                    }, e => e);
                    Console.WriteLine(helpText);
                });
            return !argumentsHasErrors;
        }
    }
}
