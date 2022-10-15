using Umbraco.Cms.Core.Models;
using PerAspera.Models.Generated;

namespace PerAspera.Models.ViewModels
{
    public class MetaTagsViewModel
    {
        private IPageBase _basePage;

        public MetaTagsViewModel(IPageBase basePage)
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
