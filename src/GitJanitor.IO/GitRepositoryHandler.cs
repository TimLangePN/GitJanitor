using System.IO.Compression;
using GitJanitor.Common.Enums;
using GitJanitor.Common.Models;
using GitJanitor.Core.Extensions;
using GitJanitor.IO.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitJanitor.IO;

public class GitRepositoryHandler : IGitRepositoryHandler
{
    private readonly ILogger<GitRepositoryHandler> _logger;

    public GitRepositoryHandler(ILogger<GitRepositoryHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(IList<Repository> repositories, CommandLineFlags flags)
    {
        Func<Repository, Task> action = flags.Action switch
        {
            GitRepositoryAction.Archive => repository => ArchiveAsync(repository.Info.WorkingDirectory,
                flags.TargetDirectory, repository.GetRepositoryName()),
            GitRepositoryAction.Delete => repository => DeleteAsync(repository.Info.WorkingDirectory),
            GitRepositoryAction.Move => repository => MoveAsync(repository.Info.WorkingDirectory, flags.TargetDirectory,
                repository.GetRepositoryName()),
            _ => throw new ArgumentException("Invalid action")
        };

        var tasks = repositories.Select(repo => action(repo));

        await Task.WhenAll(tasks);
    }

    public async Task ArchiveAsync(string workingDirectory, string? targetDirectory, string zipFileName)
    {
        await Task.Run(() =>
        {
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                // Create a .zip at the source directory
                var zipPath = Path.Combine(workingDirectory, zipFileName);
                File.Delete(zipPath);
                ZipFile.CreateFromDirectory(workingDirectory, zipPath);

                // Move the .zip file to the target directory
                var movedZipPath = Path.Combine(targetDirectory, zipFileName);
                File.Delete(movedZipPath);
                File.Move(zipPath, movedZipPath);

                // Deletes the current (working) directory
                Directory.Delete(workingDirectory, true);
                _logger.LogInformation($"Moved {zipFileName}.zip to destination at: {targetDirectory}.");
            }
        });
    }

    public async Task DeleteAsync(string workingDirectory)
    {
        await Task.Run(() =>
            { 
                Directory.Delete(workingDirectory, true);
                _logger.LogInformation($"Deleted folder: {workingDirectory}");
            }
        );
    }

    public async Task MoveAsync(string workingDirectory, string? targetDirectory, string directoryName)
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(targetDirectory) && !string.IsNullOrEmpty(targetDirectory))
            {
                var targetPath = Path.Combine(targetDirectory, directoryName);
                if (Directory.Exists(targetPath))
                    Console.WriteLine("Target directory already exists: " + targetPath);
                else
                    Directory.Move(workingDirectory, targetPath);
                    _logger.LogInformation($"Moved {directoryName} to: {targetDirectory}.");
            }
            else
            {
                throw new NotFoundException($"Location: {workingDirectory} Does not exist.");
            }
        });
    }
}