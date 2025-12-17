using DiscUtils.Btrfs;
using DiscUtils.BootConfig;
using DiscUtils.Dmg;
using DiscUtils.ExFat;
using DiscUtils.Ext;
using DiscUtils.Fat;
using DiscUtils.HfsPlus;
using DiscUtils.Iso9660;
using DiscUtils.Nfs;
using DiscUtils.Ntfs;
using DiscUtils.OpticalDisk;
using DiscUtils.Registry;
using DiscUtils.Sdi;
using DiscUtils.SquashFs;
using DiscUtils.Udf;
using DiscUtils.Wim;
using DiscUtils.Xfs;
using DiscUtils.Net.Dns;
using DiscUtils.OpticalDiscSharing;

namespace DiscUtils.Complete;

public static class SetupHelper
{
    /// <summary>
    /// Registers all DiscUtils types using reflection-based assembly scanning.
    /// This method is not compatible with Native AOT.
    /// </summary>
    public static void SetupComplete()
    {
        Setup.SetupHelper.RegisterAssembly(typeof(Store).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(BtrfsFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(ExtFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(FatFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(ExFatFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(HfsPlusFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Iscsi.Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(BuildFileInfo).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(DnsClient).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Nfs3Status).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(NtfsFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(DiscInfo).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Disc).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(RegistryHive).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(SdiFile).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(SquashFileSystemBuilder).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Swap.SwapFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(UdfReader).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Vdi.Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Vhd.Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Vhdx.Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Vmdk.Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(VirtualFileSystem.VirtualFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(WimFile).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(XfsFileSystem).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Xva.Disk).Assembly);
        Setup.SetupHelper.RegisterAssembly(typeof(Lvm.LogicalVolumeManager).Assembly);
    }

    /// <summary>
    /// Registers all DiscUtils types using source-generated registration code.
    /// This method is compatible with Native AOT compilation.
    /// </summary>
    public static void SetupCompleteAot()
    {
        // Register all assemblies using generated registration methods
        Setup.DiscUtils_Core_GeneratedRegistration.Register();
        
        // File systems
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Btrfs_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Ext_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Fat_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_ExFat_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_HfsPlus_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Ntfs_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_SquashFs_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Swap_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Xfs_GeneratedRegistration");
        
        // Virtual disk formats
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Vdi_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Vhd_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Vhdx_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Vmdk_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Dmg_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Wim_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Xva_GeneratedRegistration");
        
        // Optical formats
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Iso9660_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_OpticalDisk_GeneratedRegistration");
        
        // Volume managers
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Lvm_GeneratedRegistration");
        
        // Other
        RegisterIfExists("DiscUtils.Setup.DiscUtils_VirtualFileSystem_GeneratedRegistration");
        RegisterIfExists("DiscUtils.Setup.DiscUtils_Iscsi_GeneratedRegistration");
    }

    private static void RegisterIfExists(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null)
        {
            var method = type.GetMethod("Register", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            method?.Invoke(null, null);
        }
    }
}