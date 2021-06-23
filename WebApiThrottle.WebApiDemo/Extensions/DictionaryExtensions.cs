using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// The Extension namespace.
/// </summary>
namespace WebApiThrottle.WebApiDemo.Extensions
{
    /// <summary>
    /// Class DictionaryExtensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        public static TValue GetValueOrDefault<TValue, TKey>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>TValue.</returns>
        public static TValue GetValue<TValue, TKey>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValue)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue();
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified dictionary].
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns><c>true</c> if [is null or empty] [the specified dictionary]; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this IDictionary dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="onEachFn">The on each function.</param>
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> onEachFn)
        {
            foreach (var entry in dictionary)
            {
                onEachFn(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Unordereds the equivalent to.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="thisMap">The this map.</param>
        /// <param name="otherMap">The other map.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool UnorderedEquivalentTo<K, V>(this IDictionary<K, V> thisMap, IDictionary<K, V> otherMap)
        {
            if (thisMap == null || otherMap == null) return thisMap == otherMap;
            if (thisMap.Count != otherMap.Count) return false;

            foreach (var entry in thisMap)
            {
                V otherValue;
                if (!otherMap.TryGetValue(entry.Key, out otherValue)) return false;
                if (!Equals(entry.Value, otherValue)) return false;
            }

            return true;
        }

        /// <summary>
        /// Converts all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="map">The map.</param>
        /// <param name="createFn">The create function.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> ConvertAll<T, K, V>(IDictionary<K, V> map, Func<K, V, T> createFn)
        {
            var list = new List<T>();
            map.Each((kvp) => list.Add(createFn(kvp.Key, kvp.Value)));
            return list;
        }

        /// <summary>
        /// Gets the or add.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <param name="createFn">The create function.</param>
        /// <returns>V.</returns>
        public static V GetOrAdd<K, V>(this Dictionary<K, V> map, K key, Func<K, V> createFn)
        {
            //simulate ConcurrentDictionary.GetOrAdd
            lock (map)
            {
                V val;
                if (!map.TryGetValue(key, out val))
                {
                    map[key] = val = createFn(key);
                }
                return val;
            }
        }

        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFn">The value function.</param>
        /// <returns>V.</returns>
        public static void AddOrUpdate<K, V>(this Dictionary<K, V> map, K key, Func<K, V> valueFn)
        {
            //simulate ConcurrentDictionary.GetOrAdd
            lock (map)
            {
                var value = valueFn(key);
                if (map.ContainsKey(key))
                {
                    map[key] = value;
                }
                else
                {
                    map.Add(key, value);
                }
            }
        }
        /// <summary>
        /// Pairs the with.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>KeyValuePair&lt;TKey, TValue&gt;.</returns>
        public static KeyValuePair<TKey, TValue> PairWith<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        /// <summary>
        /// To the concurrent dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="from">From.</param>
        /// <returns>ConcurrentDictionary&lt;TKey, TValue&gt;.</returns>
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IDictionary<TKey, TValue> from)
        {
            var to = new ConcurrentDictionary<TKey, TValue>();
            foreach (var entry in from)
            {
                to[entry.Key] = entry.Value;
            }
            return to;
        }

        /// <summary>
        /// Tries the remove.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, out TValue value)
        {
            lock (map)
            {
                if (!map.TryGetValue(key, out value)) return false;
                map.Remove(key);
                return true;
            }
        }

        /// <summary>
        /// Removes the key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
        public static Dictionary<TKey, TValue> RemoveKey<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key)
        {
            map?.Remove(key);
            return map;
        }

        /// <summary>
        /// Moves the key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="oldKey">The old key.</param>
        /// <param name="newKey">The new key.</param>
        /// <param name="valueFilter">The value filter.</param>
        /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
        public static Dictionary<TKey, TValue> MoveKey<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey oldKey, TKey newKey, Func<TValue, TValue> valueFilter = null)
        {
            if (map == null)
                return null;

            TValue value;
            if (map.TryGetValue(oldKey, out value))
                map[newKey] = valueFilter != null ? valueFilter(value) : value;

            map.Remove(oldKey);
            return map;
        }
        /// <summary>
        /// To the sorted dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="from">From.</param>
        /// <returns>SortedDictionary&lt;TKey, TValue&gt;.</returns>
        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> from)
        {
            return new SortedDictionary<TKey, TValue>(from);
        }

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IDictionary&lt;System.String, T&gt;.</returns>
        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
            {
                return null;
            }
            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                AddPropertyToDictionary<T>(property, source, dictionary);
            }
            return dictionary;
        }
        /// <summary>
        /// Adds the property to dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="source">The source.</param>
        /// <param name="dictionary">The dictionary.</param>
        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
            {
                dictionary.Add(property.Name, (T)value);
            }
        }
        /// <summary>
        /// Determines whether [is of type] [the specified value].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is of type] [the specified value]; otherwise, <c>false</c>.</returns>
        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public static T ToObject<T>(this IDictionary<string, object> source)
        where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Dictionary&lt;System.String, T&gt;.</returns>
        public static Dictionary<string, T> ToDictionary<T>(this IEnumerable<KeyValuePair<string, T>> source)
        {
            Dictionary<string, T> dicResult = new Dictionary<string, T>(source.Count());

            foreach (KeyValuePair<string, T> keyValuePair in source)
            {
                if (!dicResult.ContainsKey(keyValuePair.Key))
                {
                    dicResult.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return dicResult;
        }

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <returns>IDictionary&lt;System.String, System.String&gt;.</returns>
        public static IDictionary<string, string> ToDictionary(this NameValueCollection @this)
        {
            var dict = new Dictionary<string, string>();

            if (@this != null)
            {
                foreach (string key in @this.AllKeys)
                {
                    dict.Add(key, @this[key]);
                }
            }

            return dict;
        }
    }
}