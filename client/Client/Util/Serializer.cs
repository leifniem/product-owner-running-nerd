using Newtonsoft.Json;
using System;

namespace LoadRunnerClient
{
	/// <summary>
	/// Serializes and Deserializes Objects to and from a JSON-String
	/// Uses <see cref="JsonConvert"/>
	/// </summary>
    public class Serializer
    {
		/// <summary>
		/// Serializes object to JSON-String
		/// </summary>
		/// <param name="obj">object to be serialized</param>
		/// <returns>serialized <paramref name="obj"/></returns>
        public static string Serialize(Object obj)

        {
            return JsonConvert.SerializeObject(obj);
        }

		/// <summary>
		/// Deserializes string to given Object <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Type of object that is deserialized</typeparam>
		/// <param name="str">incoming string to be deserialized</param>
		/// <returns>deserialized object of type <typeparamref name="T"/></returns>
        public static T Deserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}