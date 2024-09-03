namespace Authorization_Authentication.Products
{
    public interface IProductServices
    {

        public bool addProduct(string Id ,Product prod);

        public IEnumerable<Product> getAllProduct();

        public Product getProduct(string productId);

        public bool updateProduct(string Id, Product prod);

        public bool deleteProduct(string Id, string productId);
    }
}
