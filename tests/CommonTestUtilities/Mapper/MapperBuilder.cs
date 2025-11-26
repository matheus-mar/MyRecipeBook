using AutoMapper;
using CommonTestUtilities.IdEncryption;
using MyRecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtilities.Mapper
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var idEncrypter = IdEncrypterBuilder.Build();

            var mapper = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping(idEncrypter));
            }).CreateMapper();

            return mapper;
        }
    }
}
