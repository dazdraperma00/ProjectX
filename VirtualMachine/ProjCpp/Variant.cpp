#include "Variant.h"
#include "VirtualMachine.h"

Variant::~Variant()
{
	delete[](pValue);
}

const string Variant::ToString() const
{
	if (usNull != c_null)
	{
		return std::to_string(dValue);
	}
	else if (usType == VarType::STR)
	{
		return (char*)pValue;
	}
	else if (usType == VarType::ARR)
	{
		string str = "";
		for (int i = 0; i < nLength; ++i)
		{
			str = str + ((Variant*)pValue)[i].ToString() + "|";
		}
		return str;
	}
	else if (!pValue)
	{
		return "NULL";
	}

	return "";
}

Variant Variant::FromBytes(byte** pc, VirtualMachine* pvm)
{
	Variant var(*((double*)*pc));
	*pc += sizeof(double);

	if (var.usNull == c_null)
	{
		if (var.usType == VarType::STR)
		{
			char* str = new char[var.nLength];
			memcpy(str, *pc, var.nLength);
			*pc += var.nLength;
			var.pValue = str;
		}
		else if (var.usType == VarType::ARR)
		{
			Variant* arr = pvm->HeapAlloc(var.nLength);

			for (int i = 0; i < var.nLength; ++i)
			{
				arr[i] = Variant::FromBytes(pc, pvm);
			}

			var.pValue = arr;
		}
	}

	return var;
}

bool Variant::Equal(Variant* op1, Variant* op2)
{
	if (op1->usNull != c_null && op2->usNull != c_null)
	{
		return op1->dValue == op2->dValue;
	}
	else if (op1->usType == VarType::STR && op2->usType == VarType::STR)
	{

		if (op1->nLength != op2->nLength)
		{
			return false;
		}

		for (int i = 0; i < op1->nLength; ++i)
		{
			if (((char*)op1->pValue)[i] != ((char*)op2->pValue)[i])
			{
				return false;
			}
		}

		return true;
	}
	else if (op1->usType == VarType::ARR && op2->usType == VarType::STR)
	{
		if (op1->nLength != op2->nLength)
		{
			return false;
		}

		for (int i = 0; i < op1->nLength; ++i)
		{
			if (!Equal((Variant*)op1->pValue + i, (Variant*)op2->pValue + i))
			{
				return false;
			}
		}

		return true;
	}
	else
	{
		return false;
	}
}