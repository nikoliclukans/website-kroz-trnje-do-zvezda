namespace PerAspera.Models.Generated
{
	public partial class LatestBlogs
	{
		public IEnumerable<BlogPage> GetLatestBlogs(int numberOfBlogs)
		{
			return new List<BlogPage>();
			//List<BlogPage> blogPages = Blogs?.Children<BlogPage>().ToList() ?? new List<BlogPage>();
			//blogPages.Sort((x, y) => x.UpdateDate.CompareTo(y.UpdateDate));
			//return blogPages.GetRange(0, numberOfBlogs);
		}
	}
}
