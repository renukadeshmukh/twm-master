using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace TravelWithMe.API.Logging
{
    public static class Serializer
    {
        #region Nested type: JSON

        public static class JSON
        {
            private static readonly TypeCache<DataContractJsonSerializer> Cache =
                new TypeCache<DataContractJsonSerializer>();

            public static string Serialize<T>(T obj) where T : class
            {
                if (obj == null) return string.Empty;
                try
                {
                    DataContractJsonSerializer serializer = Cache.GetState(typeof(T),
                                                                           () =>
                                                                           new DataContractJsonSerializer(typeof(T)));
                    using (var ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, obj);
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static T Deserialize<T>(string jsonResp)
            {
                try
                {
                    if (string.IsNullOrEmpty(jsonResp)) return default(T);
                    DataContractJsonSerializer serializer = Cache.GetState(typeof(T),
                                                                           () =>
                                                                           new DataContractJsonSerializer(typeof(T)));
                    var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonResp)) as Stream;
                    return (T)serializer.ReadObject(memoryStream);
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }

        #endregion

        #region Nested type: TypeCache

        public class TypeCache<T>
        {
            private readonly Dictionary<Type, T> _cache = new Dictionary<Type, T>();
            private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

            public T GetState(Type type, Func<T> createInstance)
            {
                T match;
                _locker.EnterUpgradeableReadLock();
                try
                {
                    if (_cache.TryGetValue(type, out match) == false)
                    {
                        _locker.EnterWriteLock();
                        try
                        {
                            if (_cache.TryGetValue(type, out match) == false)
                            {
                                match = createInstance();
                                _cache[type] = match;
                            }
                        }
                        finally
                        {
                            _locker.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _locker.ExitUpgradeableReadLock();
                }
                return match;
            }
        }

        #endregion

        #region Nested type: Wcf

        public static class Wcf
        {
            private static readonly TypeCache<DataContractSerializer> Cache = new TypeCache<DataContractSerializer>();

            public static string Serialize<T>(T obj) where T : class
            {
                if (obj == null) return string.Empty;
                try
                {
                    DataContractSerializer serializer = Cache.GetState(typeof(T),
                                                                       () => new DataContractSerializer(typeof(T)));
                    using (var writer = new StringWriter())
                    {
                        using (var xw = new XmlTextWriter(writer))
                        {
                            serializer.WriteObject(xw, obj);
                            return writer.ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static T Deserialize<T>(string str)
            {
                try
                {
                    if (string.IsNullOrEmpty(str)) return default(T);
                    DataContractSerializer serializer = Cache.GetState(typeof(T),
                                                                       () => new DataContractSerializer(typeof(T)));
                    using (TextReader txtReader = new StringReader(str))
                    {
                        using (XmlReader xmlReader = new XmlTextReader(txtReader))
                        {
                            return (T)serializer.ReadObject(xmlReader);
                        }
                    }
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }

        #endregion

        #region Nested type: Xml

        public static class Xml
        {
            private static readonly TypeCache<XmlSerializer> Cache = new TypeCache<XmlSerializer>();

            public static string Serialize<T>(T obj) where T : class
            {
                try
                {
                    if (obj == null) return string.Empty;
                    XmlSerializer serializer = Cache.GetState(typeof(T), () => new XmlSerializer(typeof(T)));
                    using (var writer = new StringWriter())
                    {
                        serializer.Serialize(writer, obj);
                        return writer.ToString();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static T Deserialize<T>(string str)
            {
                try
                {
                    if (string.IsNullOrEmpty(str)) return default(T);
                    XmlSerializer serializer = Cache.GetState(typeof(T), () => new XmlSerializer(typeof(T)));
                    return (T)serializer.Deserialize(new StringReader(str));
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }

        #endregion
    }
}
