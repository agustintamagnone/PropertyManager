using System.Globalization;
using PropertyManager.models;
using PropertyManager.services;

namespace PropertyManager.core
{
    public class CommandProcessor
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
            var (cmd, args) = ParseCommandLine(commandLine);

            if (string.IsNullOrWhiteSpace(cmd))
                return;

            switch (cmd)
            {
                case "help":
                    ShowHelp();
                    break;

                case "add_owner":
                    HandleAddOwner(args);
                    break;

                case "del_owner":
                    HandleDeleteOwner(args);
                    break;

                case "add_prop":
                    HandleAddProperty(args);
                    break;

                case "del_prop":
                    HandleDeleteProperty(args);
                    break;

                case "print_owners":
                    _ownerService.DisplayOwners(_propertyService._properties);
                    break;

                case "print_props":
                    HandlePrintProperties(args);
                    break;

                default:
                    Console.WriteLine($"Unknown command '{cmd}'. Type 'help' to see available commands.");
                    break;
            }
        }

        private static (string Command, string Arguments) ParseCommandLine(string commandLine)
        {
            var parts = commandLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts.Length > 0 ? parts[0] : string.Empty;
            var args = parts.Length > 1 ? parts[1] : string.Empty;
            return (cmd, args);
        }

        // add_owner <NationalID> <Name> <PhoneNumber>
        private void HandleAddOwner(string args)
        {
            var tokens = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 3)
            {
                Console.WriteLine("Incorrect command arguments.");
                Console.WriteLine("Usage: add_owner <NationalID> <Name> <PhoneNumber>");
                return;
            }

            var nationalId = tokens[0];
            var name = tokens[1];
            var phone = tokens[2];

            // Assumes OwnerService.AddOwner(string, string, string)
            _ownerService.AddOwner(nationalId, name, phone);
        }

        // del_owner <OwnerID>
        private void HandleDeleteOwner(string args)
        {
            var tokens = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 1)
            {
                Console.WriteLine("Incorrect command arguments.");
                Console.WriteLine("Usage: del_owner <OwnerID>");
                return;
            }

            if (!int.TryParse(tokens[0], out int ownerId))
            {
                Console.WriteLine("Invalid owner ID.");
                return;
            }

            var success = _ownerService.RemoveOwner(ownerId, _propertyService._properties);
            if (!success)
            {
                Console.WriteLine("Failed to remove owner. Owner not found.");
            }
        }

        // add_prop <Name> <Price> <Type> <Area> <Address> <OwnerID>
        private void HandleAddProperty(string args)
        {
            var tokens = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 6)
            {
                Console.WriteLine("Incorrect command arguments.");
                Console.WriteLine("Usage: add_prop <Name> <Price> <Type> <Area> <Address> <OwnerID>");
                return;
            }

            var name = tokens[0];
            var priceString = tokens[1];
            var type = tokens[2];
            var areaString = tokens[3];
            var address = tokens[4];
            var ownerIdString = tokens[5];

            if (!int.TryParse(areaString, out int area))
            {
                Console.WriteLine("Invalid area value.");
                return;
            }

            if (!int.TryParse(ownerIdString, out int ownerId))
            {
                Console.WriteLine("Invalid owner ID.");
                return;
            }

            if (!TryParsePrice(priceString, out float price))
            {
                Console.WriteLine("Invalid price value.");
                return;
            }

            var property = new PropertyModel(
                id: 0,
                name: name,
                price: price,
                type: type,
                area: area,
                address: address,
                ownerId: ownerId
            );

            var success = _propertyService.AddProperty(property, _ownerService);
            if (!success)
            {
                Console.WriteLine("Failed to add property.");
            }
        }

        // del_prop <PropertyID>
        private void HandleDeleteProperty(string args)
        {
            var tokens = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 1)
            {
                Console.WriteLine("Incorrect command arguments.");
                Console.WriteLine("Usage: del_prop <PropertyID>");
                return;
            }

            if (!int.TryParse(tokens[0], out int propertyId))
            {
                Console.WriteLine("Invalid property ID.");
                return;
            }

            var success = _propertyService.RemoveProperty(propertyId);
            if (!success)
            {
                Console.WriteLine("A property with this id has not been found.");
            }
        }

        // print_props -type rent -minarea 50 -maxarea 120 -name Studio -address Madrid
        private void HandlePrintProperties(string args)
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
                    case "-min_area":
                        if (int.TryParse(tokens[++i], out int min))
                            minArea = min;
                        break;
                    case "-maxarea":
                    case "-max_area":
                        if (int.TryParse(tokens[++i], out int max))
                            maxArea = max;
                        break;
                    case "-name":
                        name = tokens[++i];
                        break;
                    case "-address":
                        address = tokens[++i];
                        break;
                }
            }

            _propertyService.DisplayProperties(
                _ownerService._owners,
                type,
                minArea,
                maxArea,
                name,
                address
            );
        }

        private bool TryParsePrice(string priceString, out float price)
        {
            if (float.TryParse(priceString, NumberStyles.Float, CultureInfo.InvariantCulture, out price))
                return true;

            if (float.TryParse(priceString, NumberStyles.Float, CultureInfo.CurrentCulture, out price))
                return true;

            return false;
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
            Console.WriteLine("  print_props [-type <rent|sell>] [-minarea/-min_area <Area>] [-maxarea/-max_area <Area>] [-name <Name>] [-address <Address>]");
        }

        public void RunInteractive()
        {
            Console.WriteLine("Enter commands (type 'exit' to quit):");
            ShowHelp();
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input is null)
                    break;
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                ExecuteCommand(input);
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
