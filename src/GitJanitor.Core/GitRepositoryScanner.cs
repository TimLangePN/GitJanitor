﻿using GitJanitor.Core.Handlers;
using GitJanitor.Core.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitJanitor.Core;

public class GitRepositoryScanner : IGitRepositoryScanner
{
    private readonly ILogger _logger;

    public GitRepositoryScanner(ILogger<GitRepositoryScanner> logger)
    {
        _logger = logger;
    }

    public async Task<List<Repository>> ScanForRepositoriesAsync(string path, string? owner)
    {
        _logger.LogInformation("Starting directory scan...");

        var gitRepositories = new List<Repository>();

        // Check if the directory exists before starting the search.
        if (Directory.Exists(path))
            await ScanDirectoryRecursivelyAsync(new DirectoryInfo(path), gitRepositories, owner);

        return gitRepositories;
    }

    private async Task ScanDirectoryRecursivelyAsync(DirectoryInfo directory, List<Repository> gitRepositories,
        string owner)
    {
        var tasks = new List<Task>();

        try
        {
            foreach (var subDirectory in directory.EnumerateDirectories())
                if (subDirectory.Name == ".git")
                    try
                    {
                        var repository = new Repository(directory.FullName);
                        var repositoryowner =
                            GitRepositoryOwnerHandler.Getowner(directory.FullName, owner);

                        if (string.Equals(repositoryowner, owner, StringComparison.OrdinalIgnoreCase))
                            gitRepositories.Add(repository);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex.Message);
                    }
                // This directory is a Git repository, add it to the list.
                else
                    // This directory is not a Git repository, search its subdirectories.
                    tasks.Add(ScanDirectoryRecursivelyAsync(subDirectory, gitRepositories, owner));
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

        // Await all tasks (i.e., searching all subdirectories) to complete.
        await Task.WhenAll(tasks);
    }
}