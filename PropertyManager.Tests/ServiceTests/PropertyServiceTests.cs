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

    // PST_005
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

    // PST_006
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

    // PST_007
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

    //PST_008
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

    //PST_009
    [Fact]
    public void DisplayProperties_Should_Filter_By_Address_Two()
    {
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "MadridProp", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "BarcelonaProp", 200000f, "rent", 80, "Barcelona", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            propertyService.DisplayProperties(ownerService._owners, null, null, null, null, "Madrid");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("MadridProp", output);
        Assert.DoesNotContain("BarcelonaProp", output);
    }

    // PST_010
    [Fact]
    public void DisplayProperties_Should_Filter_By_MaxArea()
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
            propertyService.DisplayProperties(ownerService._owners, null, null, 60);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("SmallFlat", output);
        Assert.DoesNotContain("BigFlat", output);
    }

    // PST_011
    [Fact]
    public void DisplayProperties_Should_Filter_By_Name()
    {
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "Studio", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "Apartment", 200000f, "sell", 80, "Barcelona", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            propertyService.DisplayProperties(ownerService._owners, null, null, null, "Studio");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Studio", output);
        Assert.DoesNotContain("Apartment", output);
    }

    // PST_012
    [Fact]
    public void DisplayProperties_Should_Filter_By_Address()
    {
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        propertyService.AddProperty(new PropertyModel(0, "MadridProp", 100000f, "rent", 40, "Madrid", ownerId), ownerService);
        propertyService.AddProperty(new PropertyModel(0, "BarcelonaProp", 200000f, "sell", 80, "Barcelona", ownerId), ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            propertyService.DisplayProperties(ownerService._owners, null, null, null, null, "Barcelona");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("BarcelonaProp", output);
        Assert.DoesNotContain("MadridProp", output);
    }

    // PST_013
    [Fact]
    public void DisplayProperties_Should_Print_Nothing_When_No_Match()
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
            propertyService.DisplayProperties(ownerService._owners, "sell", 200, 300);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.True(string.IsNullOrWhiteSpace(output)); // No output expected
    }
    
    // PST_014
    [Fact]
    public void DisplayProperties_Should_Exclude_Properties_With_Null_Type_When_Filter_Is_Set()
    {
        var (ownerService, ownerId) = CreateOwner();
        var propertyService = new PropertyService();

        // One property with null Type
        propertyService.AddProperty(
            new PropertyModel(0, "NoTypeProp", 100000f, null!, 40, "Madrid", ownerId),
            ownerService);

        // One normal rent property
        propertyService.AddProperty(
            new PropertyModel(0, "RentProp", 150000f, "rent", 50, "Madrid", ownerId),
            ownerService);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Apply type filter: "rent"
            propertyService.DisplayProperties(ownerService._owners, filter: "rent");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // Only RentProp should be printed; NoTypeProp must be excluded
        Assert.Contains("RentProp", output);
        Assert.DoesNotContain("NoTypeProp", output);
    }
}
