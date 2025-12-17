namespace DiscUtils.Setup;

/// <summary>
/// AOT-compatible registration for DiscUtils.Vhd
/// </summary>
public static class VhdRegistration
{
    /// <summary>
    /// Registers VHD disk factory
    /// </summary>
    public static void Register()
    {
        var factory = new Vhd.DiskFactory();
        VirtualDiskManager.TypeMap.Add("VHD", factory);
        VirtualDiskManager.ExtensionMap.Add("vhd", factory);
        VirtualDiskManager.ExtensionMap.Add("avhd", factory);
    }
}
