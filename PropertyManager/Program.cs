using System;
using System.IO;
using System.Linq;
using PropertyManager.core;
using PropertyManager.services;

namespace PropertyManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create services
            var ownerService = new OwnerService();
            var propertyService = new PropertyService();

            // Create command processor with the services
            var processor = new CommandProcessor(ownerService, propertyService);

            // Folder where the executable is running
            string folderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string filePattern = "properties*.txt";

            // Find all files matching propertiesXX.txt pattern, ordered by name
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

            // --- Fallback: interactive mode ---
            Console.WriteLine("\nStarting in interactive console mode.");
            processor.RunInteractive();
        }
    }
}