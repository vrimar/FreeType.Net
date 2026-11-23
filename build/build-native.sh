#!/usr/bin/env bash
set -euo pipefail

rid="${1:?usage: build/build-native.sh <rid>}"
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
build="$repo/artifacts/obj/native/$rid"
out="$repo/artifacts/native/$rid"

fail() {
    echo "build-native $rid: $*" >&2
    exit 1
}

[ -f "$repo/external/freetype/include/freetype/freetype.h" ] ||
    fail "external/freetype is empty: run git submodule update --init"

host_arch() {
    case "$(uname -m)" in
    x86_64 | amd64) echo x64 ;;
    aarch64 | arm64) echo arm64 ;;
    *) uname -m ;;
    esac
}

# Reads the whole pipe: grep -q stops early, and its writer's SIGPIPE then fails the check under pipefail.
has() { grep -E -- "$1" >/dev/null; }

configure=(-S "$repo/build" -B "$build" -DCMAKE_BUILD_TYPE=Release)
case "$rid" in
win-x64 | win-arm64)
    [ "${OS:-}" = "Windows_NT" ] || fail "build it on Windows"
    configure+=(-A "$([ "$rid" = win-x64 ] && echo x64 || echo ARM64)" -DBUILD_SHARED_LIBS=ON)
    lib=freetype_net.dll
    ;;
linux-x64 | linux-arm64)
    [ "$(uname -s)" = Linux ] && [ "$(host_arch)" = "${rid#linux-}" ] || fail "build it on a Linux ${rid#linux-} host"
    configure+=(-DBUILD_SHARED_LIBS=ON)
    lib=libfreetype_net.so
    ;;
osx-x64 | osx-arm64)
    [ "$(uname -s)" = Darwin ] || fail "build it on macOS"
    arch="$([ "$rid" = osx-x64 ] && echo x86_64 || echo arm64)"
    configure+=(-DBUILD_SHARED_LIBS=ON -DCMAKE_OSX_ARCHITECTURES="$arch" -DCMAKE_OSX_DEPLOYMENT_TARGET=11.0)
    lib=libfreetype_net.dylib
    ;;
ios-arm64 | iossimulator-arm64)
    [ "$(uname -s)" = Darwin ] || fail "build it on macOS"
    sysroot="$([ "$rid" = ios-arm64 ] && echo iphoneos || echo iphonesimulator)"
    configure+=(-DBUILD_SHARED_LIBS=OFF -DCMAKE_SYSTEM_NAME=iOS -DCMAKE_OSX_SYSROOT="$sysroot"
        -DCMAKE_OSX_ARCHITECTURES=arm64 -DCMAKE_OSX_DEPLOYMENT_TARGET=13.0
        -DCMAKE_TRY_COMPILE_TARGET_TYPE=STATIC_LIBRARY)
    lib=libfreetype_net.a
    ;;
android-arm64 | android-x64)
    ndk="${ANDROID_NDK_ROOT:-${ANDROID_NDK_LATEST_HOME:-}}"
    [ -f "$ndk/build/cmake/android.toolchain.cmake" ] || fail "set ANDROID_NDK_ROOT to an Android NDK"
    llvm="$(echo "$ndk"/toolchains/llvm/prebuilt/*/bin)"
    configure+=(-DBUILD_SHARED_LIBS=ON -DCMAKE_TOOLCHAIN_FILE="$ndk/build/cmake/android.toolchain.cmake"
        -DANDROID_ABI="$([ "$rid" = android-arm64 ] && echo arm64-v8a || echo x86_64)" -DANDROID_PLATFORM=android-21)
    lib=libfreetype_net.so
    ;;
browser-wasm)
    # The workload's own emscripten: an archive from another links cleanly, then traps at runtime.
    dotnet_root="${DOTNET_ROOT:-$(dirname "$(readlink -f "$(command -v dotnet)")")}"
    sdk="$(ls -d "$dotnet_root"/packs/Microsoft.NET.Runtime.Emscripten.*.Sdk.*/*/tools 2>/dev/null | sort -V | tail -1)"
    [ -n "$sdk" ] || fail "no emscripten under $dotnet_root/packs: run dotnet workload install wasm-tools"
    emver="$(echo "$sdk" | grep -oE 'Emscripten\.[0-9]+\.[0-9]+\.[0-9]+' | grep -oE '[0-9]+\.[0-9]+\.[0-9]+$')"
    export EMSDK_PATH="$sdk" EMSCRIPTEN="$sdk/emscripten"
    export DOTNET_EMSCRIPTEN_LLVM_ROOT="$sdk/bin" DOTNET_EMSCRIPTEN_BINARYEN_ROOT="$sdk"
    DOTNET_EMSCRIPTEN_NODE_JS="$(find "$dotnet_root"/packs/Microsoft.NET.Runtime.Emscripten.*.Node.*/ -name node -type f | sort -V | tail -1)"
    EM_CACHE="$(ls -d "$dotnet_root"/packs/Microsoft.NET.Runtime.Emscripten.*.Cache.*/*/tools/emscripten/cache | sort -V | tail -1)"
    export DOTNET_EMSCRIPTEN_NODE_JS EM_CACHE
    source "$sdk/emsdk_env.sh" >/dev/null
    # .NET links with wasm exception handling, and setjmp in FreeType's rasterizer must lower to the same.
    configure=("$EMSCRIPTEN/emcmake" cmake "${configure[@]}" -DBUILD_SHARED_LIBS=OFF
        -DCMAKE_TRY_COMPILE_TARGET_TYPE=STATIC_LIBRARY
        -DCMAKE_C_FLAGS="-fwasm-exceptions -sSUPPORT_LONGJMP=wasm -sWASM_LEGACY_EXCEPTIONS=0")
    lib=libfreetype_net.a
    out="$out/$emver"
    ;;
*) fail "unknown rid; one of win-x64 win-arm64 linux-x64 linux-arm64 osx-x64 osx-arm64 ios-arm64 iossimulator-arm64 android-arm64 android-x64 browser-wasm" ;;
esac

rm -rf "$build" "$repo/artifacts/native/$rid"
if [ "${configure[0]}" = "-S" ]; then
    cmake "${configure[@]}"
else
    "${configure[@]}"
fi
cmake --build "$build" --config Release --parallel

built="$(find "$build/freetype" -name "$lib" -type f | head -1)"
[ -n "$built" ] || fail "no $lib under $build/freetype"
mkdir -p "$out"

case "$rid" in
win-*)
    cp "$built" "$out/"
    vswhere="/c/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe"
    dumpbin="$("$vswhere" -latest -products '*' -find 'VC/Tools/MSVC/**/bin/Host*/*/dumpbin.exe' | tr -d '\r' | head -1)"
    [ -n "$dumpbin" ] || fail "no dumpbin.exe in the Visual Studio install"
    dll="$(cygpath -w "$out/$lib")"
    imports="$(MSYS_NO_PATHCONV=1 "$dumpbin" /nologo /dependents "$dll" | tr -d '\r' | grep -iE '^\s+\S+\.dll$' | tr -d ' ')"
    [ "$imports" = "KERNEL32.dll" ] || fail "imports $(echo "$imports" | tr '\n' ' ')beyond KERNEL32.dll"
    MSYS_NO_PATHCONV=1 "$dumpbin" /nologo /exports "$dll" | tr -d '\r' | has ' FT_Init_FreeType$' || fail "exports no FT_Init_FreeType"
    ;;
linux-*)
    cp "$built" "$out/$lib"
    strip --strip-unneeded "$out/$lib"
    dynamic="$(readelf -d "$out/$lib")"
    echo "$dynamic" | has "\(SONAME\).*\[$lib\]" || fail "its soname is not $lib"
    echo "$dynamic" | has 'SYMBOLIC' || fail "it is not linked -Bsymbolic"
    needed="$(echo "$dynamic" | grep NEEDED | grep -oE '\[[^]]+\]' | tr -d '[]' | sort | tr '\n' ' ')"
    [ "$needed" = "libc.so.6 libm.so.6 " ] || [ "$needed" = "libc.so.6 " ] || fail "needs $needed"
    nm -D --defined-only "$out/$lib" | has ' FT_Init_FreeType$' || fail "exports no FT_Init_FreeType"
    ;;
android-*)
    cp "$built" "$out/$lib"
    "$llvm/llvm-strip" --strip-unneeded "$out/$lib"
    "$llvm/llvm-readelf" -d "$out/$lib" | has 'SYMBOLIC' || fail "it is not linked -Bsymbolic"
    "$llvm/llvm-readelf" -lW "$out/$lib" | awk '$1 == "LOAD" && $NF != "0x4000" { bad = 1 } END { exit bad }' ||
        fail "a LOAD segment is not 16 KB aligned"
    "$llvm/llvm-nm" -D --defined-only "$out/$lib" | has ' FT_Init_FreeType$' || fail "exports no FT_Init_FreeType"
    ;;
osx-*)
    cp "$built" "$out/$lib"
    strip -x "$out/$lib"
    [ "$(lipo -archs "$out/$lib")" = "$arch" ] || fail "holds $(lipo -archs "$out/$lib"), not $arch"
    [ "$(otool -D "$out/$lib" | tail -1)" = "@rpath/$lib" ] || fail "its install name is not @rpath/$lib"
    deps="$(otool -L "$out/$lib" | tail -n +2 | awk '{ print $1 }' | grep -v "@rpath/$lib" | tr '\n' ' ')"
    [ "$deps" = "/usr/lib/libSystem.B.dylib " ] || fail "links $deps"
    nm -gU "$out/$lib" | has ' _FT_Init_FreeType$' || fail "exports no FT_Init_FreeType"
    ;;
ios-*)
    cp "$built" "$out/$lib"
    platform="$([ "$rid" = ios-arm64 ] && echo 2 || echo 7)"
    platforms="$(otool -l "$out/$lib" | awk '$1 == "platform" { print $2 }' | sort -u | tr '\n' ' ')"
    [ "$platforms" = "$platform " ] || fail "holds objects for Mach-O platforms $platforms, not $platform"
    nm -gU "$out/$lib" 2>/dev/null | has ' _FT_Init_FreeType$' || fail "defines no FT_Init_FreeType"
    ;;
browser-wasm)
    # No lib prefix: the wasm pinvoke table matches the library a DllImport names against the archive's stem.
    cp "$built" "$out/freetype_net.a"
    first="$(ar t "$out/freetype_net.a" | head -1)"
    [ "$(ar p "$out/freetype_net.a" "$first" | od -An -tx1 -N4 | tr -d ' ')" = "0061736d" ] || fail "holds no wasm objects"
    symbols="$("$DOTNET_EMSCRIPTEN_LLVM_ROOT/llvm-nm" "$out/freetype_net.a" 2>/dev/null)"
    echo "$symbols" | has ' T FT_Init_FreeType$' || fail "defines no FT_Init_FreeType"
    echo "$symbols" | has ' U __wasm_longjmp$' || fail "its longjmp is not wasm exception handling's"
    ;;
esac

echo "build-native $rid: built and checked $(find "$out" -type f | sed "s|$repo/||" | tr '\n' ' ')"
