using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace System
{
    public static class Serializer
    {
        public static T Decompress<T>(byte[] compressedData) where T : class
        {
            using (MemoryStream memory = new MemoryStream(compressedData))
            {
                using (GZipStream zip = new GZipStream(memory, CompressionMode.Decompress, false))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return formatter.Deserialize(zip) as T;
                }
            }
        }


        public static byte[] Compress<T>(T data)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(memory, CompressionMode.Compress, false))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(zip, data);
                }

                return memory.ToArray();
            }
        }


        public static byte[] ToBinary<T>(T data)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(memory, data);
                return memory.ToArray();
            }
        }


        public static T FromBinary<T>(byte[] binary) where T : class
        {
            using (MemoryStream memory = new MemoryStream(binary))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return formatter.Deserialize(memory) as T;
            }
        }
    }
}
