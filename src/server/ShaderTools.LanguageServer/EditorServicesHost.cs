﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using ShaderTools.LanguageServer.Protocol.MessageProtocol.Channel;
using ShaderTools.LanguageServer.Protocol.Utilities;

namespace ShaderTools.LanguageServer
{
    public enum EditorServicesHostStatus
    {
        Started,
        Failed,
        Ended
    }

    /// <summary>
    /// Provides a simplified interface for hosting the language and debug services
    /// over the named pipe server protocol.
    /// </summary>
    public sealed class EditorServicesHost
    {
        private Protocol.Server.LanguageServer _languageServer;

        public EditorServicesHostStatus Status { get; private set; }

        public int LanguageServicePort { get; private set; }

        /// <summary>
        /// Initializes a new instance of the EditorServicesHost class and waits for
        /// the debugger to attach if waitForDebugger is true.
        /// </summary>
        /// <param name="waitForDebugger">If true, causes the host to wait for the debugger to attach before proceeding.</param>
        public EditorServicesHost(bool waitForDebugger)
        {
#if DEBUG
            int waitsRemaining = 10;
            if (waitForDebugger)
            {
                while (waitsRemaining > 0 && !Debugger.IsAttached)
                {
                    Thread.Sleep(1000);
                    waitsRemaining--;
                }
            }
#endif
        }

        /// <summary>
        /// Starts the Logger for the specified file path and log level.
        /// </summary>
        /// <param name="logFilePath">The path of the log file to be written.</param>
        /// <param name="logLevel">The minimum level of log messages to be written.</param>
        public void StartLogging(string logFilePath, LogLevel logLevel)
        {
            Logger.Initialize(logFilePath, logLevel);

            FileVersionInfo fileVersionInfo =
                FileVersionInfo.GetVersionInfo(this.GetType().GetTypeInfo().Assembly.Location);

            // TODO #278: Need the correct dependency package for this to work correctly
            string osVersionString = RuntimeInformation.OSDescription;
            string processArchitecture = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? "64-bit" : "32-bit";
            string osArchitecture = RuntimeInformation.OSArchitecture == Architecture.X64 ? "64-bit" : "32-bit";

            string newLine = Environment.NewLine;

            Logger.Write(
                LogLevel.Normal,
                string.Format(
                    $"ShaderTools Editor Services Host v{fileVersionInfo.FileVersion} starting (pid {Process.GetCurrentProcess().Id})..." + newLine + newLine +
                     "  Host application details:" + newLine + newLine +
                    $"    Arch:      {processArchitecture}" + newLine + newLine +
                     "  Operating system details:" + newLine + newLine +
                    $"    Version: {osVersionString}" + newLine +
                    $"    Arch:    {osArchitecture}"));
        }

        /// <summary>
        /// Starts the language service with the specified TCP socket port.
        /// </summary>
        /// <param name="languageServicePort">The port number for the language service.</param>
        public void StartLanguageService(int languageServicePort)
        {
            this._languageServer = new Protocol.Server.LanguageServer(new TcpSocketServerChannel(languageServicePort));

            this._languageServer.Start().Wait();

            Logger.Write(
                LogLevel.Normal,
                string.Format(
                    "Language service started, listening on port {0}",
                    languageServicePort));
        }

        /// <summary>
        /// Stops the language or debug services if either were started.
        /// </summary>
        public void StopServices()
        {
            this._languageServer?.Stop().Wait();
            this._languageServer = null;
        }

        /// <summary>
        /// Waits for either the language or debug service to shut down.
        /// </summary>
        public void WaitForCompletion()
        {
            // Wait based on which server is started.  If the language server
            // hasn't been started then we may only need to wait on the debug
            // adapter to complete.
            if (this._languageServer != null)
            {
                this._languageServer.WaitForExit();
            }
        }
    }
}