using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using Take.Blip.Client;

namespace TakeBlipChatbot.Controllers
{
    public class GitHubRepositoryCollectionMessageReceiver : IMessageReceiver
    {
        private readonly GitHubClient _githubClient;
        private readonly ISender _sender;

        public GitHubRepositoryCollectionMessageReceiver(GitHubClient githubClient, ISender sender)
        {
            _githubClient = githubClient;
            _sender = sender;
        }

        [HttpGet]
        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken)
        {
            var searchRepositoriesRequest = new SearchRepositoriesRequest()
            {
                User = "takenet",
                Language = Language.CSharp,
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

            var document = new DocumentCollection
            {
                ItemType = "application/vnd.lime.document-select+json",
                Items = repositories
                    .OrderBy(a => a.CreatedAt)
                    .Take(5)
                    .Select(r => new DocumentSelect
                    {
                        Scope = SelectScope.Transient,
                        Header = new DocumentContainer
                        {
                            Value = new MediaLink
                            {
                                Title = r.FullName,
                                Text = r.Description,
                                Type = "image/png",
                                Uri = new Uri(r.Owner.AvatarUrl)
                            }
                        }
                    })
                    .ToArray()
            };

            await _sender.SendMessageAsync(document, envelope.From, cancellationToken);
        }
    }
}