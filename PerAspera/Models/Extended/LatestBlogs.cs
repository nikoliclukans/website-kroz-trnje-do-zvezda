namespace PerAspera.Models.Generated
{
	public partial class LatestBlogs
	{
		public const int defaultNumberOfSlides = 4;
		
		public IEnumerable<BlogPage> GetLatestBlogs(int maxNumberOfBlogs)
		{
			List<BlogPage> blogPages = Blogs?.Children<BlogPage>().ToList() ?? new List<BlogPage>();
			blogPages.Sort((x, y) => y.UpdateDate.CompareTo(x.UpdateDate));
			if (blogPages.Count > maxNumberOfBlogs)
			{
				return blogPages.GetRange(0, maxNumberOfBlogs);
			}

			return blogPages;
		}
	}
}
