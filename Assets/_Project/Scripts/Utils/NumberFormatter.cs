namespace _Project.Scripts.Utils
{
    public static class NumberFormatter
    {  
        public static string FormatDecimal(long number, int decimalPlaces = 1)
        {
            if (number >= 1000000000000) // Trillion
                return FormatWithSuffixAndDecimals(number, 1000000000000, "T", decimalPlaces);
            
            if (number >= 1000000000) // Billion
                return FormatWithSuffixAndDecimals(number, 1000000000, "B", decimalPlaces);
            
            if (number >= 1000000) // Million
                return FormatWithSuffixAndDecimals(number, 1000000, "M", decimalPlaces);
            
            if (number >= 1000)  
                return FormatWithSuffixAndDecimals(number, 1000, "K", decimalPlaces);
            
            return number.ToString();
        }

        private static string FormatWithSuffix(long number, long divisor, string suffix)
        {
            long result = number / divisor;
            return $"{result}{suffix}";
        }

        private static string FormatWithSuffixAndDecimals(long number, long divisor, string suffix, int decimalPlaces)
        {
            double result = (double)number / divisor;
            string format = $"F{decimalPlaces}";
            return $"{result.ToString(format)}{suffix}";
        } 
    }
}