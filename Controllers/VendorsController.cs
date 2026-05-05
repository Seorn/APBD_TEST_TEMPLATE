using APBD_TEST_TEMPLATE.DTOs;
using APBD_TEST_TEMPLATE.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_TEST_TEMPLATE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorsController(IVendorService vendorService)

        {
            _vendorService = vendorService;
        }


        [HttpGet("{code}")]
        public async Task<IActionResult> GetVendor(string code)
        {
            var vendor = await _vendorService.GetVendorAsync(code);

            if (vendor == null)
            {
                return NotFound($"Vendor {code} not found.");
            }

            return Ok(vendor);
        }


        [HttpPost]
        public async Task<IActionResult> CreateVendor(CreateVendorRequest request)
        {
            var result = await _vendorService.CreateVendorAsync(request);

            if (!result)
            {
                return BadRequest("Failed");
            }

            return StatusCode(201);
        }


    }

}
