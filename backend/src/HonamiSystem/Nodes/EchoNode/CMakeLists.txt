cmake_minimum_required(VERSION 3.20)
project(EchoNode LANGUAGES CXX)

list(APPEND CMAKE_PREFIX_PATH 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../../../../external/grpc/dist"
)

find_package(Protobuf REQUIRED)
find_package(gRPC REQUIRED)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_executable(EchoNode src/main.cpp)
target_link_libraries(EchoNode 
    PRIVATE
    gRPC::grpc++
    protobuf::libprotobuf
)
