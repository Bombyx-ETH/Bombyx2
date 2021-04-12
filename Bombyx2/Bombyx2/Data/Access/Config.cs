using System;

namespace Bombyx2.Data.Access
{
    public static class Config
    {
        private static string DBlocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Grasshopper\\Libraries\\Bombyx2.0.7\\";
        private static string connectionString = "Data Source=" + DBlocation + "MaterialsDB.db; Version=3;";
        //private static string connectionString = "Data Source=MaterialsDB.db; Version=3;";

        public static string LoadConnectionString()
        {
            return connectionString;
        }
    }
}