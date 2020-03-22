using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using ProductActorService.Interfaces;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        [HttpGet]
        public async Task<Product> GetProductById(
            [FromQuery] int id)
        {
            var actorId = new ActorId(id);

            var proxy = ActorProxy.Create<IProductActorService>(actorId, new Uri("fabric:/ProductActorApplication/ProductActorServiceActorService"));

            var product = await proxy.GetProductAsync(new CancellationToken());

            return product;
        }

        [HttpPost]
        public async Task AddProduct(
            [FromBody] Product product)
        {
            var actorId = new ActorId(product.Id);

            var proxy = ActorProxy.Create<IProductActorService>(actorId, new Uri("fabric:/ProductActorApplication/ProductActorServiceActorService"));

            await proxy.AddProductAsync(product, new CancellationToken());
        }

        [HttpDelete]
        public async Task DeleteActorById(
            [FromQuery] int id)
        {
            var actorId = new ActorId(id);

            var actorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/ProductActorApplication/ProductActorServiceActorService"),
                actorId);

            await actorServiceProxy.DeleteActorAsync(actorId, new CancellationToken());
        }
    }
}
