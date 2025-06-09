using AutoMapper;
using Shop.Library.API.Domain;
using Shop.Library.API.Features.Books;

namespace Shop.Library.API.Mappings
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<CreateBookCommand, Book>();
        }
    }
}
