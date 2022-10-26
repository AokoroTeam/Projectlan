using System;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using System.Threading;


namespace Realit
{
    public static class RealitCompressor
    {
        public static byte[] CompressGzip(byte[] bytes, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var compressStream = new GZipStream(outputStream, compressionLevel))
                    compressStream.Write(bytes, 0, bytes.Length);

                return outputStream.ToArray();
            }
        }
        public static byte[] DecompressGzip(byte[] bytes)
        {
            using (var inputStream = new MemoryStream())
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }

        public static byte[] CompressBrotli(byte[] bytes, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var compressStream = new BrotliStream(outputStream, compressionLevel))
                    compressStream.Write(bytes, 0, bytes.Length);

                return outputStream.ToArray();
            }
        }

        public static byte[] DecompressBrotli(byte[] bytes)
        {
            using (var inputStream = new MemoryStream(bytes))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new BrotliStream(inputStream, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }
    }
}