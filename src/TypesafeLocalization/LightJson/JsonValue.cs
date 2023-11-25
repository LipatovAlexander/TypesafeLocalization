using System.Diagnostics;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization.LightJson;

/// <summary>
/// A wrapper object that contains a valid JSON value.
/// </summary>
[DebuggerDisplay("{ToString(),nq}", Type = "JsonValue({Type})")]
[DebuggerTypeProxy(typeof(JsonValueDebugView))]
public readonly struct JsonValue
{
    private readonly object _reference;
    private readonly double _value;

    /// <summary>
    /// Represents a null JsonValue.
    /// </summary>
    public static readonly JsonValue Null = new(JsonValueType.Null, default, null);

    /// <summary>
    /// Gets the type of this JsonValue.
    /// </summary>
    public JsonValueType Type { get; }

    /// <summary>
    /// Gets a value indicating whether this JsonValue is Null.
    /// </summary>
    public bool IsNull => Type == JsonValueType.Null;

    /// <summary>
    /// Gets a value indicating whether this JsonValue is a Boolean.
    /// </summary>
    public bool IsBoolean => Type == JsonValueType.Boolean;

    /// <summary>
    /// Gets a value indicating whether this JsonValue is an Integer.
    /// </summary>
    public bool IsInteger
    {
        get
        {
            if (!IsNumber)
            {
                return false;
            }

            var value = _value;

            return value is >= int.MinValue and <= int.MaxValue && unchecked((int) value) == value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this JsonValue is a Number.
    /// </summary>
    public bool IsNumber => Type == JsonValueType.Number;

    /// <summary>
    /// Gets a value indicating whether this JsonValue is a String.
    /// </summary>
    public bool IsString => Type == JsonValueType.String;

    /// <summary>
    /// Gets a value indicating whether this JsonValue is a JsonObject.
    /// </summary>
    public bool IsJsonObject => Type == JsonValueType.Object;

    /// <summary>
    /// Gets a value indicating whether this JsonValue is a JsonArray.
    /// </summary>
    public bool IsJsonArray => Type == JsonValueType.Array;

    /// <summary>
    /// Gets a value indicating whether this JsonValue represents a DateTime.
    /// </summary>
    public bool IsDateTime => AsDateTime is not null;

    /// <summary>
    /// Gets this value as a Boolean type.
    /// </summary>
    public bool AsBoolean
    {
        get
        {
            switch (Type)
            {
                case JsonValueType.Boolean:
                    return _value == 1;

                case JsonValueType.Number:
                    return _value != 0;

                case JsonValueType.String:
                    return (string) _reference != "";

                case JsonValueType.Object:
                case JsonValueType.Array:
                    return true;

                case JsonValueType.Null:
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Gets this value as an Integer type.
    /// </summary>
    public int AsInteger
    {
        get
        {
            var value = AsNumber;

            return value switch
            {
                // Prevent overflow if the value doesn't fit.
                >= int.MaxValue => int.MaxValue,
                <= int.MinValue => int.MinValue,
                _ => (int) value
            };
        }
    }

    /// <summary>
    /// Gets this value as a Number type.
    /// </summary>
    public double AsNumber
    {
        get
        {
            switch (Type)
            {
                case JsonValueType.Boolean:
                    return _value == 1
                        ? 1
                        : 0;

                case JsonValueType.Number:
                    return _value;

                case JsonValueType.String:
                    if (double.TryParse((string) _reference, out var number))
                    {
                        return number;
                    }

                    goto default;

                case JsonValueType.Null:
                case JsonValueType.Object:
                case JsonValueType.Array:
                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Gets this value as a String type.
    /// </summary>
    public string AsString
    {
        get
        {
            return Type switch
            {
                JsonValueType.Boolean => _value == 1 ? "true" : "false",
                JsonValueType.Number => _value.ToString(),
                JsonValueType.String => (string) _reference,
                _ => null
            };
        }
    }

    /// <summary>
    /// Gets this value as an JsonObject.
    /// </summary>
    public JsonObject AsJsonObject =>
        IsJsonObject
            ? (JsonObject) _reference
            : null;

    /// <summary>
    /// Gets this value as an JsonArray.
    /// </summary>
    public JsonArray AsJsonArray =>
        IsJsonArray
            ? (JsonArray) _reference
            : null;

    /// <summary>
    /// Gets this value as a system.DateTime.
    /// </summary>
    public DateTime? AsDateTime
    {
        get
        {
            if (IsString && DateTime.TryParse((string) _reference, out DateTime value))
            {
                return value;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets this (inner) value as a System.object.
    /// </summary>
    public object AsObject
    {
        get
        {
            switch (Type)
            {
                case JsonValueType.Boolean:
                case JsonValueType.Number:
                    return _value;

                case JsonValueType.String:
                case JsonValueType.Object:
                case JsonValueType.Array:
                    return _reference;

                case JsonValueType.Null:
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when this JsonValue is not a JsonObject.
    /// </exception>
    public JsonValue this[string key]
    {
        get
        {
            if (IsJsonObject)
            {
                return ((JsonObject) _reference)[key];
            }

            throw new InvalidOperationException("This value does not represent a JsonObject.");
        }
        set
        {
            if (IsJsonObject)
            {
                ((JsonObject) _reference)[key] = value;
            }
            else
            {
                throw new InvalidOperationException("This value does not represent a JsonObject.");
            }
        }
    }

    /// <summary>
    /// Gets or sets the value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to get or set.</param>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when this JsonValue is not a JsonArray
    /// </exception>
    public JsonValue this[int index]
    {
        get
        {
            if (IsJsonArray)
            {
                return ((JsonArray) _reference)[index];
            }

            throw new InvalidOperationException("This value does not represent a JsonArray.");
        }
        set
        {
            if (IsJsonArray)
            {
                ((JsonArray) _reference)[index] = value;
            }
            else
            {
                throw new InvalidOperationException("This value does not represent a JsonArray.");
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonValue struct.
    /// </summary>
    /// <param name="type">The Json type of the JsonValue.</param>
    /// <param name="value">
    /// The internal value of the JsonValue.
    /// This is used when the Json type is Number or Boolean.
    /// </param>
    /// <param name="reference">
    /// The internal value reference of the JsonValue.
    /// This value is used when the Json type is String, JsonObject, or JsonArray.
    /// </param>
    private JsonValue(JsonValueType type, double value, object reference)
    {
        Type = type;
        _value = value;
        _reference = reference;
    }

    /// <summary>
    /// Initializes a new instance of the JsonValue struct, representing a Boolean value.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    public JsonValue(bool? value)
    {
        if (value.HasValue)
        {
            _reference = null;

            Type = JsonValueType.Boolean;

            _value = value.Value ? 1 : 0;
        }
        else
        {
            this = Null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonValue struct, representing a Number value.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    public JsonValue(double? value)
    {
        if (value.HasValue)
        {
            _reference = null;

            Type = JsonValueType.Number;

            _value = value.Value;
        }
        else
        {
            this = Null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonValue struct, representing a String value.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    public JsonValue(string value)
    {
        if (value is not null)
        {
            _value = default;

            Type = JsonValueType.String;

            _reference = value;
        }
        else
        {
            this = Null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonValue struct, representing a JsonObject.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    public JsonValue(JsonObject value)
    {
        if (value is not null)
        {
            _value = default;

            Type = JsonValueType.Object;

            _reference = value;
        }
        else
        {
            this = Null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonValue struct, representing a Array reference value.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    public JsonValue(JsonArray value)
    {
        if (value is not null)
        {
            _value = default;

            Type = JsonValueType.Array;

            _reference = value;
        }
        else
        {
            this = Null;
        }
    }

    /// <summary>
    /// Converts the given nullable boolean into a JsonValue.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator JsonValue(bool? value)
    {
        return new JsonValue(value);
    }

    /// <summary>
    /// Converts the given nullable double into a JsonValue.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator JsonValue(double? value)
    {
        return new JsonValue(value);
    }

    /// <summary>
    /// Converts the given string into a JsonValue.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator JsonValue(string value)
    {
        return new JsonValue(value);
    }

    /// <summary>
    /// Converts the given JsonObject into a JsonValue.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator JsonValue(JsonObject value)
    {
        return new JsonValue(value);
    }

    /// <summary>
    /// Converts the given JsonArray into a JsonValue.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator JsonValue(JsonArray value)
    {
        return new JsonValue(value);
    }

    /// <summary>
    /// Converts the given DateTime? into a JsonValue.
    /// </summary>
    /// <remarks>
    /// The DateTime value will be stored as a string using ISO 8601 format,
    /// since JSON does not define a DateTime type.
    /// </remarks>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator JsonValue(DateTime? value)
    {
        if (value is null)
        {
            return Null;
        }

        return new JsonValue(value.Value.ToString("o"));
    }

    /// <summary>
    /// Converts the given JsonValue into an Int.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator int(JsonValue jsonValue)
    {
        if (jsonValue.IsInteger)
        {
            return jsonValue.AsInteger;
        }

        return 0;
    }

    /// <summary>
    /// Converts the given JsonValue into a nullable Int.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    /// <exception cref="System.InvalidCastException">
    /// Throws System.InvalidCastException when the inner value type of the
    /// JsonValue is not the desired type of the conversion.
    /// </exception>
    public static implicit operator int?(JsonValue jsonValue)
    {
        if (jsonValue.IsNull)
        {
            return null;
        }

        return (int) jsonValue;
    }

    /// <summary>
    /// Converts the given JsonValue into a Bool.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator bool(JsonValue jsonValue)
    {
        if (jsonValue.IsBoolean)
        {
            return jsonValue._value == 1;
        }

        return false;
    }

    /// <summary>
    /// Converts the given JsonValue into a nullable Bool.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    /// <exception cref="System.InvalidCastException">
    /// Throws System.InvalidCastException when the inner value type of the
    /// JsonValue is not the desired type of the conversion.
    /// </exception>
    public static implicit operator bool?(JsonValue jsonValue)
    {
        if (jsonValue.IsNull)
        {
            return null;
        }

        return (bool) jsonValue;
    }

    /// <summary>
    /// Converts the given JsonValue into a Double.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator double(JsonValue jsonValue)
    {
        if (jsonValue.IsNumber)
        {
            return jsonValue._value;
        }

        return double.NaN;
    }

    /// <summary>
    /// Converts the given JsonValue into a nullable Double.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    /// <exception cref="System.InvalidCastException">
    /// Throws System.InvalidCastException when the inner value type of the
    /// JsonValue is not the desired type of the conversion.
    /// </exception>
    public static implicit operator double?(JsonValue jsonValue)
    {
        if (jsonValue.IsNull)
        {
            return null;
        }

        return (double) jsonValue;
    }

    /// <summary>
    /// Converts the given JsonValue into a String.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator string(JsonValue jsonValue)
    {
        if (jsonValue.IsString || jsonValue.IsNull)
        {
            return jsonValue._reference as string;
        }

        return null;
    }

    /// <summary>
    /// Converts the given JsonValue into a JsonObject.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator JsonObject(JsonValue jsonValue)
    {
        if (jsonValue.IsJsonObject || jsonValue.IsNull)
        {
            return jsonValue._reference as JsonObject;
        }

        return null;
    }

    /// <summary>
    /// Converts the given JsonValue into a JsonArray.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator JsonArray(JsonValue jsonValue)
    {
        if (jsonValue.IsJsonArray || jsonValue.IsNull)
        {
            return jsonValue._reference as JsonArray;
        }

        return null;
    }

    /// <summary>
    /// Converts the given JsonValue into a DateTime.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator DateTime(JsonValue jsonValue)
    {
        var dateTime = jsonValue.AsDateTime;

        if (dateTime.HasValue)
        {
            return dateTime.Value;
        }

        return DateTime.MinValue;
    }

    /// <summary>
    /// Converts the given JsonValue into a nullable DateTime.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to be converted.</param>
    public static implicit operator DateTime?(JsonValue jsonValue)
    {
        if (jsonValue.IsDateTime || jsonValue.IsNull)
        {
            return jsonValue.AsDateTime;
        }

        return null;
    }

    /// <summary>
    /// Returns a value indicating whether the two given JsonValues are equal.
    /// </summary>
    /// <param name="a">A JsonValue to compare.</param>
    /// <param name="b">A JsonValue to compare.</param>
    public static bool operator ==(JsonValue a, JsonValue b)
    {
        return a.Type == b.Type
            && a._value == b._value
            && Equals(a._reference, b._reference);
    }

    /// <summary>
    /// Returns a value indicating whether the two given JsonValues are unequal.
    /// </summary>
    /// <param name="a">A JsonValue to compare.</param>
    /// <param name="b">A JsonValue to compare.</param>
    public static bool operator !=(JsonValue a, JsonValue b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Returns a JsonValue by parsing the given string.
    /// </summary>
    /// <param name="text">The JSON-formatted string to be parsed.</param>
    public static JsonValue Parse(string text)
    {
        return JsonReader.Parse(text);
    }

    /// <summary>
    /// Returns a value indicating whether this JsonValue is equal to the given object.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return IsNull;
        }

        var jsonValue = obj as JsonValue?;

        if (jsonValue.HasValue)
        {
            return this == jsonValue.Value;
        }

        return false;
    }

    /// <summary>
    /// Returns a hash code for this JsonValue.
    /// </summary>
    public override int GetHashCode()
    {
        if (IsNull)
        {
            return Type.GetHashCode();
        }

        return Type.GetHashCode()
            ^ _value.GetHashCode()
            ^ EqualityComparer<object>.Default.GetHashCode(_reference);
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

    private class JsonValueDebugView
    {
        private readonly JsonValue _jsonValue;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public JsonObject ObjectView
        {
            get
            {
                if (_jsonValue.IsJsonObject)
                {
                    return (JsonObject) _jsonValue._reference;
                }

                return null;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public JsonArray ArrayView
        {
            get
            {
                if (_jsonValue.IsJsonArray)
                {
                    return (JsonArray) _jsonValue._reference;
                }

                return null;
            }
        }

        public JsonValueType Type => _jsonValue.Type;

        public object Value
        {
            get
            {
                if (_jsonValue.IsJsonObject)
                {
                    return (JsonObject) _jsonValue._reference;
                }

                if (_jsonValue.IsJsonArray)
                {
                    return (JsonArray) _jsonValue._reference;
                }

                return _jsonValue;
            }
        }

        public JsonValueDebugView(JsonValue jsonValue)
        {
            _jsonValue = jsonValue;
        }
    }
}
