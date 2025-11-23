using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.Update
{
    public class UpdateUserUseCaseTest
    {

        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestUpdateUserJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(request);

            await act.ShouldNotThrowAsync();

            user.Name.ShouldBe(request.Name);
            user.Email.ShouldBe(request.Email);
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(request); };

            var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
            exception.ErrorMessages.ShouldSatisfyAllConditions(
                errorMessages => errorMessages.Count.ShouldBe(1),
                errorMessages => errorMessages.ShouldContain(ResourceMessagesException.NAME_EMPTY)
            );

            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);
        }

        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestUpdateUserJsonBuilder.Build();

            var useCase = CreateUseCase(user, request.Email);

            Func<Task> act = async () => { await useCase.Execute(request); };

            var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
            exception.ErrorMessages.ShouldSatisfyAllConditions(
                errorMessages => errorMessages.Count.ShouldBe(1),
                errorMessages => errorMessages.ShouldContain(ResourceMessagesException.EMAIL_ALREADY_REGISTERED)
            );

            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);
        }

        private static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
        {
            var unitOfWork = UnitOfWorkBuilder.Build();
            var userUpdateRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var loggedUser = LoggedUserBuilder.Build(user);

            var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            if (email.NotEmpty())
                userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(email);

            return new UpdateUserUseCase(loggedUser, userUpdateRepository, userReadOnlyRepositoryBuilder.Build(), unitOfWork);
        }
    }
}
