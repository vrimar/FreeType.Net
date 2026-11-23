namespace FreeType.Net;

/// <summary>FreeType's error codes, from <c>fterrdef.h</c>. A module error code in the upper byte is never set, since
/// FreeType.Net builds FreeType without <c>FT_CONFIG_OPTION_USE_MODULE_ERRORS</c>.</summary>
public enum FT_Error
{
    /// <summary>No error.</summary>
    FT_Err_Ok = 0x00,

    /// <summary>Cannot open resource.</summary>
    FT_Err_Cannot_Open_Resource = 0x01,

    /// <summary>Unknown file format.</summary>
    FT_Err_Unknown_File_Format = 0x02,

    /// <summary>Broken file.</summary>
    FT_Err_Invalid_File_Format = 0x03,

    /// <summary>Invalid FreeType version.</summary>
    FT_Err_Invalid_Version = 0x04,

    /// <summary>Module version is too low.</summary>
    FT_Err_Lower_Module_Version = 0x05,

    /// <summary>Invalid argument.</summary>
    FT_Err_Invalid_Argument = 0x06,

    /// <summary>Unimplemented feature.</summary>
    FT_Err_Unimplemented_Feature = 0x07,

    /// <summary>Broken table.</summary>
    FT_Err_Invalid_Table = 0x08,

    /// <summary>Broken offset within table.</summary>
    FT_Err_Invalid_Offset = 0x09,

    /// <summary>Array allocation size too large.</summary>
    FT_Err_Array_Too_Large = 0x0A,

    /// <summary>Missing module.</summary>
    FT_Err_Missing_Module = 0x0B,

    /// <summary>Missing property.</summary>
    FT_Err_Missing_Property = 0x0C,

    /// <summary>Invalid glyph index.</summary>
    FT_Err_Invalid_Glyph_Index = 0x10,

    /// <summary>Invalid character code.</summary>
    FT_Err_Invalid_Character_Code = 0x11,

    /// <summary>Unsupported glyph image format.</summary>
    FT_Err_Invalid_Glyph_Format = 0x12,

    /// <summary>Cannot render this glyph format.</summary>
    FT_Err_Cannot_Render_Glyph = 0x13,

    /// <summary>Invalid outline.</summary>
    FT_Err_Invalid_Outline = 0x14,

    /// <summary>Invalid composite glyph.</summary>
    FT_Err_Invalid_Composite = 0x15,

    /// <summary>Too many hints.</summary>
    FT_Err_Too_Many_Hints = 0x16,

    /// <summary>Invalid pixel size.</summary>
    FT_Err_Invalid_Pixel_Size = 0x17,

    /// <summary>Invalid SVG document.</summary>
    FT_Err_Invalid_SVG_Document = 0x18,

    /// <summary>Invalid object handle.</summary>
    FT_Err_Invalid_Handle = 0x20,

    /// <summary>Invalid library handle.</summary>
    FT_Err_Invalid_Library_Handle = 0x21,

    /// <summary>Invalid module handle.</summary>
    FT_Err_Invalid_Driver_Handle = 0x22,

    /// <summary>Invalid face handle.</summary>
    FT_Err_Invalid_Face_Handle = 0x23,

    /// <summary>Invalid size handle.</summary>
    FT_Err_Invalid_Size_Handle = 0x24,

    /// <summary>Invalid glyph slot handle.</summary>
    FT_Err_Invalid_Slot_Handle = 0x25,

    /// <summary>Invalid charmap handle.</summary>
    FT_Err_Invalid_CharMap_Handle = 0x26,

    /// <summary>Invalid cache manager handle.</summary>
    FT_Err_Invalid_Cache_Handle = 0x27,

    /// <summary>Invalid stream handle.</summary>
    FT_Err_Invalid_Stream_Handle = 0x28,

    /// <summary>Too many modules.</summary>
    FT_Err_Too_Many_Drivers = 0x30,

    /// <summary>Too many extensions.</summary>
    FT_Err_Too_Many_Extensions = 0x31,

    /// <summary>Out of memory.</summary>
    FT_Err_Out_Of_Memory = 0x40,

    /// <summary>Unlisted object.</summary>
    FT_Err_Unlisted_Object = 0x41,

    /// <summary>Cannot open stream.</summary>
    FT_Err_Cannot_Open_Stream = 0x51,

    /// <summary>Invalid stream seek.</summary>
    FT_Err_Invalid_Stream_Seek = 0x52,

    /// <summary>Invalid stream skip.</summary>
    FT_Err_Invalid_Stream_Skip = 0x53,

    /// <summary>Invalid stream read.</summary>
    FT_Err_Invalid_Stream_Read = 0x54,

    /// <summary>Invalid stream operation.</summary>
    FT_Err_Invalid_Stream_Operation = 0x55,

    /// <summary>Invalid frame operation.</summary>
    FT_Err_Invalid_Frame_Operation = 0x56,

    /// <summary>Nested frame access.</summary>
    FT_Err_Nested_Frame_Access = 0x57,

    /// <summary>Invalid frame read.</summary>
    FT_Err_Invalid_Frame_Read = 0x58,

    /// <summary>Raster uninitialized.</summary>
    FT_Err_Raster_Uninitialized = 0x60,

    /// <summary>Raster corrupted.</summary>
    FT_Err_Raster_Corrupted = 0x61,

    /// <summary>Raster overflow.</summary>
    FT_Err_Raster_Overflow = 0x62,

    /// <summary>Negative height while rastering.</summary>
    FT_Err_Raster_Negative_Height = 0x63,

    /// <summary>Too many registered caches.</summary>
    FT_Err_Too_Many_Caches = 0x70,

    /// <summary>Invalid opcode.</summary>
    FT_Err_Invalid_Opcode = 0x80,

    /// <summary>Too few arguments.</summary>
    FT_Err_Too_Few_Arguments = 0x81,

    /// <summary>Stack overflow.</summary>
    FT_Err_Stack_Overflow = 0x82,

    /// <summary>Code overflow.</summary>
    FT_Err_Code_Overflow = 0x83,

    /// <summary>Bad argument.</summary>
    FT_Err_Bad_Argument = 0x84,

    /// <summary>Division by zero.</summary>
    FT_Err_Divide_By_Zero = 0x85,

    /// <summary>Invalid reference.</summary>
    FT_Err_Invalid_Reference = 0x86,

    /// <summary>Found debug opcode.</summary>
    FT_Err_Debug_OpCode = 0x87,

    /// <summary>Found ENDF opcode in execution stream.</summary>
    FT_Err_ENDF_In_Exec_Stream = 0x88,

    /// <summary>Nested DEFS.</summary>
    FT_Err_Nested_DEFS = 0x89,

    /// <summary>Invalid code range.</summary>
    FT_Err_Invalid_CodeRange = 0x8A,

    /// <summary>Execution context too long.</summary>
    FT_Err_Execution_Too_Long = 0x8B,

    /// <summary>Too many function definitions.</summary>
    FT_Err_Too_Many_Function_Defs = 0x8C,

    /// <summary>Too many instruction definitions.</summary>
    FT_Err_Too_Many_Instruction_Defs = 0x8D,

    /// <summary>SFNT font table missing.</summary>
    FT_Err_Table_Missing = 0x8E,

    /// <summary>Horizontal header (hhea) table missing.</summary>
    FT_Err_Horiz_Header_Missing = 0x8F,

    /// <summary>Locations (loca) table missing.</summary>
    FT_Err_Locations_Missing = 0x90,

    /// <summary>Name table missing.</summary>
    FT_Err_Name_Table_Missing = 0x91,

    /// <summary>Character map (cmap) table missing.</summary>
    FT_Err_CMap_Table_Missing = 0x92,

    /// <summary>Horizontal metrics (hmtx) table missing.</summary>
    FT_Err_Hmtx_Table_Missing = 0x93,

    /// <summary>PostScript (post) table missing.</summary>
    FT_Err_Post_Table_Missing = 0x94,

    /// <summary>Invalid horizontal metrics.</summary>
    FT_Err_Invalid_Horiz_Metrics = 0x95,

    /// <summary>Invalid character map (cmap) format.</summary>
    FT_Err_Invalid_CharMap_Format = 0x96,

    /// <summary>Invalid ppem value.</summary>
    FT_Err_Invalid_PPem = 0x97,

    /// <summary>Invalid vertical metrics.</summary>
    FT_Err_Invalid_Vert_Metrics = 0x98,

    /// <summary>Could not find context.</summary>
    FT_Err_Could_Not_Find_Context = 0x99,

    /// <summary>Invalid PostScript (post) table format.</summary>
    FT_Err_Invalid_Post_Table_Format = 0x9A,

    /// <summary>Invalid PostScript (post) table.</summary>
    FT_Err_Invalid_Post_Table = 0x9B,

    /// <summary>Found FDEF or IDEF opcode in glyf bytecode.</summary>
    FT_Err_DEF_In_Glyf_Bytecode = 0x9C,

    /// <summary>Missing bitmap in strike.</summary>
    FT_Err_Missing_Bitmap = 0x9D,

    /// <summary>SVG hooks have not been set.</summary>
    FT_Err_Missing_SVG_Hooks = 0x9E,

    /// <summary>Opcode syntax error.</summary>
    FT_Err_Syntax_Error = 0xA0,

    /// <summary>Argument stack underflow.</summary>
    FT_Err_Stack_Underflow = 0xA1,

    /// <summary>Ignore.</summary>
    FT_Err_Ignore = 0xA2,

    /// <summary>No Unicode glyph name found.</summary>
    FT_Err_No_Unicode_Glyph_Name = 0xA3,

    /// <summary>Glyph too big for hinting.</summary>
    FT_Err_Glyph_Too_Big = 0xA4,

    /// <summary>'STARTFONT' field missing.</summary>
    FT_Err_Missing_Startfont_Field = 0xB0,

    /// <summary>'FONT' field missing.</summary>
    FT_Err_Missing_Font_Field = 0xB1,

    /// <summary>'SIZE' field missing.</summary>
    FT_Err_Missing_Size_Field = 0xB2,

    /// <summary>'FONTBOUNDINGBOX' field missing.</summary>
    FT_Err_Missing_Fontboundingbox_Field = 0xB3,

    /// <summary>'CHARS' field missing.</summary>
    FT_Err_Missing_Chars_Field = 0xB4,

    /// <summary>'STARTCHAR' field missing.</summary>
    FT_Err_Missing_Startchar_Field = 0xB5,

    /// <summary>'ENCODING' field missing.</summary>
    FT_Err_Missing_Encoding_Field = 0xB6,

    /// <summary>'BBX' field missing.</summary>
    FT_Err_Missing_Bbx_Field = 0xB7,

    /// <summary>'BBX' too big.</summary>
    FT_Err_Bbx_Too_Big = 0xB8,

    /// <summary>Font header corrupted or missing fields.</summary>
    FT_Err_Corrupted_Font_Header = 0xB9,

    /// <summary>Font glyphs corrupted or missing fields.</summary>
    FT_Err_Corrupted_Font_Glyphs = 0xBA,
}
