using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Login.DoLogin
{
    public class DoLoginUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(new RequestLoginJson
            {
                Email = user.Email,
                Password = password
            });

            result.ShouldNotBeNull();
            result.Tokens.ShouldNotBeNull();
            result.Tokens.AccessToken.ShouldNotBeNullOrEmpty();
            result.Name.ShouldSatisfyAllConditions(
                name => name.ShouldNotBeNullOrWhiteSpace(),
                name => name.ShouldBe(user.Name)
                );
        }

        [Fact]
        public async Task Error_Invalid_User()
        {
            var request = RequestLoginJsonBuilder.Build();

            var useCase = CreateUseCase();

            Func<Task> act = async () => { await useCase.Execute(request); };

            await act.ShouldThrowAsync<InvalidLoginException>();

            var exception = await Should.ThrowAsync<InvalidLoginException>(act);
            exception.Message.ShouldContain(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID);
        }

        private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var passwordEncrypter = PasswordEncrypterBuilder.Build();
            var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

            if (user is not null)
            {
                userReadOnlyRepositoryBuilder.GetByEmailAndPassword(user);
            }

            return new DoLoginUseCase(userReadOnlyRepositoryBuilder.Build(), passwordEncrypter, accessTokenGenerator);
        }
    }
}
