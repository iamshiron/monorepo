#include <grpcpp/grpcpp.h>
#include <iostream>

int main(int argc, char** argv) {
	std::string grpc_version = grpc::Version();
	std::cout << "Using gRPC " << grpc_version << std::endl;

	return 0;
}
