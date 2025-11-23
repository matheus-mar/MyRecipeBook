using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "user/change-password";

        private readonly string _password;
        private readonly string _email;
        private readonly Guid _userIdentifier;

        public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _password = factory.GetPassword();
            _email = factory.GetEmail();
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Success()
        {
            var request = RequestChangePasswordJsonBuilder.Build();
            request.Password = _password;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPut(METHOD, request, token);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var loginRequest = new RequestLoginJson
            {
                Email = _email,
                Password = _password,
            };

            response = await DoPost(method: "login", request: loginRequest);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            loginRequest.Password = request.NewPassword;

            response = await DoPost(method: "login", request: loginRequest);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTests))]
        public async Task Error_NewPassword_Empty(string culture)
        {
            var request = new RequestChangePasswordJson
            {
                Password = _password,
                NewPassword = string.Empty
            };

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPut(METHOD, request, token, culture);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("PASSWORD_EMPTY", new CultureInfo(culture));

            errors.ShouldSatisfyAllConditions(
                e => e.Count().ShouldBe(1),
                e => e.ShouldContain(c => c.GetString()!.Equals(expectedMessage))
            );
        }
    }
}
