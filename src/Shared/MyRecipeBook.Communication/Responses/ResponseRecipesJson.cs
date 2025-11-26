using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRecipeBook.Communication.Responses
{
    public class ResponseRecipesJson
    {
        public IList<ResponseShortRecipeJson> Recipes { get; set; } = [];
    }
}
