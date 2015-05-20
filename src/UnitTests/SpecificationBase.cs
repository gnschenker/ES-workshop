using NUnit.Framework;

namespace UnitTests
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
}
