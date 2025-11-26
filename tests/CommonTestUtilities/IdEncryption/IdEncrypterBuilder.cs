using Sqids;

namespace CommonTestUtilities.IdEncryption
{
    public class IdEncrypterBuilder
    {
        public static SqidsEncoder<long> Build()
        {
            return new SqidsEncoder<long>(new()
            {
                MinLength = 3,
                Alphabet = "VUh6q3O7ZoT9aBJRSAiCpNMLIyWuvFeEKdwktHDPQx0gn8z25mXrbsl4fYc1Gj"
            });
        }
    }
}
