using FluentValidation;

namespace Shop.Library.API.Features.Books
{
    public class CreateBookValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookValidator()
        {
            RuleFor(b => b.Title).NotEmpty().WithMessage("Please enter a title");
        }
    }
}
