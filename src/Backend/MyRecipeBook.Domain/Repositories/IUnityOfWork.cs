namespace MyRecipeBook.Domain.Repositories
{
    public interface IUnityOfWork
    {
        public Task Commit();
    }
}
