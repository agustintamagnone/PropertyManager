using PropertyManager.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PropertyManager.services
{
    internal class PropertyService
    {
        public List<PropertyModel> _properties = new List<PropertyModel>();
        private readonly OwnerService _ownerService;

        public void AddProperty(PropertyModel property, _ownerService)
        {
            if (property.OwnerId == null)
                Console.WriteLine("Failed to add new property. Please try again!");
            else
            {
                _properties.Add(property);
                Console.WriteLine("Property " + property.PropertyId + " has been succesfully added!");
            }
        }

        public void RemoveProperty(int propertyId)
        {
            if (_properties.Any(p => p.PropertyId == propertyId))
            {
                _properties.RemoveAll(p => p.PropertyId == propertyId);
                Console.WriteLine("Property " + propertyId + " removed succesfully!");
            }
            else
                Console.WriteLine("Property with ID: " + propertyId + " not found, please try again!");
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

                    Console.WriteLine($"Property ID: {property.PropertyId}");
                    Console.WriteLine($"Name: {property.Name}");
                    Console.WriteLine($"Price: {property.Price}");
                    Console.WriteLine($"Type: {property.Type}");
                    Console.WriteLine($"Area: {property.Area}");
                    Console.WriteLine($"Address: {property.Address}");
                    Console.WriteLine($"Owner ID: {property.OwnerId} - Owner Name: {ownerName}");
                    Console.WriteLine("------------------------------------------------------------");
                }

        }

    }
}

