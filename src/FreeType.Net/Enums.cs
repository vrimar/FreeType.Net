namespace FreeType.Net;

/// <summary>The <c>FT_LOAD_XXX</c> flags <see cref="FT.FT_Load_Glyph"/> and <see cref="FT.FT_Get_Advance"/> take,
/// with the <c>FT_LOAD_TARGET_XXX</c> hinting targets.</summary>
[Flags]
public enum FT_LOAD
{
    FT_LOAD_DEFAULT = 0x0,
    FT_LOAD_NO_SCALE = 1 << 0,
    FT_LOAD_NO_HINTING = 1 << 1,
    FT_LOAD_RENDER = 1 << 2,
    FT_LOAD_NO_BITMAP = 1 << 3,
    FT_LOAD_VERTICAL_LAYOUT = 1 << 4,
    FT_LOAD_FORCE_AUTOHINT = 1 << 5,
    FT_LOAD_CROP_BITMAP = 1 << 6,
    FT_LOAD_PEDANTIC = 1 << 7,
    FT_LOAD_ADVANCE_ONLY = 1 << 8,
    FT_LOAD_IGNORE_GLOBAL_ADVANCE_WIDTH = 1 << 9,
    FT_LOAD_NO_RECURSE = 1 << 10,
    FT_LOAD_IGNORE_TRANSFORM = 1 << 11,
    FT_LOAD_MONOCHROME = 1 << 12,
    FT_LOAD_LINEAR_DESIGN = 1 << 13,
    FT_LOAD_SBITS_ONLY = 1 << 14,
    FT_LOAD_NO_AUTOHINT = 1 << 15,
    FT_LOAD_COLOR = 1 << 20,
    FT_LOAD_COMPUTE_METRICS = 1 << 21,
    FT_LOAD_BITMAP_METRICS_ONLY = 1 << 22,
    FT_LOAD_SVG_ONLY = 1 << 23,
    FT_LOAD_NO_SVG = 1 << 24,

    FT_LOAD_TARGET_NORMAL = FT_Render_Mode.FT_RENDER_MODE_NORMAL << 16,
    FT_LOAD_TARGET_LIGHT = FT_Render_Mode.FT_RENDER_MODE_LIGHT << 16,
    FT_LOAD_TARGET_MONO = FT_Render_Mode.FT_RENDER_MODE_MONO << 16,
    FT_LOAD_TARGET_LCD = FT_Render_Mode.FT_RENDER_MODE_LCD << 16,
    FT_LOAD_TARGET_LCD_V = FT_Render_Mode.FT_RENDER_MODE_LCD_V << 16,
}

/// <summary>The <c>FT_FACE_FLAG_XXX</c> bits of <see cref="FT_FaceRec.face_flags"/>.</summary>
[Flags]
public enum FT_FACE_FLAG : long
{
    FT_FACE_FLAG_SCALABLE = 1L << 0,
    FT_FACE_FLAG_FIXED_SIZES = 1L << 1,
    FT_FACE_FLAG_FIXED_WIDTH = 1L << 2,
    FT_FACE_FLAG_SFNT = 1L << 3,
    FT_FACE_FLAG_HORIZONTAL = 1L << 4,
    FT_FACE_FLAG_VERTICAL = 1L << 5,
    FT_FACE_FLAG_KERNING = 1L << 6,
    FT_FACE_FLAG_FAST_GLYPHS = 1L << 7,
    FT_FACE_FLAG_MULTIPLE_MASTERS = 1L << 8,
    FT_FACE_FLAG_GLYPH_NAMES = 1L << 9,
    FT_FACE_FLAG_EXTERNAL_STREAM = 1L << 10,
    FT_FACE_FLAG_HINTER = 1L << 11,
    FT_FACE_FLAG_CID_KEYED = 1L << 12,
    FT_FACE_FLAG_TRICKY = 1L << 13,
    FT_FACE_FLAG_COLOR = 1L << 14,
    FT_FACE_FLAG_VARIATION = 1L << 15,
    FT_FACE_FLAG_SVG = 1L << 16,
    FT_FACE_FLAG_SBIX = 1L << 17,
    FT_FACE_FLAG_SBIX_OVERLAY = 1L << 18,
}

/// <summary>The <c>FT_STYLE_FLAG_XXX</c> bits of <see cref="FT_FaceRec.style_flags"/>.</summary>
[Flags]
public enum FT_STYLE_FLAG : long
{
    FT_STYLE_FLAG_ITALIC = 1 << 0,
    FT_STYLE_FLAG_BOLD = 1 << 1,
}

/// <summary>How <see cref="FT.FT_Render_Glyph"/> rasterizes a glyph.</summary>
public enum FT_Render_Mode
{
    FT_RENDER_MODE_NORMAL = 0,
    FT_RENDER_MODE_LIGHT,
    FT_RENDER_MODE_MONO,
    FT_RENDER_MODE_LCD,
    FT_RENDER_MODE_LCD_V,
    FT_RENDER_MODE_SDF,
}

/// <summary>The pixel format of an <see cref="FT_Bitmap"/>.</summary>
public enum FT_Pixel_Mode : byte
{
    FT_PIXEL_MODE_NONE = 0,
    FT_PIXEL_MODE_MONO,
    FT_PIXEL_MODE_GRAY,
    FT_PIXEL_MODE_GRAY2,
    FT_PIXEL_MODE_GRAY4,
    FT_PIXEL_MODE_LCD,
    FT_PIXEL_MODE_LCD_V,
    FT_PIXEL_MODE_BGRA,
}

/// <summary>What a glyph slot holds: <see cref="FT_GlyphSlotRec.outline"/> or <see cref="FT_GlyphSlotRec.bitmap"/>,
/// among others.</summary>
public enum FT_Glyph_Format
{
    FT_GLYPH_FORMAT_NONE = 0,
    FT_GLYPH_FORMAT_COMPOSITE = 'c' << 24 | 'o' << 16 | 'm' << 8 | 'p',
    FT_GLYPH_FORMAT_BITMAP = 'b' << 24 | 'i' << 16 | 't' << 8 | 's',
    FT_GLYPH_FORMAT_OUTLINE = 'o' << 24 | 'u' << 16 | 't' << 8 | 'l',
    FT_GLYPH_FORMAT_PLOTTER = 'p' << 24 | 'l' << 16 | 'o' << 8 | 't',
    FT_GLYPH_FORMAT_SVG = 'S' << 24 | 'V' << 16 | 'G' << 8 | ' ',
}

/// <summary>A charmap's encoding.</summary>
public enum FT_Encoding
{
    FT_ENCODING_NONE = 0,
    FT_ENCODING_MS_SYMBOL = 's' << 24 | 'y' << 16 | 'm' << 8 | 'b',
    FT_ENCODING_UNICODE = 'u' << 24 | 'n' << 16 | 'i' << 8 | 'c',
    FT_ENCODING_SJIS = 's' << 24 | 'j' << 16 | 'i' << 8 | 's',
    FT_ENCODING_PRC = 'g' << 24 | 'b' << 16 | ' ' << 8 | ' ',
    FT_ENCODING_BIG5 = 'b' << 24 | 'i' << 16 | 'g' << 8 | '5',
    FT_ENCODING_WANSUNG = 'w' << 24 | 'a' << 16 | 'n' << 8 | 's',
    FT_ENCODING_JOHAB = 'j' << 24 | 'o' << 16 | 'h' << 8 | 'a',
    FT_ENCODING_ADOBE_STANDARD = 'A' << 24 | 'D' << 16 | 'O' << 8 | 'B',
    FT_ENCODING_ADOBE_EXPERT = 'A' << 24 | 'D' << 16 | 'B' << 8 | 'E',
    FT_ENCODING_ADOBE_CUSTOM = 'A' << 24 | 'D' << 16 | 'B' << 8 | 'C',
    FT_ENCODING_ADOBE_LATIN_1 = 'l' << 24 | 'a' << 16 | 't' << 8 | '1',
    FT_ENCODING_OLD_LATIN_2 = 'l' << 24 | 'a' << 16 | 't' << 8 | '2',
    FT_ENCODING_APPLE_ROMAN = 'a' << 24 | 'r' << 16 | 'm' << 8 | 'n',
}
