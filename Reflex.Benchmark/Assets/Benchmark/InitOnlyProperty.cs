using System;

namespace Benchmark
{
	public struct InitOnlyProperty<T>
	{
		private T _value;
		private bool _initialized;
		private readonly Func<T> _activator;
		
		public InitOnlyProperty(Func<T> func, bool lazy = true)
		{
			_value = default;
			_activator = func;
			_initialized = false;
			
			if (lazy == false)
			{
				var unused = Value;
			}
		}

		public T Value
		{
			get
			{
				if (_initialized == false)
				{
					_value = _activator();
					_initialized = true;
				}
				
				return _value;
			}
		}
	}
}