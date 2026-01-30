using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceSentry.Application.Price.Queries.GetPriceHistoryList;
using PriceSentry.Application.Product.Commands.Create;
using PriceSentry.Application.Product.Commands.Delete;
using PriceSentry.Application.Product.Commands.Update;
using PriceSentry.Application.Product.Queries.GetActualPrice;
using PriceSentry.Application.Product.Queries.GetListProducts;
using PriceSentry.Application.Product.Queries.GetProduct;
using PriceSentry.Domain;
using PriceSentry.WebApi.Models;

namespace PriceSentry.WebApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PriceProductController : BaseController {
        public readonly IMapper _mapper;

        public PriceProductController(IMapper mapper) => _mapper = mapper;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ProductListVm>> GetAllProducts() {
            var query = new ProductListQuery {
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpGet("actualprice/{id}")]
        [Authorize]
        public async Task<ActionResult<ActualPriceVm>> GetActualPrice(Guid id) {
            var query = new GetActualPriceQuery {
                Id = id,
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpGet("product/{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDitailsVm>> GetProductDitails(Guid id) {
            var query = new ProductDitailsQuery {
                Id = id,
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpGet("pricehistory/{id}")]
        [Authorize]
        public async Task<ActionResult<PriceListVm>> GetAllPriceOfProduct (Guid id) {
            var query = new GetPriceHistoryQuery {
                ProductId = id,
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guid>> Create([FromBody]CreateProductDto dto) {
            var command = _mapper.Map<CreateProductCommand>(dto);
            command.UserId = UserId;
            var productId = await Mediator.Send(command);
            return Ok(productId);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Guid>> Update([FromBody] UpdateProductDto dto) {
            var command = _mapper.Map<UpdateProductCommand>(dto); 
            command.UserId = UserId;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) {
            var command = new DeleteProductCommand { UserId = UserId, Id = id };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
