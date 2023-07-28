using GitJanitor.Core.Extensions;
using GitJanitor.Core.Handlers;
using GitJanitor.Core.Interfaces;
using GitJanitor.Core.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

public class GitRepositoryScanner : IGitRepositoryScanner
{
    private readonly ILogger _logger;

    public GitRepositoryScanner(ILogger<GitRepositoryScanner> logger)
    {
        _logger = logger;
    }

    public async Task<List<Repository>> ScanForRepositoriesAsync(string path, string? organization)
    {
        _logger.LogInformation("Starting directory scan...");

        var gitRepositories = new List<Repository>();

        // Check if the directory exists before starting the search.
        if (Directory.Exists(path))
        {
            await ScanDirectoryRecursivelyAsync(new DirectoryInfo(path), gitRepositories, organization);
        }

        return gitRepositories;
    }

    private async Task ScanDirectoryRecursivelyAsync(DirectoryInfo directory, List<Repository> gitRepositories, string organization)
    {
        var tasks = new List<Task>();

        foreach (var subdirectory in directory.EnumerateDirectories())
        {
            if (subdirectory.Name == ".git")
            {
                try
                {
                    var repository = new Repository(directory.FullName);
                    var repositoryOrganization = OrganizationHandler.GetOrganization(directory.FullName, organization);

                    if (string.Equals(repositoryOrganization, organization, StringComparison.OrdinalIgnoreCase))
                    {
                        gitRepositories.Add(repository);
                    }
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
