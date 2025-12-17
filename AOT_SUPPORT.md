# AOT Compilation Support for DiscUtils

This document describes the changes made to support Native AOT compilation in DiscUtils.

## Overview

DiscUtils now supports Native AOT compilation through a dual approach:
- **Reflection-based registration** (original, not AOT-compatible): `SetupHelper.SetupComplete()`
- **Source-generated registration** (AOT-compatible): Manual registration or `SetupCompleteAot()`

## Technical Changes

### 1. Source Generator (DiscUtils.SourceGenerator)

A Roslyn source generator has been created that:
- Scans each assembly during compilation for types decorated with:
  - `VfsFileSystemFactoryAttribute`
  - `VirtualDiskFactoryAttribute`
  - `LogicalVolumeFactoryAttribute`
  - `VirtualDiskTransportAttribute`
- Generates static registration methods that directly instantiate and register these types
- Output: `{AssemblyName}_GeneratedRegistration.Register()` method in each assembly

### 2. Updated Manager Classes

#### FileSystemManager
- Already had `RegisterFileSystems(VfsFileSystemFactory factory)` method (no changes needed)

#### VirtualDiskManager
- Already has public `TypeMap` and `ExtensionMap` properties for direct registration (no changes needed)

#### VolumeManager
- Added `RegisterLogicalVolumeFactory(LogicalVolumeFactory factory)` instance method
- Complements existing `RegisterLogicalVolumeFactory(Assembly assembly)` reflection method

### 3. SetupHelper

Added `RegisterAssemblyAot(string assemblyName, Action registrationAction)` method that:
- Tracks registered assemblies (avoiding duplicates)
- Calls the provided registration action
- Is fully AOT-compatible (no reflection)

## Usage

### For Reflection-Based Applications (Original Approach)

```csharp
using DiscUtils.Complete;

// Register all DiscUtils assemblies using reflection
SetupHelper.SetupComplete();

// Now use DiscUtils normally
var fs = new NtfsFileSystem(stream);
```

### For AOT Applications (New Approach)

#### Option 1: Use Manual Registration Methods (Recommended)

```csharp
using DiscUtils.Setup;

// Register Core types (always required)
DiscUtils_Core_GeneratedRegistration.Register();

// Register specific file systems you need
NtfsRegistration.Register();  // For NTFS
FatRegistration.Register();   // For FAT
// ... register other types as needed

// Register virtual disk formats
VdiRegistration.Register();   // For VDI
VhdRegistration.Register();   // For VHD
// ... register other formats as needed

// Now use DiscUtils normally
var fs = new NtfsFileSystem(stream);
```

#### Option 2: Direct Manual Registration

```csharp
using DiscUtils;

// Manually register factories you need
FileSystemManager.RegisterFileSystems(new DiscUtils.Ntfs.FileSystemFactory());

var vdiFactory = new DiscUtils.Vdi.DiskFactory();
VirtualDiskManager.TypeMap.Add("VDI", vdiFactory);
VirtualDiskManager.ExtensionMap.Add("vdi", vdiFactory);
// ... etc.
```

## Available Registration Methods

Each DiscUtils library that provides factories includes a `{LibraryName}Registration.Register()` method:

- **File Systems:**
  - `NtfsRegistration.Register()` - NTFS support
  - `FatRegistration.Register()` - FAT12/16/32 support
  - And more...

- **Virtual Disks:**
  - `VdiRegistration.Register()` - VirtualBox VDI format
  - `VhdRegistration.Register()` - Microsoft VHD format
  - And more...

- **Core:**
  - `DiscUtils_Core_GeneratedRegistration.Register()` - RAW disk format and core features

Refer to each library's Setup namespace for available registration methods.

## Limitations

1. **Dynamic Discovery**: AOT builds cannot dynamically discover all available file systems or disk formats. You must explicitly register what you need.

2. **Assembly Scanning**: The `RegisterAssembly(Assembly assembly)` method cannot be used in AOT builds.

3. **Type.GetType()**: Cannot be used to find generated types dynamically in AOT. Each registration must be explicitly called.

## Migration Guide

To migrate existing code to AOT:

### Before (Reflection-based):
```csharp
SetupHelper.SetupComplete();
```

### After (AOT-compatible):
```csharp
using DiscUtils.Setup;

// Register Core
DiscUtils_Core_GeneratedRegistration.Register();

// Register only what you need
NtfsRegistration.Register();
VdiRegistration.Register();
// etc.
```

## Build Configuration

To use the source generator, no special configuration is needed. The generator runs automatically during compilation and produces registration code for each assembly that contains factories.

Generated code is placed in:
```
obj/{Configuration}/{TargetFramework}/generated/DiscUtils.SourceGenerator/DiscUtils.SourceGenerator.DiscUtilsRegistrationGenerator/{AssemblyName}_GeneratedRegistration.g.cs
```

## Future Improvements

Potential enhancements for better AOT support:
1. Create a marker interface or attribute for users to specify which types they want to use, enabling automatic registration
2. Provide a code analyzer to suggest which registration methods to call based on usage
3. Create helper methods for common scenarios (e.g., `RegisterAllFileSystems()`, `RegisterAllVirtualDisks()`)
