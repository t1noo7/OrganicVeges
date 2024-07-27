using DiChoSaiGon.Models;

namespace DiChoSaiGon.ModelViews
{
    public class ProductHomeVM
    {
        public Category category { get; set; }
        public List<Product> lsProducts { get; set; }
    }
}
