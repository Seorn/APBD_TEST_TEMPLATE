using APBD_TEST_TEMPLATE.DTOs;

namespace APBD_TEST_TEMPLATE.Services
{
    public interface IVendorService
    {
        Task<GetVendorResponse> GetVendorAsync(string code);
        Task<bool> CreateVendorAsync(CreateVendorRequest request);
    }
}

