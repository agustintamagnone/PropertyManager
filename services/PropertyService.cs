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

        public bool AddProperty(PropertyModel property)
        {
            if (property._ownerId == 0)
            {
                return false;
            }

            _properties.Add(property);

            return true;
        }

        public bool RemoveProperty(int propertyId)
        {
            if (!_properties.Any(p => p._id == propertyId))
                return false;

            _properties.RemoveAll(p => p._id == propertyId);

            return true;
        }
        
        // Order: Owners List, Type: "rent" || "sell", minArea, maxArea, name, address
        // Example: DisplayProperties(owners, "rent", null, 100, "studio", "Madrid")
        public void DisplayProperties(List<OwnerModel> owners, string? filter = null, int? minArea = null, int? maxArea = null, string? nameFilter = null, string? addressFilter = null)
        {
            var filtered = _properties.Where(p =>
            (string.IsNullOrWhiteSpace(filter) ||
            (p._type != null && p._type.Equals(filter))) &&
            (!minArea.HasValue || p._area >= minArea.Value) &&
            (!maxArea.HasValue || p._area <= maxArea.Value) &&
            (string.IsNullOrWhiteSpace(nameFilter) ||
                (p._name != null && p._name.Equals(nameFilter, StringComparison.OrdinalIgnoreCase))) &&
            (string.IsNullOrWhiteSpace(addressFilter) ||
                (p._address != null && p._address.Equals(addressFilter, StringComparison.OrdinalIgnoreCase)))
            ).ToList();

                foreach (var property in filtered)
                {
                    string? ownerName = owners.FirstOrDefault(o => o._id == property._ownerId)?._name;

                    Console.WriteLine($"Property ID: {property._id}");
                    Console.WriteLine($"Name: {property._name}");
                    Console.WriteLine($"Price: {property._price}");
                    Console.WriteLine($"Type: {property._type}");
                    Console.WriteLine($"Area: {property._area}");
                    Console.WriteLine($"Address: {property._address}");
                    Console.WriteLine($"Owner ID: {property._ownerId} - Owner Name: {ownerName}");
                    Console.WriteLine("------------------------------------------------------------");
                }

        }

    }
}

