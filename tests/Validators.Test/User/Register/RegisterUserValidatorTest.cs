using CommonTestUtilities.Requests;
using Shouldly;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;

namespace Validators.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errorList => errorList.ShouldHaveSingleItem(),
                errorList => errorList.ShouldContain(
                    error => error.ErrorMessage.Equals(ResourceMessagesException.NAME_EMPTY)
                    )
                );
        }

        [Fact]
        public void Error_Email_Empty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errorList => errorList.ShouldHaveSingleItem(),
                errorList => errorList.ShouldContain(
                    errorList => errorList.ErrorMessage.Equals(ResourceMessagesException.EMAIL_EMPTY)
                    )
                );
        }

        [Fact]
        public void Error_Email_Invalid()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "email.com";

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errorList => errorList.ShouldHaveSingleItem(),
                errorList => errorList.ShouldContain(
                    errorList => errorList.ErrorMessage.Equals(ResourceMessagesException.EMAIL_INVALID)
                    )
                );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Password_Invalid(int passwordLength)
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build(passwordLength);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errorList => errorList.ShouldHaveSingleItem(),
                errorList => errorList.ShouldContain(
                    errorList => errorList.ErrorMessage.Equals(ResourceMessagesException.INVALID_PASSWORD)
                    )
                );
        }

        [Fact]
        public void Error_Password_Empty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Password = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();

            result.Errors.ShouldSatisfyAllConditions(
                errorList => errorList.ShouldHaveSingleItem(),
                errorList => errorList.ShouldContain(
                    errorList => errorList.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_EMPTY)
                    )
                );
        }
    }
}
