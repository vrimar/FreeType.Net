using System.Runtime.InteropServices;

namespace FreeType.Net.Tests;

/// <summary>A library with the test font open in it.</summary>
sealed unsafe class OpenFace : IDisposable
{
    public static readonly byte[] Font = File.ReadAllBytes(
        Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Subset.ttf")
    );

    GCHandle _pin;

    public OpenFace()
    {
        _pin = GCHandle.Alloc(Font, GCHandleType.Pinned);
        FT_LibraryRec* library;
        Check(FT.FT_Init_FreeType(&library));
        Library = library;
        FT_FaceRec* face;
        Check(
            FT.FT_New_Memory_Face(
                library,
                (byte*)_pin.AddrOfPinnedObject(),
                new CLong(Font.Length),
                new CLong(0),
                &face
            )
        );
        Face = face;
    }

    public FT_LibraryRec* Library { get; }

    public FT_FaceRec* Face { get; }

    public static void Check(FT_Error error)
    {
        if (error != FT_Error.FT_Err_Ok)
            throw new InvalidOperationException($"FreeType failed with {error}.");
    }

    public void Dispose()
    {
        Check(FT.FT_Done_Face(Face));
        Check(FT.FT_Done_FreeType(Library));
        _pin.Free();
    }
}
