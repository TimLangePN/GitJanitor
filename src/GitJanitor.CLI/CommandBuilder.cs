using System.CommandLine;
using GitJanitor.Common.Enums;
using GitJanitor.Common.Models;
using GitJanitor.Core.Interfaces;
using GitJanitor.IO.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GitJanitor.CLI
{
    public class CommandBuilder
    {
        public RootCommand BuildCommandLineApplication(IServiceProvider services)
        {
            // Create flag options
            var workingDirOption = new Option<string>(
                aliases: new string[] { "--workingDir", "-p" },
                description: "The workingDir to scan for Git repositories.");

            var actionOption = new Option<GitRepositoryAction>(
                aliases: new string[] { "--action", "-a" },
                description: "The action to perform on the found repositories.");

            var organizationOption = new Option<string>(
                aliases: new string[] { "--organization", "-o" },
                description: "The GitHub organization to filter repositories by.");

            var targetDirOption = new Option<string>(
                aliases: new string[] { "--target", "-t" },
                description: "The target directory to move/archive the repositories to.");

            var rootCommand = new RootCommand
            {
                Description = "GitJanitor - A CLI tool for cleaning up Git repositories.",
            };

            // Add the rootCommand options
            rootCommand.Add(workingDirOption);
            rootCommand.Add(actionOption);
            rootCommand.Add(organizationOption);
            rootCommand.Add(targetDirOption);

            rootCommand.SetHandler(async (workingDirOptionValue, actionOptionValue, organizationOptionValue, targetDirOptionValue) =>
                {
                    //var logger = services.GetRequiredService<ILogger<Program>>();
                    var dirScanner = services.GetRequiredService<IGitRepositoryScanner>();
                    var dirHandler = services.GetRequiredService<IGitRepositoryHandler>();

                    // Bind the rootCommands to the CommandLineFlags model and apply some validation on it
                    var flags = new CommandLineFlags
                    {
                        WorkingDirectory = workingDirOptionValue,
                        Action = actionOptionValue,
                        Organization = organizationOptionValue,
                        TargetDirectory = targetDirOptionValue
                    };

                    var repositories = await dirScanner.ScanForRepositoriesAsync(flags.WorkingDirectory, flags.Organization);

                },
                workingDirOption, actionOption, organizationOption, targetDirOption);
            
            return rootCommand;
        }
    }
}
