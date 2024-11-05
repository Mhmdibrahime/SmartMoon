using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class TransferProductsViewModels
    {
        public string? Name { get; set; }
        public List<Product>? products { get; set; }
        public List<Inventory>? inventories { get; set; }
    }
}
