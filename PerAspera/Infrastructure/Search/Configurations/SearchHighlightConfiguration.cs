
using Microsoft.Extensions.Configuration;

using System;

namespace PerAspera.Infrastructure.Search.Configurations
{
	internal sealed class SearchHighlightConfiguration : ISearchHighlightConfiguration
	{
		public SearchHighlightConfiguration(IConfiguration configuration)
		{
			FragmentsCountLazy = new Lazy<int>(() => configuration.GetValue<int>("SearchHighlight:FragmentsCount"));
			FragmentSizeLazy = new Lazy<int>(() => configuration.GetValue<int>("SearchHighlight:FragmentSize"));
			FragmentsSeparatorLazy = new Lazy<string>(() => configuration.GetValue<string>("SearchHighlight:FragmentsSeparator"));
			HtmlTagLazy = new Lazy<string>(() => configuration.GetValue<string>("SearchHighlight:HtmlTag"));
			HtmlTagCssClassLazy = new Lazy<string>(() => configuration.GetValue<string>("SearchHighlight:HtmlTagCssClass"));
		}

		public int FragmentsCount => FragmentsCountLazy.Value;

		public int FragmentSize => FragmentSizeLazy.Value;

		public string FragmentsSeparator => FragmentsSeparatorLazy.Value;

		public string HtmlTag => HtmlTagLazy.Value;

		public string HtmlTagCssClass => HtmlTagCssClassLazy.Value;

		private Lazy<int> FragmentsCountLazy { get; }
		private Lazy<int> FragmentSizeLazy { get; }
		private Lazy<string> FragmentsSeparatorLazy { get; }
		private Lazy<string> HtmlTagLazy { get; }
		private Lazy<string> HtmlTagCssClassLazy { get; }
	}
}
