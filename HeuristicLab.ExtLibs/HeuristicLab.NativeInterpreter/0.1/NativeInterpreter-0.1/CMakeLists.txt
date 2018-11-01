cmake_minimum_required(VERSION 3.0)

project(
    hl-native-interpreter
    VERSION 0.1
    DESCRIPTION "Native C++ interpreter for HeuristicLab symbolic expression trees"
    LANGUAGES CXX
)

add_library(hl-native-interpreter SHARED src/interpreter.cpp)
target_include_directories(hl-native-interpreter PRIVATE ${PROJECT_SOURCE_DIR}/src ${PROJECT_SOURCE_DIR}/lib)
target_compile_features(hl-native-interpreter PRIVATE cxx_std_17)

add_library(hl-native-interpreter-vdt SHARED src/interpreter.cpp)
target_include_directories(hl-native-interpreter-vdt PRIVATE ${PROJECT_SOURCE_DIR}/src ${PROJECT_SOURCE_DIR}/lib)
target_compile_features(hl-native-interpreter-vdt PRIVATE cxx_std_17)

if(MSVC)
    target_compile_options(hl-native-interpreter PRIVATE "/W4" "/Qvec-report:2" "$<$<CONFIG:Release>:/O2>")
    target_compile_options(hl-native-interpreter-vdt PRIVATE "/W4" "/Qvec-report:2" "$<$<CONFIG:Release>:/O2>")
else()
    target_compile_options(hl-native-interpreter PRIVATE "-Wall" "-Wextra" "-Werror" "-fopt-info-vec-optimized" "$<$<CONFIG:Release>:-Ofast>")
    target_compile_options(hl-native-interpreter-vdt PRIVATE "-Wall" "-Wextra" "-Werror" "-fopt-info-vec-optimized" "$<$<CONFIG:Release>:-Ofast>")
    target_compile_definitions(hl-native-interpreter-vdt PRIVATE "USE_VDT")
endif()
