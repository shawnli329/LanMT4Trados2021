using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    /// <summary>
    /// 此类用于保存数译API插件的设置
    /// </summary>
    public class DtranxTranslationOptions
    {
        #region "TranslationMethod"
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
        #endregion

        #region "TranslationProviderUriBuilder"
        TranslationProviderUriBuilder _uriBuilder;

        public DtranxTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(DtranxTranslationProvider.DtranxTranslationProviderScheme);
        }

        public DtranxTranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }
        #endregion

        /// <summary>
        /// 获取或设置翻译领域
        /// </summary>
        #region "Domain"
        public string Domain
        {
            get
            {
                string domain = GetStringParameter("domain");
                return string.IsNullOrEmpty(domain) ? "general" : domain;
            }
            set { SetStringParameter("domain", value); }
        }
        #endregion

        /// <summary>
        /// 用户名（非敏感信息，可以保存在URI中）
        /// </summary>
        #region "UserName"
        public string UserName
        {
            get { return GetStringParameter("username"); }
            set { SetStringParameter("username", value); }
        }
        #endregion

        // ==========================================
        // 为了向后兼容，保留这些属性但标记为过时
        // 实际应用中应该使用凭据存储
        // ==========================================

        /// <summary>
        /// AppKey - 已废弃，请使用凭据存储
        /// </summary>
        [Obsolete("请使用凭据存储来管理敏感信息")]
        public string AppKey
        {
            get { return GetStringParameter("appkey"); }
            set { SetStringParameter("appkey", value); }
        }

        /// <summary>
        /// SignKey - 已废弃，请使用凭据存储
        /// </summary>
        [Obsolete("请使用凭据存储来管理敏感信息")]
        public string SignKey
        {
            get { return GetStringParameter("signkey"); }
            set { SetStringParameter("signkey", value); }
        }

        /// <summary>
        /// Phone - 已废弃，请使用凭据存储
        /// </summary>
        [Obsolete("请使用凭据存储来管理敏感信息")]
        public string Phone
        {
            get { return GetStringParameter("phone"); }
            set { SetStringParameter("phone", value); }
        }

        #region "Helper Methods"
        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }

        private string GetStringParameter(string p)
        {
            string paramString = _uriBuilder[p];
            return paramString;
        }
        #endregion

        #region "Uri"
        public Uri Uri
        {
            get
            {
                return _uriBuilder.Uri;
            }
        }
        #endregion
    }
}