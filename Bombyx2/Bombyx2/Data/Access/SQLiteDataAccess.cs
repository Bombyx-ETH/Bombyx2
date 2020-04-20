using Bombyx2.Data.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Bombyx2.Data.Access
{
    public class SQLiteDataAccess
    {
        public static List<Class1> LoadPersons()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<Class1>("SELECT * FROM Test", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SavePerson(Class1 person)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("INSERT INTO Test (Name) values (@Name)", person);
            }
        }

        public static string LoadConnectionString()
        {
            return Config.connectionString;
        }
    }
}