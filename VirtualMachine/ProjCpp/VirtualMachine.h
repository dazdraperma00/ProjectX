#pragma once

#include "Variant.h"

enum ByteCommand : byte
{
	CALL,
	RET,
	VAX,
	FETCH,
	STORE,
	PUSH,
	POP,
	ADD,
	SUB,
	INC,
	DEC,
	MULT,
	DIV,
	MOD,
	AND,
	OR,
	NOT,
	LT,
	GT,
	LET,
	GET,
	EQ,
	NEQ,
	JZ,
	JNZ,
	JMP,
	HALT,

	NONE
};

class VirtualMachine
{
public:
	VirtualMachine();

	Variant* HeapAlloc(const int count = 1);

	bool Run(byte* program);

	const Variant* GetStack(int* size);

private:
	static const int c_nChunkSize = 64;

	void Resize();

	void HeapCollect();
	void HeapMove(Variant* p);

	Variant* m_pStack;
	int m_nCapacity;
	Variant* m_sp;
	Variant* m_bp;
	Variant m_vax;

	struct HeapChunk
	{
		HeapChunk* pNext = nullptr;
		Variant vData[c_nChunkSize];
	};
	HeapChunk* m_pFirstChunk;
	HeapChunk* m_pCurrentChunk;
	Variant* m_pCurrentSlot;
};