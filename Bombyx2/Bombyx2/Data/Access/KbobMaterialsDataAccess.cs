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
        public static KbobMaterialModel GetKbobMaterial(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var parameter = new { idkbob = param };
                var query = "SELECT * FROM KbobMaterials WHERE IdKbob = @idkbob";
                var output = conn.QuerySingle<KbobMaterialModel>(query, parameter);
                return output;
            }
        }

        public static List<string> GetKbobMaterialsList(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var parameter = new { idkbob = param };
                var query = "SELECT IdKbob || ': ' || NameEnglish FROM KbobMaterials WHERE IdKbob like @idkbob";
                var output = conn.Query<string>(query, parameter);
                return output.ToList();
            }
        }

        public static List<KbobMaterialModel> GetAllKbobMaterials()
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var output = conn.Query<KbobMaterialModel>("SELECT * FROM KbobMaterials", new DynamicParameters());
                return output.ToList();
            }
        }
    }
}
