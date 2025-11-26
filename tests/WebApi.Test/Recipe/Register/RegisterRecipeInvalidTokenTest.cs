using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Test.Recipe.Register
{
    public class RegisterRecipeInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string METHOD = "recipe";

        private readonly Guid _userIdentifier;

        public RegisterRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Error_Token_Invalid()
        {
            var request = RequestRecipeJsonBuilder.Build();

            var response = await DoPost(method: METHOD, request: request, token: "tokenInvalid");

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            var request = RequestRecipeJsonBuilder.Build();

            var response = await DoPost(method: METHOD, request: request, token: string.Empty);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            var request = RequestRecipeJsonBuilder.Build();

            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var response = await DoPost(method: METHOD, request: request, token);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
