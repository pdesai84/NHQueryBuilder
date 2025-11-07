namespace NHQueryBuilder.Builders
{
    internal class Alias
    {
        // ASCII of a is 97. Initialize ascii with 96
        int ascii = 96;

        public string Next()
        {
            ascii += 1;
            return ((char)ascii).ToString();
        }
    }
}
