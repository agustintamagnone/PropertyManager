using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.models
{
    internal class PropertyModel
    {
        public int PropertyId {  get; set; }
        public string? Name { get; set; }
        public float Price { get; set; }
        public string? Type { get; set; }
        public int Area { get; set; }
        public string? Address { get; set; }

        // Foreign key reference to owner
        public int? OwnerId { get; set; }

        public PropertyModel(int id, string? name, float price, string? type, int area, string? address, int ownerId)
        {
            PropertyId = id;
            Name = name;
            Price = price;
            Type = type;
            Area = area;
            Address = address;
            OwnerId = ownerId;
        }
    }
}
