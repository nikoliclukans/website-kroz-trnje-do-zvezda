using PerAspera.Models.Generated;

namespace PerAspera.Models.ViewModels
{
    public class HeaderViewModel
    {
        private readonly IPageBase currentPage;

        public HeaderViewModel(IPageBase currentPage)
        {
            this.currentPage = currentPage;
        }


        public string LogoUrl()
         => Home.Logo?.Url() ?? string.Empty;

        public IEnumerable<IPageBase> Pages =>
            Home.Children<IPageBase>().Where(p => p.DisplayInNavigation);

        private Home Home => currentPage is Home ? currentPage as Home : currentPage.Root() as Home;

    }
}
