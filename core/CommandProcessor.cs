using PropertyManager.services;
using PropertyManager.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace PropertyManager.core
{
    internal class CommandProcessor
    {
        private readonly OwnerService _ownerService;
        private readonly PropertyService _propertyService;

        public CommandProcessor(OwnerService ownerService, PropertyService propertyService)
        {
            _ownerService = ownerService;
            _propertyService = propertyService;
        }

        public void ExecuteCommand(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return;

            var parts = commandLine.Split(' ', 2);  // Separates the command from the arguments
            var cmd = parts[0]; // This will be the command to be run
            var args = parts.Length > 1 ? parts[1] : "";  // This will contain the rest of the arguments

            switch (cmd)
            {
                // Use: help to get the available commands and allowed arguments
                case "help":
                    ShowHelp();
                    break;

                // Use: add_owner 50235345 Zavoianu_Razvan 624300355
                case "add_owner":
                    {
                        var claa = args.Split(' ');
                        if (a.Length != 3)
                        {
                            Console.WriteLine("Incorrect command arguments.");
                            return;
                        }
                        var success = _ownerService?.AddOwner(
                            new OwnerModel(
                                //_idGenerator.GetNextOwnerId(),
                                a[0],   // National ID
                                a[1],   // Name
                                a[2])); // Phone Number
                        if (success == true)
                            Console.WriteLine("Owner added successfully.");
                        else
                        {
                            Console.WriteLine("Failed to add owner. Owner already exists.");
                            //_idGenerator.DecreaseOwnerId();
                        }
                        break;
                    }
                
                // Use: del_owner 4
                case "del_owner":
                    {
                        var a = args.Split(' ');
                        if (a.Length != 1)
                        {
                            Console.WriteLine("Incorrect command arguments.");
                            return;
                        }
                        var success = _ownerService?.RemoveOwner(Int32.Parse(a[0]), _propertyService._properties);
                        if (success == true)
                            Console.WriteLine("Owner removed successfully.");
                        else
                            Console.WriteLine("Failed to remove owner. Owner not found.");
                        break;
                    }

                // Use: add_prop Studio 22,400 sell 90 Madrid 4
                case "add_prop":
                    {
                        var a = args.Split(' ');
                        if (a.Length != 6)
                        {
                            Console.WriteLine("Incorrect command arguments.");
                            return;
                        }
                        var success = _propertyService.AddProperty(
                            new PropertyModel(
                                //_idGenerator.GetNextPropertyId(),
                                a[0],               // Name
                                float.Parse(a[1]),  // Price (WITH , )
                                a[2],               // Type "rent" or "sell"
                                Int32.Parse(a[3]),  // Area
                                a[4],               // Address
                                Int32.Parse(a[5])   // Owner ID
                            ));
                        if (success == true)
                            Console.WriteLine("Property added successfully.");
                        else
                        {
                            Console.WriteLine("Failed to add property. The owner doesn't exist.");
                            //_idGenerator.DecreasePropertyId();
                        }
                            break;
                    }

                // Use: del_prop 10
                case "del_prop":
                    {
                        var a = args.Split(' ');
                        if (a.Length != 1)
                        {
                            Console.WriteLine("Incorrect command arguments.");
                            return;
                        }
                        var success = _propertyService.RemoveProperty(Int32.Parse(a[0]));
                        if (success == true)
                            Console.WriteLine("Property removed successfully.");
                        else
                            Console.WriteLine("A property with this id has not been found.");
                        break;
                    }

                // Use: print_owners
                case "print_owners":
                    _ownerService.DisplayOwners(_propertyService._properties);
                    break;

                // Use: print_props -type rent -minarea 50 -maxarea 120 -name Studio -address Madrid
                case "print_props":
                    {
                        string? type = null;
                        int? minArea = null;
                        int? maxArea = null;
                        string? name = null;
                        string? address = null;

                        var tokens = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < tokens.Length - 1; i++)
                        {
                            switch (tokens[i])
                            {
                                case "-type":
                                    type = tokens[++i];
                                    break;
                                case "-minarea":
                                    if (int.TryParse(tokens[++i], out int min)) minArea = min;
                                    break;
                                case "-maxarea":
                                    if (int.TryParse(tokens[++i], out int max)) maxArea = max;
                                    break;
                                case "-name":
                                    name = tokens[++i];
                                    break;
                                case "-address":
                                    address = tokens[++i];
                                    break;
                            }
                        }

                        _propertyService.DisplayProperties(_ownerService._owners, type, minArea, maxArea, name, address);
                        break;
                    }
            }

        }

        private void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  help");
            Console.WriteLine("  add_owner <NationalID> <Name> <Phone Number>");
            Console.WriteLine("  del_owner <OwnerID>");
            Console.WriteLine("  add_prop <Name> <Price> <Type: rent | sell> <Area> <Address> <OwnerID>");
            Console.WriteLine("  del_prop <PropertyID>");
            Console.WriteLine("  print_owners");
            Console.WriteLine("  print_props -type <rent|buy> -minarea <Area> -maxarea <Area> -name <Name> -address <Address>");
        }

        public void RunInteractive()
        {
            Console.WriteLine("Enter commands (type 'exit' to quit):");
            ShowHelp();
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                    break;
                
                ExecuteCommand(input ?? "");    
            }
            
        }

        public void RunFromFile(string filePath)
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                ExecuteCommand(line);
            }
        }
    }
}
