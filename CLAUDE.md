# FreeType.Net

What the package is: `README.md`. It is also the nuget.org page, so it stays consumer-facing, and relative links break
there. This file covers working in the repo.

## Build and test

- FreeType is the `external/freetype` submodule; `git submodule update --init` fills it.
- `build/build-native.sh <rid>` builds FreeType for one RID into `artifacts/native/<rid>/` and checks what it built:
  its exports, its dependencies, and per platform its soname, install name, Mach-O platform, page alignment or longjmp
  model. Each RID builds on its own OS: Windows and Linux ones on a host of their arch, Apple ones on macOS, Android with
  `ANDROID_NDK_ROOT` set, `browser-wasm` with the .NET `wasm-tools` workload installed.
- `dotnet build FreeType.Net.slnx`. Warnings are errors. The test project needs this host's native library built first.
- Tests are TUnit executables: `dotnet run --project tests/FreeType.Net.Tests`, never `dotnet test`. They read every
  bound struct field against the test font's own tables, so they run on each desktop RID in CI: a field placed or sized
  wrong shows only where its layout differs.
- `tests/FreeType.Net.PackageSmoke` publishes NativeAOT against a packed package from `artifacts/packages` only:
  `dotnet pack src/FreeType.Net -c Release -o artifacts/packages`, then
  `dotnet publish tests/FreeType.Net.PackageSmoke -c Release -r linux-x64 -p:FreeTypeNetVersion=<packed version>` and run
  what it published.

## Bindings

- Names are FreeType's own, C structs as `*Rec` structs and handles as pointers to them, so FreeType's reference
  documents the API.
- A C `long` (`FT_Long`, `FT_ULong`, `FT_Fixed`, `FT_Pos`) is `CLong` or `CULong`, never `nint` or `long`.
- A new function is a `[LibraryImport]` in `FT.cs`; the export test then checks every platform's library has it.
- `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` list the public API; a public change not in them fails the
  build (RS0016/RS0017), and its code fix writes the line.

## Updating FreeType

Check out the new tag in `external/freetype`, then diff its public headers' structs against `Structs.cs` and its error
list against `FT_Error.cs`. The README, `THIRD-PARTY-NOTICES.md` and the changelog name the version.

## Release

Give `CHANGELOG.md` a section for the version and move every `PublicAPI.Unshipped.txt` entry into
`PublicAPI.Shipped.txt`, then push `v<version>` from `main`. `release.yml` builds every RID, tests on each desktop one,
packs with `RequireAllNatives`, runs the package smoke, publishes to nuget.org through trusted publishing (environment
`nuget`) and cuts the GitHub Release from that section; a `-` in the tag makes a prerelease.
