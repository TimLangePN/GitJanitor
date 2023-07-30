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
    public async Task HandleAsync(List<Repository> repositories, CommandLineFlags flags)
    {
        Func<Repository, Task> action;

        switch (flags.Action)
        {
            case GitRepositoryAction.Archive:
                action = repository => ArchiveAsync(repository.Info.WorkingDirectory, flags.TargetDirectory, repository.GetRepositoryName());
                break;
            case GitRepositoryAction.Delete:
                action = repository => DeleteAsync(repository.Info.WorkingDirectory);
                break;
            case GitRepositoryAction.Move:
                action = repository => MoveAsync(repository.Info.WorkingDirectory, flags.TargetDirectory, repository.GetRepositoryName());
                break;
            default:
                throw new ArgumentException("Invalid action");
        }

        var tasks = repositories.Select(repo => action(repo)).ToList();

        await Task.WhenAll(tasks);
    }

    public async Task ArchiveAsync(string workingDir, string targetDirectory, string zipFileName)
    {
        await Task.Run(() =>
        {
            // Create a .zip at the source directory
            var zipPath = Path.Combine(workingDir, zipFileName);
            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(workingDir, zipPath);

            // Move the .zip file to the target directory
            var movedZipPath = Path.Combine(targetDirectory, zipFileName);
            if (File.Exists(movedZipPath)) File.Delete(movedZipPath);
            File.Move(zipPath, movedZipPath);

            // Deletes the current (working) directory
            if (Directory.Exists(workingDir)) Directory.Delete(workingDir, true);
        });
    }

    public async Task DeleteAsync(string workingDir)
    {
        await Task.Run(() =>
            {
                if (Directory.Exists(workingDir))
                    Directory.Delete(workingDir, true);
                else
                    throw new NotFoundException($"Location: {workingDir} Does not exist.");
            }
        );
    }

    public async Task MoveAsync(string workingDir, string? targetDir, string dirName)
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(targetDir))
            {
                var targetDirPath = Path.Combine(targetDir, dirName);
                if (Directory.Exists(targetDirPath))
                    Console.WriteLine("Target directory already exists: " + targetDirPath);
                else
                    Directory.Move(workingDir, targetDirPath);
            }
            else
            {
                throw new NotFoundException($"Location: {workingDir} Does not exist.");
            }
        });
    }
}