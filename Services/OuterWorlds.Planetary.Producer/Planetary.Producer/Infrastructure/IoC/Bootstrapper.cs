using Database.Interface;
using Database.Redis;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Linq;
using System.Reflection;

namespace Planetary.Producer.Infrastructure.IoC
{
    public class Bootstrapper
    {
        public Bootstrapper(Container container)
        {
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.RegisterSingleton<IRedisCache>(() => new RedisCache());

            InjectLog(container);
        }

        private void InjectLog(Container container)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("ApplicationName", "Azul.CDO.Disruption")
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            container.Register(() => Log.Logger, Lifestyle.Singleton);
        }

        private void InjectMediator(Container container, params Assembly[] assemblies)
        {
            var mediators = new[] { typeof(IMediator).GetTypeInfo().Assembly };
            assemblies.Concat(mediators);

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);
            container.Register(typeof(IRequestHandler<>), assemblies);

            container.Collection.Register(typeof(IPipelineBehavior<,>), Enumerable.Empty<Type>());
            container.Collection.Register(typeof(IRequestPreProcessor<>), Enumerable.Empty<Type>());
            container.Collection.Register(typeof(IRequestPostProcessor<,>), Enumerable.Empty<Type>());

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }
    }
}
