using PropertyManager.core;
using PropertyManager.services;

namespace PropertyManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ownerService = new OwnerService();
            var propertyService = new PropertyService();
            
            var processor = new CommandProcessor(ownerService, propertyService);

            string folderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string filePattern = "properties*.txt";

            var files = Directory.GetFiles(folderPath, filePattern)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (files.Any())
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"Reading input file: {Path.GetFileName(file)}\n");
                    processor.RunFromFile(file);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No propertiesXX.txt files found.");
            }

            Console.WriteLine("\nStarting in interactive console mode.");
            processor.RunInteractive();
        }
    }
}