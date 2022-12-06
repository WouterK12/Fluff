namespace Fluff.Test.Attributes
{
    [TestClass]
    public class HandlerAttributeTest
    {
        [TestMethod]
        public void Constructor_HandlerAttribute_SetsProperties()
        {
            // Arrange
            string topic = "Fluff.Test1.*";

            // Act
            var sut = new HandlerAttribute(topic);

            // Assert
            Assert.AreEqual(topic, sut.Topic);
        }
    }
}
