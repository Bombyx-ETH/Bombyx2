using Bombyx2.Data.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Bombyx2.Data.Access
{
    public class KbobMaterialsDataAccess
    {
        public static List<KbobMaterial> LoadKbobMaterials()
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var output = conn.Query<KbobMaterial>("SELECT * FROM KbobMaterials", new DynamicParameters());
                return output.ToList();
            }
        }
    }
}
