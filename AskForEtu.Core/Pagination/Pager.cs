
namespace AskForEtu.Core.Pagination
{
    public class Pager
    {
        public int CurrentPage { get; set; }// O An oldugu sayfa
        public int TotalPage => (int)Math.Ceiling(TotalCount / (double)PageSize); // toplam sayfa sayısı
        public int PageSize => 4;// 1 sayfanın büyüklüğü
        public int TotalCount { get; set; }// toplam veri sayısı

        public bool HasPrevios => CurrentPage > 1; // öncesinde sayfa var mı
        public bool HasNext => CurrentPage < TotalPage; // sonrasinda sayfa var mı
    }
}
