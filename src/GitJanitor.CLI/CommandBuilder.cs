using System.CommandLine;
using GitJanitor.Common.Enums;
using GitJanitor.Common.Models;
using GitJanitor.Core.Interfaces;
using GitJanitor.IO.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitJanitor.CLI;

public class CommandBuilder
{
    public RootCommand BuildCommandLineApplication(IServiceProvider services)
    {
        // Create flag options
        var workingDirOption = new Option<string>(
            new[] { "--path", "-p" },
            "The working directory to scan for Git repositories.");

        var actionOption = new Option<GitRepositoryAction>(
            new[] { "--action", "-a" },
            "The action to perform on the found repositories.");

        var ownerOption = new Option<string>(
            new[] { "--owner", "-o" },
            "The GitHub owner to filter repositories by.");

        var targetDirOption = new Option<string>(
            new[] { "--target", "-t" },
            "The target directory to move/archive the repositories to.");

        var rootCommand = new RootCommand
        {
            Description = "GitJanitor - A CLI tool for cleaning up Git repositories."
        };

        // Add the rootCommand options
        rootCommand.Add(workingDirOption);
        rootCommand.Add(actionOption);
        rootCommand.Add(ownerOption);
        rootCommand.Add(targetDirOption);

        rootCommand.SetHandler(
            async (workingDirOptionValue, actionOptionValue, ownerOptionValue, targetDirOptionValue) =>
            {
                var dirScanner = services.GetRequiredService<IGitRepositoryScanner>();
                var dirHandler = services.GetRequiredService<IGitRepositoryHandler>();

                var logger = services.GetRequiredService<ILogger<Program>>();

                // Bind the rootCommands to the CommandLineFlags model and apply some validation on it
                var flags = new CommandLineFlags
                {
                    WorkingDirectory = workingDirOptionValue,
                    Action = actionOptionValue,
                    Owner = ownerOptionValue,
                    TargetDirectory = targetDirOptionValue
                };

                var repositories =
                    await dirScanner.ScanForRepositoriesAsync(flags.WorkingDirectory, flags.Owner);

                if (repositories == null || !repositories.Any())
                {
                    logger.LogWarning("Directory not found, exiting...");
                }
                else
                {
                    await dirHandler.HandleAsync(repositories, flags);
                    logger.LogInformation("Done processing files, please check output in the desired folder.");
                }
            },
            workingDirOption, actionOption, ownerOption, targetDirOption);
        return rootCommand;
    }
}