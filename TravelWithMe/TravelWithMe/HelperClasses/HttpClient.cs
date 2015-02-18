using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Web;
using Microsoft.Win32;

namespace TravelWithMe.HelperClasses
{
    public class HttpClient
    {
        public string Url { get; private set; }
        public ContentType MimeType;
        public IFormatter Formatter { get; private set; }

        public HttpClient(string url, ContentType contentType)
        {
            Url = url;
            MimeType = contentType;
            switch (contentType)
            {
                case ContentType.Json:
                    Formatter = new WcfFormatter<DataContractJsonSerializer>();
                    break;
                case ContentType.Xml:
                    Formatter = new WcfFormatter<DataContractSerializer>();
                    break;
                default:
                    Formatter = new WcfFormatter<DataContractJsonSerializer>();
                    break;
            }
        }

        public TRs Get<TRs>()
            where TRs : class
        {
            var request = WebRequest.Create(Url);
            request.Method = "GET";
            request.Timeout = 1000 * 60 * 5;
            using (var response = request.GetResponse())
            {
                if (response != null)
                    using (var dataStream = response.GetResponseStream())
                    {
                        return Formatter.Deserialize(typeof(TRs), dataStream) as TRs;
                    }
            }
            return null;
        }

        public TRs Post<TRq, TRs>(TRq request)
            where TRq : class
            where TRs : class
        {
            var webRequest = WebRequest.Create(Url);
            webRequest.Method = "POST";
            webRequest.Timeout = 1000 * 60 * 5;
            webRequest.ContentType = MimeType == ContentType.Json ? "application/json; charset=utf-8" : "application/xml; charset=utf-8";
            if (request != null)
            {
                using (var inputStream = webRequest.GetRequestStream())
                {
                    Formatter.Serialize(inputStream, request);
                }
            }
            using (var response = webRequest.GetResponse())
            {
                if (response != null)
                    using (var outputStream = response.GetResponseStream())
                    {
                        return Formatter.Deserialize(typeof(TRs), outputStream) as TRs;
                    }
            }
            return null;
        }

        public TRs Put<TRq, TRs>(TRq request)
            where TRq : class
            where TRs : class
        {
            var webRequest = WebRequest.Create(Url);
            webRequest.Method = "PUT";
            webRequest.ContentType = MimeType == ContentType.Json ? "application/json; charset=utf-8" : "application/xml; charset=utf-8";
            if (request != null)
            {
                using (var inputStream = webRequest.GetRequestStream())
                {
                    Formatter.Serialize(inputStream, request);
                }
            }
            using (var response = webRequest.GetResponse())
            {
                if (response != null)
                    using (var outputStream = response.GetResponseStream())
                    {
                        return Formatter.Deserialize(typeof(TRs), outputStream) as TRs;
                    }
            }
            return null;
        }


        public T Post<T>()
            where T : class
        {
            return Post<object, T>(null);
        }

        public T Put<T>()
            where T : class
        {
            return Put<object, T>(null);
        }

        public T Delete<T>()
            where T : class
        {
            return Delete<object, T>(null);
        }

        public TRs Delete<TRq, TRs>(TRq request)
            where TRq : class
            where TRs : class
        {
            var webRequest = WebRequest.Create(Url);
            webRequest.Method = "DELETE";
            webRequest.ContentType = MimeType == ContentType.Json ? "application/json; charset=utf-8" : "application/xml; charset=utf-8";
            if (request != null)
            {
                using (var inputStream = webRequest.GetRequestStream())
                {
                    Formatter.Serialize(inputStream, request);
                }
            }
            using (var response = webRequest.GetResponse())
            {
                if (response != null)
                    using (var outputStream = response.GetResponseStream())
                    {
                        return Formatter.Deserialize(typeof(TRs), outputStream) as TRs;
                    }
            }
            return null;
        }

        public T Upload<T>(string url)
            where T : class
        {
            bool islocalFile = false;
            string outFile = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(url))
                    return null;
                File img = new File();
                if (url.Contains("http"))
                {
                    img = DownloadImage(url);
                    outFile = UploadToFileSystem(img);
                }
                else
                {
                    string fileName = url.Substring(url.LastIndexOf('\\') + 1, (url.Length - url.LastIndexOf('\\') - 1));
                    string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
                    if (rk != null && rk.GetValue("Content Type") != null)
                        img.ContentType = rk.GetValue("Content Type").ToString();

                    outFile = url;
                    islocalFile = true;
                }

                WebClient client = new WebClient();
                client.Headers["Content-Type"] = img.ContentType;


                var response = client.UploadFile(this.Url, outFile);

                if (response != null)
                {
                    Stream stream = new MemoryStream(response);
                    return Formatter.Deserialize(typeof(T), stream) as T;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(outFile) && !islocalFile)
                    System.IO.File.Delete(outFile);
            }


        }

        private static File DownloadImage(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            WebRequest httpWebRequest = (WebRequest)WebRequest.Create(url);
            var webResponse = httpWebRequest.GetResponse();
            var webStream = webResponse.GetResponseStream();
            string fileName = Guid.NewGuid().ToString() + GetDefaultExtension(webResponse.ContentType);
            //httpWebRequest.Address.Segments[httpWebRequest.Address.Segments.Length - 1];
            File img = new File()
            {
                FileName = fileName
                ,
                ContentType = webResponse.ContentType
                ,
                ContentLength = webResponse.ContentLength
                ,
                Content = webStream
            };

            return img;
        }

        internal class File
        {
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public long ContentLength { get; set; }
            public Stream Content { get; set; }
        }

        private string UploadToFileSystem(File file)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (file == null)
                return string.Empty;

            int bytesRead = 0;
            var buffer = new byte[1024];
            string outFile = basePath.EndsWith(@"\") ? string.Format(@"{0}{1}", basePath, file.FileName) : string.Format(@"{0}\{1}", basePath, file.FileName);
            using (var output = new FileStream(outFile, FileMode.Create, FileAccess.Write))
            {
                do
                {
                    bytesRead = file.Content.Read(buffer, 0, 1024);
                    if (bytesRead > 0)
                        output.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);
                output.Close();
            }

            return outFile;
        }

        private static string GetDefaultExtension(string mimeType)
        {
            try
            {
                string result;
                RegistryKey key;
                object value;

                key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
                value = key != null ? key.GetValue("Extension", null) : null;
                result = value != null ? value.ToString() : ".jpg";

                return result;
            }
            catch (Exception)
            {
                return ".bin";
            }

        }
    }

    public enum ContentType
    {
        Json,
        Xml,
        File
    }

    public interface IFormatter
    {
        void Serialize<T>(Stream stream, T o);

        void Serialize(Stream stream, object o);

        object Deserialize(Type returnType, Stream stream);

    }

    public class WcfFormatter<T> : IFormatter
        where T : XmlObjectSerializer
    {
        private static readonly Dictionary<Type, T> Cache = new Dictionary<Type, T>();
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static T GetSerializer(Type type)
        {
            T serializer;
            Locker.EnterUpgradeableReadLock();
            try
            {
                if (Cache.TryGetValue(type, out serializer) == false)
                {
                    Locker.EnterWriteLock();
                    try
                    {
                        if (Cache.TryGetValue(type, out serializer) == false)
                        {
                            serializer = Activator.CreateInstance(typeof(T), new object[] { type }) as T;
                            Cache[type] = serializer;
                        }
                    }
                    finally
                    {
                        Locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                Locker.ExitUpgradeableReadLock();
            }
            return serializer;
        }
        public static readonly byte[] Empty = new byte[0];

        public void Serialize(Stream stream, object o)
        {
            var serializer = GetSerializer(o.GetType());
            serializer.WriteObject(stream, o);
        }

        public object Deserialize(Type returnType, Stream stream)
        {
            var serializer = GetSerializer(returnType);
            return serializer.ReadObject(stream);
        }

        public void Serialize<TObject>(Stream stream, TObject o)
        {
            var serializer = GetSerializer(typeof(TObject));
            serializer.WriteObject(stream, o);
        }
    }
}