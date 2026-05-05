using APBD_TEST_TEMPLATE.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_TEST_TEMPLATE.Services
{
    public class VendorService : IVendorService
    {
        private readonly string _connectionString;

        public VendorService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<GetVendorResponse> GetVendorAsync(string code)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var vendorQuery = "SELECT Code, Name FROM Vendors WHERE Code = @Code";
            using var vendorCommand = new SqlCommand(vendorQuery, connection);
            vendorCommand.Parameters.AddWithValue("@Code", code);

            using var vendorReader = await vendorCommand.ExecuteReaderAsync();

            if (!await vendorReader.ReadAsync())
            {
                return null;
            }

            var response = new GetVendorResponse
            {
                Code = vendorReader.GetString(0),
                Name = vendorReader.GetString(1)
            };
            await vendorReader.CloseAsync();

            var productsQuery = @"
            SELECT p.Id, p.Name, p.Description, p.StickerPrice,
                   pt.Id AS PtId, pt.Name AS PtName,
                   m.Id AS MId, m.Name AS MName,
                   vp.Amount, vp.PricePerUnit
            FROM VendorProducts vp
            JOIN Products p ON vp.ProductId = p.Id
            JOIN ProductTypes pt ON p.ProductTypeId = pt.Id
            JOIN Makers m ON p.MakerId = m.Id
            WHERE vp.VendorCode = @Code";

            using var productsCommand = new SqlCommand(productsQuery, connection);
            productsCommand.Parameters.AddWithValue("@Code", code);

            using var productsReader = await productsCommand.ExecuteReaderAsync();

            while (await productsReader.ReadAsync())
            {
                response.Products.Add(new ProductDto
                {
                    Id = productsReader.GetInt32(0),
                    Name = productsReader.GetString(1),
                    Description = productsReader.IsDBNull(2) ? null : productsReader.GetString(2),
                    StrickerPrice = productsReader.GetDecimal(3),
                    ProductType = new ProductTypeDto
                    {
                        Id = productsReader.GetInt32(4),
                        Name = productsReader.GetString(5)
                    },
                    Maker = new MakerDto
                    {
                        Id = productsReader.GetInt32(6),
                        Name = productsReader.GetString(7)
                    },
                    VendorOffer = new VendorOfferDto
                    {
                        Amount = productsReader.GetInt32(8),
                        PricePerUnit = productsReader.GetDecimal(9)
                    }
                });
            }

            return response;
        }

        public async Task<bool> CreateVendorAsync(CreateVendorRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var insertVendorQuery = "INSERT INTO Vendors (Code, Name) VALUES (@Code, @Name)";
                using var vendorCommand = new SqlCommand(insertVendorQuery, connection, transaction);
                vendorCommand.Parameters.AddWithValue("@Code", request.Code);
                vendorCommand.Parameters.AddWithValue("@Name", request.Name);
                await vendorCommand.ExecuteNonQueryAsync();

                if (request.Products != null && request.Products.Count > 0)
                {
                    foreach (var product in request.Products)
                    {
                        var insertProductQuery = @"
                        INSERT INTO VendorProducts (ProductId, VendorCode, Amount, PricePerUnit)
                        VALUES (@ProductId, @VendorCode, @Amount, @PricePerUnit)";

                        using var productCommand = new SqlCommand(insertProductQuery, connection, transaction);
                        productCommand.Parameters.AddWithValue("@ProductId", product.Id);
                        productCommand.Parameters.AddWithValue("@VendorCode", request.Code);
                        productCommand.Parameters.AddWithValue("@Amount", product.Amount);
                        productCommand.Parameters.AddWithValue("@PricePerUnit", product.PricePerUnit);

                        await productCommand.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}