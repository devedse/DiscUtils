using DiscUtils.Vfs;

namespace DiscUtils.Setup;

/// <summary>
/// AOT-compatible registration for DiscUtils.Fat
/// </summary>
public static class FatRegistration
{
    /// <summary>
    /// Registers FAT filesystem factory
    /// </summary>
    public static void Register()
    {
        FileSystemManager.RegisterFileSystems(new Fat.FileSystemFactory());
    }
}
