# FreeType.Net

.NET bindings for [FreeType](https://freetype.org) 2.13.2, with FreeType built for every platform the package
targets, so an app installs nothing else.

```sh
dotnet add package FreeType.Net
```

## Use

The bindings are FreeType's C API under its own names, so the
[FreeType API reference](https://freetype.org/freetype2/docs/reference/) documents each one.

```csharp
using System.Runtime.InteropServices;
using FreeType.Net;
using static FreeType.Net.FT;

unsafe
{
    FT_LibraryRec* library;
    FT_Init_FreeType(&library);

    fixed (byte* data = fontBytes)
    {
        FT_FaceRec* face;
        FT_New_Memory_Face(library, data, new CLong(fontBytes.Length), new CLong(0), &face);
        FT_Set_Char_Size(face, new CLong(0), new CLong(16 * 64), 72, 72);
        FT_Load_Glyph(face, FT_Get_Char_Index(face, new CULong('A')), FT_LOAD.FT_LOAD_DEFAULT);
        FT_Render_Glyph(face->glyph, FT_Render_Mode.FT_RENDER_MODE_NORMAL);
        var bitmap = face->glyph->bitmap;
        // bitmap.rows rows of bitmap.width pixels, bitmap.pitch bytes apart
        FT_Done_Face(face);
    }

    FT_Done_FreeType(library);
}
```

- A pointer to a `*Rec` struct stands for FreeType's handle type: `FT_FaceRec*` is an `FT_Face`.
- `FT_Long`, `FT_ULong`, `FT_Fixed` and `FT_Pos` are `CLong` and `CULong`, since a C `long` is 32 bits on Windows and
  64 bits on Linux and macOS.
- FreeType's objects belong to one thread at a time.
- The package binds the functions its users have needed so far: library setup and properties, memory faces, sizes,
  glyph loading, advances and rendering. Ask for another in an issue.

## Platforms

| Runtime | FreeType arrives as |
|---|---|
| `win-x64`, `win-arm64` | `freetype_net.dll` |
| `linux-x64`, `linux-arm64` | `libfreetype_net.so` |
| `osx-x64`, `osx-arm64` | `libfreetype_net.dylib` |
| `android-arm64`, `android-x64` | `libfreetype_net.so`, aligned to 16 KB pages |
| `ios-arm64`, `iossimulator-arm64` | A static library a NativeAOT publish links |
| `browser-wasm` | A static library the app's wasm link takes in |

- The library is named `freetype_net`, so it never clashes with a system FreeType or another package's copy, and on
  Linux and Android it binds its calls to itself even when the process loaded another FreeType first.
- The Windows DLL carries its C runtime, so it needs no Visual C++ redistributable.
- iOS reaches FreeType only from a NativeAOT publish (`PublishAot=true`).
- The browser archive is compiled by emscripten 6.0.2, the one .NET 11's wasm workload links with, for wasm exception
  handling, .NET's default. An archive from another emscripten would link and then trap at runtime, so a build that
  links with another emscripten fails with FTNET003 instead.

FreeType is built with its own copy of zlib and without its optional dependencies, so it reads TrueType, OpenType, WOFF,
Type 1, CFF, PCF, BDF and Windows FNT fonts, but not WOFF2 or PNG-compressed color bitmaps.

## License

The bindings are MIT-licensed. FreeType comes under the FreeType License, which asks an app to credit the FreeType
Project in its documentation; that credit, and the licenses of the parts of FreeType under their own, are in
[THIRD-PARTY-NOTICES.md](https://github.com/vrimar/FreeType.Net/blob/main/THIRD-PARTY-NOTICES.md).
