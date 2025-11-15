using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.models
{
    internal class OwnerModel
    {
        public int _id { get; set; }
        public string? _nationalId { get; set; }
        public string? _name { get; set; }
        public string? _phoneNumber { get; set; }

        public OwnerModel(int id, string nationalId, string name, string phoneNumber) 
        { 
            _id = id;
            _nationalId = nationalId;
            _name = name;
            _phoneNumber = phoneNumber;
        }
    }
}
