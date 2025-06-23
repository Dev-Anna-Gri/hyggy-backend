using dotenv.net;
using System.Data.SqlClient;
using hyggy_backend.Models.ProductModels;
using hyggy_backend.Models.ProductModels.Enums;
using hyggy_backend.Models;
using hyggy_backend.Models.BlogModels;

namespace hyggy_backend.Services
{
    public static class DBService
    {
        private static readonly string? _connectionString;

        static DBService()
        {
            DotEnv.Load();
            _connectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION");
        }

        private static void PrintConsoleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR!]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[{DateTime.Now}]: {ex.Message}");
        }

        #region Assets

        public static async Task<int> AddAssetAsync(Asset asset)
        {
            try
            {
                const string sql = @"
                    INSERT INTO [Assets] (Id, Path, Alt, Type)
                    VALUES (@Id, @Path, @Alt, @Type);
                ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", asset.Id);
                cmd.Parameters.AddWithValue("@Path", asset.Path);
                cmd.Parameters.AddWithValue("@Alt", (object?)asset.Alt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Type", (object?)asset.Type ?? DBNull.Value);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<Asset?> GetAssetByIdAsync(string id)
        {
            try
            {
                const string sql = @"SELECT Id, Path, Alt, Type FROM [Assets] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return null;

                // Используем GetOrdinal
                var ordId = reader.GetOrdinal("Id");
                var ordPath = reader.GetOrdinal("Path");
                var ordAlt = reader.GetOrdinal("Alt");
                var ordType = reader.GetOrdinal("Type");

                return new Asset
                {
                    Id = reader.GetString(ordId),
                    Path = reader.GetString(ordPath),
                    Alt = reader.IsDBNull(ordAlt) ? null : reader.GetString(ordAlt),
                    Type = reader.IsDBNull(ordType) ? null : reader.GetString(ordType)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Asset>> GetAllAssetsAsync()
        {
            var list = new List<Asset>();
            try
            {
                const string sql = @"SELECT Id, Path, Alt, Type FROM [Assets]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                // Определяем ordinals один раз перед циклом
                var ordId = reader.GetOrdinal("Id");
                var ordPath = reader.GetOrdinal("Path");
                var ordAlt = reader.GetOrdinal("Alt");
                var ordType = reader.GetOrdinal("Type");

                while (await reader.ReadAsync())
                {
                    list.Add(new Asset
                    {
                        Id = reader.GetString(ordId),
                        Path = reader.GetString(ordPath),
                        Alt = reader.IsDBNull(ordAlt) ? null : reader.GetString(ordAlt),
                        Type = reader.IsDBNull(ordType) ? null : reader.GetString(ordType)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }

            return list;
        }

        public static async Task<int> UpdateAssetAsync(Asset asset)
        {
            try
            {
                const string sql = @"
                    UPDATE [Assets]
                    SET Path = @Path,
                        Alt  = @Alt,
                        Type = @Type
                    WHERE Id = @Id
                ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", asset.Id);
                cmd.Parameters.AddWithValue("@Path", asset.Path);
                cmd.Parameters.AddWithValue("@Alt", (object?)asset.Alt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Type", (object?)asset.Type ?? DBNull.Value);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteAssetAsync(string id)
        {
            try
            {
                const string sql = @"DELETE FROM [Assets] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region Product

        public static async Task<Product?> GetProductByIdAsync(int productId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        p.Id, p.Name, p.ImageAssetId, p.SmallDescription, p.Price,
                        p.BrandId, p.SubcategoryId, p.IsDiscount, p.DiscountType, p.DiscountAmount,
                        p.DiscountTime, p.FullDescriptionHTML, p.AddedAt
                    FROM [Products] p
                    WHERE p.Id = @Id
                ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", productId);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return null;

                var ord = new
                {
                    Id = reader.GetOrdinal("Id"),
                    Name = reader.GetOrdinal("Name"),
                    ImgId = reader.GetOrdinal("ImageAssetId"),
                    SmDesc = reader.GetOrdinal("SmallDescription"),
                    Price = reader.GetOrdinal("Price"),
                    BrandId = reader.GetOrdinal("BrandId"),
                    SubcatId = reader.GetOrdinal("SubcategoryId"),
                    IsDisc = reader.GetOrdinal("IsDiscount"),
                    DiscType = reader.GetOrdinal("DiscountType"),
                    DiscAmt = reader.GetOrdinal("DiscountAmount"),
                    DiscTime = reader.GetOrdinal("DiscountTime"),
                    FullHtml = reader.GetOrdinal("FullDescriptionHTML"),
                    AddedAt = reader.GetOrdinal("AddedAt")
                };

                return new Product
                {
                    Id = reader.GetInt32(ord.Id),
                    Name = reader.GetString(ord.Name),
                    ImageAssetId = reader.IsDBNull(ord.ImgId) ? null : reader.GetString(ord.ImgId),
                    SmallDescription = reader.GetString(ord.SmDesc),
                    Price = reader.GetDecimal(ord.Price),
                    BrandId = reader.GetInt32(ord.BrandId),
                    SubcategoryId = reader.GetInt32(ord.SubcatId),
                    IsDiscount = reader.GetBoolean(ord.IsDisc),
                    DiscountType = reader.IsDBNull(ord.DiscType) ? null : (DiscountType?)reader.GetInt32(ord.DiscType),
                    DiscountAmount = reader.IsDBNull(ord.DiscAmt) ? null : reader.GetDecimal(ord.DiscAmt),
                    DiscountTime = reader.IsDBNull(ord.DiscTime) ? null : reader.GetDateTime(ord.DiscTime),
                    FullDescriptionHTML = reader.IsDBNull(ord.FullHtml) ? null : reader.GetString(ord.FullHtml),
                    AddedAt = reader.GetDateTime(ord.AddedAt)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> AddProductAsync(Product product)
        {
            try
            {
                const string sql = @"
                INSERT INTO [Products]
                    (Name, ImageAssetId, SmallDescription, Price,
                     BrandId, SubcategoryId, IsDiscount, DiscountType,
                     DiscountAmount, DiscountTime, FullDescriptionHTML, AddedAt)
                VALUES
                    (@Name, @ImageAssetId, @SmallDescription, @Price,
                     @BrandId, @SubcategoryId, @IsDiscount, @DiscountType,
                     @DiscountAmount, @DiscountTime, @FullDescriptionHTML, @AddedAt);
                SELECT SCOPE_IDENTITY();
            ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@ImageAssetId", (object?)product.ImageAssetId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SmallDescription", product.SmallDescription);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@BrandId", product.BrandId);
                cmd.Parameters.AddWithValue("@SubcategoryId", product.SubcategoryId);
                cmd.Parameters.AddWithValue("@IsDiscount", product.IsDiscount);
                cmd.Parameters.AddWithValue("@DiscountType", (object?)(int?)product.DiscountType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountAmount", (object?)product.DiscountAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountTime", (object?)product.DiscountTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FullDescriptionHTML", (object?)product.FullDescriptionHTML ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddedAt", product.AddedAt);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Product>> GetAllProductsAsync()
        {
            var list = new List<Product>();
            try
            {
                const string sql = @"
                SELECT 
                    Id, Name, ImageAssetId, SmallDescription, Price,
                    BrandId, SubcategoryId, IsDiscount, DiscountType,
                    DiscountAmount, DiscountTime, FullDescriptionHTML, AddedAt
                FROM [Products]
            ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                // Определяем ordinals
                var o = new
                {
                    Id = reader.GetOrdinal("Id"),
                    Name = reader.GetOrdinal("Name"),
                    Img = reader.GetOrdinal("ImageAssetId"),
                    SmD = reader.GetOrdinal("SmallDescription"),
                    Price = reader.GetOrdinal("Price"),
                    BrId = reader.GetOrdinal("BrandId"),
                    SubId = reader.GetOrdinal("SubcategoryId"),
                    IsD = reader.GetOrdinal("IsDiscount"),
                    DT = reader.GetOrdinal("DiscountType"),
                    DA = reader.GetOrdinal("DiscountAmount"),
                    DTime = reader.GetOrdinal("DiscountTime"),
                    HTML = reader.GetOrdinal("FullDescriptionHTML"),
                    Add = reader.GetOrdinal("AddedAt")
                };

                while (await reader.ReadAsync())
                {
                    list.Add(new Product
                    {
                        Id = reader.GetInt32(o.Id),
                        Name = reader.GetString(o.Name),
                        ImageAssetId = reader.IsDBNull(o.Img) ? null : reader.GetString(o.Img),
                        SmallDescription = reader.GetString(o.SmD),
                        Price = reader.GetDecimal(o.Price),
                        BrandId = reader.GetInt32(o.BrId),
                        SubcategoryId = reader.GetInt32(o.SubId),
                        IsDiscount = reader.GetBoolean(o.IsD),
                        DiscountType = reader.IsDBNull(o.DT) ? null : (DiscountType?)reader.GetInt32(o.DT),
                        DiscountAmount = reader.IsDBNull(o.DA) ? null : reader.GetDecimal(o.DA),
                        DiscountTime = reader.IsDBNull(o.DTime) ? null : reader.GetDateTime(o.DTime),
                        FullDescriptionHTML = reader.IsDBNull(o.HTML) ? null : reader.GetString(o.HTML),
                        AddedAt = reader.GetDateTime(o.Add)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }

            return list;
        }

        public static async Task<int> UpdateProductAsync(Product product)
        {
            try
            {
                const string sql = @"
                UPDATE [Products]
                SET 
                    Name                = @Name,
                    ImageAssetId        = @ImageAssetId,
                    SmallDescription    = @SmallDescription,
                    Price               = @Price,
                    BrandId             = @BrandId,
                    SubcategoryId       = @SubcategoryId,
                    IsDiscount          = @IsDiscount,
                    DiscountType        = @DiscountType,
                    DiscountAmount      = @DiscountAmount,
                    DiscountTime        = @DiscountTime,
                    FullDescriptionHTML = @FullDescriptionHTML
                WHERE Id = @Id
            ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Id", product.Id);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@ImageAssetId", (object?)product.ImageAssetId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SmallDescription", product.SmallDescription);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@BrandId", product.BrandId);
                cmd.Parameters.AddWithValue("@SubcategoryId", product.SubcategoryId);
                cmd.Parameters.AddWithValue("@IsDiscount", product.IsDiscount);
                cmd.Parameters.AddWithValue("@DiscountType", (object?)(int?)product.DiscountType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountAmount", (object?)product.DiscountAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountTime", (object?)product.DiscountTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FullDescriptionHTML", (object?)product.FullDescriptionHTML ?? DBNull.Value);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteProductAsync(int productId)
        {
            try
            {
                const string sql = @"DELETE FROM [Products] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Id", productId);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region Brand

        public static async Task<int> AddBrandAsync(Brand brand)
        {
            try
            {
                const string sql = @"
                INSERT INTO [Brands] (Name)
                VALUES (@Name);
                SELECT SCOPE_IDENTITY();
            ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", brand.Name);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<Brand?> GetBrandByIdAsync(int id)
        {
            try
            {
                const string sql = @"SELECT Id, Name FROM [Brands] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var ordId = reader.GetOrdinal("Id");
                var ordName = reader.GetOrdinal("Name");

                return new Brand
                {
                    Id = reader.GetInt32(ordId),
                    Name = reader.GetString(ordName)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Brand>> GetAllBrandsAsync()
        {
            var list = new List<Brand>();
            try
            {
                const string sql = @"SELECT Id, Name FROM [Brands]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var ordId = reader.GetOrdinal("Id");
                var ordName = reader.GetOrdinal("Name");

                while (await reader.ReadAsync())
                {
                    list.Add(new Brand
                    {
                        Id = reader.GetInt32(ordId),
                        Name = reader.GetString(ordName)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }

            return list;
        }

        public static async Task<int> UpdateBrandAsync(Brand brand)
        {
            try
            {
                const string sql = @"
                UPDATE [Brands]
                SET Name = @Name
                WHERE Id = @Id
            ";

                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", brand.Id);
                cmd.Parameters.AddWithValue("@Name", brand.Name);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteBrandAsync(int id)
        {
            try
            {
                const string sql = @"DELETE FROM [Brands] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region Store

        public static async Task<int> AddStoreAsync(Store store)
        {
            try
            {
                const string sql = @"
                INSERT INTO [Stores] (Name, Address, City)
                VALUES (@Name, @Address, @City);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", store.Name);
                cmd.Parameters.AddWithValue("@Address", (object?)store.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object?)store.City ?? DBNull.Value);

                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<Store?> GetStoreByIdAsync(int id)
        {
            try
            {
                const string sql = "SELECT Id, Name, Address, City FROM [Stores] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oName = reader.GetOrdinal("Name");
                var oAddress = reader.GetOrdinal("Address");
                var oCity = reader.GetOrdinal("City");

                return new Store
                {
                    Id = reader.GetInt32(oId),
                    Name = reader.GetString(oName),
                    Address = reader.IsDBNull(oAddress) ? null : reader.GetString(oAddress),
                    City = reader.IsDBNull(oCity) ? null : reader.GetString(oCity)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Store>> GetAllStoresAsync()
        {
            var list = new List<Store>();
            try
            {
                const string sql = "SELECT Id, Name, Address, City FROM [Stores]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oName = reader.GetOrdinal("Name");
                var oAddress = reader.GetOrdinal("Address");
                var oCity = reader.GetOrdinal("City");

                while (await reader.ReadAsync())
                {
                    list.Add(new Store
                    {
                        Id = reader.GetInt32(oId),
                        Name = reader.GetString(oName),
                        Address = reader.IsDBNull(oAddress) ? null : reader.GetString(oAddress),
                        City = reader.IsDBNull(oCity) ? null : reader.GetString(oCity)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateStoreAsync(Store store)
        {
            try
            {
                const string sql = @"
                UPDATE [Stores]
                SET Name = @Name,
                    Address = @Address,
                    City = @City
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", store.Id);
                cmd.Parameters.AddWithValue("@Name", store.Name);
                cmd.Parameters.AddWithValue("@Address", (object?)store.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object?)store.City ?? DBNull.Value);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteStoreAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [Stores] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region ProductCategory

        public static async Task<int> AddProductCategoryAsync(ProductCategory cat)
        {
            try
            {
                const string sql = @"
                INSERT INTO [ProductCategories] (Name)
                VALUES (@Name);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", cat.Name);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<ProductCategory?> GetProductCategoryByIdAsync(int id)
        {
            try
            {
                const string sql = "SELECT Id, Name FROM [ProductCategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oName = reader.GetOrdinal("Name");

                return new ProductCategory
                {
                    Id = reader.GetInt32(oId),
                    Name = reader.GetString(oName)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<ProductCategory>> GetAllProductCategoriesAsync()
        {
            var list = new List<ProductCategory>();
            try
            {
                const string sql = "SELECT Id, Name FROM [ProductCategories]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oName = reader.GetOrdinal("Name");

                while (await reader.ReadAsync())
                {
                    list.Add(new ProductCategory
                    {
                        Id = reader.GetInt32(oId),
                        Name = reader.GetString(oName)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateProductCategoryAsync(ProductCategory cat)
        {
            try
            {
                const string sql = @"
                UPDATE [ProductCategories]
                SET Name = @Name
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", cat.Id);
                cmd.Parameters.AddWithValue("@Name", cat.Name);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteProductCategoryAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [ProductCategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region ProductSubcategory

        public static async Task<int> AddProductSubcategoryAsync(ProductSubcategory sub)
        {
            try
            {
                const string sql = @"
                INSERT INTO [ProductSubcategories] (CategoryId, Name)
                VALUES (@CategoryId, @Name);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CategoryId", sub.CategoryId);
                cmd.Parameters.AddWithValue("@Name", sub.Name);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<ProductSubcategory?> GetProductSubcategoryByIdAsync(int id)
        {
            try
            {
                const string sql = "SELECT Id, CategoryId, Name FROM [ProductSubcategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oCat = reader.GetOrdinal("CategoryId");
                var oName = reader.GetOrdinal("Name");

                return new ProductSubcategory
                {
                    Id = reader.GetInt32(oId),
                    CategoryId = reader.GetInt32(oCat),
                    Name = reader.GetString(oName)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<ProductSubcategory>> GetAllProductSubcategoriesAsync()
        {
            var list = new List<ProductSubcategory>();
            try
            {
                const string sql = "SELECT Id, CategoryId, Name FROM [ProductSubcategories]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oCat = reader.GetOrdinal("CategoryId");
                var oName = reader.GetOrdinal("Name");

                while (await reader.ReadAsync())
                {
                    list.Add(new ProductSubcategory
                    {
                        Id = reader.GetInt32(oId),
                        CategoryId = reader.GetInt32(oCat),
                        Name = reader.GetString(oName)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateProductSubcategoryAsync(ProductSubcategory sub)
        {
            try
            {
                const string sql = @"
                UPDATE [ProductSubcategories]
                SET CategoryId = @CategoryId,
                    Name       = @Name
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", sub.Id);
                cmd.Parameters.AddWithValue("@CategoryId", sub.CategoryId);
                cmd.Parameters.AddWithValue("@Name", sub.Name);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteProductSubcategoryAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [ProductSubcategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region BlogCategory

        public static async Task<int> AddBlogCategoryAsync(BlogCategory cat)
        {
            try
            {
                const string sql = @"
                INSERT INTO [BlogCategories] (Name)
                VALUES (@Name);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", cat.Name);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<BlogCategory?> GetBlogCategoryByIdAsync(int id)
        {
            try
            {
                const string sql = "SELECT Id, Name FROM [BlogCategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oName = reader.GetOrdinal("Name");

                return new BlogCategory
                {
                    Id = reader.GetInt32(oId),
                    Name = reader.GetString(oName)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<BlogCategory>> GetAllBlogCategoriesAsync()
        {
            var list = new List<BlogCategory>();
            try
            {
                const string sql = "SELECT Id, Name FROM [BlogCategories]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oName = reader.GetOrdinal("Name");

                while (await reader.ReadAsync())
                {
                    list.Add(new BlogCategory
                    {
                        Id = reader.GetInt32(oId),
                        Name = reader.GetString(oName)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateBlogCategoryAsync(BlogCategory cat)
        {
            try
            {
                const string sql = @"
                UPDATE [BlogCategories]
                SET Name = @Name
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", cat.Id);
                cmd.Parameters.AddWithValue("@Name", cat.Name);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteBlogCategoryAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [BlogCategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region BlogSubcategory

        public static async Task<int> AddBlogSubcategoryAsync(BlogSubcategory sub)
        {
            try
            {
                const string sql = @"
                INSERT INTO [BlogSubcategories] (BlogCategoryId, Name)
                VALUES (@BlogCategoryId, @Name);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BlogCategoryId", sub.BlogCategoryId);
                cmd.Parameters.AddWithValue("@Name", sub.Name);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<BlogSubcategory?> GetBlogSubcategoryByIdAsync(int id)
        {
            try
            {
                const string sql = "SELECT Id, BlogCategoryId, Name FROM [BlogSubcategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oCat = reader.GetOrdinal("BlogCategoryId");
                var oName = reader.GetOrdinal("Name");

                return new BlogSubcategory
                {
                    Id = reader.GetInt32(oId),
                    BlogCategoryId = reader.GetInt32(oCat),
                    Name = reader.GetString(oName)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<BlogSubcategory>> GetAllBlogSubcategoriesAsync()
        {
            var list = new List<BlogSubcategory>();
            try
            {
                const string sql = "SELECT Id, BlogCategoryId, Name FROM [BlogSubcategories]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oCat = reader.GetOrdinal("BlogCategoryId");
                var oName = reader.GetOrdinal("Name");

                while (await reader.ReadAsync())
                {
                    list.Add(new BlogSubcategory
                    {
                        Id = reader.GetInt32(oId),
                        BlogCategoryId = reader.GetInt32(oCat),
                        Name = reader.GetString(oName)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateBlogSubcategoryAsync(BlogSubcategory sub)
        {
            try
            {
                const string sql = @"
                UPDATE [BlogSubcategories]
                SET BlogCategoryId = @BlogCategoryId,
                    Name           = @Name
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", sub.Id);
                cmd.Parameters.AddWithValue("@BlogCategoryId", sub.BlogCategoryId);
                cmd.Parameters.AddWithValue("@Name", sub.Name);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteBlogSubcategoryAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [BlogSubcategories] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region Blog

        public static async Task<int> AddBlogAsync(Blog blog)
        {
            try
            {
                const string sql = @"
                INSERT INTO [Blog]
                  (ImageAssetId, Name, Description, SubcategoryId, FullTextHTML, CreatedAt, EditedAt)
                VALUES
                  (@ImageAssetId, @Name, @Description, @SubcategoryId, @FullTextHTML, @CreatedAt, @EditedAt);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ImageAssetId", (object?)blog.ImageAssetId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", blog.Name);
                cmd.Parameters.AddWithValue("@Description", blog.Description);
                cmd.Parameters.AddWithValue("@SubcategoryId", blog.SubcategoryId);
                cmd.Parameters.AddWithValue("@FullTextHTML", blog.FullTextHTML);
                cmd.Parameters.AddWithValue("@CreatedAt", blog.CreatedAt);
                cmd.Parameters.AddWithValue("@EditedAt", (object?)blog.EditedAt ?? DBNull.Value);

                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<Blog?> GetBlogByIdAsync(int id)
        {
            try
            {
                const string sql = @"
                SELECT Id, ImageAssetId, Name, Description, SubcategoryId, FullTextHTML, CreatedAt, EditedAt
                  FROM [Blog] WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var ord = new
                {
                    Id = reader.GetOrdinal("Id"),
                    Img = reader.GetOrdinal("ImageAssetId"),
                    Name = reader.GetOrdinal("Name"),
                    Desc = reader.GetOrdinal("Description"),
                    Sub = reader.GetOrdinal("SubcategoryId"),
                    HTML = reader.GetOrdinal("FullTextHTML"),
                    CrAt = reader.GetOrdinal("CreatedAt"),
                    EdAt = reader.GetOrdinal("EditedAt")
                };

                return new Blog
                {
                    Id = reader.GetInt32(ord.Id),
                    ImageAssetId = reader.IsDBNull(ord.Img) ? null : reader.GetString(ord.Img),
                    Name = reader.GetString(ord.Name),
                    Description = reader.GetString(ord.Desc),
                    SubcategoryId = reader.GetInt32(ord.Sub),
                    FullTextHTML = reader.GetString(ord.HTML),
                    CreatedAt = reader.GetDateTime(ord.CrAt),
                    EditedAt = reader.IsDBNull(ord.EdAt) ? null : reader.GetDateTime(ord.EdAt)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Blog>> GetAllBlogsAsync()
        {
            var list = new List<Blog>();
            try
            {
                const string sql = @"
                SELECT Id, ImageAssetId, Name, Description, SubcategoryId, FullTextHTML, CreatedAt, EditedAt
                  FROM [Blog]
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var ord = new
                {
                    Id = reader.GetOrdinal("Id"),
                    Img = reader.GetOrdinal("ImageAssetId"),
                    Name = reader.GetOrdinal("Name"),
                    Desc = reader.GetOrdinal("Description"),
                    Sub = reader.GetOrdinal("SubcategoryId"),
                    HTML = reader.GetOrdinal("FullTextHTML"),
                    CrAt = reader.GetOrdinal("CreatedAt"),
                    EdAt = reader.GetOrdinal("EditedAt")
                };

                while (await reader.ReadAsync())
                {
                    list.Add(new Blog
                    {
                        Id = reader.GetInt32(ord.Id),
                        ImageAssetId = reader.IsDBNull(ord.Img) ? null : reader.GetString(ord.Img),
                        Name = reader.GetString(ord.Name),
                        Description = reader.GetString(ord.Desc),
                        SubcategoryId = reader.GetInt32(ord.Sub),
                        FullTextHTML = reader.GetString(ord.HTML),
                        CreatedAt = reader.GetDateTime(ord.CrAt),
                        EditedAt = reader.IsDBNull(ord.EdAt) ? null : reader.GetDateTime(ord.EdAt)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateBlogAsync(Blog blog)
        {
            try
            {
                const string sql = @"
                UPDATE [Blog]
                SET
                  ImageAssetId = @ImageAssetId,
                  Name         = @Name,
                  Description  = @Description,
                  SubcategoryId= @SubcategoryId,
                  FullTextHTML = @FullTextHTML,
                  EditedAt     = @EditedAt
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Id", blog.Id);
                cmd.Parameters.AddWithValue("@ImageAssetId", (object?)blog.ImageAssetId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", blog.Name);
                cmd.Parameters.AddWithValue("@Description", blog.Description);
                cmd.Parameters.AddWithValue("@SubcategoryId", blog.SubcategoryId);
                cmd.Parameters.AddWithValue("@FullTextHTML", blog.FullTextHTML);
                cmd.Parameters.AddWithValue("@EditedAt", (object?)blog.EditedAt ?? DBNull.Value);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteBlogAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [Blog] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region ProductSpecification

        public static async Task<int> AddProductSpecificationAsync(ProductSpecification spec)
        {
            try
            {
                const string sql = @"
                INSERT INTO [ProductSpecifications] (ProductId, Name, Description)
                VALUES (@ProductId, @Name, @Description);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductId", spec.ProductId);
                cmd.Parameters.AddWithValue("@Name", spec.Name);
                cmd.Parameters.AddWithValue("@Description", spec.Description);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<ProductSpecification?> GetProductSpecificationByIdAsync(int id)
        {
            try
            {
                const string sql = @"
                SELECT Id, ProductId, Name, Description
                FROM [ProductSpecifications]
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oPid = reader.GetOrdinal("ProductId");
                var oName = reader.GetOrdinal("Name");
                var oDesc = reader.GetOrdinal("Description");

                return new ProductSpecification
                {
                    Id = reader.GetInt32(oId),
                    ProductId = reader.GetInt32(oPid),
                    Name = reader.GetString(oName),
                    Description = reader.GetString(oDesc)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<ProductSpecification>> GetAllProductSpecificationsAsync()
        {
            var list = new List<ProductSpecification>();
            try
            {
                const string sql = "SELECT Id, ProductId, Name, Description FROM [ProductSpecifications]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oPid = reader.GetOrdinal("ProductId");
                var oName = reader.GetOrdinal("Name");
                var oDesc = reader.GetOrdinal("Description");

                while (await reader.ReadAsync())
                {
                    list.Add(new ProductSpecification
                    {
                        Id = reader.GetInt32(oId),
                        ProductId = reader.GetInt32(oPid),
                        Name = reader.GetString(oName),
                        Description = reader.GetString(oDesc)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateProductSpecificationAsync(ProductSpecification spec)
        {
            try
            {
                const string sql = @"
                UPDATE [ProductSpecifications]
                SET 
                  ProductId   = @ProductId,
                  Name        = @Name,
                  Description = @Description
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", spec.Id);
                cmd.Parameters.AddWithValue("@ProductId", spec.ProductId);
                cmd.Parameters.AddWithValue("@Name", spec.Name);
                cmd.Parameters.AddWithValue("@Description", spec.Description);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteProductSpecificationAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [ProductSpecifications] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region Warehouse

        public static async Task<int> AddWarehouseEntryAsync(Warehouse wh)
        {
            try
            {
                const string sql = @"
                INSERT INTO [Warehouse] (ProductId, StoreId, Amount)
                VALUES (@ProductId, @StoreId, @Amount);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductId", wh.ProductId);
                cmd.Parameters.AddWithValue("@StoreId", wh.StoreId);
                cmd.Parameters.AddWithValue("@Amount", wh.Amount);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<Warehouse?> GetWarehouseByIdAsync(int id)
        {
            try
            {
                const string sql = @"
                SELECT Id, ProductId, StoreId, Amount
                FROM [Warehouse]
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oPid = reader.GetOrdinal("ProductId");
                var oSid = reader.GetOrdinal("StoreId");
                var oAmt = reader.GetOrdinal("Amount");

                return new Warehouse
                {
                    Id = reader.GetInt32(oId),
                    ProductId = reader.GetInt32(oPid),
                    StoreId = reader.GetInt32(oSid),
                    Amount = reader.GetInt32(oAmt)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Warehouse>> GetAllWarehouseEntriesAsync()
        {
            var list = new List<Warehouse>();
            try
            {
                const string sql = "SELECT Id, ProductId, StoreId, Amount FROM [Warehouse]";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oPid = reader.GetOrdinal("ProductId");
                var oSid = reader.GetOrdinal("StoreId");
                var oAmt = reader.GetOrdinal("Amount");

                while (await reader.ReadAsync())
                {
                    list.Add(new Warehouse
                    {
                        Id = reader.GetInt32(oId),
                        ProductId = reader.GetInt32(oPid),
                        StoreId = reader.GetInt32(oSid),
                        Amount = reader.GetInt32(oAmt)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateWarehouseAsync(Warehouse wh)
        {
            try
            {
                const string sql = @"
                UPDATE [Warehouse]
                SET ProductId = @ProductId,
                    StoreId   = @StoreId,
                    Amount    = @Amount
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", wh.Id);
                cmd.Parameters.AddWithValue("@ProductId", wh.ProductId);
                cmd.Parameters.AddWithValue("@StoreId", wh.StoreId);
                cmd.Parameters.AddWithValue("@Amount", wh.Amount);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteWarehouseAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [Warehouse] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion


        #region Review

        public static async Task<int> AddReviewAsync(Review review)
        {
            try
            {
                const string sql = @"
                INSERT INTO [Reviews] (ProductId, AuthorName, Text, Rating, CreatedAt)
                VALUES (@ProductId, @AuthorName, @Text, @Rating, @CreatedAt);
                SELECT SCOPE_IDENTITY();
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductId", review.ProductId);
                cmd.Parameters.AddWithValue("@AuthorName", review.AuthorName);
                cmd.Parameters.AddWithValue("@Text", review.Text);
                cmd.Parameters.AddWithValue("@Rating", review.Rating);
                cmd.Parameters.AddWithValue("@CreatedAt", review.CreatedAt);
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<Review?> GetReviewByIdAsync(int id)
        {
            try
            {
                const string sql = @"
                SELECT Id, ProductId, AuthorName, Text, Rating, CreatedAt
                FROM [Reviews]
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;

                var oId = reader.GetOrdinal("Id");
                var oPid = reader.GetOrdinal("ProductId");
                var oAuth = reader.GetOrdinal("AuthorName");
                var oText = reader.GetOrdinal("Text");
                var oRate = reader.GetOrdinal("Rating");
                var oCrt = reader.GetOrdinal("CreatedAt");

                return new Review
                {
                    Id = reader.GetInt32(oId),
                    ProductId = reader.GetInt32(oPid),
                    AuthorName = reader.GetString(oAuth),
                    Text = reader.GetString(oText),
                    Rating = reader.GetByte(oRate),
                    CreatedAt = reader.GetDateTime(oCrt)
                };
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<List<Review>> GetAllReviewsAsync()
        {
            var list = new List<Review>();
            try
            {
                const string sql = @"
                SELECT Id, ProductId, AuthorName, Text, Rating, CreatedAt
                FROM [Reviews]
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var oId = reader.GetOrdinal("Id");
                var oPid = reader.GetOrdinal("ProductId");
                var oAuth = reader.GetOrdinal("AuthorName");
                var oText = reader.GetOrdinal("Text");
                var oRate = reader.GetOrdinal("Rating");
                var oCrt = reader.GetOrdinal("CreatedAt");

                while (await reader.ReadAsync())
                {
                    list.Add(new Review
                    {
                        Id = reader.GetInt32(oId),
                        ProductId = reader.GetInt32(oPid),
                        AuthorName = reader.GetString(oAuth),
                        Text = reader.GetString(oText),
                        Rating = reader.GetByte(oRate),
                        CreatedAt = reader.GetDateTime(oCrt)
                    });
                }
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
            return list;
        }

        public static async Task<int> UpdateReviewAsync(Review review)
        {
            try
            {
                const string sql = @"
                UPDATE [Reviews]
                SET
                  ProductId  = @ProductId,
                  AuthorName = @AuthorName,
                  Text       = @Text,
                  Rating     = @Rating
                WHERE Id = @Id
            ";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", review.Id);
                cmd.Parameters.AddWithValue("@ProductId", review.ProductId);
                cmd.Parameters.AddWithValue("@AuthorName", review.AuthorName);
                cmd.Parameters.AddWithValue("@Text", review.Text);
                cmd.Parameters.AddWithValue("@Rating", review.Rating);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        public static async Task<int> DeleteReviewAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM [Reviews] WHERE Id = @Id";
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                PrintConsoleError(ex);
                throw;
            }
        }

        #endregion

    }
}
