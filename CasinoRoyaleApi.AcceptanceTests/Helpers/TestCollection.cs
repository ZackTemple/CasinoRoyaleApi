using Xunit;

namespace CasinoRoyaleApi.AcceptanceTests.Helpers
{
    public class TestCollection
    {
        [CollectionDefinition(TestBase.TestCollectionName)]
        public class CollectionFixture :
            ICollectionFixture<TestFixture>
        {
        }
    }
}