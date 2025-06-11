using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    /// <summary>
    /// 凭据管理器 - 使用Trados凭据存储
    /// </summary>
    public static class DtranxCredentialManager
    {
        /// <summary>
        /// 保存凭据到Trados凭据存储
        /// </summary>
        public static void SaveCredentials(ITranslationProviderCredentialStore credentialStore,
            Uri providerUri, string appKey, string signKey, string phone)
        {
            if (credentialStore == null) return;

            try
            {
                // 创建凭据对象
                var credentialData = new CredentialData
                {
                    AppKey = appKey ?? "",
                    SignKey = signKey ?? "",
                    Phone = phone ?? ""
                };

                string credentialString = JsonConvert.SerializeObject(credentialData);

                // 创建凭据对象
                var credential = new TranslationProviderCredential(credentialString, true);

                // 保存到Trados的安全凭据存储
                credentialStore.AddCredential(providerUri, credential);

                System.Diagnostics.Debug.WriteLine("Credentials saved to secure store");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save credentials: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 从Trados凭据存储获取凭据
        /// </summary>
        public static DtranxCredentials GetCredentials(ITranslationProviderCredentialStore credentialStore,
            Uri providerUri)
        {
            if (credentialStore == null)
                return new DtranxCredentials();

            try
            {
                var credential = credentialStore.GetCredential(providerUri);

                if (credential != null && !string.IsNullOrEmpty(credential.Credential))
                {
                    // 使用强类型反序列化，避免dynamic
                    var credentialData = JsonConvert.DeserializeObject<CredentialData>(credential.Credential);

                    if (credentialData != null)
                    {
                        return new DtranxCredentials
                        {
                            AppKey = credentialData.AppKey ?? "",
                            SignKey = credentialData.SignKey ?? "",
                            Phone = credentialData.Phone ?? ""
                        };
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"JSON deserialization error: {jsonEx.Message}");
                // 尝试作为旧格式处理
                return TryParseOldFormat(credentialStore, providerUri);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load credentials: {ex.Message}");
            }

            return new DtranxCredentials();
        }

        /// <summary>
        /// 尝试解析旧格式的凭据（兼容性处理）
        /// </summary>
        private static DtranxCredentials TryParseOldFormat(ITranslationProviderCredentialStore credentialStore, Uri providerUri)
        {
            try
            {
                var credential = credentialStore.GetCredential(providerUri);
                if (credential?.Credential != null)
                {
                    // 尝试手动解析JSON（避免使用dynamic）
                    string json = credential.Credential;

                    // 简单的JSON解析（针对我们的格式）
                    var credentials = ParseCredentialJson(json);
                    if (credentials != null)
                    {
                        return credentials;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to parse old format: {ex.Message}");
            }

            return new DtranxCredentials();
        }

        /// <summary>
        /// 手动解析凭据JSON（避免dynamic）
        /// </summary>
        private static DtranxCredentials ParseCredentialJson(string json)
        {
            try
            {
                // 使用Newtonsoft.Json的JObject来解析，避免dynamic
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);

                return new DtranxCredentials
                {
                    AppKey = jObject["AppKey"]?.ToString() ?? "",
                    SignKey = jObject["SignKey"]?.ToString() ?? "",
                    Phone = jObject["Phone"]?.ToString() ?? ""
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 检查是否有保存的凭据
        /// </summary>
        public static bool HasCredentials(ITranslationProviderCredentialStore credentialStore, Uri providerUri)
        {
            if (credentialStore == null) return false;

            try
            {
                var credential = credentialStore.GetCredential(providerUri);
                return credential != null && !string.IsNullOrEmpty(credential.Credential);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除保存的凭据
        /// </summary>
        public static void ClearCredentials(ITranslationProviderCredentialStore credentialStore, Uri providerUri)
        {
            if (credentialStore == null) return;

            try
            {
                credentialStore.RemoveCredential(providerUri);
                System.Diagnostics.Debug.WriteLine("Credentials cleared from secure store");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to clear credentials: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 凭据数据的序列化类（用于JSON序列化/反序列化）
    /// </summary>
    [Serializable]
    public class CredentialData
    {
        public string AppKey { get; set; } = "";
        public string SignKey { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}