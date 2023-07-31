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

    public async Task ArchiveAsync(string workingDir, string targetDirectory, string zipFileName)
    {
        await Task.Run(() =>
        {
            // Create a .zip at the source directory
            var zipPath = Path.Combine(workingDir, zipFileName);
            File.Delete(zipPath);
            ZipFile.CreateFromDirectory(workingDir, zipPath);

            // Move the .zip file to the target directory
            var movedZipPath = Path.Combine(targetDirectory, zipFileName);
            File.Delete(movedZipPath);
            File.Move(zipPath, movedZipPath);

            // Deletes the current (working) directory
            Directory.Delete(workingDir, true);
        });
    }

    public async Task DeleteAsync(string workingDir)
    {
        await Task.Run(() =>
            { 
                Directory.Delete(workingDir, true);
            }
        );
    }

    public async Task MoveAsync(string workingDir, string targetDir, string dirName)
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