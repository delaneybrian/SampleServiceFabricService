using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ProductActorService.Interfaces;
using Contracts;

namespace ProductActorService
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class ProductActorService : Actor, IProductActorService, IRemindable
    {
        private string ProductStateName = "ProductState";

        private IActorTimer _actorTimer;

        public ProductActorService(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        public async Task AddProductAsync(Product product, CancellationToken cancellationToken)
        {
            await this.StateManager.AddOrUpdateStateAsync(ProductStateName, product, (k, v) => product, cancellationToken);

            await this.StateManager.SaveStateAsync(cancellationToken);
        }

        public async Task<Product> GetProductAsync(CancellationToken cancellationToken)
        {
            var product = await this.StateManager.GetStateAsync<Product>(ProductStateName, cancellationToken);

            return product;
        }

        protected override Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"{actorMethodContext.MethodName} has finished");

            return base.OnPostActorMethodAsync(actorMethodContext);
        }

        protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"{actorMethodContext.MethodName} will start soon");

            return base.OnPostActorMethodAsync(actorMethodContext);
        }

        protected override Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor deactivated.");

            return base.OnDeactivateAsync();
        }

        protected override Task OnActivateAsync()
        {
            var dbPort = Environment.GetEnvironmentVariable("DbPort");

            //var dbConfig = this.ActorService.Context
            //    .CodePackageActivationContext
            //    .GetConfigurationPackageObject("Config")
            //    .Settings
            //    .Sections["Database"]
            //    .Parameters["DbConfig"]
            //    .Value;

            //var dataPackage = this.ActorService.Context.CodePackageActivationContext.GetDataPackageObject("Data");

            //var dataPath = Path.Combine(dataPackage.Path, "test.csv");

            //var contents = File.ReadAllText(dataPath);

            this.RegisterReminderAsync("TaskReminder", null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));

            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return this.StateManager.TryAddStateAsync("count", 0);
        }


        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == "TaskReminder")
            {
                ActorEventSource.Current.ActorMessage(this, $"Reminder is doing work");
            }
        }
    }
}
