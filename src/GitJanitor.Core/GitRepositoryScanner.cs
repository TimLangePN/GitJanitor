using GitJanitor.Core.Handlers;
using GitJanitor.Core.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitJanitor.Core;

public class GitRepositoryScanner : IGitRepositoryScanner
{
    private readonly ILogger<GitRepositoryScanner> _logger;

    public GitRepositoryScanner(ILogger<GitRepositoryScanner> logger)
    {
        _logger = logger;
    }

    public async Task<IList<Repository>?> ScanForRepositoriesAsync(string path, string? owner)
    {
        _logger.LogInformation("Starting directory scan...");

        // Check if the directory exists before starting the search.
        if (Directory.Exists(path))
        {
            if (owner != null)
                return await ScanDirectoryRecursivelyAsync(new DirectoryInfo(path), owner);
        }

        return default;
    }

    private async Task<IList<Repository>> ScanDirectoryRecursivelyAsync(DirectoryInfo directory,
        string owner)
    {
        var gitRepositories = new List<Repository>();
        var repositoryName = directory.FullName;

        try
        {
            foreach(var subDirectory in directory.EnumerateDirectories())
                if (subDirectory.Name == ".git")
                    try
                    {
                        var repository = new Repository(repositoryName);
                        var repositoryOwner =
                            GitRepositoryOwnerHandler.Getowner(repositoryName, owner);

                        if (string.Equals(repositoryOwner, owner, StringComparison.OrdinalIgnoreCase))
                            gitRepositories.Add(repository);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Exception occurred while trying to scan for directories: {ex.Message}");
                    }
                // This directory is a Git repository, add it to the list.
                else
                    // This directory is not a Git repository, search its subdirectories.
                    await ScanDirectoryRecursivelyAsync(subDirectory, owner);
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.LogWarning(
                $"Access denied to directory: {directory.FullName}. Skipping this directory. Exception: {uae.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An error occurred while processing directory: {directory.FullName}. Exception: {ex.Message}");
        }

        return gitRepositories;
    }
}