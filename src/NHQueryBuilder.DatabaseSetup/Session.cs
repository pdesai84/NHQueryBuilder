using NHibernate;

namespace NHQueryBuilder.DatabaseSetup
{
    public class DatabaseProfile
    {
        public DatabaseProfile()
        {
            Path = System.IO.Path.GetTempPath();

            string fileName = System.IO.Path.GetFileName(System.IO.Path.GetTempFileName());
            Name = System.IO.Path.ChangeExtension(fileName, ".sqlite");

            Recreate = false;
            SeedDynamicData = false;
            SeedStaticData = false;
        }

        public DatabaseProfile(
            string? path = null, 
            string? name = null, 
            bool recreate = false, 
            bool seedDynamicData = false, 
            bool seedStaticData = false) : this()
        {
            if (!string.IsNullOrEmpty(path))
                Path = path;
            
            if (!string.IsNullOrEmpty(name))
                Name = name;

            Recreate = recreate;
            SeedDynamicData = seedDynamicData;
            SeedStaticData = seedStaticData;
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public string FullDatabasePath => System.IO.Path.Combine(Path, Name);
        public bool Recreate { get; set; }
        public bool SeedDynamicData { get; set; }
        public bool SeedStaticData { get; set; }
    }

    public class Session
    {
        private static object lockObj = new object();

        private ISessionFactory _sessionFactory = null!;
        public void Open(DatabaseProfile dbSetup)
        {
            lock(lockObj)
            {
                bool dbExist = File.Exists(dbSetup.FullDatabasePath);
                if (dbSetup.Recreate && dbExist)
                    File.Delete(dbSetup.FullDatabasePath);

                _sessionFactory = NhFactory.GetSessionFactory(dbSetup.FullDatabasePath);

                if (dbSetup.SeedDynamicData || !dbExist)
                {
                    DatabaseSeeder.SeedDynamicData(_sessionFactory);
                    dbExist = true;
                }

                if (dbSetup.SeedStaticData || !dbExist)
                {
                    DatabaseSeeder.SeedStaticData(_sessionFactory);
                    dbExist = true;
                }
            }
        }

        public ISession GetSession()
        {
            return _sessionFactory.OpenSession();
        }

        public void Close()
        {
            _sessionFactory.Close();
        }
    }
}
