using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.models
{
    internal class PropertyModel
    {
        public int _id {  get; set; }
        public string? _name { get; set; }
        public float _price { get; set; }
        public string? _type { get; set; }
        public int _area { get; set; }
        public string? _address { get; set; }

        // Foreign key reference to owner
        public int _ownerId { get; set; } = 0;

        public PropertyModel(int id, string? name, float price, string? type, int area, string? address, int ownerId)
        {
            _id = id;
            _name = name;
            _price = price;
            _type = type;
            _area = area;
            _address = address;
            _ownerId = ownerId;
        }
    }
}
