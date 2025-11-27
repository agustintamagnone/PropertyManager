using PropertyManager.models;
using PropertyManager.services;

namespace PropertyManager.Tests.ServiceTests;

[Collection("Non-Parallel")]

public class PropertyServiceTests
{
    // Helper to create a default owner and return OwnerService + ownerId
    private static (OwnerService ownerService, int ownerId) CreateOwner()
    {
        var ownerService = new OwnerService();
        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;
        return (ownerService, ownerId);
    }

    // PST_001
    [Fact]
    public void AddProperty_Should_Add_When_Owner_Exists()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        var property = new PropertyModel(0, "Studio", 150000f, "rent", 50, "Madrid", ownerId);

        // Act
        var result = propertyService.AddProperty(property, ownerService);

        // Assert
        Assert.True(result); 
        Assert.Single(propertyService._properties);
        var stored = propertyService._properties[0];
        Assert.Equal("Studio", stored.Name);
        Assert.Equal(ownerId, stored.OwnerId);
        Assert.NotEqual(0, stored.PropertyId); // ID assigned by service
    }

    // PST_002
    [Fact]
    public void AddProperty_Should_Return_False_When_Owner_Does_Not_Exist()
    {
        // Arrange
        var ownerService = new OwnerService(); // no owners
        var propertyService = new PropertyService();

        var property = new PropertyModel(0, "Studio", 150000f, "rent", 50, "Madrid", 99);

        // Act
        var result = propertyService.AddProperty(property, ownerService);

        // Assert
        Assert.False(result); 
        Assert.Empty(propertyService._properties);
    }

    // PST_003
    [Fact]
    public void RemoveProperty_Should_Remove_Existing_Property()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        var property = new PropertyModel(0, "Studio", 150000f, "rent", 50, "Madrid", ownerId);
        propertyService.AddProperty(property, ownerService);
        int propertyId = propertyService._properties[0].PropertyId;

        // Act
        var result = propertyService.RemoveProperty(propertyId);

        // Assert
        Assert.True(result);
        Assert.Empty(propertyService._properties);
    }

    // PST_004
    [Fact]
    public void RemoveProperty_Should_Return_False_When_Property_Not_Found()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        // Add one property with some ID
        var property = new PropertyModel(0, "Studio", 150000f, "rent", 50, "Madrid", ownerId);
        propertyService.AddProperty(property, ownerService);
        int existingId = propertyService._properties[0].PropertyId;

        // Act
        var result = propertyService.RemoveProperty(existingId + 100); // non-existing ID

        // Assert
        Assert.False(result);
        Assert.Single(propertyService._properties);
        Assert.Equal(existingId, propertyService._properties[0].PropertyId);
    }

    // PST_05
    [Fact]
    public void DisplayProperties_Should_Print_All_When_No_Filters()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "Studio", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "Apartment", 200000f, "sell", 80, "Barcelona", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            propertyService.DisplayProperties(ownerService._owners);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // Assert
        Assert.Contains("Studio", output);
        Assert.Contains("Apartment", output);
    }

    // PST_06
    [Fact]
    public void DisplayProperties_Should_Filter_By_Type()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "RentProp", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "SellProp", 200000f, "sell", 80, "Madrid", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            propertyService.DisplayProperties(ownerService._owners, filter: "rent");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // Assert
        Assert.Contains("RentProp", output);
        Assert.DoesNotContain("SellProp", output);
    }

    // PST_07
    [Fact]
    public void DisplayProperties_Should_Apply_Combined_Filters()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "SmallFlat", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "BigFlat", 200000f, "rent", 120, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "OtherCity", 180000f, "rent", 120, "Barcelona", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            propertyService.DisplayProperties(
                owners: ownerService._owners,
                filter: null,
                minArea: 100,
                maxArea: null,
                nameFilter: "BigFlat",
                addressFilter: "Madrid");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // Assert
        Assert.Contains("BigFlat", output);
        Assert.DoesNotContain("SmallFlat", output);
        Assert.DoesNotContain("OtherCity", output);
    }

    // PST_08
    [Fact]
    public void DisplayProperties_Should_Filter_By_Area_Range()
    {
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "SmallFlat", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "BigFlat", 200000f, "rent", 120, "Madrid", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            propertyService.DisplayProperties(ownerService._owners, null, 30, 60);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("SmallFlat", output);
        Assert.DoesNotContain("BigFlat", output);
    }

    // PST_09
    [Fact]
    public void DisplayProperties_Should_Exclude_Property_With_Null_Address_When_Address_Filter_Is_Present()
    {
        // Arrange
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        // Property with a null Address
        var nullAddressProp = new PropertyModel(0, "NullAddressProp", 100000f, "rent", 40, "temp", ownerId);
        // **CRITICAL STEP:** Force the Address to be null
        nullAddressProp.Address = null;

        propertyService.AddProperty(nullAddressProp, ownerService);

        // Control Property (must be included)
        propertyService.AddProperty(new PropertyModel(0, "SpecificAddressProp", 200000f, "sell", 80, "Barcelona", ownerId), ownerService);

        // Console setup boilerplate...
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act: Filter by a specific address
            propertyService.DisplayProperties(ownerService._owners, addressFilter: "Barcelona");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // Assert: The null-address property is excluded, hitting the 'p.Address != null' == false branch.
        Assert.DoesNotContain("NullAddressProp", output);
        Assert.Contains("SpecificAddressProp", output);
    }

    // PST_010:
    [Fact]
    public void DisplayProperties_Should_Handle_Property_With_NonExisting_Owner()
    {
        // Arrange
        // 1. Setup OwnerService with a legitimate owner (for CreateOwner helper)
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        // 2. Create a property that references an ID which DOES NOT EXIST (e.g., ownerId + 999)
        // The property must first be added to the service to be in the _properties list.
        // NOTE: Your AddProperty method prevents this, so we must add it directly!
        // We assume the PropertyModel constructor will accept a high ID.
        var propertyWithMissingOwner = new PropertyModel(
            id: 0,
            name: "GhostProp",
            price: 100000f,
            type: "rent",
            area: 40,
            address: "Phantom",
            ownerId: ownerId + 999 // ID that is guaranteed not to be in ownersService._owners
        );
        // Directly inject the property into the service's internal list, bypassing AddProperty validation
        // This is a standard practice in unit testing when you need to test downstream logic.
        propertyService._properties.Add(propertyWithMissingOwner);
        propertyService._properties[0].PropertyId = 1; // Manually assign an ID if needed for filtering

        // Console setup boilerplate...
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act: Display properties using the legitimate owners list
            // The GhostProp will be in the filtered list, but its ownerId will not match any owner in 'ownerService._owners'
            propertyService.DisplayProperties(ownerService._owners);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // Assert: The output should still contain the property name, but the Owner Name will be an empty string or 'Owner Name: '
        Assert.Contains("GhostProp", output);
        // Check the line where ownerName is printed: it should contain 'Owner Name: ' followed by nothing/space/newline
        Assert.Contains($"Owner ID: {ownerId + 999} - Owner Name: ", output);
        Assert.Contains("Owner Name: ", output);
        Assert.DoesNotContain("Owner Name: Owner_One", output);
    }
}
