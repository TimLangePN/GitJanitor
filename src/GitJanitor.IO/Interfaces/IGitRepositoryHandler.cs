using GitJanitor.Common.Models;
using LibGit2Sharp;

namespace GitJanitor.IO.Interfaces;

public interface IGitRepositoryHandler
{
    Task HandleAsync(IList<Repository> repositories, CommandLineFlags flags);

    Task ArchiveAsync(string workingDirectory, string targetDirectory, string zipFileName);

    Task DeleteAsync(string workingDirectory);

    Task MoveAsync(string workingDirectory, string? targetDirectory, string dirName);
}