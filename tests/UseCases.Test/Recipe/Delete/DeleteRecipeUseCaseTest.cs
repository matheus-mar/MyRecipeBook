using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Test.Recipe.Delete
{
    public class DeleteRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);

            var useCase = CreateUseCase(user, recipe);

            var act = async () => { await useCase.Execute(recipe.Id); };

            await act.ShouldNotThrowAsync();
        }

        [Fact]
        public async Task Error_Recipe_NotFound()
        {
            (var user, _) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(recipeId: 1000); };

            var exception = await Should.ThrowAsync<NotFoundException>(act);

            exception.Message.ShouldContain(ResourceMessagesException.RECIPE_NOT_FOUND);
        }

        private static DeleteRecipeUseCase CreateUseCase(
            MyRecipeBook.Domain.Entities.User user,
            MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var repositoryRead = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
            var repositoryWrite = RecipeWriteOnlyRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new DeleteRecipeUseCase(loggedUser, repositoryRead, repositoryWrite, unitOfWork);
        }
    }
}
