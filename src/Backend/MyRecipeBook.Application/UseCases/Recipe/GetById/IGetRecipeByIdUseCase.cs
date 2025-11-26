using MyRecipeBook.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById
{
    public interface IGetRecipeByIdUseCase
    {
        Task<ResponseRecipeJson> Execute(long recipeId);
    }
}
