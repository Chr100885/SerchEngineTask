using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceEngine.SearchRunners;
using System.IO;
using ServiceEngine.Utilities;
using System.Xml.Serialization;

namespace ServiceEngine
{
    public class Program
    {
        
                  
            static void Main(string[] args)
            {
                try
                {
                    Run(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("An unexpected exception has occurred: " + Environment.NewLine + ex.ToString());
                }

                //Console.Read();
            }

            private static void Run(string[] args)
            {
                try
                {
                    if (args.Length == 0)
                        throw new ConfigurationException("Expected at least one argument.");

                    var runners = ReadConfiguration().SearchRunners.Where(runner => !runner.Disabled).ToList();
                    var results = CollectResults(args, runners).Result;

                    Console.WriteLine();

                    ConsoleHelpers.PrintAsTable(results.Languages, results.Runners, results.Counts); // Using 'ConsoleHelpers.PrintAsList' will print as a list instead.

                    Console.WriteLine();

                    foreach (var winner in results.Winners)
                        Console.WriteLine("{0} winner: {1}", winner.Key, winner.Value);

                    Console.WriteLine();

                    Console.WriteLine("Total winner: {0}", results.Winner);

                    if (results.Winner != results.NormalizedWinner)
                        Console.WriteLine("Normalized winner: {0}", results.NormalizedWinner);
                }
                catch (ConfigurationException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                }
                catch (AggregateException ex)
                {
                    ex.Handle(e =>
                    {
                        var searchException = e as SearchException;

                        if (searchException != null)
                        {
                            Console.WriteLine();
                            Console.WriteLine(string.Format("Runner '{0}' failed. {1}", searchException.Runner, searchException.Message));
                            return true;
                        }
                        else
                            return false;
                    });
                }
            }

            private static async Task<Results> CollectResults(IReadOnlyList<string> languages, IReadOnlyList<ISearchRunner> runners)
            {
                using (var reporter = new ConsoleProgressReporter("Running..."))
                {
                    return await Results.Collect(languages, runners, reporter);
                }
            }

            private static Configuration ReadConfiguration()
            {
                try
                {
                    using (var stream = File.OpenRead("Configuration.xml"))
                    {
                        try
                        {
                            var serializer = new XmlSerializer(typeof(Configuration));
                            return (Configuration)serializer.Deserialize(stream);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ConfigurationException("The configuration file is invalid. " + ex.Message, ex);
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new ConfigurationException("Unauthorized exception trying to access cofiguration file.", ex);
                }
                catch (FileNotFoundException ex)
                {
                    throw new ConfigurationException("Could not find configuration file.", ex);
                }
                catch (IOException ex)
                {
                    throw new ConfigurationException("An error occurred when reading configuration file.", ex);
                }
            }
        }
    }


