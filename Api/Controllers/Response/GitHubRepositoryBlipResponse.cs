using System;

namespace TakeBlipChatbot.Controllers.Response
{
    public class GitHubRepositoryBlipResponse
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Language { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}