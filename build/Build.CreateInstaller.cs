using System.Diagnostics.CodeAnalysis;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using Serilog.Events;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    /// <summary>
    ///     Create the .msi installers.
    /// </summary>
    Target CreateInstaller => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            const string configuration = "Release";

            var wixTarget = Solution.Revit.RevitLookup;
            var wixInstaller = Solution.Automation.Installer;

            Log.Information("Project: {Name}", wixTarget.Name);

            DotNetBuild(settings => settings
                .SetProjectFile(wixInstaller)
                .SetConfiguration(configuration)
                .SetVersion(ReleaseVersionNumber)
                .SetVerbosity(DotNetVerbosity.minimal));

            var builderFile = Directory
                .EnumerateFiles(wixInstaller.Directory / "bin" / configuration,  $"{wixInstaller.Name}.exe")
                .FirstOrDefault()
                .NotNull($"No installer builder was found for the project: {wixInstaller.Name}");

            var targetDirectories = Directory.GetDirectories(wixTarget.Directory, $"* {configuration} *", SearchOption.AllDirectories);
            Assert.NotEmpty(targetDirectories, "No content were found to create an installer");

            foreach (var targetDirectory in targetDirectories)
            {
                var process = ProcessTasks.StartProcess(builderFile, targetDirectory.DoubleQuoteIfNeeded(), logInvocation: false, logger: InstallerLogger);
                process.AssertZeroExitCode();
            }
        });

    /// <summary>
    ///     Logs the output of the installer process.
    /// </summary>
    [SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
    void InstallerLogger(OutputType outputType, string output)
    {
        if (outputType == OutputType.Err)
        {
            Log.Error(output);
            return;
        }

        var arguments = ArgumentsRegex.Matches(output);
        var logLevel = arguments.Count switch
        {
            0 => LogEventLevel.Debug,
            > 0 when output.Contains("error", StringComparison.OrdinalIgnoreCase) => LogEventLevel.Error,
            _ => LogEventLevel.Information
        };

        if (arguments.Count == 0)
        {
            Log.Write(logLevel, output);
            return;
        }

        var properties = arguments
            .Select(match => match.Value.Substring(1, match.Value.Length - 2))
            .Cast<object>()
            .ToArray();

        var messageTemplate = ArgumentsRegex.Replace(output, match => $"{{Property{match.Index}}}");
        Log.Write(logLevel, messageTemplate, properties);
    }
}