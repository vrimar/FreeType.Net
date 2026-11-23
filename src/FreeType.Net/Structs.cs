using System.Runtime.InteropServices;

namespace FreeType.Net;

// FreeType's FT_Long, FT_ULong, FT_Fixed and FT_Pos are C longs: 32 bits on Windows, 64 elsewhere, hence CLong.

/// <summary>A FreeType library instance, opaque; <c>FT_Library</c> is a pointer to one.</summary>
public readonly struct FT_LibraryRec { }

/// <summary>Client data and the function FreeType calls to finalize it.</summary>
public unsafe struct FT_Generic
{
    public void* data;
    public delegate* unmanaged<void*, void> finalizer;
}

/// <summary>A 2D vector, in 26.6 fractional pixels or in font units.</summary>
public struct FT_Vector
{
    public CLong x;
    public CLong y;
}

/// <summary>A bounding box.</summary>
public struct FT_BBox
{
    public CLong xMin;
    public CLong yMin;
    public CLong xMax;
    public CLong yMax;
}

/// <summary>One of a face's bitmap strikes.</summary>
public struct FT_Bitmap_Size
{
    public short height;
    public short width;
    public CLong size;
    public CLong x_ppem;
    public CLong y_ppem;
}

/// <summary>A charmap of a face; <c>FT_CharMap</c> is a pointer to one.</summary>
public unsafe struct FT_CharMapRec
{
    public FT_FaceRec* face;
    public FT_Encoding encoding;
    public ushort platform_id;
    public ushort encoding_id;
}

/// <summary>A list's ends, private to FreeType.</summary>
public unsafe struct FT_ListRec
{
    public void* head;
    public void* tail;
}

/// <summary>A face of a font file; <c>FT_Face</c> is a pointer to one. Its metrics are in font units.</summary>
public unsafe struct FT_FaceRec
{
    public CLong num_faces;
    public CLong face_index;
    public CLong face_flags;
    public CLong style_flags;
    public CLong num_glyphs;
    public byte* family_name;
    public byte* style_name;
    public int num_fixed_sizes;
    public FT_Bitmap_Size* available_sizes;
    public int num_charmaps;
    public FT_CharMapRec** charmaps;
    public FT_Generic generic;
    public FT_BBox bbox;
    public ushort units_per_EM;
    public short ascender;
    public short descender;
    public short height;
    public short max_advance_width;
    public short max_advance_height;
    public short underline_position;
    public short underline_thickness;
    public FT_GlyphSlotRec* glyph;
    public FT_SizeRec* size;
    public FT_CharMapRec* charmap;
    public void* driver;
    public void* memory;
    public void* stream;
    public FT_ListRec sizes_list;
    public FT_Generic autohint;
    public void* extensions;
    public void* @internal;
}

/// <summary>The metrics of a face at one size, in 26.6 fractional pixels; the scales are 16.16.</summary>
public struct FT_Size_Metrics
{
    public ushort x_ppem;
    public ushort y_ppem;
    public CLong x_scale;
    public CLong y_scale;
    public CLong ascender;
    public CLong descender;
    public CLong height;
    public CLong max_advance;
}

/// <summary>A face at one size; <c>FT_Size</c> is a pointer to one.</summary>
public unsafe struct FT_SizeRec
{
    public FT_FaceRec* face;
    public FT_Generic generic;
    public FT_Size_Metrics metrics;
    public void* @internal;
}

/// <summary>A glyph's metrics, in 26.6 fractional pixels, or in font units when loaded with
/// <see cref="FT_LOAD.FT_LOAD_NO_SCALE"/>.</summary>
public struct FT_Glyph_Metrics
{
    public CLong width;
    public CLong height;
    public CLong horiBearingX;
    public CLong horiBearingY;
    public CLong horiAdvance;
    public CLong vertBearingX;
    public CLong vertBearingY;
    public CLong vertAdvance;
}

/// <summary>A bitmap or pixmap; <see cref="pitch"/> is negative when its rows run bottom-up.</summary>
public unsafe struct FT_Bitmap
{
    public uint rows;
    public uint width;
    public int pitch;
    public byte* buffer;
    public ushort num_grays;
    public FT_Pixel_Mode pixel_mode;
    public byte palette_mode;
    public void* palette;
}

/// <summary>A glyph's outline: its points, each one's <c>FT_CURVE_TAG_XXX</c> tag, and the index of each contour's
/// last point.</summary>
public unsafe struct FT_Outline
{
    public short n_contours;
    public short n_points;
    public FT_Vector* points;
    public byte* tags;
    public short* contours;
    public int flags;
}

/// <summary>The glyph a face last loaded; <c>FT_GlyphSlot</c> is a pointer to one.</summary>
public unsafe struct FT_GlyphSlotRec
{
    public FT_LibraryRec* library;
    public FT_FaceRec* face;
    public FT_GlyphSlotRec* next;
    public uint glyph_index;
    public FT_Generic generic;
    public FT_Glyph_Metrics metrics;
    public CLong linearHoriAdvance;
    public CLong linearVertAdvance;
    public FT_Vector advance;
    public FT_Glyph_Format format;
    public FT_Bitmap bitmap;
    public int bitmap_left;
    public int bitmap_top;
    public FT_Outline outline;
    public uint num_subglyphs;
    public void* subglyphs;
    public void* control_data;
    public CLong control_len;
    public CLong lsb_delta;
    public CLong rsb_delta;
    public void* other;
    public void* @internal;
}
