using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Migrator migrator = new Migrator(args[0].ToString());
                Console.WriteLine("Starting processing for solution: " + migrator.vSolutionPath);
                Console.WriteLine("Checking the current version of Solution");
                Model.Solution oSolution = migrator.CheckProjectVersion();
                if (oSolution.AreAllProjectofSameVersion)
                    Console.WriteLine("Current Version of the solution : " + oSolution.Framework);
                else
                    Console.WriteLine("Solution has projects with multiple framework version. Current version of Main Project : " + oSolution.Framework);
                if (oSolution.Framework == FrameworkVersion.v48)
                {
                    if (oSolution.AreAllProjectofSameVersion)
                        Console.WriteLine("Code is already built using latest framework version, Proceeding with the Build and deployment");
                    else
                    {
                        migrator.Migrate(FrameworkVersion.v48);
                        Console.WriteLine("Code has been upgraded to latest framework version, Proceeding with the Build and deployment");
                    }
                }
                else if (args.Count() > 1 && oSolution.Framework == args[1].ToString())
                {
                    if (oSolution.AreAllProjectofSameVersion)
                        Console.WriteLine("Code is already built using selected framework version, Proceeding with the Build and deployment");
                    else
                    {
                        migrator.Migrate(args[1].ToString());
                        Console.WriteLine("Code has been upgraded to " + args[1].ToString() + ", Proceeding with the Build and deployment");
                    }
                }
                else
                {
                    if (args.Count() > 1)
                    {
                        migrator.Migrate(args[1].ToString());
                        Console.WriteLine("Code has been upgraded to " + args[1].ToString() + ", Proceeding with the Build and deployment");
                    }
                    else
                    {
                        migrator.Migrate(FrameworkVersion.v48);
                        Console.WriteLine("Code has been upgraded to latest framework version, Proceeding with the Build and deployment");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
