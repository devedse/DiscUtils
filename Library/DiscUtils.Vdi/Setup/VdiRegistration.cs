namespace DiscUtils.Setup;

/// <summary>
/// AOT-compatible registration for DiscUtils.Vdi
/// </summary>
public static class VdiRegistration
{
    /// <summary>
    /// Registers VDI disk factory
    /// </summary>
    public static void Register()
    {
        var factory = new Vdi.DiskFactory();
        VirtualDiskManager.TypeMap.Add("VDI", factory);
        VirtualDiskManager.ExtensionMap.Add("vdi", factory);
    }
}
