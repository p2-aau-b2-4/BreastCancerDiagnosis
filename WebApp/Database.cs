using System;
using System.Configuration;
using System.Data.Common;
using LiteDB;

namespace WebApp
{
    public class Database
    {
        public static LiteDatabase CreateConnection()
        {
            // code from: https://stackoverflow.com/questions/4874375/how-connect-to-sql-server-compact-4-0-in-asp-net
            //Get connection string named "db"    
            String dbName = "database.db";
            return new LiteDatabase(dbName);
        }
    }
}