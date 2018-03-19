using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;

namespace ShaderTools.VisualStudio.LanguageClients
{
    [ContentType(ContentTypeDefinitions.Hlsl)]
    [ContentType(ContentTypeDefinitions.ShaderLab)]
    [Export(typeof(ILanguageClient))]
    internal sealed class ShaderToolsLanguageClient : ILanguageClient
    {
        public string Name => $"Shader Tools Language Extension";

        public IEnumerable<string> ConfigurationSections => null;

        public object InitializationOptions => null;

        public IEnumerable<string> FilesToWatch => null;

        public event AsyncEventHandler<EventArgs> StartAsync;

#pragma warning disable CS0067
        public event AsyncEventHandler<EventArgs> StopAsync;
#pragma warning restore CS0067

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            await Task.Yield();

            var vsixDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var languageServerExe = Path.Combine(
                vsixDirectory,
                "Server",
                "ShaderTools.LanguageServer.exe");
            var languageServerArguments = $@"--logfilepath ""{vsixDirectory}/LanguageServerLog.txt""";

            string fileName, arguments;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fileName = languageServerExe;
                arguments = languageServerArguments;
            }
            else
            {
                fileName = "mono";
                arguments = $@"--debug --debugger-agent=transport=dt_socket,address=127.0.0.1:63000 ""{languageServerExe}"" {languageServerArguments}";
            }

            var info = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process();
            process.StartInfo = info;

            process.ErrorDataReceived += (sender, e) =>
            {
                var error = e.Data;
            };

            if (process.Start())
            {
                process.BeginErrorReadLine();

                return new Connection(
                    process.StandardOutput.BaseStream,
                    process.StandardInput.BaseStream);
            }

            return null;
        }

        public async Task OnLoadedAsync()
        {
            await StartAsync?.InvokeAsync(this, EventArgs.Empty);
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnServerInitializeFailedAsync(Exception e)
        {
            return Task.CompletedTask;
        }
    }
}
