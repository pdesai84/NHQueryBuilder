using Bogus;

namespace NHQueryBuilder.DatabaseSetup.Helper
{
    internal static class BogusHelper
    {
        private static readonly Faker Faker;
        static BogusHelper()
        {
            Faker = new Faker("en");
            Faker.Locale = "en";
        }

        public static string RandomNumberAsString(int length)
        {
            return RandomNumber(length).ToString();
        }

        public static string Fax => RandomPhone;

        public static string Sentence(int limit)
        {
            string phrase = Faker.Hacker.Phrase();
            return phrase.Length > limit ? phrase.Substring(0, limit) : phrase;
        }

        public static string YesNo
        {
            get
            {
                return Faker.PickRandom(new string[] { "Y", "N" });
            }
        }

        public static bool Boolean
        {
            get
            {
                return Faker.PickRandom(new bool[] { true, false });
            }
        }

        /// <summary>
        /// Generate sequence of random alphanumeric characters in upper case.
        /// </summary>
        /// <param name="length">Length of the code</param>
        /// <returns></returns>
        public static string Code(int length)
        {
            return Faker.Random.AlphaNumeric(length).ToUpper();
        }

        public static string RandomName => Faker.Name.FullName();
        public static string RandomEmail => Faker.Internet.Email();
        public static string RandomCity => Faker.Address.City();
        public static string RandomCountry => Faker.Address.Country();
        public static string RandomNationality => Faker.Address.Country();
        public static string RandomPhone => Faker.Phone.PhoneNumber();

        public static string PickRandom(params string[] items)
        {
            return Faker.PickRandom(items);
        }

        public static T PickRandom<T>(IList<T> items)
        {
            return Faker.PickRandom(items);
        }

        public static string BookName => Faker.Lorem.Sentence(3, 5);

        /// <summary>
        /// Generate a random number
        /// </summary>
        /// <param name="length">Number of digits</param>
        /// <param name="minValue">Minimum value to generate number. Default is 1</param>
        /// <returns></returns>
        public static long RandomNumber(int length)
        {
            long maxNumber = Convert.ToInt64("9".PadLeft(length, '9'));
            return Faker.Random.Long(maxNumber);
        }

        public static long RandomNumber(int minValue, int maxNumber)
        {
            return Faker.Random.Long(minValue, maxNumber);
        }

        public static DateTime DateTimeBetween(DateTime start, DateTime end)
        {
            return Faker.Date.Between(start, end);
        }

        public static DateTime DateBetween(DateTime start, DateTime end)
        {
            return Faker.Date.Between(start, end).Date;
        }
    }
}
