using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyNotes.Models
{
    public class ShellNavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        private IDictionary<string, object> _parameters = new Dictionary<string, object>();

        public ShellNavigationParameters()
        {

        }

        public ShellNavigationParameters(IDictionary<string, object> parameters)
        {
            _parameters = parameters;
        }

        public void Add(string key, object value)
        {
            _parameters.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public int Count()
        {
            return _parameters.Count;
        }

        public T GetValue<T>(string key)
        {
            try
            {
                if (_parameters.TryGetValue(key, out object value))
                {
                    return (T)value;
                }
                else
                {
                    return default;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(ShellNavigationParameters)}.{nameof(GetValue)} - Cannot cast value to {typeof(T)}");
                return default;
            }
        }

        public ICollection<string> GetKeys()
        {
            return _parameters.Keys;
        }

        public ICollection<object> GetValues()
        {
            return _parameters.Values;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
