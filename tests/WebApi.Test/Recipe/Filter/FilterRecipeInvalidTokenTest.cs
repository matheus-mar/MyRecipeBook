using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;

namespace WebApi.Test.Recipe.Filter
{
    public class FilterRecipeInvalidTokenTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe/filter";

        public FilterRecipeInvalidTokenTest(CustomWebApplicationFactory webApplication) : base(webApplication)
        {
        }

        [Fact]
        public async Task Error_Token_Invalid()
        {
            var request = RequestFilterRecipeJsonBuilder.Build();

            var response = await DoPost(method: METHOD, request: request, token: "tokenInvalid");

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            var request = RequestFilterRecipeJsonBuilder.Build();

            var response = await DoPost(method: METHOD, request: request, token: string.Empty);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            var request = RequestFilterRecipeJsonBuilder.Build();

            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var response = await DoPost(method: METHOD, request: request, token: token);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
