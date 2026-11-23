using System.Reflection;
using System.Runtime.InteropServices;
using FreeType.Net;

return Smoke.Run();

static unsafe class Smoke
{
    public static int Run()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("NotoSans-Subset.ttf")!;
        var font = new byte[stream.Length];
        stream.ReadExactly(font);

        FT_LibraryRec* library;
        Check(FT.FT_Init_FreeType(&library));
        int major,
            minor,
            patch;
        FT.FT_Library_Version(library, &major, &minor, &patch);

        var inked = 0;
        fixed (byte* data = font)
        {
            FT_FaceRec* face;
            Check(
                FT.FT_New_Memory_Face(library, data, new CLong(font.Length), new CLong(0), &face)
            );
            Check(FT.FT_Set_Char_Size(face, new CLong(0), new CLong(16 * 64), 72, 72));
            Check(
                FT.FT_Load_Glyph(
                    face,
                    FT.FT_Get_Char_Index(face, new CULong('H')),
                    FT_LOAD.FT_LOAD_DEFAULT
                )
            );
            Check(FT.FT_Render_Glyph(face->glyph, FT_Render_Mode.FT_RENDER_MODE_MONO));
            var bitmap = face->glyph->bitmap;
            foreach (
                var bits in new ReadOnlySpan<byte>(bitmap.buffer, (int)bitmap.rows * bitmap.pitch)
            )
                inked += System.Numerics.BitOperations.PopCount(bits);
            Check(FT.FT_Done_Face(face));
        }
        Check(FT.FT_Done_FreeType(library));

        Console.WriteLine($"FreeType {major}.{minor}.{patch} inked {inked} pixels of 'H'");
        return inked > 0 ? 0 : 1;
    }

    static void Check(FT_Error error)
    {
        if (error != FT_Error.FT_Err_Ok)
            throw new InvalidOperationException($"FreeType failed with {error}.");
    }
}
