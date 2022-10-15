using PerAspera.Models.Generated;

namespace PerAspera.Models.ViewModels
{
    public class FooterViewModel
    {
        private readonly IPageBase currentPage;

        public FooterViewModel(IPageBase currentPage)
        {
            this.currentPage = currentPage;
        }

        //public string SocialMediaUrl()
        //    => Home.SocialMedia?

        public string LogoUrl()
         => Home.FooterLogo?.Url() ?? string.Empty;

        public string Signature()
            => Home?.Signature ?? string.Empty;



        public IEnumerable<IPageBase> Pages =>
            Home.Children<IPageBase>().Where(p => p.DisplayInNavigation);

        //public IEnumerable<SocialMediaItem> SocialNetworks =>
        //    Home.Children<SocialMediaItem>().Where(p => p.SocialMediaName);

        private Home Home => currentPage is Home ? currentPage as Home : currentPage.Root() as Home;

        public IEnumerable<SocialMediaItem> GetSocialMediaItems()
            => this.Home.SocialMedia ?? Array.Empty<SocialMediaItem>();
    }
}
