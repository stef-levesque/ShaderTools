using System;
using System.CommandLine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ShaderTools.LanguageServer.Protocol.Utilities;

namespace ShaderTools.LanguageServer
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var waitForDebugger = false;
            string logFilePath = null;
            var logLevel = LogLevel.Normal;

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("waitfordebugger", ref waitForDebugger, false, "Set whether to wait for the debugger or not.");
                syntax.DefineOption("logfilepath", ref logFilePath, true, "Fully qualified path to the log file.");
                syntax.DefineOption("loglevel", ref logLevel, x => Enum.Parse<LogLevel>(x), false, "Logging level.");
            });

            EditorServicesHost editorServicesHost;
            try
            {
                editorServicesHost = new EditorServicesHost(waitForDebugger);

                var languageServicePort = GetAvailablePort();
                if (languageServicePort == null)
                {
                    throw new Exception("Could not find an available port");
                }

                editorServicesHost.StartLogging(logFilePath, logLevel);
                editorServicesHost.StartLanguageService(languageServicePort.Value);

                Console.WriteLine(SerializeToJson(new ProgramStartResult
                {
                    Status = "started",
                    Channel = "tcp",
                    LanguageServicePort = languageServicePort.Value
                }));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ShaderTools Editor Services host initialization failed, terminating.");
                Console.Error.WriteLine(ex);
                return 2;
            }

            try
            {
                editorServicesHost.WaitForCompletion();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Caught error while waiting for Editor Services host to complete.");
                Console.Error.WriteLine(ex);
                return 3;
            }

            return 0;
        }

        private static readonly Random Random = new Random();

        private static int? GetAvailablePort()
        {
            var triesRemaining = 10;

            while (triesRemaining > 0)
            {
                var port = Random.Next(10000, 30000);
                if (TestPortAvailability(port))
                {
                    return port;
                }

                triesRemaining--;
            }

            return null;
        }

        private static bool TestPortAvailability(int portNumber)
        {
            var portAvailable = true;

            try
            {
                var ipAddress = Dns.GetHostEntryAsync("localhost").Result.AddressList[0];
                var tcpListener = new TcpListener(ipAddress, portNumber);
                tcpListener.Start();
                tcpListener.Stop();
            }
            catch (SocketException ex)
            {
                // Check the SocketErrorCode to see if it's the expected exception
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    portAvailable = false;
                }
                else
                {
                    Console.WriteLine($"Error code: " + ex.SocketErrorCode);
                }
            }

            return portAvailable;
        }

        [DataContract]
        private sealed class ProgramStartResult
        {
            [DataMember(Name = "status")]
            public string Status { get; set; }

            [DataMember(Name = "channel")]
            public string Channel { get; set; }

            [DataMember(Name = "languageServicePort")]
            public int LanguageServicePort { get; set; }
        }

        private static string SerializeToJson(object value)
        {
            var jsonSerializer = new DataContractJsonSerializer(value.GetType());
            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, value);

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                    return streamReader.ReadToEnd();
            }
        }
    }
}
