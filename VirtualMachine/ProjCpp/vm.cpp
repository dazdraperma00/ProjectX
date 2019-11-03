#include <fstream>
#include <iostream>
#include <ctime>
#include "VirtualMachine.h"

using std::cout;

int main(int argc, char** argv)
{
	if (argc < 2)
	{
		cout << "expected file name\n";
		return 1;
	}

	string path = argv[1];
	string ext = path.substr(path.find_last_of('.'));
	if (ext != ".bpsc")
	{
		cout << "wrong file extension\n";
		return 1;
	}

	std::ifstream file(argv[1], std::ifstream::ate | std::ifstream::binary);
	if (!file.is_open())
	{
		cout << "file opening error\n";
		return 1;
	}

	const int length = file.tellg();
	file.seekg(0);
	byte* code = new byte[length];
	file.read((char*)code, length);

	VirtualMachine vm;

	const unsigned int tStart = clock();

	bool bSuccess = vm.Run(code);

	const unsigned int tEnd = clock();

	cout << tEnd - tStart << std::endl;
	if (bSuccess)
	{
		cout << "successfull\n";
	}
	else
	{
		cout << "broken\n";
	}

	int size;
	const Variant* pStack = vm.GetStack(&size);
	for (int i = size - 1; i >= 0; --i)
	{
		cout << pStack[i].ToString() << std::endl;
	}

	return 0;
}