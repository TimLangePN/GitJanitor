using GitJanitor.Common.Models;
using LibGit2Sharp;

namespace GitJanitor.IO.Interfaces
{
    public interface IGitRepositoryHandler
    {
        Task HandleAsync(List<Repository> repositories, CommandLineFlags flags);
        Task ArchiveAsync(Repository repository, string? targetDirectory);
        Task DeleteAsync(Repository repository);
        Task MoveAsync(Repository repository, string? targetDirectory);
    }
}
