using Bombyx2.Data.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Bombyx2.Data.Access
{
    public static class BrasilComponentsDataAccess
    {
        public static List<string> GetBrasilComponentsList(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString(), true))
            {
                var parameter = new { code = param };
                var query = "SELECT Code || ' - ' || DescriptionBrasil FROM BrasilComponents WHERE IDElement LIKE @code";
                var output = conn.Query<string>(query, parameter);
                return output.ToList();
            }
        }

        public static List<BrasilComponentsModel> GetBrasilComponents(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString(), true))
            {
                var parameter = new { code = param };
                var query = "SELECT bm.NameBrasil, cm.Amount, cm.Unit, bm.ResourcesConsumMin, bm.ResourcesConsumMax, " +
                            "bm.EnergyConsumMin, bm.EnergyConsumMax, bm.WaterConsumMin, bm.WaterConsumMax, " +
                            "bm.CO2Min, bm.CO2Max, bm.WasteGenMin, bm.WasteGenMax " +
                            "FROM BrasilComponents bc " +
                            "LEFT JOIN BrasilCompMat cm " +
                            "ON bc.IDElement = cm.IDComp " +
                            "LEFT JOIN BrasilMaterials bm " +
                            "ON cm.IDMats = bm.IDBR " +
                            "WHERE DescriptionBrasil LIKE @code";
                var output = conn.Query<BrasilComponentsModel>(query, parameter);
                return output.ToList();
            }
        }
    }
}
