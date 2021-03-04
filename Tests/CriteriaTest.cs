using ImageFetcher.Images;
using NUnit.Framework;
using System;

namespace Tests
{
    public class CriteriaTest
    {
        [Test]
        public void TestInvalidCriteriaThrowsException()
        {
            Assert.Throws<ArgumentException>(() => ImageCriteria.NewBuilder()
                .SetBackgroundColour(""));

            Assert.Throws<ArgumentException>(() => ImageCriteria.NewBuilder()
                .SetSize(100000, 1000000));

            Assert.Throws<ArgumentException>(() => ImageCriteria.NewBuilder()
                .SetSize(-1, -20));
        }

        [Test]
        public void TestDefaultCriteria()
        {
            var criteria = ImageCriteria.NewBuilder()
                .Build("test.pdf");

            Assert.AreEqual("test.pdf", criteria.File);
            Assert.AreEqual(ImageFormat.Png, criteria.Format);
            Assert.AreEqual(null, criteria.RGBBackgroundColour);
        }

        [Test]
        public void TestRGBCriteria()
        {
            var criteria = ImageCriteria.NewBuilder()
                .SetBackgroundColour("255,0,0")
                .Build("test.pdf");

            Assert.AreEqual(ImageFormat.Png, criteria.Format);
            Assert.AreEqual(Tuple.Create(255,0,0), criteria.RGBBackgroundColour);
        }
    }
}