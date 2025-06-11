using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    /// <summary>
    /// 管理全局设置的存储和读取
    /// </summary>
    public static class DtranxSettingsManager
    {
        // 使用注册表存储（推荐）
        private const string REGISTRY_KEY = @"SOFTWARE\DtranxTranslationProvider";

        // 或使用文件存储
        private static readonly string SETTINGS_FILE = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DtranxTranslationProvider",
            "settings.xml"
        );

        #region 注册表方式（推荐）

        /// <summary>
        /// 保存设置到注册表
        /// </summary>
        public static void SaveToRegistry(DtranxTranslationOptions options)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("AppKey", options.AppKey ?? "");
                        key.SetValue("SignKey", options.SignKey ?? "");
                        key.SetValue("Phone", options.Phone ?? "");
                        key.SetValue("Domain", options.Domain ?? "general");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从注册表读取设置
        /// </summary>
        public static void LoadFromRegistry(DtranxTranslationOptions options)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        options.AppKey = key.GetValue("AppKey") as string ?? "";
                        options.SignKey = key.GetValue("SignKey") as string ?? "";
                        options.Phone = key.GetValue("Phone") as string ?? "";
                        options.Domain = key.GetValue("Domain") as string ?? "general";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"读取设置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查是否有保存的设置
        /// </summary>
        public static bool HasSavedSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        string appKey = key.GetValue("AppKey") as string;
                        return !string.IsNullOrEmpty(appKey);
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        #endregion

        #region 清除设置方法

        /// <summary>
        /// 清除注册表中保存的所有设置
        /// </summary>
        public static void ClearSavedSettings()
        {
            ClearRegistrySettings();
            ClearFileSettings(); // 同时清除文件设置（如果存在）
        }

        /// <summary>
        /// 清除注册表中的设置
        /// </summary>
        public static void ClearRegistrySettings()
        {
            try
            {
                // 删除整个注册表键
                Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_KEY, false);
                System.Diagnostics.Debug.WriteLine("注册表设置已清除");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清除注册表设置失败: {ex.Message}");
                throw; // 重新抛出异常以便上层处理
            }
        }

        /// <summary>
        /// 清除文件中的设置
        /// </summary>
        public static void ClearFileSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    File.Delete(SETTINGS_FILE);
                    System.Diagnostics.Debug.WriteLine("文件设置已清除");
                }

                // 如果目录为空，也删除目录
                string directory = Path.GetDirectoryName(SETTINGS_FILE);
                if (Directory.Exists(directory) && Directory.GetFiles(directory).Length == 0)
                {
                    Directory.Delete(directory);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清除文件设置失败: {ex.Message}");
                // 文件清除失败不抛出异常，因为主要使用注册表
            }
        }

        /// <summary>
        /// 检查是否存在任何保存的设置（注册表或文件）
        /// </summary>
        public static bool HasAnySavedSettings()
        {
            return HasSavedSettings() || HasSavedFileSettings();
        }

        /// <summary>
        /// 检查是否有保存的文件设置
        /// </summary>
        public static bool HasSavedFileSettings()
        {
            return File.Exists(SETTINGS_FILE);
        }

        #endregion

        #region 文件方式（备选）

        /// <summary>
        /// 保存设置到文件
        /// </summary>
        public static void SaveToFile(DtranxTranslationOptions options)
        {
            try
            {
                // 创建目录
                string directory = Path.GetDirectoryName(SETTINGS_FILE);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 创建设置对象
                var settings = new DtranxSettings
                {
                    AppKey = options.AppKey,
                    SignKey = options.SignKey,
                    Phone = options.Phone,
                    Domain = options.Domain
                };

                // 序列化到文件
                XmlSerializer serializer = new XmlSerializer(typeof(DtranxSettings));
                using (FileStream fs = new FileStream(SETTINGS_FILE, FileMode.Create))
                {
                    serializer.Serialize(fs, settings);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存设置到文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从文件读取设置
        /// </summary>
        public static void LoadFromFile(DtranxTranslationOptions options)
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DtranxSettings));
                    using (FileStream fs = new FileStream(SETTINGS_FILE, FileMode.Open))
                    {
                        var settings = (DtranxSettings)serializer.Deserialize(fs);
                        options.AppKey = settings.AppKey;
                        options.SignKey = settings.SignKey;
                        options.Phone = settings.Phone;
                        options.Domain = settings.Domain;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从文件读取设置失败: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// 设置数据类
    /// </summary>
    [Serializable]
    public class DtranxSettings
    {
        public string AppKey { get; set; }
        public string SignKey { get; set; }
        public string Phone { get; set; }
        public string Domain { get; set; }
    }
}