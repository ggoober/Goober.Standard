using Goober.Base.Comparators;

namespace Goober.Standard.Tests.Base
{
    public class StringNaturalComparerTests
    {
        [Theory]
        [InlineData("", null, -1)]
        [InlineData(null, null, 0)]
        [InlineData(null, "", 1)]

        public void EmptyNullStrings_CompareNullLast_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);
            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("00", "000", -1)]
        [InlineData("00", "00", 0)]
        [InlineData("000", "00", 1)]
        [InlineData("00000000000000000000000000000", "0000000000000000000", 1)]
        public void ZeroStrings_CompareMoreDigitsLast_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);

            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }     

        [Theory]
        [InlineData("AA", "BBB", -1)]
        [InlineData("AAA", "AAB", -1)]
        [InlineData("AA", "AA", 0)]
        [InlineData("AAB", "AAA", 1)]
        [InlineData("BBB", "AA", 1)]
        public void LatinStrings_CompareAlphabetically_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);

            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("AA", "БББ", -1)]
        [InlineData("ААА", "ААБ", -1)]
        [InlineData("АА", "АА", 0)]
        [InlineData("ААБ", "ААА", 1)]
        [InlineData("БББ", "АА", 1)]
        public void CyrillicStrings_CompareAlphabetically_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);

            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]  
        [InlineData("", " ", -1)]
        [InlineData("", "", 0)]
        [InlineData(" ", " ", 0)]
        [InlineData(" ", "", 1)]
       
        public void EmptyStrings_CompareByLength_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);
            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("  ", "A", -1)]
        [InlineData("", "A", -1)]
        [InlineData("A", "  ", 1)]
        [InlineData("A", "", 1)]
      
        public void SybolEmptyStrings_CompareEmptyFirst_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);

            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("", "1", -1)]
        [InlineData("  ", "1", -1)]
        [InlineData("1", "", 1)]
        [InlineData("1", "  ", 1)]
        public void NumberEmptyStrings_CompareEmptyFirst_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);

            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("A AB", "AAA", -1)]
        [InlineData(" A AB", " A AB", 0)]
        [InlineData("AAA", "A AB", 1)]
        public void WhiteSpaceStrings_CompareAlphabeticallyBeforeWhiteSpace_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);

            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("A9", "A12", -1)]
        [InlineData("A12", "В12", -1)]
        [InlineData("A9", "A9", 0)]
        [InlineData("A12", "A9", 1)]
        [InlineData("В12", "A12", 1)]
        [InlineData("В1000000000000000000000000000000000000000000000002", "B1000000000002", 1)]

        public void SymbolNumberStrings_CompareNumberAfterAlphabetically_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);
            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]
        [InlineData("9A", "12A", -1)]
        [InlineData("9A", "9B", -1)]
        [InlineData("9A", "9A", 0)]
        [InlineData("9B", "9A", 1)]
        [InlineData("12B", "9A", 1)]
        public void NumberSymbolStrings_CompareAlphabeticallyAfterNumber_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);
            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }

        [Theory]       
        [InlineData("A01B02", "A1B003", -1)]
        [InlineData("A1B003", "A1B003", 0)]
        [InlineData("A001B2", "A1B002", 0)]
        [InlineData("A1B003", "A01B02", 1)]
        [InlineData("A001", "A1", 1)]
        [InlineData("A0B3", "A00B2", 1)]
        public void SymbolZeroNumberStrings_СompareAlphabeticallyAfterNumberWithoutLeadingZeros_ReturnSuccess(string strX, string strY, int result)
        {
            var comparer = new StringNaturalComparer();
            var res = comparer.Compare(strX, strY);
            Assert.Equal(Math.Sign(result), Math.Sign(res));
        }
    }
}
