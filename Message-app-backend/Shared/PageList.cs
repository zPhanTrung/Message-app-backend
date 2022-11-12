using System.Diagnostics;

namespace Message_app_backend.Shared
{
    public class PageList<T> where T : class
    {
        private int Total;
        private int Amount;
        private int PageAmount;
        public List<T> Result { get; }
        public PageList(List<T> list, int pageIndex, int pageSize)
        {
            Total = list.Count;
            PageAmount = Total % pageSize == 0 ? Total / pageSize : Total / pageSize + 1;
            Amount = Total - pageSize * pageIndex >= pageSize ? pageSize : Total - pageSize * (pageIndex - 1);

            Result = list.Skip(pageSize * (pageIndex - 1)).Take(Amount).ToList();
        }
    }
}
