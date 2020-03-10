using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath LibraryDirectory => RootDirectory / "lib";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath AssetsDirectory => RootDirectory / "assets";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath AssetsOutputDirectory => OutputDirectory / "assets";

    static Build()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            AssetsDirectory.GlobFiles("**/*.*dump").ForEach(DeleteFile);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target BuildClient => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution.GetProject("Crida"))
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetOutputDirectory(OutputDirectory)
                .EnableNoRestore());
            (LibraryDirectory / "SDL2").GlobFiles("*.*").ForEach(file =>
                CopyFileToDirectory(file, OutputDirectory, FileExistsPolicy.Overwrite));
            (LibraryDirectory / "HarfBuzz").GlobFiles("*.*").ForEach(file =>
                CopyFileToDirectory(file, OutputDirectory, FileExistsPolicy.Overwrite));
        });

    Target BuildAssets => _ => _
        .Executes(() =>
        {
            DotNetRun(s => s
                .SetProjectFile(Solution.GetProject("_buildassets"))
                .SetApplicationArguments(@$"""{AssetsDirectory}"" ""{AssetsOutputDirectory}"""));
        });

    Target Compile => _ => _
        .DependsOn(BuildAssets)
        .DependsOn(BuildClient);

    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);
}
