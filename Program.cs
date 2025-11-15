using PropertyManager.core;
using PropertyManager.services;

var ownerService = new OwnerService();
var propertyService = new PropertyService();
var processor = new CommandProcessor(ownerService, propertyService);

string folderPath = AppContext.BaseDirectory;
string filePattern = "properties*.txt";

var files = Directory.GetFiles(folderPath, filePattern)
            .OrderBy(f => f)
            .ToList();

if (files.Any())
{
    foreach (var file in files)
    {
        Console.WriteLine($"Reading input file: {Path.GetFileName(file)}\n");
        processor.RunFromFile(file);
    }
}
else
{
    Console.WriteLine("No propertiesXX.txt files found.");
}

// --- Fallback: interactive mode ---
Console.WriteLine("\nStarting in interactive console mode.");
processor.RunInteractive();