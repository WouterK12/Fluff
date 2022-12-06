namespace Fluff.Test.Runtime
{
    [TestClass]
    public class TypeFinderTest
    {
        [TestMethod]
        public void FindAllTypes_TypeFinder_HasThisAssemblyName()
        {
            // Arrange
            var sut = new TypeFinder();
            string? thisAssemblyName = GetType().Assembly.FullName;

            // Act
            var result = sut.FindAllTypes();

            // Assert
            Assert.IsTrue(result.Any(r => r.Assembly.FullName == thisAssemblyName));
        }
    }
}
