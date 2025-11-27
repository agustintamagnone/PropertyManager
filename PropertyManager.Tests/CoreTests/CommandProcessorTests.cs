using System.Globalization;
using PropertyManager.core;
using PropertyManager.services;

namespace PropertyManager.Tests.CoreTests;

[Collection("Non-Parallel")]

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

    // CPT_009
    [Fact]
    public void ExecuteCommand_Should_Print_Help_For_Help_Command()
    {
        var (processor, _, _) = CreateProcessor();
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("help");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("add_owner", output);
        Assert.Contains("add_prop", output);
        Assert.Contains("del_prop", output);
        Assert.Contains("print_props", output);
    }
    
    //CPT_010 Antes 12
    [Fact]
    public void AddProperty_With_Invalid_OwnerId_Should_Print_Error()
    {
        var (processor, _, propertyService) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("add_prop Studio 150000 rent 50 Madrid 999");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Property cannot be added. Owner with ID 999 does not exist.", output);
        Assert.Empty(propertyService._properties);
    }

    //CPT_011 Antes 13
    [Fact]
    public void AddProperty_Should_Fail_When_Price_Is_Invalid()
    {
        var (processor, ownerService, _) = CreateProcessor();
        ownerService.AddOwner("11111111", "Owner_One", "600000000");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("add_prop Studio abc rent 50 Madrid 1");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Invalid price value.", output);
    }

    // CPT_012 Antes 14
    [Fact]
    public void AddProperty_Should_Fail_When_OwnerId_Is_Invalid()
    {
        var (processor, _, _) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("add_prop Studio 150000 rent 50 Madrid XYZ");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Invalid owner ID.", output);
    }

    // CPT_013 Antes 16
    [Fact]
    public void PrintProps_Should_Ignore_Unknown_Filter_And_Still_Print_Properties()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();
        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        processor.ExecuteCommand($"add_prop Studio 150000 rent 50 Madrid {ownerId}");
        Assert.Single(propertyService._properties);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // "-unknown" is ignored by HandlePrintProperties; it still calls DisplayProperties
            processor.ExecuteCommand("print_props -unknown something");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();

        // There is no "Unknown filter option" message in the current code.
        // We just verify that the property is still printed and that no exception is thrown.
        Assert.Contains("Studio", output);
    }

    // CPT_014 Antes 17
    [Fact]
    public void DeleteOwner_With_Missing_Arguments_Should_Print_Error()
    {
        var (processor, ownerService, _) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("del_owner");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Incorrect command arguments.", output);
        Assert.Contains("Usage: del_owner <OwnerID>", output);
        Assert.Empty(ownerService._owners);
    }

    // CPT_015 Antes 18
    [Fact]
    public void DeleteOwner_With_NonNumeric_Id_Should_Print_Invalid_Id()
    {
        var (processor, ownerService, _) = CreateProcessor();

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("del_owner abc");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Invalid owner ID.", output);
        Assert.Empty(ownerService._owners);
    }


    // CPT_016 ANtes 19
    [Fact]
    public void DeleteOwner_With_NonExisting_Id_Should_Print_Failure_Message()
    {
        var (processor, ownerService, _) = CreateProcessor();

        // Ensure there is no owner with id 99
        Assert.Empty(ownerService._owners);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("del_owner 99");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Failed to remove owner. Owner not found.", output);
    }

    // CPT_017 Antes 21
    [Fact]
    public void AddProperty_With_Missing_Arguments_Should_Print_Error()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();
        ownerService.AddOwner("11111111", "Owner_One", "600000000");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Only 4 arguments instead of 6
            processor.ExecuteCommand("add_prop Studio 150000 rent 50");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Incorrect command arguments.", output);
        Assert.Contains("Usage: add_prop <Name> <Price> <Type> <Area> <Address> <OwnerID>", output);
        Assert.Empty(propertyService._properties);
    }
 
    // CPT_028 Antes 22
    [Fact]
    public void DeleteProperty_With_Too_Many_Arguments_Should_Print_Error()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;
        processor.ExecuteCommand($"add_prop Studio 150000 rent 50 Madrid {ownerId}");
        Assert.Single(propertyService._properties);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("del_prop 1 extra");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("Incorrect command arguments.", output);
        Assert.Contains("Usage: del_prop <PropertyID>", output);
        Assert.Single(propertyService._properties);
    }
 
    // CPT_019 Antes 23
    [Fact]
    public void DeleteProperty_With_NonExisting_Id_Should_Print_NotFound_Message()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        processor.ExecuteCommand($"add_prop Studio 150000 rent 50 Madrid {ownerId}");
        Assert.Single(propertyService._properties);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("del_prop 999"); // numeric but non-existing
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("A property with this id has not been found.", output);
        Assert.Single(propertyService._properties);
    }

    // CPT_020 Antes 24
    [Fact]
    public void PrintProps_Should_Apply_Min_Max_Name_And_Address_Filters()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        processor.ExecuteCommand($"add_prop SmallFlat 100000 rent 40 Madrid {ownerId}");
        processor.ExecuteCommand($"add_prop BigFlat 200000 rent 120 Madrid {ownerId}");
        processor.ExecuteCommand($"add_prop OtherCity 180000 rent 120 Barcelona {ownerId}");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("print_props -minarea 100 -maxarea 150 -name BigFlat -address Madrid");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("BigFlat", output);
        Assert.DoesNotContain("SmallFlat", output);
        Assert.DoesNotContain("OtherCity", output);
    }

 
    // CPT_021 Antes 25
    [Fact]
    public void PrintProps_Should_Support_Min_Max_Area_With_Underscore_Flags()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        processor.ExecuteCommand($"add_prop SmallFlat 100000 rent 40 Madrid {ownerId}");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            processor.ExecuteCommand("print_props -min_area 0 -max_area 1000");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("SmallFlat", output);
    }
 
    // CPT_022 Antes 26
    [Fact]
    public void AddProperty_Should_Parse_Price_Using_CurrentCulture_When_Invariant_Fails()
    {
        var (processor, ownerService, propertyService) = CreateProcessor();

        ownerService.AddOwner("11111111", "Owner_One", "600000000");
        int ownerId = ownerService._owners[0].Id;

        // Save and change culture to one that uses comma as decimal separator
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // InvariantCulture ("." decimal) will fail on "150,5"
            // CurrentCulture (es-ES) will succeed
            processor.ExecuteCommand($"add_prop Studio 150,5 rent 50 Madrid {ownerId}");
        }
        finally
        {
            Console.SetOut(originalOut);
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }

        // Property should have been added and price parsed correctly
        Assert.Single(propertyService._properties);
        var prop = propertyService._properties[0];
        Assert.Equal("Studio", prop.Name);
        Assert.Equal(ownerId, prop.OwnerId);
        Assert.True(Math.Abs(prop.Price - 150.5f) < 0.01f);
    }
    
   
    // CPT_023 Antes 27
    [Fact]
    public void RunInteractive_Should_Process_Command_Then_Exit_On_Exit_Input()
    {
        // Arrange
        var (processor, _, _) = CreateProcessor();

        // Simulate two lines of user input: first "help", then "exit"
        var input = new StringReader("help\nexit\n");
        var output = new StringWriter();

        var originalIn = Console.In;
        var originalOut = Console.Out;

        Console.SetIn(input);
        Console.SetOut(output);

        try
        {
            // Act
            processor.RunInteractive();
        }
        finally
        {
            // Restore console
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        var text = output.ToString();

        // Assert: we entered interactive mode and executed at least one command ("help")
        Assert.Contains("Enter commands (type 'exit' to quit):", text);
        Assert.Contains("Available commands:", text); // from ShowHelp() via ExecuteCommand("help")
    }

    // CPT_024 Antes 29
    [Fact]
    public void RunInteractive_Should_Process_Command_Then_Exit_On_EndOfInput()
    {
        var (processor, _, _) = CreateProcessor();

        // One command ("help") and then EOF.
        var input = new StringReader("help\n");  // after this line, ReadLine() returns null
        var output = new StringWriter();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        Console.SetIn(input);
        Console.SetOut(output);

        try
        {
            processor.RunInteractive();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        var text = output.ToString();
        
        Assert.Contains("Enter commands (type 'exit' to quit):", text);
        Assert.Contains("Available commands:", text);
    }
}