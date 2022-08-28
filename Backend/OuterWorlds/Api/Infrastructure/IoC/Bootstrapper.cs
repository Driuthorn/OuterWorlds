using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using OuterWorlds.Api.Infrastructure.Pipelines;
using Serilog;
using SimpleInjector;
using System;
using System.Linq;
using System.Reflection;

namespace OuterWorlds.Api.Infrastructure.IoC
{
    public class Bootstrapper
    {
        public void Inject(Container container)
        {
            container.Register(() => Log.Logger, Lifestyle.Singleton);
            container.RegisterSingleton(typeof(IValidator<>));


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
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[] { typeof(ValidatorRequestBehavior<,>) });
        }

        private void InjectRepositories(Container container)
        {
            
        }
    }
}
