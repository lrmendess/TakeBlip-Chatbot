using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using TakeBlipChatbot.Controllers.Response;

namespace TakeBlipChatbot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GitHubRepositoriesController : ControllerBase
    {
        private GitHubClient _githubClient;

        public GitHubRepositoriesController(GitHubClient githubClient)
        {
            _githubClient = githubClient;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<GitHubRepositoryResponse>>> GetOlderRepositories(
            [FromRoute] string username,
            [FromQuery] Language? language = null,
            [FromQuery] int take = 10)
        {
            var searchRepositoriesRequest = new SearchRepositoriesRequest()
            {
                User = username,
                Language = language,
                Fork = ForkQualifier.IncludeForks,
                PerPage = 100
            };

            List<Repository> repositories = new List<Repository>();
            SearchRepositoryResult searchRepositoriesResult = null;

            do
            {
                searchRepositoriesResult = await _githubClient.Search.SearchRepo(searchRepositoriesRequest);
                repositories.AddRange(searchRepositoriesResult.Items);
                searchRepositoriesRequest.Page += 1;
            }
            while (searchRepositoriesResult.TotalCount > repositories.Count);

            var filteredRepositories = repositories
                .OrderBy(a => a.CreatedAt)
                .Take(take)
                .Select(r => new GitHubRepositoryResponse
                {
                    Url = r.Url,
                    Name = r.FullName,
                    Description = r.Description,
                    ThumbnailUrl = r.Owner.AvatarUrl,
                    Language = r.Language,
                    CreatedAt = r.CreatedAt.DateTime
                });

            return filteredRepositories.ToList();
        }
    }
}