using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OuterWorlds.Api.Infrastructure.Pipelines
{
    public class ValidatorRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest :IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        public ValidatorRequestBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failure = _validator.Validate(request).Errors;

            if (failure.Any())
                throw new ValidationException(failure);

            return next();
        }
    }
}
