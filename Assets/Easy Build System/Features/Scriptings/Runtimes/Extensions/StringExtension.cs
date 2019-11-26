namespace EasyBuildSystem.Runtimes.Extensions
{
    public static class StringExtension
    {
        #region Public Methods

        /// <summary>
        /// This allows to truncate a string.
        /// </summary>
        public static string Truncate(string s, int maxLength)
        {
            return s != null && s.Length > maxLength ? s.Substring(0, maxLength) : s;
        }

        #endregion Public Methods
    }
}