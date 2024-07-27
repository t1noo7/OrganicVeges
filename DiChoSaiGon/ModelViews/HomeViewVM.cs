using DiChoSaiGon.Models;

namespace DiChoSaiGon.ModelViews
{
    public class HomeViewVM
    {
        public List<Post> Posts { get; set; }
        public List<ProductHomeVM> Products { get; set; }
        public List<Banner> Banners { get; set; }

    }
}
