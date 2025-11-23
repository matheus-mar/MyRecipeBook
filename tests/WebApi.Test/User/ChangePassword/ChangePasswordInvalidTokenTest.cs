using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests;
using Shouldly;
using System.Net;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordInvalidTokenTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "user/change-password";

        public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory webApplication) : base(webApplication)
        {
        }

        [Fact]
        public async Task Error_Token_Invalid()
        {
            var request = new RequestChangePasswordJson();

            var response = await DoPut(METHOD, request, token: "tokenInvalid");

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            var request = new RequestChangePasswordJson();

            var response = await DoPut(METHOD, request, token: string.Empty);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var request = new RequestChangePasswordJson();

            var response = await DoPut(METHOD, request, token);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
