namespace _Project.Scripts.Utils
{
    public static class NumberFormatter
    {  
        public static string FormatDecimal(long number, int decimalPlaces = 1)
        {
            return number switch
            {
                >= 1000000000000 => FormatWithSuffixAndDecimals(number, 1000000000000, "T", decimalPlaces),
                >= 1000000000 => FormatWithSuffixAndDecimals(number, 1000000000, "B", decimalPlaces),
                >= 1000000 => FormatWithSuffixAndDecimals(number, 1000000, "M", decimalPlaces),
                >= 1000 => FormatWithSuffixAndDecimals(number, 1000, "K", decimalPlaces),
                _ => number.ToString()
            };
        } 

        private static string FormatWithSuffixAndDecimals(long number, long divisor, string suffix, int decimalPlaces)
        {
            double result = (double)number / divisor;
            string format = $"F{decimalPlaces}";
            return $"{result.ToString(format)}{suffix}";
        } 
    }
}