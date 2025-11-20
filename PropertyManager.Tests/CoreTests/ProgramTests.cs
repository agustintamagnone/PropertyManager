
namespace PropertyManager.Tests.CoreTests;

[Collection("Non-Parallel")]
public class ProgramTests
{
    // PRT_001
    [Fact]
    public void Main_Should_Handle_No_Property_Files_And_Enter_Interactive_Mode()
    {
        // Arrange
        var originalOut = Console.Out;
        var originalIn = Console.In;

        var sw = new StringWriter();
        var sr = new StringReader("exit\n"); // immediately exit interactive mode

        Console.SetOut(sw);
        Console.SetIn(sr);

        try
        {
            Program.Main(Array.Empty<string>());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);
        }

        var output = sw.ToString();

        Assert.Contains("No propertiesXX.txt files found.", output);
        Assert.Contains("Starting in interactive console mode.", output);
    }

    // PRT_002
    [Fact]
    public void Main_Should_Read_Properties_File_And_Execute_Commands()
    {
        // Arrange: compute same folderPath as in Program.Main
        string folderPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

        string filePath = Path.Combine(folderPath, "properties.txt");

        // Create a simple properties file
        File.WriteAllText(filePath,
            "add_owner 12345678 Test_User 600000000\n" +
            "print_owners\n");

        var originalOut = Console.Out;
        var originalIn = Console.In;

        var sw = new StringWriter();
        var sr = new StringReader("exit\n"); // exit interactive mode

        Console.SetOut(sw);
        Console.SetIn(sr);

        try
        {
            Program.Main(Array.Empty<string>());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        var output = sw.ToString();

        Assert.Contains("Reading input file: properties.txt", output);
        Assert.Contains("Owner ID:", output);
        Assert.Contains("Test_User", output);
    }
}
