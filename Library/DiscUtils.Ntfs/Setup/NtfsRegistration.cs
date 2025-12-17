using DiscUtils.Vfs;

namespace DiscUtils.Setup;

/// <summary>
/// AOT-compatible registration for DiscUtils.Ntfs
/// </summary>
public static class NtfsRegistration
{
    /// <summary>
    /// Registers NTFS filesystem factory
    /// </summary>
    public static void Register()
    {
        FileSystemManager.RegisterFileSystems(new Ntfs.FileSystemFactory());
    }
}
