using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.services
{
    internal class IdGenerator
    {
        private int _ownerId = 0;
        private int _propertyId = 0;

        public int GetNextOwnerId() => ++_ownerId;
        public int GetNextPropertyId() => ++_propertyId;

        public int DecreaseOwnerId() => --_ownerId;
        public int DecreasePropertyId() => --_propertyId;

    }
}
