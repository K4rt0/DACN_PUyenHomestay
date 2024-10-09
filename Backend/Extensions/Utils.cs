using DotEnv.Core;

namespace Backend.Extensions
{
    public class Utils
    {
        public static string DB_MYSQL
        {
            get
            {
                try
                {
                    return "Server=" + EnvReader.Instance["DB_MYSQL_HOST"]
                    + ";User=" + EnvReader.Instance["DB_MYSQL_USER"]
                    + ";Password=" + EnvReader.Instance["DB_MYSQL_PASS"] + ";Database=" + EnvReader.Instance["DB_MYSQL_NAME"] + ";Port=3306";
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(EnvReader.Instance["DB_MYSQL_USER"]);
                    Console.WriteLine("Can't get DB_MYSQL!");
                    Console.ResetColor();
                }
                return string.Empty;
            }
        }
    }
}