using CommonTestUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Delete
{
    public class DeleteRecipeTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe";

        private readonly Guid _userIdentifier;
        private readonly string _recipeId;

        public DeleteRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _recipeId = factory.GetRecipeId();
        }

        [Fact]
        public async Task Success()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoDelete($"{METHOD}/{_recipeId}", token);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            response = await DoGet($"{METHOD}/{_recipeId}", token);

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTests))]
        public async Task Error_Recipe_Not_Found(string culture)
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var id = IdEncrypterBuilder.Build().Encode(1000);

            var response = await DoDelete($"{METHOD}/{id}", token, culture);

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

            errors.ShouldSatisfyAllConditions(
                errors => errors.ShouldHaveSingleItem(),
                errors => errors.ShouldContain(
                    error => error.GetString()!.Equals(expectedMessage)
                )
            );
        }
    }
}
