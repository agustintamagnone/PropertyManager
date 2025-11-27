using PropertyManager.services;
using PropertyManager.models;

namespace PropertyManager.Tests.ServiceTests;

[Collection("Non-Parallel")]

public class OwnerServiceTests
{
    //Tests de AddOwner
    [Fact] //Caso base para probar funcionamiento del metodo AddOwner
    public void addValidOwner()
    {
        var ownerService = new OwnerService();

        string newNationalId = "12345678A";
        string newName = "Fernando Alonso";
        string newPhoneNumber = "666666666";

        bool result = ownerService.AddOwner(newNationalId, newName, newPhoneNumber);

        Assert.True(result);
    }

    [Fact] //Caso base para probar funcionamiento del metodo AddOwner con un nationalId ya existente
    public void existingNationalId()
    {
        var ownerService = new OwnerService();

        string newNationalId = "99999999Z";
        string newName = "Lewis Hamilton";
        string newPhoneNumber = "999999999";

        //Crear un owner nuevo
        ownerService.AddOwner(newNationalId, newName, newPhoneNumber);

        //Intentar crear owner con mismo NationalId
        bool result = ownerService.AddOwner(newNationalId, "Nico Rosberg", "888888888");

        Assert.False(result);
    }

    [Fact] //Caso base para probar funcionamiento del metodo AddOwner con un phoneNumber ya existente
    public void existingPhoneNumber()
    {
        var ownerService = new OwnerService();

        string newNationalId = "88888888Y";
        string newName = "Max Verstappen";
        string newPhoneNumber = "333333333";

        //Crear un owner nuevo
        ownerService.AddOwner(newNationalId, newName, newPhoneNumber);

        //Intentar crear owner con mismo PhoneNumber
        bool result = ownerService.AddOwner("77777777X", "Sergio Perez", newPhoneNumber);

        Assert.False(result);
    }


    //Tests de RemoveOwner
    [Fact] //Caso base para probar funcionamiento del metodo RemoveOwner
    public void removeExistingOwner()
    {
        var ownerService = new OwnerService();
        var properties = new List<PropertyModel>();

        //Crear un owner nuevo
        ownerService.AddOwner("66666666E", "Franco Colapinto", "444444444");
        ownerService.AddOwner("55555555W", "Michael Schumacher", "555555555");

        properties.Add(new PropertyModel(1, "Casa 1", 130795, "Adosado", 1, "Avenida de la Constitución, 5", ownerId: 1));
        properties.Add(new PropertyModel(2, "Piso 1", 90795, "Piso", 2, "Calle Mayor, 10", ownerId: 2));

        //Eliminar el owner creado
        bool result = ownerService.RemoveOwner(1, properties);

        Assert.True(result);
        Assert.DoesNotContain(properties, p => p.OwnerId == 1);
        Assert.Single(properties);
    }

    [Fact] //Caso base para probar funcionamiento del metodo RemoveOwner, owner no existente
    public void removeNonExistingOwner()
    {
        var ownerService = new OwnerService();
        var properties = new List<PropertyModel>();
        const int NonRealOwnerId = 1; //Solo la instancia del id 1, no de la creacion del objeto Owner

        properties.Add(new PropertyModel(1, "Casa 1", 130795, "Adosado", 1, "Avenida de la Constitución, 5", ownerId: NonRealOwnerId));
        
        //Intentar eliminar un owner no existente
        bool result = ownerService.RemoveOwner(NonRealOwnerId, properties);

        Assert.False(result);
        Assert.Single(properties);
    }


    //Tests de DisplayOwners
    [Fact] //Caso base para probar funcionamiento del metodo DisplayOwners con lista vacia
    public void displayOwnersEmptyListTest()
    {
        var ownerService = new OwnerService();
        var properties = new List<PropertyModel>();

        //Capturar la salida de la consola
        using var consoleOutput = new ConsoleOutput();
        ownerService.DisplayOwners(properties);
        Assert.Equal(string.Empty, consoleOutput.GetOuput());
    }

    [Fact] //Caso base para probar funcionamiento del metodo DisplayOwners con owners en la lista
    public void displayOwnersListTest()
    {
        var ownerService = new OwnerService();
        var properties = new List<PropertyModel>();

        // Crear owners nuevos
        ownerService.AddOwner("44444444Q", "Carlos Sainz", "777777777");
        ownerService.AddOwner("33333333P", "Sebastian Vettel", "888888888");

        properties.Add(new PropertyModel(1, "Casa 1", 130795, "Adosado", 1, "Avenida de la Constitución, 5", ownerId: 1));
        properties.Add(new PropertyModel(2, "Piso 1", 90795, "Piso", 2, "Calle Mayor, 10", ownerId: 1));
        properties.Add(new PropertyModel(3, "Chalet 1", 230795, "Chalet", 3, "Calle de la Paz, 15", ownerId: 2));

        // Capturar la salida de la consola
        using var consoleOutput = new ConsoleOutput();
        ownerService.DisplayOwners(properties);

        var output = consoleOutput.GetOuput();

        // Verificar contenido clave en la salida
        Assert.Contains("Owner ID: 1", output);
        Assert.Contains("National ID: 44444444Q", output);
        Assert.Contains("Name: Carlos Sainz", output);
        Assert.Contains("Phone number: 777777777", output);
        Assert.Contains("Properties owned: 2", output);

        Assert.Contains("Owner ID: 2", output);
        Assert.Contains("National ID: 33333333P", output);
        Assert.Contains("Name: Sebastian Vettel", output);
        Assert.Contains("Phone number: 888888888", output);
        Assert.Contains("Properties owned: 1", output);
    }
}


