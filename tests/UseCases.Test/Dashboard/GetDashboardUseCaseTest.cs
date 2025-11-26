using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Dashboard;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Test.Dashboard
{
    public class GetDashboardUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();
            var recipes = RecipeBuilder.Collection(user);

            var useCase = CreateUseCase(user, recipes);

            var result = await useCase.Execute();

            result.ShouldNotBeNull();
            result.Recipes.ShouldSatisfyAllConditions(
                recipes => recipes.Count.ShouldBeGreaterThan(0),
                recipes => recipes.Select(recipe => recipe.Id).ShouldBeUnique(),
                recipes => recipes.ShouldAllBe(recipe =>
                    !string.IsNullOrWhiteSpace(recipe.Id) &&
                    !string.IsNullOrWhiteSpace(recipe.Title) &&
                    recipe.AmountIngredients > 0
                )
            );
        }

        private static GetDashboardUseCase CreateUseCase(
            MyRecipeBook.Domain.Entities.User user,
            IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var repository = new RecipeReadOnlyRepositoryBuilder().GetForDashboard(user, recipes).Build();

            return new GetDashboardUseCase(repository, mapper, loggedUser);
        }
    }
}
