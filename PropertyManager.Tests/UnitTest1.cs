using PropertyManager.services;

namespace PropertyManager.Tests;

public class OwnerServiceTests
{
    [Fact]
    public void Builder_ValidClass()
    {
        var servicio = new OwnerService();
        Assert.NotNull(servicio);
    }
}
