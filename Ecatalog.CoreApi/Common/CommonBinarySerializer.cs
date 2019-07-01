using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ecatalog.CoreApi.Common
{
    /// <summary>
    /// Provides Serialize and Deserialize methods for Binary format
    /// </summary>
    internal class CommonBinarySerializer
    {
        /// <summary>
        /// Binary formatter object
        /// </summary>
        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        /// <summary>
        /// Serialize object of T class to byte array
        /// </summary>
        /// <typeparam name="T">Type of serialized data</typeparam>
        /// <param name="forSerializeObj">Serialized object</param>
        /// <returns></returns>
        public byte[] Serialize<T>(T forSerializeObj)
        {
            using (var stream = new MemoryStream())
            {
                _formatter.Serialize(stream, forSerializeObj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize byte array to T class object
        /// </summary>
        /// <typeparam name="T">Type of deserialized data</typeparam>
        /// <param name="serialized">Byte array for deserialize</param>
        /// <returns>Deserialized object of class T</returns>
        public T Deserialize<T>(byte[] serialized)
        {
            using (var stream = new MemoryStream(serialized))
            {
                var result = (T)_formatter.Deserialize(stream);
                return result;
            }
        }
    }
}
