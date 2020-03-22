using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace ProductActorService.Interfaces
{
    public interface IProductActorService : IActor
    {
        Task<Product> GetProductAsync(CancellationToken cancellationToken);

        Task AddProductAsync(Product product, CancellationToken cancellationToken);
    }
}
