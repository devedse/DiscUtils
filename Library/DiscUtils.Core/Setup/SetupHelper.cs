using System;
using System.Collections.Generic;
using System.Reflection;

namespace DiscUtils.Setup;

/// <summary>
/// Helps setup new DiscUtils dependencies, when loaded into target programs
/// </summary>
public static class SetupHelper
{
    private static readonly HashSet<string> _alreadyLoaded;

    static SetupHelper()
    {
        _alreadyLoaded = [];

        // Register the core DiscUtils lib
        RegisterAssembly(typeof(SetupHelper).Assembly);
    }

    /// <summary>
    /// Registers the types provided by an assembly to all relevant DiscUtils managers
    /// </summary>
    /// <param name="assembly"></param>
    public static void RegisterAssembly(Assembly assembly)
    {
        lock (_alreadyLoaded)
        {
            if (!_alreadyLoaded.Add(assembly.FullName ?? assembly.GetName().FullName))
            {
                return;
            }

            FileSystemManager.RegisterFileSystems(assembly);
            VirtualDiskManager.RegisterVirtualDiskTypes(assembly);
            VolumeManager.RegisterLogicalVolumeFactory(assembly);
        }
    }

    /// <summary>
    /// Registers types from an assembly using a generated registration delegate.
    /// This method is AOT-compatible and should be used instead of RegisterAssembly when using Native AOT.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly being registered (for tracking)</param>
    /// <param name="registrationAction">The generated registration action from source generator</param>
    public static void RegisterAssemblyAot(string assemblyName, Action registrationAction)
    {
        lock (_alreadyLoaded)
        {
            if (!_alreadyLoaded.Add(assemblyName))
            {
                return;
            }

            registrationAction();
        }
    }

    /// <summary>
    /// Allows intercepting any file open operation
    /// </summary>
    /// <remarks>
    /// Can be used to wrap the opened file for special use cases,
    /// modify the parameters for opening files, validate file names 
    /// and many more.
    /// </remarks>
    public static event EventHandler<FileOpenEventArgs>? OpeningFile;

    internal static void OnOpeningFile(object sender, FileOpenEventArgs e)
    {
        OpeningFile?.Invoke(sender, e);
    }
}