using System.IO;

namespace Ecatalog.CoreApi.Common
{
    /// <summary>
    /// Provides Serialize and Deserialize methods for XML format
    /// </summary>
    internal class CommonXmlSerializer
    {
        /// <summary>
        /// Serialize object of T class to XML format
        /// </summary>
        /// <typeparam name="T">Type of serialized data</typeparam>
        /// <param name="forSerializeObj">Serialized object</param>
        /// <returns>Xml</returns>
        public string ObjectToString<T>(T forSerializeObj) where T : class
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(forSerializeObj.GetType());
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, forSerializeObj);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Deserialize XML to T class object
        /// </summary>
        /// <typeparam name="T">Type of deserialized data</typeparam>
        /// <param name="serialized">XML for deserialize</param>
        /// <returns>Deserialized object of class T</returns>
        public T StringToObject<T>(string serialized) where T : class
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var stringReader = new StreamReader(serialized))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
