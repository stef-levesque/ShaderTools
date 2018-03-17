#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var outputDir = "./artifacts/";

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir)) {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Restore")
    .Does(() => {
        NuGetRestore("src/server");
        NuGetRestore("src/clients/vs");
    });

GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });        
    });

Task("BuildServer")
    .Does(() => {
        MSBuild("./src/server/ShaderTools.LanguageServer.sln");
    });

Task("BuildClientVS")
    .Does(() => {
        MSBuild("./src/clients/vs/ShaderTools.VisualStudio.sln");
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

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);