using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.ChangePassword
{
    public class ChangePasswordValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new ChangePasswordValidator();

            var request = RequestChangePasswordJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Password_Invalid(int passwordLength)
        {
            var validator = new ChangePasswordValidator();

            var request = RequestChangePasswordJsonBuilder.Build(passwordLength);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errors => errors.ShouldHaveSingleItem(),
                errors => errors.ShouldContain(
                    error => error.ErrorMessage.Equals(ResourceMessagesException.INVALID_PASSWORD)));
        }

        [Fact]
        public void Error_Password_Empty()
        {
            var validator = new ChangePasswordValidator();

            var request = RequestChangePasswordJsonBuilder.Build();
            request.NewPassword = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errors => errors.ShouldHaveSingleItem(),
                errors => errors.ShouldContain(
                    error => error.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_EMPTY)));
        }
    }
}
