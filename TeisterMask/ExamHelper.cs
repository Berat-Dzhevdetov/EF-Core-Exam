using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask
{
    public static class ExamHelper
    {

        /// <summary>
        /// Recives a dto class and make the class to XML string
        /// </summary>
        /// <typeparam name="T">Type of dto(need to be class)</typeparam>
        /// <param name="dto">Real DataTransferObject</param>
        /// <param name="rootName">Root name you want</param>
        /// <returns>Xml string from given dto</returns>
        public static string FromDTOToStringXML<T>(T dto, string rootName)
            where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);
            XmlSerializerNamespaces nameSpace = new XmlSerializerNamespaces();

            StringWriter sw = new StringWriter();
            nameSpace.Add("", "");

            xmlSerializer.Serialize(sw, dto, nameSpace);
            string result = sw.ToString().TrimEnd();
            return result;
        }
        /// <summary>
        /// Receives a string and class that you need to desrialize to
        /// </summary>
        /// <typeparam name="T">Type you want to desrialize</typeparam>
        /// <param name="xmlObjectsAsString">Xml string with your objects</param>
        /// <param name="xmlRootAttributeName">How you want to call your root</param>
        /// <returns>List with entities of your dto</returns>
        public static List<T> FromStringXMLToClass<T>(string xmlObjectsAsString, string xmlRootAttributeName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute(xmlRootAttributeName));

            var dataTransferObjects = serializer.Deserialize(new StringReader(xmlObjectsAsString)) as List<T>;

            return dataTransferObjects;
        }
        /// <summary>
        /// From string make to the given class
        /// </summary>
        /// <typeparam name="T">To what you want to deserialize</typeparam>
        /// <param name="json">Json string with your objects</param>
        /// <returns>List of entities with given type</returns>
        public static List<T> FromStringJSONToClass<T>(string json)
        {
            var deserializer = JsonConvert.DeserializeObject<List<T>>(json);
            return deserializer;
        }
        /// <summary>
        /// Receives dto class and make it to string
        /// </summary>
        /// <param name="dto">Data Transfer Object</param>
        /// <returns>Json string with your objs</returns>
        public static string FromDTOToJSONString(object dto)
        {
            var serializer = JsonConvert.SerializeObject(dto,Formatting.Indented);
            return serializer;
        }
        /// <summary>
        /// Receives dto class and make it to string
        /// </summary>
        /// <param name="dto">Data Transfer Object</param>
        /// <param name="format">What you like to be your format</param>
        /// <returns>Json string with your objs</returns>
        public static string FromDTOToJSONString(object dto, Formatting format)
        {
            var serializer = JsonConvert.SerializeObject(dto, format);
            return serializer;
        }
        /// <summary>
        /// Receives string value and tries to parse it to your enum
        /// if cannot find your enum property will throw an exception
        /// </summary>
        /// <typeparam name="T">Your enum type</typeparam>
        /// <param name="value">Value you want to parse</param>
        /// <param name="ignoreCase">To ignore case or not; If set to true(default) will ignore case;If set to false will NOT ignore case</param>
        /// <returns>Your enum type</returns>
        public static T FromStringToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// Receives your date as a string and format needed;
        /// if parsed successfully will return your date in DateTime
        /// otherwise will return null
        /// </summary>
        /// <param name="dateTime">Date as a string</param>
        /// <param name="format">Format you need</param>
        /// <returns></returns>
        public static DateTime? CanParseDateTime(string dateTime, string format)
        {

            var canParse = DateTime.TryParseExact(dateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
            if (canParse == false)
            {
                return null;
            }
            return startDate;
        }
        /// <summary>
        /// Receives number and make enum from it
        /// </summary>
        /// <typeparam name="T">Your enum</typeparam>
        /// <param name="number">Number</param>
        /// <returns>Enum of your type</returns>
        public static object FromNumberToEnum<T>(int number)
        {
            var enumValue = Enum.ToObject(typeof(T), number);
            return enumValue;
        }
    }
}
