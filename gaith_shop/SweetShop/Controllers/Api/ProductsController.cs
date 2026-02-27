using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SweetShop.Models;
using System.Collections.Generic;

namespace SweetShop.Controllers.Api
{
    /// <summary>
    /// API Controller for managing products in the e-commerce store.
    /// Requires Bearer JWT Authentication for write operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of all active products.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Products?pageNumber=1&amp;pageSize=10
        ///
        /// </remarks>
        /// <param name="pageNumber">The page number to retrieve</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns>A list of product objects</returns>
        /// <response code="200">Returns the list of matching products</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Get all products",
            Description = "Fetches a paginated list of all currently active products available in the store.",
            OperationId = "GetProducts"
        )]
        public IActionResult GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Placeholder: Fetch from actual MediatR query or Repository
            var dummyProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Chocolate Cake", Price = 20.50m },
                new Product { Id = 2, Name = "Strawberry Tart", Price = 15.00m }
            };

            return Ok(dummyProducts);
        }

        /// <summary>
        /// Retrieves a specific product by unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product</param>
        /// <returns>A single product object</returns>
        /// <response code="200">Returns the requested product</response>
        /// <response code="404">If the product is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get product by ID",
            Description = "Fetches a single product item by its unique database ID.",
            OperationId = "GetProductById"
        )]
        public IActionResult GetProductById(int id)
        {
            // Placeholder
            if (id <= 0)
                return NotFound();

            var product = new Product { Id = id, Name = "Test Product", Price = 10.00m };
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product. (Admin &amp; Staff Only)
        /// </summary>
        /// <param name="product">The product details to create from the request body</param>
        /// <returns>The newly created product</returns>
        /// <response code="201">Returns the newly created product</response>
        /// <response code="400">If the item is null or validation fails</response>
        /// <response code="401">If the user is unauthorized (No valid JWT)</response>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(
            Summary = "Create a new product",
            Description = "Creates a new active product in the catalog. **Requires valid Bearer Token.**",
            OperationId = "CreateProduct"
        )]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Placeholder logic: Saving product via MediatR Command
            product.Id = new System.Random().Next(100, 1000);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
    }
}
