using PropertyManager.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.services
{
    internal class OwnerService
    {
        public List<OwnerModel> _owners = new List<OwnerModel>();

        public void AddOwner(OwnerModel owner)
        {
            var isDuplicate = _owners.Any(o =>
                o.PhoneNumber == owner.PhoneNumber || o.NationalId == owner.NationalId);

            if (isDuplicate)
                Console.WriteLine("Owner cannot be added as it already exits.");
            else
            {
                _owners.Add(owner);
                Console.WriteLine("Owner " + owner.Name + " added succesfully!");
            }              
        }

        public void RemoveOwner(int ownerId, List<PropertyModel> properties)
        {
            if (_owners.Any(o => o.Id == ownerId))
            {
                properties.RemoveAll(p => p.OwnerId == ownerId);
                _owners.RemoveAll(o => o.Id == ownerId);
            }
            else
                Console.WriteLine("Owner does not exist.");
        }

        public void DisplayOwners(List<PropertyModel> properties)
        {
            foreach (var owner in _owners)
            {
                int count = properties.Count(p => p.OwnerId == owner.Id);

                Console.WriteLine($"Owner ID: {owner.Id}");
                Console.WriteLine($"National ID: {owner.NationalId}");
                Console.WriteLine($"Name: {owner.Name}");
                Console.WriteLine($"Phone number: {owner.PhoneNumber}");
                Console.WriteLine($"Properties owned: {count}");
                Console.WriteLine("-----------------------------------------");
            }
        }
    }
}
