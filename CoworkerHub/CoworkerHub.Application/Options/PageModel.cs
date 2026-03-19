
namespace CoworkerHub.Application.Options
{
    public class PageModel<T>
    {
        public int CurrentPage { get; set; }
        public int? NextPage { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }

        public PageModel(int currentPage, int? nextPage, int totalCount, List<T> items)
        {
            CurrentPage = currentPage;
            NextPage = nextPage;
            TotalCount = totalCount;
            Items = items;
        }
    }
}
