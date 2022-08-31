using AssisToad.database;
using AssisToad.Handlers;

class Program
    {

        static void Main(string[] args)
        {

            var Context = new ApplicationContext();
            BotHandler.SetContext(Context);
            BotHandler.StartBot();
            Console.ReadLine();

        }

        // private static void migration()
        // {
        //     var config = new FlywayConfiguration
        //     {
        //         FlywayPath = @"C:\Users\Kuzmi\RiderProjects\AssisToad\Properties\migrations\flyway",
        //         User =
        //         {
        //             Value = "postgres"
        //         },
        //         Password =
        //         {
        //             Value = "root"
        //         },
        //         Locations =
        //         {
        //             Value = new List<string> { "C:\\Users\\Kuzmi\\RiderProjects\\AssisToad\\Properties\\migrations\\flyway" }
        //         },
        //         Schemas =
        //         {
        //             Value = new List<string> { "public" }
        //         },
        //         Url =
        //         {
        //             Value = "jdbc:postgresql://localhost:5432/assistoad_db"
        //         }
        //     };
        //     config.Save();
        //     var flyway = new Flyway.net.Flyway(config);
        //
        //     // if (flyway.Validate().HasError)
        //     flyway.Migrate();
        // }
    }