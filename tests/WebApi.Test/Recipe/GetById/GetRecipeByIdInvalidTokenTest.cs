using CommonTestUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;

namespace WebApi.Test.Recipe.GetById
{
    public class GetRecipeByIdInvalidTokenTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe";

        public GetRecipeByIdInvalidTokenTest(CustomWebApplicationFactory webApplication) : base(webApplication)
        {
        }

        [Fact]
        public async Task Error_Token_Invalid()
        {
            var id = IdEncrypterBuilder.Build().Encode(1);

            var response = await DoGet($"{METHOD}/{id}", token: "tokenInvalid");

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            var id = IdEncrypterBuilder.Build().Encode(1);

            var response = await DoGet($"{METHOD}/{id}", token: string.Empty);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            var id = IdEncrypterBuilder.Build().Encode(1);

            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var response = await DoGet($"{METHOD}/{id}", token: token);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
