using System;
using System.Collections.Generic;

namespace Reflex
{
    public class LazyDictionary<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _valueFactory;
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public LazyDictionary(Func<TKey, TValue> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!_dictionary.TryGetValue(key, out var value))
                {
                    value = _valueFactory.Invoke(key);
                    _dictionary.Add(key, value);
                }

                return value;
            }
        }
    }
}