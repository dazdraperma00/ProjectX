#include "VirtualMachine.h"
#include <cstring>

VirtualMachine::VirtualMachine()
	: m_nCapacity(16)
{
	m_pStack = new Variant[m_nCapacity];
	m_sp = m_pStack + m_nCapacity;
	m_bp = m_pStack - 1;

	m_pFirstChunk = m_pCurrentChunk = new HeapChunk;
	m_pCurrentSlot = m_pCurrentChunk->vData;
}

void VirtualMachine::Resize()
{
	HeapCollect();
	const int inc = m_nCapacity >> 1;
	Variant* pStack = new Variant[m_nCapacity + inc];
	memcpy(pStack, m_pStack, (byte*)(m_bp + 1) - (byte*)m_pStack);
	Variant* sp = pStack + (m_sp - m_pStack + inc);
	memcpy(sp, m_sp, (byte*)(m_pStack + m_nCapacity) - (byte*)m_sp);
	m_sp = sp;
	m_bp = pStack + (m_bp - m_pStack);
	delete[](m_pStack);
	m_pStack = pStack;
	m_nCapacity = m_nCapacity + inc;
}

Variant* VirtualMachine::HeapAlloc(const int count)
{
	if (m_pCurrentSlot + count >= m_pCurrentChunk->vData + c_nChunkSize)
	{
		m_pCurrentChunk->pNext = new HeapChunk;
		m_pCurrentChunk = m_pCurrentChunk->pNext;
		m_pCurrentSlot = m_pCurrentChunk->vData;
	}

	return m_pCurrentSlot++;
}

void VirtualMachine::HeapCollect()
{
	HeapChunk* pFirstChunk = m_pCurrentChunk = new HeapChunk;
	m_pCurrentSlot = m_pCurrentChunk->vData;

	for (Variant* p = m_sp; p < m_pStack + m_nCapacity; ++p)
	{
		HeapMove(p);
	}

	HeapChunk* iter = m_pFirstChunk;
	m_pFirstChunk = pFirstChunk;
	while (iter != nullptr)
	{
		HeapChunk* next = iter->pNext;
		delete(iter);
		iter = next;
	}
}

void VirtualMachine::HeapMove(Variant* p)
{
	if (!p->pValue)
	{
		return;
	}

	if (p->usNull == Variant::c_null && p->usType == VarType::ARR)
	{
		//Looks awful but I cant see the better way
		Variant* pChild = (Variant*)p->pValue;
		Variant* pCopy = HeapAlloc();
		pCopy->dValue = pChild->dValue;
		p->pValue = pCopy;
		HeapMove(pCopy);
		++pChild;

		while (pChild < pChild + p->nLength)
		{
			pCopy = HeapAlloc();
			pCopy->dValue = pChild->dValue;
			pCopy->pValue = pChild->pValue;
			HeapMove(pCopy);
			++pChild;
		}
	}
}

bool VirtualMachine::Run(byte* program)
{
	byte* pc = program;
	(++m_bp)->dValue = 0;

	while (true)
	{
		switch (*(pc++))
		{
		case ByteCommand::CALL:
		{
			const int mark = *((int*)pc);
			pc += sizeof(int);
			if (m_bp + 2 == m_sp)
			{
				Resize();
			}
			(++m_bp)->dValue = pc - program;
			(++m_bp)->dValue = m_pStack + m_nCapacity - m_sp;
			pc = program + mark;
		}
		break;
		case ByteCommand::RET:
		{
			Variant* ssp = m_pStack + m_nCapacity - (int)(m_bp--)->dValue;
			m_vax = m_sp != ssp ? *(m_sp++) : Variant();
			if (m_sp < ssp)
			{
				m_sp = ssp;
			}
			pc = program + (int)(m_bp--)->dValue;
		}
		break;
		case ByteCommand::VAX:
		{
			if (m_sp - 1 == m_bp)
			{
				Resize();
			}
			*(--m_sp) = m_vax;
		}
		break;
		case ByteCommand::FETCH:
		{
			if (m_sp - 1 == m_bp)
			{
				Resize();
			}
			*(--m_sp) = *(m_pStack + m_nCapacity - (int)m_bp->dValue - *((int*)pc));
			pc += sizeof(int);
		}
		break;
		case ByteCommand::STORE:
		{
			*(m_pStack + m_nCapacity - (int)m_bp->dValue - *((int*)pc)) = *(m_sp++);
			pc += sizeof(int);
		}
		break;
		case ByteCommand::PUSH:
		{
			if (m_sp - 1 == m_bp)
			{
				Resize();
			}
			*(--m_sp) = Variant::FromBytes(&pc, this);
		}
		break;
		case ByteCommand::POP:
		{
			(m_sp++)->pValue = nullptr;
		}
		break;
		case ByteCommand::ADD:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue += op2->dValue;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::SUB:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue -= op2->dValue;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::INC:
		{
			++m_sp->dValue;
			m_sp->pValue = nullptr;
			break;
		}
		break;
		case ByteCommand::DEC:
		{
			--m_sp->dValue;
			m_sp->pValue = nullptr;
		}
		break;
		case ByteCommand::MULT:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue *= op2->dValue;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::DIV:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			if (op2->dValue != 0.0)
			{
				op1->dValue /= op2->dValue;
			}
			else
			{
				op1->usNull = Variant::c_null;
			}

			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::MOD:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			if (op2->dValue != 0.0)
			{
				op1->dValue = (int)op1->dValue % (int)op2->dValue;
			}
			else
			{
				op1->usNull = Variant::c_null;
			}

			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::AND:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = (op1->dValue != 0.0 && op2->dValue != 0.0
				&& op1->usNull != Variant::c_null && op2->usNull != Variant::c_null) ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::OR:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = (op1->dValue != 0.0 && op1->usNull != Variant::c_null
				|| op2->dValue != 0.0 && op2->usNull != Variant::c_null) ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::NOT:
		{
			m_sp->dValue = (m_sp->dValue == 0.0 || m_sp->usNull == Variant::c_null) ? 1.0 : 0.0;
			m_sp->pValue = nullptr;
		}
		break;
		case ByteCommand::LT:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = op1->dValue < op2->dValue ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::GT:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = op1->dValue > op2->dValue ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::LET:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = op1->dValue <= op2->dValue ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::GET:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = op1->dValue >= op2->dValue ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::EQ:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = Variant::Equal(op1, op2) ? 1.0 : 0.0;
			op1->pValue = nullptr;
			op2->pValue = nullptr;
		}
		break;
		case ByteCommand::NEQ:
		{
			Variant* op2 = m_sp++;
			Variant* op1 = m_sp;

			op1->dValue = Variant::Equal(op1, op2) ? 0.0 : 1.0;
			op1->pValue = nullptr;
			op1->pValue = nullptr;
		}
		break;
		case ByteCommand::JZ:
		{
			pc = (m_sp++)->dValue == 0.0 ? program + *((int*)pc) : pc + sizeof(int);
		}
		break;
		case ByteCommand::JNZ:
		{
			pc = (m_sp++)->dValue != 0.0 ? program + *((int*)pc) : pc + sizeof(int);
		}
		break;
		case ByteCommand::JMP:
		{
			pc = program + *((int*)pc);
		}
		break;
		case ByteCommand::NONE:
		{
		}
		break;
		case ByteCommand::HALT:
		{
			return true;
		}
		default:
		{
			return false;
		}
		}
	}
}

const Variant* VirtualMachine::GetStack(int* size)
{
	*size = m_nCapacity;
	return m_pStack;
}