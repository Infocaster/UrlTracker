using System.Collections.Generic;

namespace UrlTracker.Resources.Testing.Clients.Models
{
    public class ContentTreeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public IReadOnlyCollection<ContentTreeViewModel> Children { get; set; }
    }

    public class ContentTreeViewModelCollection
    {
        public IReadOnlyCollection<ContentTreeViewModel> RootContent { get; set; }
    }
}
