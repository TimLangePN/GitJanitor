using GitJanitor.Common.Models;
using LibGit2Sharp;

namespace GitJanitor.IO.Interfaces;

public interface IGitRepositoryHandler
{
    Task HandleAsync(List<Repository> repositories, CommandLineFlags flags);
    Task ArchiveAsync(string workingDir, string targetDirectory, string zipFileName);
    Task DeleteAsync(string workingDir);
    Task MoveAsync(string workingDir, string? targetDirectory, string dirName);
}