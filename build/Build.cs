using System;
using System.Collections.Generic;
using System.Linq;
using _build;
using DebuggableSourceGenerators;
using FluentSourceGenerators;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
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
[GitHubActions("dotnetcore",
	GitHubActionsImage.Ubuntu1804,
	ImportSecrets = new[] {"NUGET_API_KEY"},
	AutoGenerate = true,
	On = new[] {GitHubActionsTrigger.Push},
	InvokedTargets = new[] {"Test", "Push"}
)]
class Build : NukeBuild
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode

	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Parameter("NuGet server URL.")] readonly string NugetSource = "https://api.nuget.org/v3/index.json";
	[Parameter("API Key for the NuGet server.")] readonly string NugetApiKey;

	[Solution] readonly Solution Solution;

	AbsolutePath SourceDirectory => RootDirectory / "src";
	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
	AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";
	Project PackageProject => Solution.GetProject("LiveLinq");
	IEnumerable<Project> TestProjects => Solution.GetProjects("*.Tests");

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			EnsureCleanDirectory(ArtifactsDirectory);
		});

	Target Restore => _ => _
		.Executes(() =>
		{
			DotNetRestore(s => s
				.SetProjectFile(Solution)
			);
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.EnableNoRestore()
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
			);

			DotNetPublish(s => s
				.EnableNoRestore()
				.EnableNoBuild()
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.CombineWith(
					from project in new[] {PackageProject}
					from framework in project.GetTargetFrameworks()
					select new {project, framework}, (cs, v) => cs
						.SetProject(v.project)
						.SetFramework(v.framework)
				)
			);
		});

	Target GenerateCode => _ => _
		.Executes(() => {
			var codeIndex = new CodeIndex();

			codeIndex.AddProject(Solution, "LiveLinq");

			var iface = codeIndex.ResolveType("LiveLinq.Dictionary.Interfaces.IObservableReadOnlyDictionary", 2);
			
			var iface2 = codeIndex.ResolveType("ComposableCollections.Dictionary.Interfaces.IDisposableQueryableDictionary", 2);

			// FluentSourceGenerator.Execute(SourceDirectory / "LiveLinq" / "FluentApiSourceGenerator.xml", compilation,
			// 	(fileName, contents) =>
			// 	{
			// 		 Console.WriteLine(fileName);
			// 	});
		});

	Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
	        DeleteDirectory(TestResultDirectory);
	        
	        // To do unit tests, add SetDataCollector("XPlat Code Coverage") like below, and
	        // add coverlet.collector as a dependency in all unit test projects.
            DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .ResetVerbosity()
                .SetDataCollector("XPlat Code Coverage")
                .SetResultsDirectory(TestResultDirectory)
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)
                    .SetLogger($"trx;LogFileName={v.Name}.trx")
                ));
        });

    Target Pack => _ => _
	    .DependsOn(Compile)
		.Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetPack(s => s
                .EnableNoRestore()
                .EnableNoBuild()
				.SetProject(PackageProject)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.NuGetVersionV2)
				.SetIncludeSymbols(true)
				.SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
            );
        });
    
    Target Push => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
				.SetSource(NugetSource)
				.SetApiKey(NugetApiKey)
				.SetSkipDuplicate(true)
				.CombineWith(ArtifactsDirectory.GlobFiles("*.nupkg"), (s, v) => s
					.SetTargetPath(v)
				)
            );
        });
    
    public GitVersion GitVersion
    {
	    get
	    {
		    var package = NuGetPackageResolver.GetGlobalInstalledPackage("GitVersion.Tool", "5.6.0", null);
		    var settings = new GitVersionSettings().SetProcessToolPath(package.Directory / "tools/net5.0/any/gitversion.dll");
		    var gitVersion = GitVersionTasks.GitVersion(settings).Result;
		    return gitVersion;
	    }
    }
}
