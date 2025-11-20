using PropertyManager.models;

namespace PropertyManager.Tests.ModelsTests;

[Collection("Non-Parallel")]
public class PropertyModelTests
{
    // PMT_001
    [Fact]
    public void PropertyConstructor_Should_Assign_All_Properties_Correctly()
    {
        // Arrange
        int id = 10;
        string name = "TestProperty";
        float price = 123456.78f;
        string type = "rent";
        int area = 85;
        string address = "Madrid";
        int ownerId = 5;

        // Act
        var property = new PropertyModel(id, name, price, type, area, address, ownerId);

        // Assert
        Assert.Equal(id, property.PropertyId);
        Assert.Equal(name, property.Name);
        Assert.Equal(price, property.Price);
        Assert.Equal(type, property.Type);
        Assert.Equal(area, property.Area);
        Assert.Equal(address, property.Address);
        Assert.Equal(ownerId, property.OwnerId);
    }

    // PMT_002
    [Fact]
    public void PropertyConstructor_Should_Accept_Nullable_Fields()
    {
        // Arrange
        int id = 0;
        string? name = null;
        float price = 0f;
        string? type = null;
        int area = 0;
        string? address = null;
        int ownerId = 1;

        // Act
        var property = new PropertyModel(id, name, price, type, area, address, ownerId);

        // Assert
        Assert.Equal(id, property.PropertyId);
        Assert.Null(property.Name);
        Assert.Equal(price, property.Price);
        Assert.Null(property.Type);
        Assert.Equal(area, property.Area);
        Assert.Null(property.Address);
        Assert.Equal(ownerId, property.OwnerId);
    }
}