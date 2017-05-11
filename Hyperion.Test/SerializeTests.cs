using System;
using System.IO;
using NUnit.Framework;

namespace Hyperion.Test
{
    public class TestDTO
    {
        public int Cislo1 { get; set; }
        public int Cislo2 { get; set; }
        public string Retezec1 { get; set; }
        public string Retezec2 { get; set; }
        public DateTime Datum1 { get; set; }

        public TestDTO()
        {
            Cislo1 = 1;
            Cislo2 = 2;
            Retezec1 = "Honza";
            Retezec2 = "De";
            Datum1 = new DateTime(2015,1,1);
        }
    }


    [TestFixture]
    class SerializeTests
    {
        [Test]
        public void Test()
        {
            var wireSerializer = new Serializer(new SerializerOptions(false, true, null));
            using (var ms = new MemoryStream())
            {
                wireSerializer.Serialize(new TestDTO(), ms);
                ms.Seek(0, SeekOrigin.Begin);

                var result = wireSerializer.Deserialize<TestDTO>(ms);

                Assert.AreEqual(1, result.Cislo1);
                Assert.AreEqual(2, result.Cislo2);
                Assert.AreEqual("Honza", result.Retezec1);
                Assert.AreEqual("De", result.Retezec2);
                Assert.AreEqual(new DateTime(2015, 1, 1), result.Datum1);
            }

        }
    }
}
