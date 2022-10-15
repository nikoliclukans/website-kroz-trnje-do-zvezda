namespace PerAspera.Infrastructure.Search.Configurations
{
	public interface ISearchHighlightConfiguration
	{
		int FragmentsCount { get; }
		int FragmentSize { get; }
		string FragmentsSeparator { get; }
		string HtmlTag { get; }
		string HtmlTagCssClass { get; }
	}
}
