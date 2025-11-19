using PropertyManager.models;

namespace PropertyManager.services
{
    public class OwnerService
    {
        public List<OwnerModel> _owners = new List<OwnerModel>();
        
        // _nextOwnerId is an auto-increment ID for Owners.
        private int _nextOwnerId = 1;
        
        public bool AddOwner(string nationalId, string name, string phoneNumber)
        {
            var isDuplicate = _owners.Any(o =>
                o.NationalId == nationalId || o.PhoneNumber == phoneNumber);

            if (isDuplicate)
            {
                Console.WriteLine("Owner cannot be added as it already exists.");
                return false;
            }

            var owner = new OwnerModel(_nextOwnerId++, nationalId, name, phoneNumber);
            _owners.Add(owner);

            Console.WriteLine($"Owner {owner.Name} added successfully with ID {owner.Id}!");
            return true;
        }

        public bool RemoveOwner(int ownerId, List<PropertyModel> properties)
        {
            if (_owners.Any(o => o.Id == ownerId))
            {
                properties.RemoveAll(p => p.OwnerId == ownerId);
                _owners.RemoveAll(o => o.Id == ownerId);
                Console.WriteLine($"Owner {ownerId} removed successfully!");
                return true;
            }
            
            Console.WriteLine($"Owner {ownerId} does not exist.");
            return false;
        }

        public void DisplayOwners(List<PropertyModel> properties)
        {
            foreach (var owner in _owners)
            {
                int count = properties.Count(p => p.OwnerId == owner.Id);

                Console.WriteLine("------------------OWNER------------------");
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
