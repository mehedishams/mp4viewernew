using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace mp4viewernew.Utilities
{
    public static class FileHelpers
    {
        public static bool FileLengthInAcceptableRange(IFormFile formFile, long sizeLimit)
        {
            return formFile.Length > sizeLimit * 1024 * 1024;
        }

        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".mp4", new List<byte[]> { new byte[] { 0x66, 0x74, 0x79, 0x70, 0x6D, 0x6D, 0x70, 0x34 } } }
        };

        public async static Task<bool> ValidMp4File(
            IFormFile formFile,
            string[] permittedExtensions
        )
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);

                    if (memoryStream.Length == 0)
                    {
                        return false;
                    }

                    return (ValidFileExtensionAndSignature(formFile.FileName, memoryStream, permittedExtensions));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static bool ValidFileExtensionAndSignature(string fileName, Stream data, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            data.Position = 0;


            using (var reader = new BinaryReader(data))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
