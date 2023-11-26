using System.Collections.ObjectModel;
using System.Diagnostics;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization.LightJson;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable NotAccessedField.Local

/// <summary>
/// Represents a key-value pair collection of JsonValue objects.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(JsonObjectDebugView))]
public sealed class JsonObject : IEnumerable<KeyValuePair<string, JsonValue>>, IEnumerable<JsonValue>
{
    private readonly IDictionary<string, JsonValue> _properties;

    /// <summary>
    /// Gets the number of properties in this JsonObject.
    /// </summary>
    public int Count => _properties.Count;

    /// <summary>
    /// Gets or sets the property with the given key.
    /// </summary>
    /// <param name="key">The key of the property to get or set.</param>
    /// <remarks>
    /// The getter will return JsonValue.Null if the given key is not associated with any value.
    /// </remarks>
    public JsonValue this[string key]
    {
        get => _properties.TryGetValue(key, out var value) ? value : JsonValue.Null;
        set => _properties[key] = value;
    }

    /// <summary>
    /// Initializes a new instance of JsonObject.
    /// </summary>
    public JsonObject()
    {
        _properties = new Dictionary<string, JsonValue>();
    }

    /// <summary>
    /// Adds a key with a null value to this collection.
    /// </summary>
    /// <param name="key">The key of the property to be added.</param>
    /// <remarks>Returns this JsonObject.</remarks>
    public JsonObject Add(string key)
    {
        return Add(key, JsonValue.Null);
    }

    /// <summary>
    /// Adds a value associated with a key to this collection.
    /// </summary>
    /// <param name="key">The key of the property to be added.</param>
    /// <param name="value">The value of the property to be added.</param>
    /// <returns>Returns this JsonObject.</returns>
    public JsonObject Add(string key, JsonValue value)
    {
        _properties.Add(key, value);
        return this;
    }

    /// <summary>
    /// Adds a value associated with a key to this collection only if the value is not null.
    /// </summary>
    /// <param name="key">The key of the property to be added.</param>
    /// <param name="value">The value of the property to be added.</param>
    /// <returns>Returns this JsonObject.</returns>
    public JsonObject AddIfNotNull(string key, JsonValue value)
    {
        if (!value.IsNull)
        {
            Add(key, value);
        }

        return this;
    }

    /// <summary>
    /// Removes a property with the given key.
    /// </summary>
    /// <param name="key">The key of the property to be removed.</param>
    /// <returns>
    /// Returns true if the given key is found and removed; otherwise, false.
    /// </returns>
    public bool Remove(string key)
    {
        return _properties.Remove(key);
    }

    /// <summary>
    /// Clears the contents of this collection.
    /// </summary>
    /// <returns>Returns this JsonObject.</returns>
    public JsonObject Clear()
    {
        _properties.Clear();
        return this;
    }

    /// <summary>
    /// Changes the key of one of the items in the collection.
    /// </summary>
    /// <remarks>
    /// This method has no effects if the <i>oldKey</i> does not exists.
    /// If the <i>newKey</i> already exists, the value will be overwritten.
    /// </remarks>
    /// <param name="oldKey">The name of the key to be changed.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <returns>Returns this JsonObject.</returns>
    public JsonObject Rename(string oldKey, string newKey)
    {
        if (_properties.TryGetValue(oldKey, out var value))
        {
            Remove(oldKey);
            this[newKey] = value;
        }

        return this;
    }

    /// <summary>
    /// Determines whether this collection contains an item associated with the given key.
    /// </summary>
    /// <param name="key">The key to locate in this collection.</param>
    /// <returns>Returns true if the key is found; otherwise, false.</returns>
    public bool ContainsKey(string key)
    {
        return _properties.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether this collection contains an item associated with the given key
    /// </summary>
    /// <param name="key">The key to locate in this collection.</param>
    /// <param name="value">
    /// When this method returns, this value gets assigned the JsonValue associated with
    /// the key, if the key is found; otherwise, JsonValue.Null is assigned.
    /// </param>
    /// <returns>Returns true if the key is found; otherwise, false.</returns>
    public bool ContainsKey(string key, out JsonValue value)
    {
        return _properties.TryGetValue(key, out value);
    }

    /// <summary>
    /// Determines whether this collection contains the given JsonValue.
    /// </summary>
    /// <param name="value">The value to locate in this collection.</param>
    /// <returns>Returns true if the value is found; otherwise, false.</returns>
    public bool Contains(JsonValue value)
    {
        return _properties.Values.Contains(value);
    }

    /// <summary>
    /// Returns an enumerator that iterates through this collection.
    /// </summary>
    public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through this collection.
    /// </summary>
    IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
    {
        return _properties.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through this collection.
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Returns a JSON string representing the state of the object.
    /// </summary>
    /// <remarks>
    /// The resulting string is safe to be inserted as is into dynamically
    /// generated JavaScript or JSON code.
    /// </remarks>
    public override string ToString()
    {
        return ToString(false);
    }

    /// <summary>
    /// Returns a JSON string representing the state of the object.
    /// </summary>
    /// <remarks>
    /// The resulting string is safe to be inserted as is into dynamically
    /// generated JavaScript or JSON code.
    /// </remarks>
    /// <param name="pretty">
    /// Indicates whether the resulting string should be formatted for human-readability.
    /// </param>
    public string ToString(bool pretty)
    {
        return JsonWriter.Serialize(this, pretty);
    }

    public IReadOnlyDictionary<string, JsonValue> AsDictionary()
    {
        return new ReadOnlyDictionary<string, JsonValue>(_properties);
    }

    private class JsonObjectDebugView
    {
        private readonly JsonObject _jsonObject;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair[] Keys
        {
            get
            {
                var keys = new KeyValuePair[_jsonObject.Count];

                var i = 0;
                foreach (var property in _jsonObject)
                {
                    keys[i] = new KeyValuePair(property.Key, property.Value);
                    i += 1;
                }

                return keys;
            }
        }

        public JsonObjectDebugView(JsonObject jsonObject)
        {
            _jsonObject = jsonObject;
        }

        [DebuggerDisplay("{_value.ToString(),nq}", Name = "{key}", Type = "JsonValue({Type})")]
        public class KeyValuePair
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string _key;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private JsonValue _value;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private JsonValueType Type => _value.Type;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object View
            {
                get
                {
                    if (_value.IsJsonObject)
                    {
                        return (JsonObject) _value;
                    }

                    if (_value.IsJsonArray)
                    {
                        return (JsonArray) _value;
                    }

                    return _value;
                }
            }

            public KeyValuePair(string key, JsonValue value)
            {
                _key = key;
                _value = value;
            }
        }
    }
}
