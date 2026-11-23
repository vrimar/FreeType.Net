cmake_minimum_required(VERSION 3.21)
project(freetype_net C)

# FreeType 2.13.2 asks for CMake 3.0, which CMake 4 refuses to configure below this floor.
set(CMAKE_POLICY_VERSION_MINIMUM 3.5)
# Links the C runtime in, so the DLL needs no Visual C++ redistributable on the machine.
set(CMAKE_MSVC_RUNTIME_LIBRARY MultiThreaded)

foreach(dependency ZLIB BZIP2 PNG HARFBUZZ BROTLI)
  set(FT_DISABLE_${dependency} ON CACHE BOOL "" FORCE)
endforeach()
set(SKIP_INSTALL_ALL ON CACHE BOOL "" FORCE)

add_subdirectory(../external/freetype freetype)

set_target_properties(freetype PROPERTIES OUTPUT_NAME freetype_net)
set_property(TARGET freetype PROPERTY VERSION)
set_property(TARGET freetype PROPERTY SOVERSION)

# Binds FreeType's calls to itself, never to another FreeType a process loaded first.
if(CMAKE_SYSTEM_NAME MATCHES "^(Linux|Android)$")
  target_link_options(freetype PRIVATE "LINKER:-Bsymbolic")
endif()

if(ANDROID)
  target_link_options(freetype PRIVATE "LINKER:-z,max-page-size=16384")
endif()
