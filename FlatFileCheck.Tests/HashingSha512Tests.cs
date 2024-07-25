namespace FlatFileCheck.Tests
{
    [TestClass]
    public class HashingSha512Tests
    {
        [TestMethod]
        [DataRow("f1ef73e4f976417ea8650fb04cbf673620d1b80a7f90da1d7fbb518e7ea6dcab40a1d84bfa92be014a7908d6674ff8af0d7d7f02556bd2b909df070396c76d5e", 
            "20240724", "6750001923", "23124069608070101200041035", 5000)]
        [DataRow("3df971eca473f05f240c7b05903713d32635c4a534cb32e058931d1a14cf83cfad233a3288766c3a5033459ebd486f94c197376ccf4f1cb63151f63e9667b5c0",
            "20240720", "5512374568", "27124041971111000046934152", 2345)]
        public void Sha512Test(string s, string date, string nip, string nrb, int iterations)
        {
            // Arrange
            var sha512 = HashingSha512.GetSha512(date, nip, nrb, iterations);

            // Assert
            Assert.AreEqual(s, sha512, "Sha512 hashes are not equal.");
        }
    }
}