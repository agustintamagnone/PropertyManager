using PropertyManager.core;
using PropertyManager.services;

namespace PropertyManager.Tests.CoreTests;

public class CommandProcessorTests
{
    private static (CommandProcessor processor, OwnerService ownerService, PropertyService propertyService)
        CreateProcessor()
    {
        var ownerService = new OwnerService();
        var propertyService = new PropertyService();
        var processor = new CommandProcessor(ownerService, propertyService);
        return (processor, ownerService, propertyService);
    }

    // CPT_001
    [Fact]
    public void ExecuteCommand_Should_Do_Nothing_For_Empty_Command()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("");
            processor.ExecuteCommand("   ");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Empty(ownerService._owners);
        Assert.Empty(propertyService._properties);
        // We do not assert on output here; just that no exception and no state change
    }

    // CPT_002
    [Fact]
    public void ExecuteCommand_Should_Print_Unknown_For_Invalid_Command()
    {
        var (processor, _, _) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("unknown_command");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Unknown command 'unknown_command'.", output);
        Assert.Contains("Type 'help' to see available commands.", output);
    }

    // CPT_003
    [Fact]
    public void AddOwner_With_Wrong_Number_Of_Arguments_Should_Print_Error()
    {
        var (processor, ownerService, _) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("add_owner 12345678 OnlyTwoArgs");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Incorrect command arguments.", output);
        Assert.Contains("Usage: add_owner <NationalID> <Name> <PhoneNumber>", output);
        Assert.Empty(ownerService._owners);
    }

    // CPT_004
    [Fact]
    public void AddOwner_With_Valid_Arguments_Should_Add_Owner()
    {
        var (processor, ownerService, _) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("add_owner 12345678 Test_User 600000000");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Single(ownerService._owners);
        Assert.Equal("12345678", ownerService._owners[0].NationalId);
        Assert.Equal("Test_User", ownerService._owners[0].Name);
    }

    // CPT_005
    [Fact]
    public void AddProperty_With_NonNumeric_Area_Should_Print_Error_And_Not_Add()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        // create owner with Id = 1
        ownerService.AddOwner("11111111", "Owner_One", "600000000");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("add_prop Studio 150000 rent NotANumber Madrid 1");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Invalid area value.", output);
        Assert.Empty(propertyService._properties);
    }

    // CPT_006
    [Fact]
    public void AddProperty_With_Valid_Arguments_Should_Add_Property()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        var command = $"add_prop Studio 150000 rent 50 Madrid {ownerId}";

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand(command);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Single(propertyService._properties);
        var prop = propertyService._properties[0];
        Assert.Equal("Studio", prop.Name);
        Assert.Equal(50, prop.Area);
        Assert.Equal(ownerId, prop.OwnerId);
    }

    // CPT_007
    [Fact]
    public void DeleteProperty_With_Invalid_Id_Should_Print_Error_And_Not_Remove()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        // Add a valid property first
        processor.ExecuteCommand($"add_prop Studio 150000 rent 50 Madrid {ownerId}");
        Assert.Single(propertyService._properties);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("del_prop abc");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Invalid property ID.", output);
        Assert.Single(propertyService._properties); // unchanged
    }

    // CPT_008
    [Fact]
    public void PrintProps_With_Type_Filter_Should_Show_Only_Filtered_Type()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        processor.ExecuteCommand($"add_prop RentProp 100000 rent 40 Madrid {ownerId}");
        processor.ExecuteCommand($"add_prop SellProp 200000 sell 80 Madrid {ownerId}");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("print_props -type rent");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("RentProp", output);
        Assert.DoesNotContain("SellProp", output);
    }
}