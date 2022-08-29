namespace EasyButtons.Editor.Utils
{
    using System;

    internal static class StringExtensions
    {
        public static string CapitalizeFirstChar(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            char firstChar = input[0];

            if (char.IsUpper(firstChar))
                return input;

            var chars = input.ToCharArray();
            chars[0] = char.ToUpper(firstChar);
            return new string(chars);
        }
    }
}