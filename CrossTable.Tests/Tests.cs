using NUnit.Framework;
using System.Globalization;
using System.Text;

namespace CrossTable.Tests {

    [TestFixture]
    public class Tests {
        [Test]
        public void Test2x2() {
            AssertCrossTable(
@"Bananas,09/12/2020,20
Apples,08/08/2021,10
Bananas,07/25/2021,30
Apples,05/07/2020,10
Apples,01/02/2021,60
Bananas,05/11/2020,40
Bananas,02/22/2021,55
Apples,02/04/2020,12",

@"||2020|2021|
|-|-|-|
|Apples|$22.00|$70.00|
|Bananas|$60.00|$85.00|
");
        }

        [Test]
        public void Test1x1() {
            AssertCrossTable(
@"Bananas,09/12/2021,20.17
Bananas,07/25/2021,30.22",

@"||2021|
|-|-|
|Bananas|$50.39|
");
        }

        [Test]
        public void Culture() {
            CultureInfo culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("RU-ru");
            try {
                AssertCrossTable(
@"Bananas,09/12/2021,20.17",

@"||2021|
|-|-|
|Bananas|$20.17|
");
            } finally {
                CultureInfo.CurrentCulture = culture;
            }        
        }

        [Test]
        public void EmptyCell() {
            AssertCrossTable(
@"Bananas,09/12/2020,20
Apples,08/08/2021,10
Apples,05/07/2020,10",

@"||2020|2021|
|-|-|-|
|Apples|$10.00|$10.00|
|Bananas|$20.00||
");
        }

        protected virtual void AssertCrossTable(string input, string output) {
            //AssertCrossTable(Solution_Original.GenerateCrossTable, input, output);
            AssertCrossTable(Solution.GenerateCrossTable, input, output);
        }
        protected void AssertCrossTable(Action<TextReader, TextWriter> generate, string input, string output) {
            using var reader = new StringReader(input);
            var builder = new StringBuilder();
            using var writer = new StringWriter(builder);
            generate(reader, writer);
            string result = builder.ToString();
            Assert.That(result, Is.EqualTo(output.Replace("\r\n", Environment.NewLine)));
        }
    }
}