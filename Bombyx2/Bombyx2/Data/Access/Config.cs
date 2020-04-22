namespace Bombyx2.Data.Access
{
    public static class Config
    {
        public static string connectionString = "Data Source=.\\Data\\Access\\MaterialsDB.db; Version=3;";

        public static string LoadConnectionString()
        {
            return connectionString;
        }
    }
}