

using Microsoft.Data.SqlClient;

namespace Authorization_Authentication.Products
{
    public class ProductServices : IProductServices
    {
        private readonly string _connectionStringProd;
        public ProductServices(IConfiguration configuration)
        {
            _connectionStringProd = configuration.GetConnectionString("ProdDbcs");
        }

        public bool addProduct(string Id, Product prod)
        {
            throw new NotImplementedException();
        }

        public bool deleteProduct(string Id, string productId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> getAllProduct()
        {
            var products = new List<Product>();
            SqlConnection conn = new SqlConnection(_connectionStringProd);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchItem = "select * from Products ";
                    SqlCommand cmd = new SqlCommand(fetchItem, conn);
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            var prod = new Product
                            {
                                Id = rd["Id"].ToString(),
                                Name = rd["Name"].ToString(),
                                Description = rd["Description"].ToString(),
                                Price = rd.GetDecimal(rd.GetOrdinal("Price")),
                                NoOfProduct = rd.GetInt32(rd.GetOrdinal("NoOfProduct"))
                            };
                            if (prod != null)
                            {
                                products.Add(prod);
                            }
                        }

                    }

                }
                return products;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public Product getProduct(string productId)
        {
            var products = new Product();
            SqlConnection conn = new SqlConnection(_connectionStringProd);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchItem = "select * from Products where Id = @Id";
                    
                    SqlCommand cmd = new SqlCommand(fetchItem, conn);
                    cmd.Parameters.AddWithValue("Id", productId);
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            products = new Product
                            {
                                Id = rd["Id"].ToString(),
                                Name = rd["Name"].ToString(),
                                Description = rd["Description"].ToString(),
                                Price = rd.GetDecimal(rd.GetOrdinal("Price")),
                                NoOfProduct = rd.GetInt32(rd.GetOrdinal("NoOfProduct"))
                            };
                            
                        }

                    }

                }
                return products;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public bool updateProduct(string Id, Product prod)
        {
            throw new NotImplementedException();
        }
    }
}
