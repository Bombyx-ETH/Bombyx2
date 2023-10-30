using Bombyx2.Data.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Bombyx2.Data.Access
{
    public class BtkComponentsDataAccess
    {
        #region Components

        public static List<BtkComponentMaterialModel> GetBtkComponent(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString(), true))
            {
                var parameter = new { code = param };
                var query = "SELECT bk.SortCode, mat.Ubp13Embodied * bk.Percentage / 100 as Ubp13Embodied, mat.Ubp13EoL * bk.Percentage / 100 as Ubp13EoL, " +
                            "mat.TotalEmbodied * bk.Percentage / 100 as TotalEmbodied, mat.TotalEoL * bk.Percentage / 100 as TotalEoL, " +
                            "mat.RenewableEmbodied * bk.Percentage / 100 as RenewableEmbodied, mat.RenewableEoL * bk.Percentage / 100 as RenewableEoL, " +
                            "mat.NonRenewableEmbodied * bk.Percentage / 100 as NonRenewableEmbodied, mat.NonRenewableEoL * bk.Percentage / 100 as NonRenewableEoL, " +
                            "mat.GHGEmbodied * bk.Percentage / 100 as GHGEmbodied, mat.GHGEoL * bk.Percentage / 100 as GHGEoL, " +
                            "CASE " +
                                "WHEN bk.ThermalCond = 0.0 THEN 0.0 " +
                                "ELSE bk.Thickness / bk.ThermalCond " +
                            "END Resistance " +
                            "FROM KbobMaterials mat " +
                            "LEFT JOIN BtkKbob bk " +
                            "ON mat.Id = bk.IdKbob " +
                            "WHERE bk.SortCode = @code";
                var output = conn.Query<BtkComponentMaterialModel>(query, parameter);
                return output.ToList();
            }
        }

        public static List<string> GetBtkComponentsGroups()
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString(), true))
            {
                var query = "SELECT DISTINCT ComponentCode || ': ' || CategoryEnglish FROM BtkComponent WHERE ComponentCode NOT IN('B6.2', 'D1', 'D7', 'D8', 'D5.2', 'D5.4') " +
                            "UNION " +
                            "SELECT DISTINCT ComponentCode || ': ' || CategoryEnglish FROM BtkWindows";
                var output = conn.Query<string>(query, new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<string> GetBtkComponentsList(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString(), true))
            {
                var parameter = new { code = param };
                var query = "";
                if (param.Equals("E3.1"))
                {
                    query = "SELECT SortCode || ': ' || CategoryTextEnglish FROM BtkWindows WHERE ComponentCode = @code";
                }
                else
                {
                    query = "SELECT SortCode || ': ' || CategoryTextEnglish FROM BtkComponent WHERE ComponentCode = @code";
                }
                
                var output = conn.Query<string>(query, parameter);
                return output.ToList();
            }
        }

        #endregion

        #region WindowComponent

        public static List<BtkComponentWindowModel> GetBtkWindowComponent(string param)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString(), true))
            {
                var parameter = new { code = param };
                var query = "SELECT bkw.SortCode, mat.NameEnglish, mat.NameGerman, mat.NameFrench, mat.Ubp13Embodied * bkw.FramePercentage / 100 as Ubp13Embodied, " +
                            "mat.Ubp13EoL * bkw.FramePercentage / 100 as Ubp13EoL, mat.TotalEmbodied * bkw.FramePercentage / 100 as TotalEmbodied, " +
                            "mat.TotalEoL * bkw.FramePercentage / 100 as TotalEoL, mat.RenewableEmbodied * bkw.FramePercentage / 100 as RenewableEmbodied, " +
                            "mat.RenewableEoL * bkw.FramePercentage / 100 as RenewableEoL, mat.NonRenewableEmbodied * bkw.FramePercentage / 100 as NonRenewableEmbodied, " +
                            "mat.NonRenewableEoL * bkw.FramePercentage / 100 as NonRenewableEoL, mat.GHGEmbodied * bkw.FramePercentage / 100 as GHGEmbodied, " +
                            "mat.GHGEoL * bkw.FramePercentage / 100 as GHGEoL, bw.Uvalue, bw.Gvalue " +
                            "FROM KbobMaterials mat " +
                            "LEFT JOIN BtkKbobWindow bkw " +
                            "ON mat.Id = bkw.IdKbob " +
                            "LEFT JOIN BtkWindows bw " +
                            "ON bkw.SortCode = bw.SortCode " +
                            "WHERE bkw.SortCode = @code";
                var output = conn.Query<BtkComponentWindowModel>(query, parameter);
                return output.ToList();
            }
        }

        #endregion
    }
}
