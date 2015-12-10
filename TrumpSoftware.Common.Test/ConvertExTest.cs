using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrumpSoftware.Common.Test
{
    public class Descendant { }
    public class Ancestor : Descendant { }

    [TestClass]
    public class ConvertExTest
    {
        [TestMethod]
        public void CanConvertObjectToObject()
        {
            var value = new object();
            CanConvert<object>(value, value);
        }

        [TestMethod]
        public void CanConvertNullToObject()
        {
            CanConvert<object>(null, null);
        }

        [TestMethod]
        public void CanConvertAncestorToDescendant()
        {
            var ancestor = new Ancestor();
            CanConvert<Descendant>(ancestor, ancestor);
        }

        [TestMethod]
        public void CannotConvertDescendantToAncestor()
        {
            CannotConvert<Ancestor>(new Descendant());
        }

        [TestMethod]
        public void CannotConvertNotConvertibleClassToStruct()
        {
            CannotConvert<int>(new Descendant());
        }

        [TestMethod]
        public void CannotConvertStructToNotConvertibleClass()
        {
            CannotConvert<Descendant>(4);
        }

        [TestMethod]
        public void CanConvertDoubleToInt32()
        {
            CanConvert<int>(4.3, 4);
        }

        [TestMethod]
        public void CanConvertNegativeDoubleToInt32()
        {
            CanConvert<int>(-4.3, -4);
        }

        [TestMethod]
        public void CanConvertInt32ToDouble()
        {
            CanConvert<double>(4, 4.0);
        }

        [TestMethod]
        public void CanConvertStringToInt32()
        {
            CanConvert<int>("4", 4);
        }

        [TestMethod]
        public void CanConvertStringToDouble()
        {
            CanConvert<double>("4,3", 4.3);
        }

        [TestMethod]
        public void CannotConvertEmptyStringToInt32()
        {
            CannotConvert<int>(string.Empty);
        }

        [TestMethod]
        public void CannotConvertInvalidStringToInt32()
        {
            CannotConvert<int>("qqq");
        }

        [TestMethod]
        public void CanConvertInt32ToString()
        {
            CanConvert<string>(4, "4");
        }

        [TestMethod]
        public void CanConvertDoubleToString()
        {
            CanConvert<string>(4.3, "4,3");
        }

        [TestMethod]
        public void CanConvertNullableInt32ToString()
        {
            CanConvert<string>(new int?(4), "4");
        }

        [TestMethod]
        public void CanConvertNullableDoubleToString()
        {
            CanConvert<string>(new double?(4), "4");
        }

        [TestMethod]
        public void CanConvertNullOfNullableToString()
        {
            CanConvert<string>((double?)null, null);
        }

        [TestMethod]
        public void CanConvertStringToNullableInt32()
        {
            CanConvert<int?>("4", 4);
        }

        [TestMethod]
        public void CanConvertStringToNullableDouble()
        {
            CanConvert<double?>("4,3", 4.3);
        }

        [TestMethod]
        public void CannotConvertNullToDouble()
        {
            CannotConvert<double>(null);
        }

        [TestMethod]
        public void CannotConvertNullToInt32()
        {
            CannotConvert<int>(null);
        }

        [TestMethod]
        public void CanConvertNullToNullableInt32()
        {
            CanConvert<int?>(null, null);
        }

        [TestMethod]
        public void CanConvertNullToNullableDouble()
        {
            CanConvert<double?>(null, null);
        }

        [TestMethod]
        public void CanConvertValidStringToChar()
        {
            CanConvert<char>("q", 'q');
        }

        [TestMethod]
        public void CannotConvertInvalidStringToChar()
        {
            CannotConvert<char>("qqq");
        }

        [TestMethod]
        public void CannotConvertEmptyStringToChar()
        {
            CannotConvert<char>(string.Empty);
        }

        [TestMethod]
        public void CannotConvertNullToChar()
        {
            CannotConvert<char>(null);
        }

        [TestMethod]
        public void CanConvertCharToString()
        {
            CanConvert<string>('q', "q");
        }

        [TestMethod]
        public void CanConvertCharToInt()
        {
            CanConvert<int>('q', (int)'q');
        }

        [TestMethod]
        public void CannotConvertCharToDouble()
        {
            CannotConvert<double>('q');
        }

        [TestMethod]
        public void CanConvertIntOneToBool()
        {
            CanConvert<bool>(1, true);
        }

        [TestMethod]
        public void CanConvertIntZeroToBool()
        {
            CanConvert<bool>(0, false);
        }

        [TestMethod]
        public void CanConvertNotZeroIntToBool()
        {
            CanConvert<bool>(2, true);
        }

        [TestMethod]
        public void CanConvertTrueStringToBool()
        {
            CanConvert<bool>("true", true);
        }

        [TestMethod]
        public void CanConvertFalseStringToBool()
        {
            CanConvert<bool>("false", false);
        }

        [TestMethod]
        public void CannotConvertInvalidStringToBool()
        {
            CannotConvert<bool>("qqq");
        }

        [TestMethod]
        public void CannotConvertNullToBool()
        {
            CannotConvert<bool>(null);
        }

        private void CanConvert<T>(object value, T expectedValue)
        {
            var convertedValue = Convert<T>(value, true);
            Assert.AreEqual(expectedValue, convertedValue);
        }

        private void CannotConvert<T>(object value)
        {
            Convert<T>(value, false);
        }

        private T Convert<T>(object value, bool expectedResult)
        {
            T convertedValue;
            var converted = ConvertEx.TryConvert<T>(value, out convertedValue);
            Assert.AreEqual(expectedResult, converted);
            return convertedValue;
        }
    }
}
