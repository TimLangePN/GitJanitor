using GitJanitor.Core.Interfaces;
using GitJanitor.Core.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using GitJanitor.Core.Handlers;

public class GitRepositoryScanner : IGitRepositoryScanner
{
    private readonly ILogger _logger;

    public GitRepositoryScanner(ILogger<GitRepositoryScanner> logger)
    {
        _logger = logger;
    }

    public async Task<List<GitRepository>> ScanForRepositoriesAsync(string path, string? organization)
    {
        _logger.LogInformation("Starting directory scan...");

        var gitRepositories = new List<GitRepository>();

        // Check if the directory exists before starting the search.
        if (Directory.Exists(path))
        {
            await ScanDirectoryRecursivelyAsync(new DirectoryInfo(path), gitRepositories, organization);
        }

        return gitRepositories;
    }

    private async Task ScanDirectoryRecursivelyAsync(DirectoryInfo directory, List<GitRepository> gitRepositories, string organization)
    {
        var tasks = new List<Task>();

        foreach (var subdirectory in directory.EnumerateDirectories())
        {
            if (subdirectory.Name == ".git")
            {
                try
                {
                    var repositoryOrganizationString = OrganizationHandler.GetOrganization(directory.ToString(), organization);
                    gitRepositories.Add(new GitRepository { Path = directory.FullName, Name = directory.Name, Owner = repositoryOrganizationString});
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);
                }
                // This directory is a Git repository, add it to the list.
                // GitRepository is your custom class. Replace with your actual implementation.
            }
            else
            {
                // This directory is not a Git repository, search its subdirectories.
                tasks.Add(ScanDirectoryRecursivelyAsync(subdirectory, gitRepositories, organization));
            }
        }

        // Await all tasks (i.e., searching all subdirectories) to complete.
        await Task.WhenAll(tasks);
    }
}
