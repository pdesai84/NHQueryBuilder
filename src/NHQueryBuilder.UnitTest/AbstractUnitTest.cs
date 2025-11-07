using NHQueryBuilder.DatabaseSetup;
using NUnit.Framework;

namespace NHQueryBuilder.UnitTest
{
    public class AbstractUnitTest
    {
        public static Session Session { get; set; } = new Session()!;

        [OneTimeSetUp]
        public void Initialize()
        {
            Session.Open(new DatabaseProfile(path: "D:\\Temp", seedStaticData: true));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Session.Close();
        }
    }
}
