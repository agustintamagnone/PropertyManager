using PropertyManager.models;

namespace PropertyManager.services
{
    internal class PropertyService
    {
        public List<PropertyModel> _properties = new List<PropertyModel>();
        private readonly OwnerService _ownerService = new OwnerService();
        private int _nextPropertyId = 1;

        public bool AddProperty(PropertyModel property, OwnerService ownerService)
        {
            // Check owner exists
            bool ownerExists = ownerService._owners.Any(o => o.Id == property.OwnerId);
            if (!ownerExists)
            {
                Console.WriteLine($"Property cannot be added. Owner with ID {property.OwnerId} does not exist.");
                return false;
            }
            
            property.PropertyId = _nextPropertyId++;

            _properties.Add(property);
            Console.WriteLine($"Property with ID {property.PropertyId} has been successfully added!");
            return true;
        }

        public bool RemoveProperty(int propertyId)
        {
            int removed = _properties.RemoveAll(p => p.PropertyId == propertyId);

            if (removed > 0)
            {
                Console.WriteLine($"Property {propertyId} removed successfully!");
                return true;
            }

            Console.WriteLine($"Property with ID: {propertyId} not found, please try again!");
            return false;
        }
        
        // Order: Owners List, Type: "rent" || "sell", minArea, maxArea, name, address
        // Example: DisplayProperties(owners, "rent", null, 100, "studio", "Madrid")
        public void DisplayProperties(List<OwnerModel> owners, string? filter = null, int? minArea = null, int? maxArea = null, string? nameFilter = null, string? addressFilter = null)
        {
            var filtered = _properties.Where(p =>
            (string.IsNullOrWhiteSpace(filter) ||
            (p.Type != null && p.Type.Equals(filter))) &&
            (!minArea.HasValue || p.Area >= minArea.Value) &&
            (!maxArea.HasValue || p.Area <= maxArea.Value) &&
            (string.IsNullOrWhiteSpace(nameFilter) ||
                (p.Name != null && p.Name.Equals(nameFilter, StringComparison.OrdinalIgnoreCase))) &&
            (string.IsNullOrWhiteSpace(addressFilter) ||
                (p.Address != null && p.Address.Equals(addressFilter, StringComparison.OrdinalIgnoreCase)))
            ).ToList();

                foreach (var property in filtered)
                {
                    string? ownerName = owners.FirstOrDefault(o => o.Id == property.OwnerId)?.Name;

                    Console.WriteLine("----------------PROPERTY-----------------");
                    Console.WriteLine($"Property ID: {property.PropertyId}");
                    Console.WriteLine($"Name: {property.Name}");
                    Console.WriteLine($"Price: {property.Price}");
                    Console.WriteLine($"Type: {property.Type}");
                    Console.WriteLine($"Area: {property.Area}");
                    Console.WriteLine($"Address: {property.Address}");
                    Console.WriteLine($"Owner ID: {property.OwnerId} - Owner Name: {ownerName}");
                    Console.WriteLine("-----------------------------------------");
                }

        }

    }
}

