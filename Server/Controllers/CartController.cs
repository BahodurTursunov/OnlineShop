using BaseLibrary.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("v1/api/carts")]
    public class CartController(ICartService cartService, ILogger<CartController> logger) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;
        private readonly ILogger<CartController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartDTO createCartDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var item in createCartDTO.CartItems)
            {
                await _cartService.AddItemToCartAsync(createCartDTO.UserId, item.ProductId, item.Quantity, cancellationToken);
            }
            var cart = await _cartService.GetCartByUserIdAsync(createCartDTO.UserId, cancellationToken);

            return CreatedAtAction(nameof(GetCartByUserId), new { userId = createCartDTO.UserId }, cart);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId, cancellationToken);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }
        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItemToCart(int userId, [FromBody] CartItemDTO itemDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var addedItem = await _cartService.AddItemToCartAsync(userId, itemDto.ProductId, itemDto.Quantity, cancellationToken);
                return Ok(addedItem);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}/items/{productId}")]
        public async Task<IActionResult> UpdateCartItem(int userId, int productId, [FromBody] UpdateCartItemDTO updateDto, CancellationToken cancellationToken)
        {
            if (updateDto.ProductId != productId)
            {
                return BadRequest(new { message = "Product ID mismatch." });
            }

            try
            {
                var updatedItem = await _cartService.UpdateCartItemAsync(userId, productId, updateDto.Quantity, cancellationToken);
                return Ok(updatedItem);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int userId, int productId, CancellationToken cancellationToken)
        {
            var success = await _cartService.RemoveItemFromCartAsync(userId, productId, cancellationToken);
            if (!success)
            {
                return NotFound(new { message = "Item not found in cart." });
            }

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(int userId, CancellationToken cancellationToken)
        {
            var success = await _cartService.ClearCartAsync(userId, cancellationToken);
            if (!success)
            {
                return NotFound(new { message = "Cart not found or already empty." });
            }

            return NoContent();
        }
    }
}
