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
        public event AsyncEventHandler<EventArgs> StopAsync;

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            await Task.Yield();

            var info = new ProcessStartInfo
            {
                FileName = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Server",
                    "ShaderTools.LanguageServer.exe"),
                Arguments = "",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process();
            process.StartInfo = info;

            if (process.Start())
            {
                return new Connection(
                    Console.OpenStandardOutput(),
                    Console.OpenStandardInput());
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
