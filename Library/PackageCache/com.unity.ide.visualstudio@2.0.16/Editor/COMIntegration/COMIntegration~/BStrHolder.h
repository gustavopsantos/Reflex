#pragma once
#include <OleAuto.h>

struct BStrHolder
{
	BStrHolder() :
		m_Str(NULL)
	{
	}

	BStrHolder(const wchar_t* str) :
		m_Str(SysAllocString(str))
	{
	}

	~BStrHolder()
	{
		if (m_Str != NULL)
			SysFreeString(m_Str);
	}

	operator BSTR() const
	{
		return m_Str;
	}

	BSTR* operator&()
	{
		if (m_Str != NULL)
		{
			SysFreeString(m_Str);
			m_Str = NULL;
		}

		return &m_Str;
	}

private:
	BSTR m_Str;
};
