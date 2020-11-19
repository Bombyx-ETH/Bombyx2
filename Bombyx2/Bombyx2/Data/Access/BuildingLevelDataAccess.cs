using Bombyx2.Data.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Bombyx2.Data.Access
{
    public class BuildingLevelDataAccess
    {
        private static string Roof = "AND eBKP IN('C 4.4', 'F 1.1', 'F 1.2', 'F 1.3', 'G 4.1', 'G 4.2') ";
        private static string InteriorWalls = "AND eBKP IN('C 2.2', 'G 3.1') ";
        private static string PartitionWalls = "AND eBKP IN('G 1.1', 'G 1.3', 'G 1.4', 'G 3.1', 'G 3.2') ";
        private static string Windows = "AND eBKP IN('E 3.1') ";
        private static string Balcony = "AND eBKP IN('C 4.3') ";
        private static string TechnicalEquipment = "AND eBKP IN('D 1.1', 'D 5.2', 'D 5.3', 'D 5.4', 'D 7.1', 'D 8.5') ";
        private static string Ceilings = "AND eBKP IN('C 4.1', 'G 4.1', 'G 4.2', 'G 2.1', 'G 2.2') ";
        private static string Columns = "AND eBKP IN('C 3.2') ";
        private static string ExtWallsAboveGround = "AND eBKP IN('C 2.1', 'E 2.1', 'E 2.2', 'E 2.3', 'E 2.4', 'E 2.5', 'E 2.6', 'G 3.1', 'G 3.2') ";
        private static string ExtWallsUnderGround = "AND eBKP IN('C 2.1', 'E 1.1', 'E 1.2', 'E 1.3') ";
        private static string Foundation = "AND eBKP IN('C 1.1', 'C 1.2', 'C 1.3', 'C 1.4', 'C 1.5', 'G 2.1', 'G 2.2') ";

        public static BuildingLevelModel GetComponentsForBuilding(string element, string component, List<string> inputs, string type)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var query = "";

                switch (type)
                {
                    case "min":
                        query += "SELECT ec.eBKP as eBKP, ec.ComponentID as ComponentID, ec.ComponentTitle as ComponentTitle, " +
                                 "MIN(CASE WHEN eBKP = 'E 3.1' AND km.IdKbob IN('05.001', '05.009', '05.010', '05.002', '05.011', '05.003', '05.012', '05.013', '05.014', '05.015', '05.016') " +
                                        "THEN km.ThermalCond " +
                                        "ELSE 1 / (ec.Thickness / km.ThermalCond) " +
                                     "END) Uvalue, " +
                                 "MIN(km.Ubp13Embodied) as UBP13Embodied, MIN(km.Ubp13EoL) as UBP13EoL, MIN(km.TotalEmbodied) as TotalEmbodied, MIN(km.TotalEoL) as TotalEoL, " +
                                 "MIN(km.RenewableEmbodied) as RenewableEmbodied, MIN(km.RenewableEoL) as RenewableEoL, MIN(km.NonRenewableEmbodied) as NonRenewableEmbodied, " +
                                 "MIN(km.NonRenewableEoL) as NonRenewableEoL, MIN(km.GHGEmbodied) as GHGEmbodied, MIN(km.GHGEoL) as GHGEoL ";
                        break;

                    case "max":
                        query += "SELECT ec.eBKP as eBKP, ec.ComponentID  as ComponentID, ec.ComponentTitle as ComponentTitle, " +
                                 "MAX(CASE WHEN eBKP = 'E 3.1' AND km.IdKbob IN('05.001', '05.009', '05.010', '05.002', '05.011', '05.003', '05.012', '05.013', '05.014', '05.015', '05.016') " +
                                        "THEN km.ThermalCond " +
                                        "ELSE 1 / (ec.Thickness / km.ThermalCond) " +
                                     "END) Uvalue, " +
                                 "MAX(km.Ubp13Embodied) as UBP13Embodied, MAX(km.Ubp13EoL) as UBP13EoL, MAX(km.TotalEmbodied) as TotalEmbodied, MAX(km.TotalEoL) as TotalEoL, " +
                                 "MAX(km.RenewableEmbodied) as RenewableEmbodied, MAX(km.RenewableEoL) as RenewableEoL, MAX(km.NonRenewableEmbodied) as NonRenewableEmbodied, " +
                                 "MAX(km.NonRenewableEoL) as NonRenewableEoL, MAX(km.GHGEmbodied) as GHGEmbodied, MAX(km.GHGEoL) as GHGEoL ";
                        break;

                    case "avg":
                        query += "SELECT ec.eBKP as eBKP, ec.ComponentID  as ComponentID, ec.ComponentTitle as ComponentTitle, " +
                                 "AVG(CASE WHEN eBKP = 'E 3.1' AND km.IdKbob IN('05.001', '05.009', '05.010', '05.002', '05.011', '05.003', '05.012', '05.013', '05.014', '05.015', '05.016') " +
                                        "THEN km.ThermalCond " +
                                        "ELSE 1 / (ec.Thickness / km.ThermalCond) " +
                                     "END) Uvalue, " +
                                 "AVG(km.Ubp13Embodied) as UBP13Embodied, AVG(km.Ubp13EoL) as UBP13EoL, AVG(km.TotalEmbodied) as TotalEmbodied, AVG(km.TotalEoL) as TotalEoL, " +
                                 "AVG(km.RenewableEmbodied) as RenewableEmbodied, AVG(km.RenewableEoL) as RenewableEoL, AVG(km.NonRenewableEmbodied) as NonRenewableEmbodied, " +
                                 "AVG(km.NonRenewableEoL) as NonRenewableEoL, AVG(km.GHGEmbodied) as GHGEmbodied, AVG(km.GHGEoL) as GHGEoL ";
                        break;
                }

                query += "FROM EcoKompositComponents ec " +
                         "LEFT JOIN KbobMaterials km " +
                         "ON ec.KBOBID = km.IdKbob " +
                         "WHERE ec.KBOBID between '00.001' AND '21.013' ";

                if(type.Equals("avg"))
                {
                    if (element.Equals("Roof"))
                    {
                        query += Roof;
                    }
                    if (element.Equals("InteriorWalls"))
                    {
                        query += InteriorWalls;
                    }
                    if (element.Equals("PartitionWalls"))
                    {
                        query += PartitionWalls;
                    }
                    if (element.Equals("Windows"))
                    {
                        query += Windows;
                    }
                    if (element.Equals("Balcony"))
                    {
                        query += Balcony;
                    }
                    if (element.Equals("TechnicalEquipment"))
                    {
                        query += TechnicalEquipment;
                    }
                    if (element.Equals("Ceilings"))
                    {
                        query += Ceilings;
                    }
                    if (element.Equals("Columns"))
                    {
                        query += Columns;
                    }
                    if (element.Equals("ExtWallsAboveGround"))
                    {
                        query += ExtWallsAboveGround;
                    }
                    if (element.Equals("ExtWallsUnderGround"))
                    {
                        query += ExtWallsUnderGround;
                    }
                    if (element.Equals("Foundation"))
                    {
                        query += Foundation;
                    }
                }
                else
                {
                    query += component;
                }

                query += "AND BuildingHeight LIKE '%" + inputs[0] + "%' " +
                         "AND BuildingUse LIKE '%" + inputs[1] + "%' " +
                         "AND (EnergyStandard LIKE '%" + inputs[2] + "%' OR EnergyStandard = 'Not applicable') " +
                         "AND StructuralMaterial LIKE '%" + inputs[3] + "%' " +
                         "AND km.ThermalCond IS NOT NULL " +
                         "AND ec.Thickness IS NOT NULL";

                var output = conn.QueryFirst<BuildingLevelModel>(query);
                return output;
            }
        }

        public static List<BuildingLevelModel> GetAllComponentsForBuilding(string element, List<string> inputs, double area)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var query = "SELECT ec.eBKP as eBKP, ec.ComponentID as ComponentID, ec.ComponentTitle as ComponentTitle, " +
                            "CASE WHEN eBKP = 'E 3.1' AND km.IdKbob IN('05.001', '05.009', '05.010', '05.002', '05.011', '05.003', '05.012', '05.013', '05.014', '05.015', '05.016') " +
                                "THEN km.ThermalCond " +
                                "ELSE 1 / (ec.Thickness / km.ThermalCond) " +
                            "END Uvalue, " +
                            "km.Ubp13Embodied * " + area + " as UBP13Embodied, km.Ubp13EoL * " + area + " as UBP13EoL, km.TotalEmbodied * " + area + " as TotalEmbodied, " +
                            "km.TotalEoL * " + area + " as TotalEoL, km.RenewableEmbodied * " + area + " as RenewableEmbodied, km.RenewableEoL * " + area + " as RenewableEoL, " +
                            "km.NonRenewableEmbodied * " + area + " as NonRenewableEmbodied, km.NonRenewableEoL * " + area + " as NonRenewableEoL, " +
                            "km.GHGEmbodied * " + area + " as GHGEmbodied, km.GHGEoL * " + area + " as GHGEoL ";

                query += "FROM EcoKompositComponents ec " +
                         "LEFT JOIN KbobMaterials km " +
                         "ON ec.KBOBID = km.IdKbob " +
                         "WHERE ec.KBOBID between '00.001' AND '21.013' ";

                if (element.Equals("Roof"))
                {
                    query += Roof;
                }
                if (element.Equals("InteriorWalls"))
                {
                    query += InteriorWalls;
                }
                if (element.Equals("PartitionWalls"))
                {
                    query += PartitionWalls;
                }
                if (element.Equals("Windows"))
                {
                    query += Windows;
                }
                if (element.Equals("Balcony"))
                {
                    query += Balcony;
                }
                if (element.Equals("TechnicalEquipment"))
                {
                    query += TechnicalEquipment;
                }
                if (element.Equals("Ceilings"))
                {
                    query += Ceilings;
                }
                if (element.Equals("Columns"))
                {
                    query += Columns;
                }
                if (element.Equals("ExtWallsAboveGround"))
                {
                    query += ExtWallsAboveGround;
                }
                if (element.Equals("ExtWallsUnderGround"))
                {
                    query += ExtWallsUnderGround;
                }
                if (element.Equals("Foundation"))
                {
                    query += Foundation;
                }

                query += "AND BuildingHeight LIKE '%" + inputs[0] + "%' " +
                         "AND BuildingUse LIKE '%" + inputs[1] + "%' " +
                         "AND (EnergyStandard LIKE '%" + inputs[2] + "%' OR EnergyStandard = 'Not applicable') " +
                         "AND StructuralMaterial LIKE '%" + inputs[3] + "%' " +
                         "AND km.ThermalCond IS NOT NULL " +
                         "AND ec.Thickness IS NOT NULL";

                var output = conn.Query<BuildingLevelModel>(query);
                return output.ToList();
            }
        }


        public static List<string> GetComponentsForElement(string comp, List<string> inputs)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var query = "SELECT DISTINCT ec.eBKP || ': ' || ec.ComponentTitle " +
                            "FROM EcoKompositComponents ec " +
                            "LEFT JOIN KbobMaterials km " +
                            "ON ec.KBOBID = km.IdKbob " +
                            "WHERE ec.eBKP = '" + comp + "' " +
                            "AND BuildingHeight LIKE '%" + inputs[0] + "%' " +
                            "AND BuildingUse LIKE '%" + inputs[1] + "%' " +
                            "AND EnergyStandard LIKE '%" + inputs[2] + "%' " +
                            "AND StructuralMaterial LIKE '%" + inputs[3] + "%' " +
                            "AND km.ThermalCond IS NOT NULL " +
                            "AND ec.Thickness IS NOT NULL " +
                            "ORDER BY ec.eBKP";

                var output = conn.Query<string>(query);
                return output.ToList();
            }
        }

        public static BuildingLevelModel GetSpecificForBuilding(string ebkp, string name)
        {
            using (IDbConnection conn = new SQLiteConnection(Config.LoadConnectionString()))
            {
                var query = "SELECT ec.eBKP as eBKP, ec.ComponentID as ComponentID, ec.ComponentTitle as ComponentTitle, " +
                            "CASE WHEN eBKP = 'E 3.1' AND km.IdKbob IN('05.001', '05.009', '05.010', '05.002', '05.011', '05.003', '05.012', '05.013', '05.014', '05.015', '05.016') " +
                                "THEN km.ThermalCond " +
                                "ELSE 1 / (ec.Thickness / km.ThermalCond) " +
                            "END Uvalue, " +
                            "km.Ubp13Embodied as UBP13Embodied, km.Ubp13EoL as UBP13EoL, km.TotalEmbodied as TotalEmbodied, km.TotalEoL as TotalEoL, " +
                            "km.RenewableEmbodied as RenewableEmbodied, km.RenewableEoL as RenewableEoL, km.NonRenewableEmbodied as NonRenewableEmbodied, " +
                            "km.NonRenewableEoL as NonRenewableEoL, km.GHGEmbodied as GHGEmbodied, km.GHGEoL as GHGEoL " +            
                            "FROM EcoKompositComponents ec " +
                            "LEFT JOIN KbobMaterials km " +
                            "ON ec.KBOBID = km.IdKbob " +
                            "WHERE ec.eBKP = '" + ebkp + "' " +
                            "AND ec.ComponentTitle = '" + name + "' " +
                            "AND km.ThermalCond IS NOT NULL " +
                            "AND ec.Thickness IS NOT NULL";

                var output = conn.QueryFirst<BuildingLevelModel>(query);
                return output;
            }
        }
    }
}
