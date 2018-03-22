// Addins

#addin "nuget:?package=Cake.Npm&version=0.10.0"
#addin "nuget:?package=Cake.VsCode&version=0.8.0"

// Tools

#tool "nuget:?package=GitVersion.CommandLine"

// Setup

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

var buildResultDir = Directory("build-results");

var shouldPublish = AppVeyor.IsRunningOnAppVeyor 
    && !AppVeyor.Environment.PullRequest.IsPullRequest
    && AppVeyor.Environment.Repository.Name == "ShaderTools/ShaderTools"
    && AppVeyor.Environment.Repository.Branch == "master"
    && AppVeyor.Environment.Repository.Tag.IsTag
    && !string.IsNullOrWhiteSpace(AppVeyor.Environment.Repository.Tag.Name);

var serverBinPath = $"src/server/ShaderTools.LanguageServer/bin/{configuration}/net461/**/*";

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(buildResultDir);
    });

GitVersion versionInfo = new GitVersion { SemVer = "0.0.1"};
Task("Version")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });
    });

var msBuildSettingsForRestore = new MSBuildSettings()
    .WithTarget("restore")
    .SetConfiguration(configuration)
    .SetVerbosity(Verbosity.Quiet);

var msBuildSettings = new MSBuildSettings()
    .SetConfiguration(configuration)
    .SetVerbosity(Verbosity.Quiet)
    .SetMSBuildPlatform(MSBuildPlatform.x86); // VSSDK requires x86

// Server

Task("Server-Restore")
    .Does(() => {
        MSBuild("src/server/ShaderTools.LanguageServer.sln", msBuildSettingsForRestore);
    });

Task("Server-Build")
    .IsDependentOn("Server-Restore")
    .Does(() => {
        MSBuild("./src/server/ShaderTools.LanguageServer.sln", msBuildSettings);
    });

Task("Server")
    .IsDependentOn("Server-Restore")
    .IsDependentOn("Server-Build");

// VS for Windows client

Task("Client-VS-Windows-Restore")
    .Does(() => {
        MSBuild("src/clients/vs/ShaderTools.VisualStudio.Windows.sln", msBuildSettingsForRestore);
    });

Task("Client-VS-Windows-CopyServer")
    .Does(() => {
        var serverDir = Directory("src/clients/vs/ShaderTools.VisualStudio.Windows/Server");
        EnsureDirectoryExists(serverDir);
        CopyFiles(serverBinPath, serverDir);
    });

Task("Client-VS-Windows-Build")
    .IsDependentOn("Server")
    .IsDependentOn("Client-VS-Windows-Restore")
    .IsDependentOn("Client-VS-Windows-CopyServer")
    .WithCriteria(() => IsRunningOnWindows())
    .Does(() => {
        MSBuild("./src/clients/vs/ShaderTools.VisualStudio.Windows.sln", msBuildSettings);

        CopyFile(
            $"src/clients/vs/ShaderTools.VisualStudio.Windows/bin/{configuration}/ShaderTools.VisualStudio.Windows.vsix", 
            buildResultDir + File($"ShaderTools-VisualStudio-Windows-{versionInfo.FullSemVer}.vsix"));
    });

Task("Client-VS-Windows-Publish")
    .IsDependentOn("Client-VS-Windows-Build")
    .Does(() => {
        // TODO: Publish to VS Gallery using web browser automation
    });

// VS for Mac client

Task("Client-VS-Mac-Restore")
    .Does(() => {
        MSBuild("src/clients/vs/ShaderTools.VisualStudio.Mac.sln", msBuildSettingsForRestore);
    });

Task("Client-VS-Mac-CopyServer")
    .Does(() => {
        var serverDir = Directory("src/clients/vs/ShaderTools.VisualStudio.Mac/Server");
        EnsureDirectoryExists(serverDir);
        CopyFiles(serverBinPath, serverDir);
    });

Task("Client-VS-Mac-Build")
    .IsDependentOn("Server")
    .IsDependentOn("Client-VS-Mac-Restore")
    .IsDependentOn("Client-VS-Mac-CopyServer")
    .Does(() => {
        var msBuildSettingsPackageAddIn = new MSBuildSettings()
            .WithTarget("PackageAddin")
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Quiet);
        MSBuild("./src/clients/vs/ShaderTools.VisualStudio.Mac.sln", msBuildSettingsPackageAddIn);
    });

Task("Client-VS-Mac-Publish")
    .IsDependentOn("Client-VS-Mac-Build")
    .Does(() => {
        // TODO: Publish to MonoDevelop addins repo
    });

// VSCode client

Task("Client-VSCode-Npm-Install")
    .Does(() =>
    {
        NpmInstall(new NpmInstallSettings {
            WorkingDirectory = "./src/clients/vscode",
            LogLevel = NpmLogLevel.Silent
        });
    });

Task("Client-VSCode-Install-Vsce")
    .IsDependentOn("Client-VSCode-Npm-Install")
    .Does(() =>
    {
        var settings = new NpmInstallSettings();
        settings.Global = true;
        settings.AddPackage("vsce", "1.37.5");
        settings.LogLevel = NpmLogLevel.Silent;
        NpmInstall(settings);
    });

Task("Client-VSCode-Package-Extension")
    .IsDependentOn("Client-VSCode-Install-Vsce")
    .Does(() => {
        var packageFile = File("shadertools-vscode-" + versionInfo.FullSemVer + ".vsix");

        VscePackage(new VscePackageSettings() {
            WorkingDirectory = "src/clients/vscode",
            OutputFilePath = buildResultDir + packageFile
        });
    });

Task("Client-VSCode-Build")
    .IsDependentOn("Server")
    //.IsDependentOn("Client-VSCode-Update-Project-Json-Version")
    .IsDependentOn("Client-VSCode-Npm-Install")
    //.IsDependentOn("Client-VSCode-Install-TypeScript")
    .IsDependentOn("Client-VSCode-Install-Vsce")
    .IsDependentOn("Client-VSCode-Package-Extension")
    .Does(() => {
        // TODO
    });

Task("Client-VSCode-Publish")
    .IsDependentOn("Client-VSCode-Build")
    .Does(() => {
        // TODO: Publish to VS Gallery using VscePublish
    });

// General

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Server")
    .IsDependentOn("Client-VS-Windows-Build")
    .IsDependentOn("Client-VS-Mac-Build")
    .IsDependentOn("Client-VSCode-Build");

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        MSTest("./src/server/*.Tests");
    });

Task("UploadArtifacts")
    .IsDependentOn("Test")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
    {
        foreach (var file in System.IO.Directory.GetFiles(buildResultDir))
        {
            AppVeyor.UploadArtifact(file);
        }
    });

Task("GitHubRelease")
    .WithCriteria(() => shouldPublish)
    .Does(() =>
    {
        // TODO: Create GitHub release and upload assets
    });

Task("Publish")
    .IsDependentOn("GitHubRelease")
    .IsDependentOn("Client-VS-Windows-Publish")
    .IsDependentOn("Client-VS-Mac-Publish")
    .IsDependentOn("Client-VSCode-Publish")
    .WithCriteria(() => shouldPublish);

Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("UploadArtifacts")
    .IsDependentOn("Publish");

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

RunTarget(target);