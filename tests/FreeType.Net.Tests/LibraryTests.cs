using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FreeType.Net.Tests;

public sealed class LibraryTests
{
    [Test]
    public async Task EveryBoundFunction_IsExportedByTheNativeLibrary()
    {
        var entryPoints = typeof(FT)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(method =>
                method.GetCustomAttribute<LibraryImportAttribute>() is { } import
                    ? import.EntryPoint ?? method.Name
                    : null
            )
            .OfType<string>()
            .ToArray();
        var library = NativeLibrary.Load(FT.LibraryName, typeof(FT).Assembly, null);

        await Assert.That(entryPoints).IsNotEmpty();
        await Assert
            .That(entryPoints.Where(name => !NativeLibrary.TryGetExport(library, name, out _)))
            .IsEmpty();
    }

    [Test]
    public async Task TheNativeLibrary_IsTheFreeTypeTheSubmoduleHolds()
    {
        var header = File.ReadAllText(
            Path.Combine(
                RepositoryRoot(),
                "external",
                "freetype",
                "include",
                "freetype",
                "freetype.h"
            )
        );

        await Assert
            .That(LibraryVersion())
            .IsEqualTo(
                $"{Define(header, "MAJOR")}.{Define(header, "MINOR")}.{Define(header, "PATCH")}"
            );
    }

    static unsafe string LibraryVersion()
    {
        using var open = new OpenFace();
        int major,
            minor,
            patch;
        FT.FT_Library_Version(open.Library, &major, &minor, &patch);
        return $"{major}.{minor}.{patch}";
    }

    static string Define(string header, string part) =>
        Regex.Match(header, $@"#define FREETYPE_{part}\s+(\d+)").Groups[1].Value;

    static string RepositoryRoot()
    {
        for (
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            directory is not null;
            directory = directory.Parent
        )
        {
            if (File.Exists(Path.Combine(directory.FullName, "FreeType.Net.slnx")))
                return directory.FullName;
        }
        throw new InvalidOperationException("The tests run outside the repository.");
    }
}
