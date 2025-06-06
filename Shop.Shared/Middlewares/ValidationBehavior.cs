using FluentValidation;
using MediatR;

namespace Shop.Shared.Middlewares
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var errors = validators
                .Select(v => v.Validate(context))
                .SelectMany(res => res.Errors)
                .Where(er  => er != null)
                .ToList();

            if (errors.Any()) throw new ValidationException(errors);

            return await next(cancellationToken);
        }
    }
}
