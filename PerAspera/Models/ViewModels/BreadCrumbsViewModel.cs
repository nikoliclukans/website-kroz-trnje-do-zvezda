using PerAspera.Models.Generated;

namespace PerAspera.Models.ViewModels
{
    public class BreadCrumbsViewModel
    {
        public BreadCrumbsViewModel(IPageBase page)
        {
            Page = page;
        }

        public IPageBase Page { get; }


        public IEnumerable<string> GetBreadCrumbs()
            => Page.Ancestors<IPageBase>().Where(p => p.DisplayInNavigation && p is not Home)
            .Select(p => p.Title).Reverse();
    }
}
