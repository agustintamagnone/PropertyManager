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

        public bool AddOwner(OwnerModel owner)
        {
            bool isDuplicate = _owners.Any(o =>
                o._phoneNumber == owner._phoneNumber || o._nationalId == owner._nationalId);

            if (isDuplicate)
                return false;

            _owners.Add(owner);
            return true;
        }

        public bool RemoveOwner(int ownerId, List<PropertyModel> properties)
        {
            if (!_owners.Any(o => o._id == ownerId))
                return false;

            properties.RemoveAll(p => p._ownerId == ownerId);

            _owners.RemoveAll(o => o._id == ownerId);

            return true;
        }

        public void DisplayOwners(List<PropertyModel> properties)
        {
            foreach (var owner in _owners)
            {
                int count = properties.Count(p => p._ownerId == owner._id);

                Console.WriteLine($"Owner ID: {owner._id}");
                Console.WriteLine($"National ID: {owner._nationalId}");
                Console.WriteLine($"Name: {owner._name}");
                Console.WriteLine($"Phone number: {owner._phoneNumber}");
                Console.WriteLine($"Properties owned: {count}");
                Console.WriteLine("-----------------------------------------");
            }
        }
    }
}
