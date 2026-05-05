namespace APBD_TEST_TEMPLATE.DTOs
{
    public class GetVendorResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal StrickerPrice { get; set; }
        public ProductTypeDto ProductType { get; set; }
        public MakerDto Maker { get; set; }
        public VendorOfferDto VendorOffer { get; set; }
    }

    public class ProductTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MakerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class VendorOfferDto
    {
        public int Amount { get; set; }
        public decimal PricePerUnit { get; set; }
    }

    public class CreateVendorRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<CreateVendorProductRequest> Products { get; set; } = new List<CreateVendorProductRequest>();
    }

    public class CreateVendorProductRequest
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
