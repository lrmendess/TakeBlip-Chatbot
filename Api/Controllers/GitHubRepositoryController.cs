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
    public class GitHubRepositoryController : ControllerBase
    {
        private GitHubClient _githubClient;

        public GitHubRepositoryController(GitHubClient githubClient)
        {
            _githubClient = githubClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GitHubRepositoryBlipResponse>>> Get(
            [FromQuery] string username,
            [FromQuery] Language language = Octokit.Language.CSharp,
            [FromQuery] int take = 5)
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
                .Select(r => new GitHubRepositoryBlipResponse
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