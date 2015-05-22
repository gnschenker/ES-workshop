using NUnit.Framework;

namespace IntegrationTests
{
    public abstract class SpecificationBase
    {
        [SetUp]
        public void SetUp()
        {
            Given();
            When();
        }

        protected virtual void When() { }

        protected virtual void Given() { }
    }

    public class ThenAttribute : TestAttribute { }
}
