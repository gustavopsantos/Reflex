cmake_minimum_required(VERSION 3.15)

project(com)
set(SOURCES
        COMIntegration.cpp
        BStrHolder.h
        ComPtr.h
        dte80a.tlh
        )
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -O3 -Wall")
add_executable(COMIntegration ${SOURCES})
set_property(TARGET COMIntegration PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreaded")
target_link_libraries(COMIntegration Shlwapi.lib)
