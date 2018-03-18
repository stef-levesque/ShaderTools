#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var outputDir = "./artifacts/";

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir)) {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Restore")
    .Does(() => {
        MSBuild("src/server/ShaderTools.LanguageServer.sln", new MSBuildSettings().WithTarget("restore"));
        MSBuild("src/clients/vs/ShaderTools.VisualStudio.sln", new MSBuildSettings().WithTarget("restore"));
    });

GitVersion versionInfo = null;
Task("Version")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });
    });

var msBuildSettings = new MSBuildSettings()
    .SetConfiguration(configuration)
    .SetMSBuildPlatform(MSBuildPlatform.x86); // VSSDK requires x86

Task("BuildServer")
    .Does(() => {
        MSBuild("./src/server/ShaderTools.LanguageServer.sln", msBuildSettings);
    });

Task("CopyServerToClientVS")
    .Does(() => {
        CopyFiles("./src/server/ShaderTools.LanguageServer/bin/Release/net461/**/*", "./src/clients/vs/ShaderTools.VisualStudio/Server");
    });

Task("BuildClientVS")
    .IsDependentOn("CopyServerToClientVS")
    .Does(() => {
        MSBuild("./src/clients/vs/ShaderTools.VisualStudio.sln", msBuildSettings);
    });

Task("BuildClientVSCode")
    .Does(() => {
        // TODO
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .IsDependentOn("BuildServer")
    .IsDependentOn("BuildClientVS")
    .IsDependentOn("BuildClientVSCode");

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        MSTest("./src/server/*.Tests");
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        // TODO
    });

Task("UploadArtifacts")
    .Description("Uploads artifacts to AppVeyor")
    .IsDependentOn("Package")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
    {
        var vsixPath = $"src/clients/vs/ShaderTools.VisualStudio/bin/{configuration}/ShaderTools.VisualStudio.vsix";
        AppVeyor.UploadArtifact(vsixPath);
    });

Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .IsDependentOn("UploadArtifacts");

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

RunTarget(target);