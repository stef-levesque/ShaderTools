using System;
using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ShaderTools.LanguageServer
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var launchDebugger = false;
            string logFilePath = null;
            var logLevel = LogLevel.Warning;

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("launchdebugger", ref launchDebugger, false, "Set whether to launch the debugger or not.");
                syntax.DefineOption("logfilepath", ref logFilePath, true, "Fully qualified path to the log file.");
                syntax.DefineOption("loglevel", ref logLevel, x => Enum.Parse<LogLevel>(x), false, "Logging level.");
            });

            if (launchDebugger)
            {
                // TODO: Doesn't work yet: https://github.com/dotnet/coreclr/issues/12074
                // Debugger.Launch();
            }

            LanguageServerHost languageServerHost = null;
            try
            {
                languageServerHost = new LanguageServerHost(
                    Console.OpenStandardInput(),
                    Console.OpenStandardOutput(),
                    logFilePath,
                    logLevel);

                await languageServerHost.Initialize();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return;
            }

            try
            {
                await languageServerHost.WaitForExit;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return;
            }
        }
    }
}
