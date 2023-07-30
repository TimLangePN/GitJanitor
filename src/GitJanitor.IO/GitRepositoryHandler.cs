using GitJanitor.Common.Enums;
using GitJanitor.Common.Models;
using GitJanitor.Core.Extensions;
using GitJanitor.IO.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitJanitor.IO
{
    public class GitRepositoryHandler : IGitRepositoryHandler
    {
        private readonly ILogger _logger;

        public GitRepositoryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(List<Repository> repositories, CommandLineFlags flags)
        {
            Func<Repository, Task> action;

            switch (flags.Action)
            {
                case GitRepositoryAction.Archive:
                    action = (repo) => ArchiveAsync(repo, flags.TargetDirectory);
                    break;
                case GitRepositoryAction.Delete:
                    action = DeleteAsync;
                    break;
                case GitRepositoryAction.Move:
                    action = (repo) => MoveAsync(repo, flags.TargetDirectory);
                    break;
                default:
                    throw new ArgumentException("Invalid action");
            }

            var tasks = repositories.Select(repo => action(repo)).ToList();

            await Task.WhenAll(tasks);
        }

        public async Task ArchiveAsync(Repository repository, string? targetDirectory)
        {

        }

        public async Task DeleteAsync(Repository repository)
        {

        }

        public async Task MoveAsync(Repository repository, string? targetDirectory)
        {

        }
    }

    //foreach (var repository in repositories)
    //{
    //    var remote = repository.Network.Remotes["origin"];
    //    if (remote != null)
    //    {
    //        Console.WriteLine($"{repository.GetRepositoryName()} {remote.Url}");
    //    }
    //}
}
