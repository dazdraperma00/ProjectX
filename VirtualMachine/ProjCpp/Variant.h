#pragma once

#include <string>

using std::string;

typedef unsigned char byte;

class VirtualMachine;

enum VarType : unsigned short
{
	STR,
	ARR,
};

struct Variant
{
	static const unsigned short c_null = 0x7FF0;

	Variant()
		: pValue(nullptr),
		  usNull(c_null)
	{}

	Variant(const double val)
		: pValue(nullptr),
		  dValue(val)
	{}

	Variant(char* str, const int length)
		: pValue(str),
		  usNull(c_null),
		  usType(VarType::STR),
		  nLength(length)
	{}

	Variant(Variant* arr, const int length)
		: pValue(arr),
		  usNull(c_null),
		  usType(VarType::ARR),
		  nLength(length)
	{}

	~Variant();

	static bool Equal(Variant* op1, Variant* op2);

	const string ToString() const;
	static Variant FromBytes(byte** pc, VirtualMachine* pvm);

	union
	{
		double dValue;

		struct
		{
			unsigned short usNull;
			unsigned short usType;
			int nLength;
		};
	};

	void* pValue;
};