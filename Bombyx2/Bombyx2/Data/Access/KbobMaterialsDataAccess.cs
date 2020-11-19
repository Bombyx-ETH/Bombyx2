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
        #region Material

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

        #endregion

        #region Energy

        public static KbobEnergyModel GetKbobEnergy(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var parameter = new { idkbob = param };
                var query = "SELECT * FROM KbobEnergy WHERE IdKbob = @idkbob";
                var output = conn.QuerySingle<KbobEnergyModel>(query, parameter);
                return output;
            }
        }

        public static List<string> GetKbobEnergyList()
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var output = conn.Query<string>("SELECT IdKbob || ': ' || NameGerman FROM KbobEnergy", new DynamicParameters());
                return output.ToList();
            }
        }

        #endregion

        #region Services

        public static KbobServiceModel GetKbobService(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var parameter = new { idkbob = param };
                var query = "SELECT * FROM KbobServices WHERE IdKbob = @idkbob";
                var output = conn.QuerySingle<KbobServiceModel>(query, parameter);
                return output;
            }
        }

        public static List<string> GetKbobServicesList(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var parameter = new { idkbob = param };
                var query = "SELECT IdKbob || ': ' || NameGerman FROM KbobServices WHERE IdKbob like @idkbob";
                var output = conn.Query<string>(query, parameter);
                return output.ToList();
            }
        }

        public static List<KbobServiceModel> GetAllKbobServices()
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var output = conn.Query<KbobServiceModel>("SELECT * FROM KbobServices", new DynamicParameters());
                return output.ToList();
            }
        }

        #endregion

        #region Transport

        public static List<KbobTransportModel> GetTransportMaterial()
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var output = conn.Query<KbobTransportModel>("SELECT * FROM KbobTransport WHERE IdKbob LIKE '62%'", new DynamicParameters());
                return output.ToList();
            }
        }

        #endregion
    }
}
