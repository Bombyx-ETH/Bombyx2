using Grasshopper.Kernel;
using System;
using Bombyx2;
using Rhino.PlugIns;

namespace Bombyx2.Data.Access
{
    public static class Config
    {
        public static readonly string Version = "2.1.0";
        //private static string DBlocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Grasshopper\\Libraries\\Bombyx" + Version + "\\";

        public static Guid BombyxGuid = new Guid("5af956a8-f831-4691-9d71-621053263843");
        public static string BombyxName = "Bombyx" + Version;
        public static GH_AssemblyInfo info = Grasshopper.Instances.ComponentServer.FindAssembly(BombyxGuid);
        public static string DBlocation = info.Location.Substring(0, info.Location.Length - BombyxName.Length);

        private static string connectionString = "Data Source=" + DBlocation + "MaterialsDB.db; Version=3;";
        //private static string connectionString = "Data Source=MaterialsDB.db; Version=3;";

        public static string LoadConnectionString()
        {
            return connectionString;
        }
    }
}