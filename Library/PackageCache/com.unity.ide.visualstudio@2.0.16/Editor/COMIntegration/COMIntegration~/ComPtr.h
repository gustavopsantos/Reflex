#pragma once

namespace win
{
	template<typename T>
	class ComPtr;

	template<typename T>
	class ComPtrRef
	{
	private:
		ComPtr<T>& m_ComPtr;

		ComPtrRef(ComPtr<T>& comPtr) :
			m_ComPtr(comPtr)
		{
		}

		friend class ComPtr<T>;

	public:
		inline operator T**()
		{
			return m_ComPtr.ReleaseAndGetAddressOf();
		}

		inline operator void**()
		{
			return reinterpret_cast<void**>(m_ComPtr.ReleaseAndGetAddressOf());
		}

		inline T* operator*() throw ()
		{
			return m_ComPtr;
		}

	};

	template<typename T>
	class ComPtr
	{
	private:
		T *ptr;

	public:
		inline ComPtr(void) : ptr(NULL) {}
		inline ~ComPtr(void) { this->Free(); }

		ComPtr(T *ptr)
		{
			if (NULL != (this->ptr = ptr))
			{
				this->ptr->AddRef();
			}
		}

		ComPtr(const ComPtr &ptr)
		{
			if (NULL != (this->ptr = ptr.ptr))
			{
				this->ptr->AddRef();
			}
		}

		inline bool operator!() const
		{
			return (NULL == this->ptr);
		}

		inline operator T*() const { return this->ptr; }

		inline T *operator->() const
		{
			//_assert(NULL != this->ptr);
			return this->ptr;
		}

		inline T &operator*()
		{
			//_assert(NULL != this->ptr);
			return *this->ptr;
		}

		inline ComPtrRef<T> operator&()
		{
			return ComPtrRef<T>(*this);
		}

		const ComPtr &operator=(T *ptr)
		{
			if (this->ptr != ptr)
			{
				this->Free();

				if (NULL != (this->ptr = ptr))
				{
					this->ptr->AddRef();
				}
			}

			return *this;
		}

		const ComPtr &operator=(const ComPtr &ptr)
		{
			if (this->ptr != ptr.ptr)
			{
				this->Free();

				if (NULL != (this->ptr = ptr.ptr))
				{
					this->ptr->AddRef();
				}
			}

			return *this;
		}

		void Free(void)
		{
			if (NULL != this->ptr)
			{
				this->ptr->Release();
				this->ptr = NULL;
			}
		}

		inline T** ReleaseAndGetAddressOf()
		{
			Free();
			return &ptr;
		}

		template<typename U>
		inline HRESULT As(ComPtrRef<U> p) const throw ()
		{
			return ptr->QueryInterface(__uuidof(U), p);
		}

		inline bool operator==(std::nullptr_t) const
		{
			return this->ptr == nullptr;
		}

		template<typename U>
		inline bool operator==(U* other)
		{
			if (ptr == nullptr || other == nullptr)
				return ptr == other;

			ComPtr<IUnknown> meUnknown;
			ComPtr<IUnknown> otherUnknown;

			if (FAILED(this->ptr->QueryInterface(__uuidof(IUnknown), &meUnknown)))
				return false;

			if (FAILED(other->QueryInterface(__uuidof(IUnknown), &otherUnknown)))
				return false;

			return static_cast<IUnknown*>(meUnknown) == static_cast<IUnknown*>(otherUnknown);
		}

		template<typename U>
		inline bool operator==(ComPtr<U>& other)
		{
			return *this == static_cast<U*>(other);
		}

		inline bool operator!=(std::nullptr_t) const
		{
			return this->ptr != nullptr;
		}

		template<typename U>
		inline bool operator!=(U* other)
		{
			return !(*this == other);
		}

		template<typename U>
		inline bool operator!=(ComPtr<U>& other)
		{
			return *this != static_cast<U*>(other);
		}
	};
}
