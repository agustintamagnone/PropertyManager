using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.models
{
    internal class OwnerModel
    {
        public int Id { get; set; }
        public string? NationalId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

        public OwnerModel(int id, string nationalId, string name, string phoneNumber) 
        { 
            Id = id;
            NationalId = nationalId;
            Name = name;
            PhoneNumber = phoneNumber;
        }
    }
}
