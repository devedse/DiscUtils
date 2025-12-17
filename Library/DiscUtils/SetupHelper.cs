using DiscUtils.BootConfig;
using DiscUtils.OpticalDisk;
using DiscUtils.Registry;
using DiscUtils.Sdi;
using DiscUtils.SquashFs;
using DiscUtils.Udf;
using DiscUtils.Wim;
using DiscUtils.Xfs;
using DiscUtils.Iso9660;
using DiscUtils.ExFat;
using DiscUtils.HfsPlus;
using DiscUtils.Net.Dns;
using DiscUtils.Nfs;
using DiscUtils.Ntfs;
using DiscUtils.Iscsi;
using DiscUtils.Btrfs;
using DiscUtils.Ext;
using DiscUtils.Fat;
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
        // Register Core (includes RAW disk format and core transports)
        Setup.DiscUtils_Core_GeneratedRegistration.Register();
        
        // Register all other assemblies using their generated registration methods
        Setup.DiscUtils_Btrfs_GeneratedRegistration.Register();
        Setup.DiscUtils_Ext_GeneratedRegistration.Register();
        Setup.DiscUtils_Fat_GeneratedRegistration.Register();
        Setup.DiscUtils_ExFat_GeneratedRegistration.Register();
        Setup.DiscUtils_HfsPlus_GeneratedRegistration.Register();
        Setup.DiscUtils_Ntfs_GeneratedRegistration.Register();
        Setup.DiscUtils_SquashFs_GeneratedRegistration.Register();
        Setup.DiscUtils_Swap_GeneratedRegistration.Register();
        Setup.DiscUtils_Xfs_GeneratedRegistration.Register();
        Setup.DiscUtils_Vdi_GeneratedRegistration.Register();
        Setup.DiscUtils_Vhd_GeneratedRegistration.Register();
        Setup.DiscUtils_Vhdx_GeneratedRegistration.Register();
        Setup.DiscUtils_Vmdk_GeneratedRegistration.Register();
        Setup.DiscUtils_Dmg_GeneratedRegistration.Register();
        Setup.DiscUtils_Xva_GeneratedRegistration.Register();
        Setup.DiscUtils_Lvm_GeneratedRegistration.Register();
        Setup.DiscUtils_VirtualFileSystem_GeneratedRegistration.Register();
        Setup.DiscUtils_Iscsi_GeneratedRegistration.Register();
        
        // TODO: Uncomment these once their assemblies are built and registration is generated
        // Setup.DiscUtils_Iso9660_GeneratedRegistration.Register();
        // Setup.DiscUtils_Udf_GeneratedRegistration.Register();
        // Setup.DiscUtils_OpticalDisk_GeneratedRegistration.Register();
        // Setup.DiscUtils_Wim_GeneratedRegistration.Register();
        // Setup.DiscUtils_Net_GeneratedRegistration.Register();
        // Setup.DiscUtils_Nfs_GeneratedRegistration.Register();
        // Setup.DiscUtils_OpticalDiscSharing_GeneratedRegistration.Register();
        // Setup.DiscUtils_BootConfig_GeneratedRegistration.Register();
        // Setup.DiscUtils_Registry_GeneratedRegistration.Register();
        // Setup.DiscUtils_Sdi_GeneratedRegistration.Register();
    }
}