using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string SafeSQL(this string value)
        {
            return value
                .Replace("\\", "\\\\")
                // This simply replaces the silly 'smart quotes' with normal quotes.
                .Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201c', '\"').Replace('\u201d', '\"')
                .Replace("'", "''");
        }


        public static bool IsEmpty(this string value)
        {
            if (value == null)
                return true;

            return value.Trim().Length == 0;
        }


        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }


        public static bool IsValidEmail(this string value)
        {
            // source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
            string pattern = @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$";
            return Regex.IsMatch(value, pattern);
        }


        public static int ToInt32(this string value)
        {
            int result;

            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }


        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }


        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(provider, format, args);
        }
    }
}
