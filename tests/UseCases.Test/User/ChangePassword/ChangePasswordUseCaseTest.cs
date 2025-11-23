using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Test.User.ChangePassword
{
    public class ChangePasswordUseCaseTest
    {

        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestChangePasswordJsonBuilder.Build();
            request.Password = password;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(request); };

            await act.ShouldNotThrowAsync();

            var passwordEncrypter = PasswordEncrypterBuilder.Build();

            user.Password.ShouldBe(passwordEncrypter.Encrypt(request.NewPassword));
        }

        [Fact]
        public async Task Error_NewPassword_Empty()
        {
            (var user, var password) = UserBuilder.Build();

            var request = new RequestChangePasswordJson
            {
                Password = password,
                NewPassword = string.Empty
            };

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(request); };

            var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
            exception.ErrorMessages.ShouldSatisfyAllConditions(
                errorMessages => errorMessages.Count.ShouldBe(1),
                errorMessages => errorMessages.ShouldContain(ResourceMessagesException.PASSWORD_EMPTY)
            );
        }

        [Fact]
        public async Task Error_CurrentPassword_Different()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestChangePasswordJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(request); };

            var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
            exception.ErrorMessages.ShouldSatisfyAllConditions(
                errorMessages => errorMessages.Count.ShouldBe(1),
                errorMessages => errorMessages.ShouldContain(ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD)
            );
        }

        private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var unitOfWork = UnitOfWorkBuilder.Build();
            var userUpdateRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var passwordEncrypter = PasswordEncrypterBuilder.Build();

            return new ChangePasswordUseCase(loggedUser, passwordEncrypter, userUpdateRepository, unitOfWork);
        }
    }
}
