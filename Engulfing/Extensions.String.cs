
using System.Globalization;

namespace cTrader.Guru.Extensions
{

    public static class ExtString
    {

        public static double ToDouble(this string thisString, string Culture = "en-EN")
        {

            var culture = CultureInfo.GetCultureInfo(Culture);
            return double.Parse(thisString.Replace(',', '.').ToString(CultureInfo.InvariantCulture), culture);

        }

    }

}
