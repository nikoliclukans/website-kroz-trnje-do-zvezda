using PerAspera.Models.Generated;
using Umbraco.Cms.Core.Models;

namespace PerAspera.Models.ViewModels
{
    public class OpenGraphViewModel
    {
        private IPageBase _basePage;

        public OpenGraphViewModel(IPageBase basePage)
        {
            _basePage = basePage;
            Title = basePage.Title;
            Description = basePage.SEodescription;
            Keywords = basePage.SEokeywords;
            CanonicalUrl = basePage.SEocanonicalLink;
            HideFromSearchEngine = basePage.SEohideFromSearchEngine;
            this.Image = basePage.Image;
        }

        public string Title { get; }

        public string Description { get; }

        public string Keywords { get; set; }

        public string CanonicalUrl { get; }
        public bool HideFromSearchEngine { get; set; }
        public MediaWithCrops Image { get; }
    }
}
