using System;
using System.Collections.Generic;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    public class DtranxTranslationProvider : ITranslationProvider
    {
        public static readonly string DtranxTranslationProviderScheme = "dtranxprovider";

        private DtranxTranslationOptions _options;
        private ITranslationProviderCredentialStore _credentialStore;
        private DtranxCredentials _credentials;

        #region "Constructors"

        /// <summary>
        /// 使用凭据存储的构造函数（推荐）
        /// </summary>
        public DtranxTranslationProvider(DtranxTranslationOptions options, ITranslationProviderCredentialStore credentialStore)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _credentialStore = credentialStore;

            // 从凭据存储加载敏感信息
            LoadCredentials();
        }

        /// <summary>
        /// 向后兼容的构造函数
        /// </summary>
        public DtranxTranslationProvider(DtranxTranslationOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _credentialStore = null;
            _credentials = new DtranxCredentials();
        }

        #endregion

        #region "Properties"

        public DtranxTranslationOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        public DtranxCredentials Credentials
        {
            get { return _credentials; }
        }

        public ITranslationProviderCredentialStore CredentialStore
        {
            get { return _credentialStore; }
        }

        #endregion

        #region "Configuration Management"

        /// <summary>
        /// 加载凭据信息
        /// </summary>
        private void LoadCredentials()
        {
            _credentials = new DtranxCredentials(); // 先初始化为空凭据

            if (_credentialStore != null && _options != null)
            {
                try
                {
                    // 尝试从特定URI加载凭据
                    _credentials = DtranxCredentialManager.GetCredentials(_credentialStore, _options.Uri);

                    // 如果没有找到特定URI的凭据，尝试使用默认凭据
                    if (!_credentials.IsValid())
                    {
                        Uri defaultUri = new Uri($"{DtranxTranslationProviderScheme}://default");
                        _credentials = DtranxCredentialManager.GetCredentials(_credentialStore, defaultUri);
                    }

                    // 如果还是没有找到，尝试从全局设置加载（向后兼容）
                    if (!_credentials.IsValid() && DtranxSettingsManager.HasSavedSettings())
                    {
                        var tempOptions = new DtranxTranslationOptions();
                        DtranxSettingsManager.LoadFromRegistry(tempOptions);

                        if (!string.IsNullOrEmpty(tempOptions.AppKey))
                        {
                            _credentials.AppKey = tempOptions.AppKey;
                            _credentials.SignKey = tempOptions.SignKey;
                            _credentials.Phone = tempOptions.Phone;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load credentials: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Credentials loaded - Valid: {_credentials?.IsValid() ?? false}");
        }


        /// <summary>
        /// 刷新凭据信息
        /// </summary>
        public void RefreshCredentials()
        {
            LoadCredentials();
        }

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        public bool IsConfigurationValid()
        {
            if (_options == null)
            {
                System.Diagnostics.Debug.WriteLine("Options is null");
                return false;
            }

            // 检查凭据是否有效
            if (_credentials == null || !_credentials.IsValid())
            {
                System.Diagnostics.Debug.WriteLine("Credentials invalid, attempting to reload...");

                // 尝试重新加载凭据
                LoadCredentials();

                if (_credentials == null || !_credentials.IsValid())
                {
                    System.Diagnostics.Debug.WriteLine("Invalid or missing credentials after reload");
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine("Configuration validation passed");
            return true;
        }

        /// <summary>
        /// 获取详细的配置状态信息
        /// </summary>
        public string GetConfigurationStatus()
        {
            if (_options == null)
                return "配置对象为空";

            if (_credentials == null)
                return "凭据对象为空";

            if (string.IsNullOrEmpty(_credentials.AppKey))
                return "AppKey缺失";

            if (string.IsNullOrEmpty(_credentials.SignKey))
                return "SignKey缺失";

            if (string.IsNullOrEmpty(_credentials.Phone))
                return "Phone缺失";

            return "配置完整";
        }

        #endregion

        #region ITranslationProvider Members

        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        public bool SupportsMultipleResults
        {
            get { return false; }
        }

        public bool SupportsFilters
        {
            get { return false; }
        }

        public bool SupportsPenalties
        {
            get { return true; }
        }

        public bool SupportsStructureContext
        {
            get { return false; }
        }

        public bool SupportsDocumentSearches
        {
            get { return false; }
        }

        public bool SupportsUpdate
        {
            get { return false; }
        }

        public bool SupportsPlaceables
        {
            get { return false; }
        }

        public bool SupportsTranslation
        {
            get { return true; }
        }

        public bool SupportsFuzzySearch
        {
            get { return false; }
        }

        public bool SupportsConcordanceSearch
        {
            get { return false; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return false; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return false; }
        }

        public bool SupportsWordCounts
        {
            get { return false; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return DtranxTranslationOptions.ProviderTranslationMethod; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void LoadState(string translationProviderState)
        {
            // Not needed for this implementation
            // 可以在这里加载序列化的状态信息
        }

        public void RefreshStatusInfo()
        {
            // 刷新配置和凭据状态
            LoadCredentials();

            if (!IsConfigurationValid())
            {
                System.Diagnostics.Debug.WriteLine($"Dtranx Provider: Configuration is invalid - {GetConfigurationStatus()}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Dtranx Provider: Configuration is valid");
            }
        }

        public string SerializeState()
        {
            // 可以在这里序列化非敏感的状态信息
            // 敏感信息不应该序列化到状态中
            return null;
        }

        public ProviderStatusInfo StatusInfo
        {
            get
            {
                bool isAvailable = IsConfigurationValid();
                string message = isAvailable ?
                    "数译翻译提供者 - 配置正常" :
                    $"数译翻译提供者 - {GetConfigurationStatus()}";

                return new ProviderStatusInfo(isAvailable, message);
            }
        }

        public bool SupportsTaggedInput
        {
            get { return true; }
        }

        public bool SupportsScoring
        {
            get { return false; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            if (languageDirection == null)
            {
                return false;
            }

            // 基本语言支持检查（不依赖凭据）
            if (!IsLanguagePairSupported(languageDirection))
            {
                System.Diagnostics.Debug.WriteLine($"Language pair not supported: {languageDirection.SourceCultureName} -> {languageDirection.TargetCultureName}");
                return false;
            }

            // 配置检查放在后面，允许语言对检查通过
            if (!IsConfigurationValid())
            {
                System.Diagnostics.Debug.WriteLine($"Configuration invalid but language pair supported: {GetConfigurationStatus()}");
                // 返回 true 让用户可以配置，而不是直接拒绝
                return true;
            }

            return true;
        }

        /// <summary>
        /// 检查语言对是否被数译API支持
        /// </summary>
        private bool IsLanguagePairSupported(LanguagePair languageDirection)
        {
            // 根据数译API文档中的支持语言列表进行检查
            var supportedLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "zh-CN", "zh-TW", "zh-HK", "en-US", "en-GB", "ja-JP", "ko-KR",
        "de-DE", "fr-FR", "es-ES", "pt-PT", "ru-RU", "it-IT", "vi-VN",
        "th-TH", "ar-SA", "hi-IN", "id-ID", "ms-MY", "tr-TR", "pl-PL",
        "nl-NL", "cs-CZ", "sv-SE", "da-DK", "fi-FI", "no-NO", "hu-HU",
        "el-GR", "ro-RO", "bg-BG", "hr-HR", "sk-SK", "sl-SI", "et-EE",
        "lv-LV", "lt-LT", "uk-UA", "he-IL"
    };

            string sourceCode = languageDirection.SourceCultureName;
            string targetCode = languageDirection.TargetCultureName;

            // 检查源语言和目标语言是否都支持
            bool sourceSupported = supportedLanguages.Contains(sourceCode) ||
                                  sourceCode.StartsWith("zh") || sourceCode.StartsWith("en");
            bool targetSupported = supportedLanguages.Contains(targetCode) ||
                                  targetCode.StartsWith("zh") || targetCode.StartsWith("en");

            return sourceSupported && targetSupported;
        }

        public Uri Uri
        {
            get { return _options?.Uri; }
        }

        public string Name
        {
            get { return "DtranX LanMT Translation Provider"; }
        }

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            if (languageDirection == null)
            {
                throw new ArgumentNullException(nameof(languageDirection));
            }

            // 先检查语言对支持
            if (!IsLanguagePairSupported(languageDirection))
            {
                throw new NotSupportedException($"不支持的语言对: {languageDirection.SourceCultureName} -> {languageDirection.TargetCultureName}");
            }

            // 再检查配置
            if (!IsConfigurationValid())
            {
                string status = GetConfigurationStatus();
                throw new InvalidOperationException($"数译API配置信息不完整：{status}。请在翻译提供者设置中配置API凭据。");
            }

            try
            {
                return new DtranxTranslationProviderLanguageDirection(this, languageDirection);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create language direction: {ex.Message}");
                throw new InvalidOperationException($"无法创建语言方向实例：{ex.Message}", ex);
            }
        }


        #endregion

        #region "Dispose Pattern"

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 清理托管资源
                _credentials = null;
                _credentialStore = null;
                _options = null;
            }
        }

        #endregion

        #region "Debug and Utility Methods"

        /// <summary>
        /// 获取调试信息
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Provider: {Name}\n" +
                   $"URI: {Uri}\n" +
                   $"Configuration Status: {GetConfigurationStatus()}\n" +
                   $"Has CredentialStore: {_credentialStore != null}\n" +
                   $"Credentials Valid: {_credentials?.IsValid() ?? false}";
        }

        #endregion
    }
}