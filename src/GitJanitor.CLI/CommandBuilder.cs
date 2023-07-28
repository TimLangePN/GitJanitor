using System.CommandLine;
using GitJanitor.CLI.Enums;
using GitJanitor.Core.Extensions;
using GitJanitor.Core.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitJanitor.CLI
{
    public class CommandBuilder
    {
        public RootCommand BuildCommandLineApplication(IServiceProvider services)
        {
            // Create flag options
            var pathOption = new Option<string>(
                aliases: new string[] { "--path", "-p" },
                description: "The path to scan for Git repositories.");

            var actionOption = new Option<Actions>(
                aliases: new string[] { "--action", "-a" },
                description: "The action to perform on the found repositories.");

            var organizationOption = new Option<string>(
                aliases: new string[] { "--organization", "-o" },
                description: "The GitHub organization to filter repositories by.");

            var targetDirOption = new Option<string>(
                aliases: new string[] { "--target", "-o" },
                description: "The target directory to move/archive the repositories to.");

            var rootCommand = new RootCommand
            {
                Description = "GitJanitor - A CLI tool for cleaning up Git repositories.",
            };

            // Add the rootCommand options
            rootCommand.Add(pathOption);
            rootCommand.Add(actionOption);
            rootCommand.Add(organizationOption);
            rootCommand.Add(targetDirOption);

            rootCommand.SetHandler(async (pathOptionValue, actionOptionValue, organizationOptionValue) =>
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    var dirScanner = services.GetRequiredService<IGitRepositoryScanner>();

                    logger.LogInformation("Processing path {pathOptionValue}, action {actionOptionValue}, and organization {organizationOptionValue}",
                        pathOptionValue, actionOptionValue, organizationOptionValue);

                    var repositories = await dirScanner.ScanForRepositoriesAsync(pathOptionValue, organizationOptionValue);

                    foreach (var repository in repositories)
                    {
                        var remote = repository.Network.Remotes["origin"];
                        if (remote != null)
                        {
                            Console.WriteLine($"{repository.GetRepositoryName()} {remote.Url}");
                        }
                    }
                },
                pathOption, actionOption, organizationOption);
            
            return rootCommand;
        }
    }
}
