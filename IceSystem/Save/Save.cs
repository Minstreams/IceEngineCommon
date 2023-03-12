using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IceEngine;
using System;

namespace Ice
{
    public sealed class Save
    {
        #region Path Configuration
        public static string DataPath => System.Environment.CurrentDirectory;
        public static string ToAbsolutePath(string relativePath) => $"{DataPath}\\{relativePath}";
        #endregion

        public static class Json
        {
            public static Encoding Format => Encoding.UTF8;
            readonly static JsonSerializerOptions optionsNormal = new JsonSerializerOptions()
            {
                IgnoreReadOnlyProperties = true,
                IncludeFields = true,
                WriteIndented = false,
            };
            readonly static JsonSerializerOptions optionsPretty = new JsonSerializerOptions()
            {
                IgnoreReadOnlyProperties = true,
                IncludeFields = true,
                WriteIndented = true,
            };

            #region Sync Interface
            // Load
            public static T LoadFromFile<T>(string path, bool absolutePath = false) where T : class
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                if (!File.Exists(path)) return null;
                var json = File.ReadAllText(path, Format);
                return JsonSerializer.Deserialize<T>(json, optionsNormal);
            }

            // Save
            public static void SaveToFile(object data, string path, bool absolutePath = false, bool prettyPrint = false)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var json = JsonSerializer.Serialize(data, data.GetType(), prettyPrint ? optionsPretty : optionsNormal);
                File.WriteAllText(path, json, Format);
            }
            #endregion
        }

        public static class Binary
        {
            #region Sync Interface
            // Load
            public static object LoadFromFile(string path, bool absolutePath = false, int offset = 0, Type baseType = null)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var bts = File.ReadAllBytes(path);
                return IceBinaryUtility.FromBytes(bts, offset, baseType);
            }
            public static void LoadFromFileOverwrite(object objectToOverwrite, string path, bool absolutePath = false, Type type = null, bool withHeader = true, int offset = 0)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var bts = File.ReadAllBytes(path);
                IceBinaryUtility.FromBytesOverwrite(bts, objectToOverwrite, type, withHeader, offset);
            }

            // Save
            public static void SaveToFile(object data, string path, bool absolutePath = false, bool withHeader = true, Type baseType = null)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var bts = IceBinaryUtility.ToBytes(data, withHeader, baseType);
                File.WriteAllBytes(path, bts);
            }
            #endregion

            #region Async Interface
            // Load
            public static async Task<object> LoadFromFileAsync(string path, bool absolutePath = false, int offset = 0, Type baseType = null, CancellationToken cancellationToken = default)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var bts = await File.ReadAllBytesAsync(path, cancellationToken);
                return IceBinaryUtility.FromBytes(bts, offset, baseType);
            }
            public static async void LoadFromFileOverwriteAsync(object objectToOverwrite, string path, bool absolutePath = false, Type type = null, bool withHeader = true, int offset = 0, CancellationToken cancellationToken = default)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var bts = await File.ReadAllBytesAsync(path, cancellationToken);
                IceBinaryUtility.FromBytesOverwrite(bts, objectToOverwrite, type, withHeader, offset);
            }

            // Save
            public static Task SaveToFileAsync(object data, string path, bool absolutePath = false, bool withHeader = true, Type baseType = null, CancellationToken cancellationToken = default)
            {
                if (!absolutePath) path = ToAbsolutePath(path);
                var bts = IceBinaryUtility.ToBytes(data, withHeader, baseType);
                return File.WriteAllBytesAsync(path, bts, cancellationToken);
            }
            #endregion
        }
    }
}
