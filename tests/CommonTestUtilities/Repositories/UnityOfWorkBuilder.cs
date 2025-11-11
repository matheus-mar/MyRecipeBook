using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories
{
    public class UnityOfWorkBuilder
    {
        public static IUnityOfWork Build()
        {
            var mock = new Mock<IUnityOfWork>();

            return mock.Object;
        }
    }
}
