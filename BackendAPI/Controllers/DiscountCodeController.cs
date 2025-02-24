using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Authorize(Roles = "ConcernAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCodeController : Controller
    {
        private readonly DatabaseContext _Context;

        public DiscountCodeController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpPost("CreateDiscountCode")]
        public async Task<ActionResult> CreateHotel([FromBody] CreateDCDTO discountCodeDTO)
        {
            if(discountCodeDTO.Percentage < 1 ||  discountCodeDTO.Percentage > 100)
            {
                return BadRequest("Only discounts between 1 and 100 is allowed");
            }
            var discountCode = new DiscountCode()
            {
                ID = Guid.NewGuid().ToString(),
                Code = discountCodeDTO.Code,
                Percentage = discountCodeDTO.Percentage,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };
            _Context.DiscountCodes.Add(discountCode);
            await _Context.SaveChangesAsync();

            return Ok(discountCodeDTO);
        }

        [HttpGet("GetAllDiscountCodes")]
        public async Task<ActionResult<DiscountCode>> GetDiscountCodes()
        {
            var discountCodes = await _Context.DiscountCodes.ToListAsync();
            return Ok(discountCodes);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountCode(string id)
        {
            var discountCode = await _Context.DiscountCodes.FindAsync(id);

            _Context.DiscountCodes.Remove(discountCode);

            await _Context.SaveChangesAsync();

            return StatusCode(200, $"Discount code deleted succesfully!");
        }
    }
}