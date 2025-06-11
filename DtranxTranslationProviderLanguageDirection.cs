using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.Globalization;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    public class DtranxTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        private DtranxTranslationProvider _provider;
        private LanguagePair _languageDirection;
        private DtranxTranslationOptions _options;
        private DtranxCredentials _credentials;
        private DtranxTranslationProviderElementVisitor _visitor;

        #region "Constructor"

        public DtranxTranslationProviderLanguageDirection(DtranxTranslationProvider provider, LanguagePair languages)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _languageDirection = languages ?? throw new ArgumentNullException(nameof(languages));
            _options = _provider.Options ?? throw new ArgumentNullException("Provider options cannot be null");
            _credentials = _provider.Credentials; // 从提供者获取凭据
            _visitor = new DtranxTranslationProviderElementVisitor(_options);

            System.Diagnostics.Debug.WriteLine($"LanguageDirection created: {languages.SourceCultureName} -> {languages.TargetCultureName}");
        }

        #endregion

        #region ITranslationProviderLanguageDirection Members

        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return _languageDirection.SourceCulture; }
        }

        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return _languageDirection.TargetCulture; }
        }

        public ITranslationProvider TranslationProvider
        {
            get { return _provider; }
        }

        public bool CanReverseLanguageDirection
        {
            get { return false; }
        }

        #endregion

        #region "Search Methods"

        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            // 添加详细的调试信息
            System.Diagnostics.Debug.WriteLine("=== SearchSegment 开始 ===");
            System.Diagnostics.Debug.WriteLine($"Segment text: {segment?.ToPlain() ?? "null"}");

            // 验证配置
            bool configValid = IsConfigurationValid();
            System.Diagnostics.Debug.WriteLine($"Configuration valid: {configValid}");

            if (!configValid)
            {
                System.Diagnostics.Debug.WriteLine("=== SearchSegment 结束 (配置无效) ===");
                SearchResults emptyResults = new SearchResults();
                emptyResults.SourceSegment = segment.Duplicate();
                return emptyResults;
            }

            // 提取纯文本
            _visitor.Reset();
            foreach (var element in segment.Elements)
            {
                element.AcceptSegmentElementVisitor(_visitor);
            }

            SearchResults results = new SearchResults();
            results.SourceSegment = segment.Duplicate();

            try
            {
                string sourceText = _visitor.PlainText;
                if (string.IsNullOrWhiteSpace(sourceText))
                {
                    System.Diagnostics.Debug.WriteLine("Source text is empty, skipping translation");
                    return results;
                }

                System.Diagnostics.Debug.WriteLine($"Calling translation API with text: {sourceText}");
                string translatedText = TranslateWithDtranxAPI(sourceText);
                System.Diagnostics.Debug.WriteLine($"Translation result: {translatedText}");

                if (!string.IsNullOrEmpty(translatedText))
                {
                    Segment translation = new Segment(_languageDirection.TargetCulture);
                    translation.Add(translatedText);
                    results.Add(CreateSearchResult(segment, translation, sourceText));
                    System.Diagnostics.Debug.WriteLine("Translation result added to SearchResults");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Translation result is empty");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Translation error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // 可以选择是否在界面显示错误，这里只记录日志
            }

            System.Diagnostics.Debug.WriteLine("=== SearchSegment 结束 ===");
            return results;
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));

            SearchResults[] results = new SearchResults[segments.Length];

            // 如果配置无效，返回所有空结果
            if (!IsConfigurationValid())
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    results[i] = new SearchResults();
                    results[i].SourceSegment = segments[i].Duplicate();
                }
                return results;
            }

            // 配置有效，正常处理
            for (int i = 0; i < segments.Length; i++)
            {
                results[i] = SearchSegment(settings, segments[i]);
            }
            return results;
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            if (mask == null || mask.Length != segments.Length)
                throw new ArgumentException("Mask array is null or has incorrect length", nameof(mask));

            SearchResults[] results = new SearchResults[segments.Length];
            for (int i = 0; i < segments.Length; i++)
            {
                if (mask[i])
                {
                    results[i] = SearchSegment(settings, segments[i]);
                }
                else
                {
                    results[i] = null;
                }
            }
            return results;
        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                SearchResults emptyResults = new SearchResults();
                return emptyResults;
            }

            Segment s = new Segment(_languageDirection.SourceCulture);
            s.Add(segment);
            return SearchSegment(settings, s);
        }

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            if (translationUnit?.SourceSegment == null)
            {
                throw new ArgumentNullException(nameof(translationUnit));
            }

            return SearchSegment(settings, translationUnit.SourceSegment);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            if (translationUnits == null)
                throw new ArgumentNullException(nameof(translationUnits));

            SearchResults[] results = new SearchResults[translationUnits.Length];
            for (int i = 0; i < translationUnits.Length; i++)
            {
                results[i] = SearchTranslationUnit(settings, translationUnits[i]);
            }
            return results;
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            if (translationUnits == null)
                throw new ArgumentNullException(nameof(translationUnits));
            if (mask == null || mask.Length != translationUnits.Length)
                throw new ArgumentException("Mask array is null or has incorrect length", nameof(mask));

            SearchResults[] results = new SearchResults[translationUnits.Length];
            for (int i = 0; i < translationUnits.Length; i++)
            {
                if (mask[i])
                {
                    results[i] = SearchTranslationUnit(settings, translationUnits[i]);
                }
                else
                {
                    results[i] = null;
                }
            }
            return results;
        }

        #endregion

        #region "Unsupported Methods"

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            throw new NotImplementedException("数译翻译提供者不支持添加翻译单元");
        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            throw new NotImplementedException("数译翻译提供者不支持添加翻译单元");
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException("数译翻译提供者不支持添加翻译单元");
        }

        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            throw new NotImplementedException("数译翻译提供者不支持更新翻译单元");
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException("数译翻译提供者不支持更新翻译单元");
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            throw new NotImplementedException("数译翻译提供者不支持更新翻译单元");
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException("数译翻译提供者不支持更新翻译单元");
        }

        #endregion

        #region "Configuration Validation"

        /// <summary>
        /// 验证配置是否有效（使用凭据存储）
        /// </summary>
        private bool IsConfigurationValid()
        {
            // 检查提供者配置
            if (!_provider.IsConfigurationValid())
            {
                System.Diagnostics.Debug.WriteLine("Provider configuration is invalid");
                return false;
            }

            // 检查凭据
            if (_credentials == null || !_credentials.IsValid())
            {
                System.Diagnostics.Debug.WriteLine("Credentials are invalid or missing");

                // 尝试刷新凭据
                _provider.RefreshCredentials();
                _credentials = _provider.Credentials;

                if (_credentials == null || !_credentials.IsValid())
                {
                    System.Diagnostics.Debug.WriteLine("Credentials still invalid after refresh");
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine("Configuration validation passed");
            return true;
        }

        /// <summary>
        /// 获取配置状态信息
        /// </summary>
        private string GetConfigurationStatus()
        {
            if (_provider == null)
                return "提供者对象为空";

            return _provider.GetConfigurationStatus();
        }

        #endregion

        #region "Translation API"

        /// <summary>
        /// 调用数译API进行翻译
        /// </summary>
        private string TranslateWithDtranxAPI(string text)
        {
            // 使用安全存储的凭据
            if (_credentials == null || !_credentials.IsValid())
            {
                throw new InvalidOperationException("API凭据无效或缺失");
            }

            string url = "https://openapi.dtranx.com/mt/yyq/translate";

            // 生成时间戳
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string path = "/mt/yyq/translate";
            string version = "1.0.0";

            // 使用从安全存储获取的凭据生成签名
            string strToSign = $"path{path}timestamp{timestamp}version{version}{_credentials.SignKey}";
            string sign = GetMD5Hash(strToSign).ToUpper();

            // 准备请求体
            var requestBody = new
            {
                src_lang = ConvertLanguageCode(_languageDirection.SourceCulture.Name, true),
                tgt_lang = ConvertLanguageCode(_languageDirection.TargetCulture.Name, false),
                domain = _options.Domain ?? "general",
                data = new
                {
                    src_text = new[] { text }
                }
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            System.Diagnostics.Debug.WriteLine($"Request body: {jsonBody}");

            // 创建请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("timestamp", timestamp);
            request.Headers.Add("path", path);
            request.Headers.Add("version", version);
            request.Headers.Add("sign", sign);
            request.Headers.Add("appKey", _credentials.AppKey);     // 使用安全凭据
            request.Headers.Add("phone", _credentials.Phone);      // 使用安全凭据
            request.Timeout = 30000; // 30秒超时

            // 发送请求
            byte[] data = Encoding.UTF8.GetBytes(jsonBody);
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            // 获取响应
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseText = reader.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine($"API Response: {responseText}");

                    JObject result = JObject.Parse(responseText);
                    string status = result["status"]?.ToString();

                    if (status == "200")
                    {
                        JArray translations = (JArray)result["data"]["translation"];
                        return translations[0].ToString();
                    }
                    else
                    {
                        string errorMsg = result["msg"]?.ToString() ?? "未知错误";
                        throw new Exception($"API错误 (状态码: {status}): {errorMsg}");
                    }
                }
            }
            catch (WebException ex)
            {
                string errorDetails = "网络连接错误";
                if (ex.Response != null)
                {
                    using (var errorStream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(errorStream))
                    {
                        errorDetails = reader.ReadToEnd();
                    }
                }
                throw new Exception($"网络错误: {ex.Message}. 详情: {errorDetails}");
            }
        }

        #endregion

        #region "Helper Methods"

        /// <summary>
        /// 计算MD5哈希
        /// </summary>
        private string GetMD5Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 转换Trados语言代码到数译API语言代码
        /// </summary>
        private string ConvertLanguageCode(string tradosCode, bool isSource)
        {
            if (string.IsNullOrEmpty(tradosCode))
            {
                return isSource ? "auto" : "en";
            }

            // 语言代码映射表
            Dictionary<string, string> languageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "zh-CN", "zh" },
                { "zh-TW", "zh-TW" },
                { "zh-HK", "zh-HK" },
                { "en-US", "en" },
                { "en-GB", "en" },
                { "ja-JP", "ja" },
                { "ko-KR", "ko" },
                { "de-DE", "de" },
                { "fr-FR", "fr" },
                { "es-ES", "es" },
                { "pt-PT", "pt" },
                { "ru-RU", "ru" },
                { "it-IT", "it" },
                { "vi-VN", "vi" },
                { "th-TH", "th" },
                { "ar-SA", "ar" },
                { "hi-IN", "hin_Deva" },
                { "id-ID", "id" },
                { "ms-MY", "zsm_Latn" },
                { "tr-TR", "tur_Latn" },
                { "pl-PL", "pol_Latn" },
                { "nl-NL", "nld_Latn" },
                { "cs-CZ", "cs" },
                { "sv-SE", "swe_Latn" },
                { "da-DK", "dan_Latn" },
                { "fi-FI", "fin_Latn" },
                { "no-NO", "nob_Latn" },
                { "hu-HU", "hun_Latn" },
                { "el-GR", "ell_Grek" },
                { "ro-RO", "ron_Latn" },
                { "bg-BG", "bul_Cyrl" },
                { "hr-HR", "hr" },
                { "sk-SK", "slk_Latn" },
                { "sl-SI", "slv_Latn" },
                { "et-EE", "est_Latn" },
                { "lv-LV", "lvs_Latn" },
                { "lt-LT", "lit_Latn" },
                { "uk-UA", "uk" },
                { "he-IL", "heb_Hebr" }
            };

            if (languageMap.ContainsKey(tradosCode))
            {
                return languageMap[tradosCode];
            }

            // 如果没有找到精确匹配，尝试基础语言代码
            if (tradosCode.Contains("-"))
            {
                string baseCode = tradosCode.Split('-')[0];
                if (languageMap.ContainsValue(baseCode))
                {
                    return baseCode;
                }
            }

            // 最后的回退
            if (isSource)
            {
                return "auto"; // 源语言可以使用自动检测
            }
            else
            {
                // 对于目标语言，尝试提取基础语言代码
                string baseCode = tradosCode.Split('-')[0];
                return baseCode;
            }
        }

        /// <summary>
        /// 创建搜索结果
        /// </summary>
        private SearchResult CreateSearchResult(Segment searchSegment, Segment translation, string sourceText)
        {
            TranslationUnit tu = new TranslationUnit();
            tu.SourceSegment = searchSegment.Duplicate();
            tu.TargetSegment = translation;

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
            tu.Origin = TranslationUnitOrigin.MachineTranslation;

            SearchResult searchResult = new SearchResult(tu);
            searchResult.ScoringResult = new ScoringResult();
            searchResult.ScoringResult.BaseScore = 85; // 机器翻译通常给85分

            tu.ConfirmationLevel = ConfirmationLevel.Draft;

            return searchResult;
        }

        #endregion

        #region "Debug Methods"

        /// <summary>
        /// 获取调试信息
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Language Direction: {_languageDirection?.SourceCultureName} -> {_languageDirection?.TargetCultureName}\n" +
                   $"Configuration Status: {GetConfigurationStatus()}\n" +
                   $"Credentials Valid: {_credentials?.IsValid() ?? false}\n" +
                   $"Provider Valid: {_provider?.IsConfigurationValid() ?? false}";
        }

        #endregion
    }
}