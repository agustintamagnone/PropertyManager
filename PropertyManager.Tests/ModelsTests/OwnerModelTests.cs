using PropertyManager.models;

namespace PropertyManager.Tests.ModelsTests;

[Collection("Non-Parallel")]

public class OwnerModelTests
{
    // OMT_01
    [Fact]
    public void Owner_Build()
    {
        int id = 1;
        string nationalId = "12345678A";
        string name = "Grabriel Bortoleto";
        string phoneNumber = "555-1234";

        var owner = new OwnerModel(id, nationalId, name, phoneNumber);
        Assert.Equal(id, owner.Id);
        Assert.Equal(nationalId, owner.NationalId);
        Assert.Equal(name, owner.Name);
        Assert.Equal(phoneNumber, owner.PhoneNumber);
    }
}   