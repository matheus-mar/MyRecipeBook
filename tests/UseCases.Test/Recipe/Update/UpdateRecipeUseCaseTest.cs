using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Recipe.Update
{
    public class UpdateRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var request = RequestRecipeJsonBuilder.Build();

            var useCase = CreateUseCase(user, recipe);

            Func<Task> act = async () => await useCase.Execute(recipe.Id, request);

            await act.ShouldNotThrowAsync();
        }

        [Fact]
        public async Task Error_Recipe_NotFound()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestRecipeJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(recipeId: 1000, request);

            var exception = await Should.ThrowAsync<NotFoundException>(act);
            exception.Message.ShouldContain(ResourceMessagesException.RECIPE_NOT_FOUND);
        }

        [Fact]
        public async Task Error_Title_Empty()
        {
            (var user, _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;

            var useCase = CreateUseCase(user, recipe);

            Func<Task> act = async () => await useCase.Execute(recipe.Id, request);

            var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
            exception.ErrorMessages.ShouldSatisfyAllConditions(
                errorMessages => errorMessages.Count.ShouldBe(1),
                errorMessages => errorMessages.ShouldContain(ResourceMessagesException.RECIPE_TITLE_EMPTY)
            );
        }

        private static UpdateRecipeUseCase CreateUseCase(
            MyRecipeBook.Domain.Entities.User user,
            MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();
            var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();

            return new UpdateRecipeUseCase(loggedUser, unitOfWork, mapper, repository);
        }
    }
}
