using System.Runtime.InteropServices;

namespace FreeType.Net;

/// <summary>FreeType's C API under its own names, so the
/// <see href="https://freetype.org/freetype2/docs/reference/">FreeType API reference</see> documents each member.
/// FreeType's objects belong to one thread at a time.</summary>
public static unsafe partial class FT
{
    /// <summary>The native library the functions bind to: <c>freetype_net.dll</c>, <c>libfreetype_net.so</c> or
    /// <c>libfreetype_net.dylib</c>, and on iOS and in the browser the archive the app links.</summary>
    public const string LibraryName = "freetype_net";

    public const byte FT_CURVE_TAG_CONIC = 0x00;
    public const byte FT_CURVE_TAG_ON = 0x01;
    public const byte FT_CURVE_TAG_CUBIC = 0x02;

    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Init_FreeType(FT_LibraryRec** alibrary);

    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Done_FreeType(FT_LibraryRec* library);

    [LibraryImport(LibraryName)]
    public static partial void FT_Library_Version(
        FT_LibraryRec* library,
        int* amajor,
        int* aminor,
        int* apatch
    );

    /// <summary>Sets <paramref name="property_name"/> of <paramref name="module_name"/>, both NUL-terminated, such as
    /// the <c>truetype</c> module's <c>interpreter-version</c>.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Property_Set(
        FT_LibraryRec* library,
        byte* module_name,
        byte* property_name,
        void* value
    );

    /// <summary>Opens face <paramref name="face_index"/> of the font at <paramref name="file_base"/>, which must stay
    /// where it is until the face is done.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_New_Memory_Face(
        FT_LibraryRec* library,
        byte* file_base,
        CLong file_size,
        CLong face_index,
        FT_FaceRec** aface
    );

    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Done_Face(FT_FaceRec* face);

    /// <summary>Sizes the face's active size in 26.6 points at the given dpi; zero for a width or a resolution takes
    /// the other.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Set_Char_Size(
        FT_FaceRec* face,
        CLong char_width,
        CLong char_height,
        uint horz_resolution,
        uint vert_resolution
    );

    /// <summary>The glyph the face's charmap maps <paramref name="charcode"/> to, or 0 for none.</summary>
    [LibraryImport(LibraryName)]
    public static partial uint FT_Get_Char_Index(FT_FaceRec* face, CULong charcode);

    /// <summary>A glyph's advance: in font units with <see cref="FT_LOAD.FT_LOAD_NO_SCALE"/>, else 16.16
    /// pixels.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Get_Advance(
        FT_FaceRec* face,
        uint gindex,
        FT_LOAD load_flags,
        CLong* padvance
    );

    /// <summary>Loads a glyph into <see cref="FT_FaceRec.glyph"/>, which holds it until the face's next load.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Load_Glyph(
        FT_FaceRec* face,
        uint glyph_index,
        FT_LOAD load_flags
    );

    /// <summary>Rasterizes the slot's glyph into <see cref="FT_GlyphSlotRec.bitmap"/>.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Render_Glyph(
        FT_GlyphSlotRec* slot,
        FT_Render_Mode render_mode
    );

    /// <summary>Makes another size of the face, which <see cref="FT_Activate_Size"/> switches to without redoing
    /// its hinting setup.</summary>
    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_New_Size(FT_FaceRec* face, FT_SizeRec** size);

    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Done_Size(FT_SizeRec* size);

    [LibraryImport(LibraryName)]
    public static partial FT_Error FT_Activate_Size(FT_SizeRec* size);

    public static bool FT_IS_SCALABLE(FT_FaceRec* face) =>
        Has(face, FT_FACE_FLAG.FT_FACE_FLAG_SCALABLE);

    public static bool FT_IS_TRICKY(FT_FaceRec* face) =>
        Has(face, FT_FACE_FLAG.FT_FACE_FLAG_TRICKY);

    static bool Has(FT_FaceRec* face, FT_FACE_FLAG flag) =>
        (face->face_flags.Value & (nint)flag) != 0;
}
