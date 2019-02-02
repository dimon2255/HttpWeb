namespace Dna
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if string is null or empty
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsNulOrEmpty(this string content)
        {
            return string.IsNullOrEmpty(content);
        }

        /// <summary>
        /// Returns true if string is null or white space
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsNulOrWhiteSpace(this string content)
        {
            return string.IsNullOrWhiteSpace(content);
        }
    }
}
