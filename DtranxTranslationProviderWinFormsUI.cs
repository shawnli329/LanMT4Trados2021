using System;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    #region "Declaration"
    [TranslationProviderWinFormsUi(
        Id = "DtranxTranslationProviderWinFormsUI",
        Name = "DtranxTranslationProviderWinFormsUI",
        Description = "Dtranx Translation Provider UI")]
    #endregion
    public class DtranxTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        /// <summary>
        /// 显示插件设置窗体，当用户通过GUI添加翻译提供者插件时调用
        /// </summary>
        #region "Browse"
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            DtranxTranslationOptions options = new DtranxTranslationOptions();

            // 从凭据存储加载现有凭据（如果有）
            DtranxCredentials credentials = new DtranxCredentials();
            if (credentialStore != null)
            {
                // 使用基础URI来检查凭据
                Uri baseUri = new Uri($"{DtranxTranslationProvider.DtranxTranslationProviderScheme}://default");
                credentials = DtranxCredentialManager.GetCredentials(credentialStore, baseUri);
            }

            DtranxProviderConfDialog dialog = new DtranxProviderConfDialog(options, credentials, credentialStore);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                DtranxTranslationProvider provider = new DtranxTranslationProvider(dialog.Options, credentialStore);
                return new ITranslationProvider[] { provider };
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 确定插件设置是否可以更改
        /// </summary>
        #region "SupportsEditing"
        public bool SupportsEditing
        {
            get { return true; }
        }
        #endregion

        /// <summary>
        /// 如果插件设置可以被用户更改，SDL Trados Studio将显示一个设置按钮
        /// </summary>
        #region "Edit"
        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            DtranxTranslationProvider editProvider = translationProvider as DtranxTranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            // 获取当前凭据
            DtranxCredentials credentials = new DtranxCredentials();
            if (credentialStore != null)
            {
                credentials = DtranxCredentialManager.GetCredentials(credentialStore, editProvider.Uri);

                // 如果没有找到凭据，尝试使用默认URI
                if (!credentials.IsValid())
                {
                    Uri baseUri = new Uri($"{DtranxTranslationProvider.DtranxTranslationProviderScheme}://default");
                    credentials = DtranxCredentialManager.GetCredentials(credentialStore, baseUri);
                }
            }

            DtranxProviderConfDialog dialog = new DtranxProviderConfDialog(editProvider.Options, credentials, credentialStore);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                editProvider.Options = dialog.Options;
                // 刷新提供者的凭据
                editProvider.RefreshCredentials();
                return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// 处理用户认证 - 现在通过凭据存储处理
        /// </summary>
        #region "GetCredentialsFromUser"
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            // 检查是否已有有效凭据
            if (credentialStore != null)
            {
                DtranxCredentials credentials = DtranxCredentialManager.GetCredentials(credentialStore, translationProviderUri);
                if (credentials.IsValid())
                {
                    return true; // 已有有效凭据，无需重新输入
                }
            }

            // 没有有效凭据，显示配置对话框
            try
            {
                DtranxTranslationOptions options = new DtranxTranslationOptions(translationProviderUri);
                DtranxCredentials emptyCredentials = new DtranxCredentials();

                DtranxProviderConfDialog dialog = new DtranxProviderConfDialog(options, emptyCredentials, credentialStore);

                // 设置对话框标题以表明这是凭据输入
                dialog.Text = "数译机器翻译 - 输入API凭据";

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取凭据时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
        #endregion

        /// <summary>
        /// 用于显示插件信息，如插件名称、图标等
        /// </summary>
        #region "GetDisplayInfo"
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo();

            // 检查配置状态
            try
            {
                DtranxTranslationOptions options = new DtranxTranslationOptions(translationProviderUri);

                // 检查是否有基本配置
                bool hasBasicConfig = !string.IsNullOrEmpty(options.Domain);

                if (hasBasicConfig)
                {
                    info.Name = "数译机器翻译";
                    info.TooltipText = "使用数译API提供高质量机器翻译";
                }
                else
                {
                    info.Name = "数译机器翻译 (需要配置)";
                    info.TooltipText = "数译API需要配置，请点击设置按钮进行配置";
                }
            }
            catch
            {
                info.Name = "数译机器翻译 (配置错误)";
                info.TooltipText = "数译API配置信息有误，请重新配置";
            }

            // 使用嵌入的Base64图标
            try
            {
                info.TranslationProviderIcon = EmbeddedIconManager.GetMainIcon();
                info.SearchResultImage = EmbeddedIconManager.GetSearchResultImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load embedded icons: {ex.Message}");
                // 如果加载失败，图标会保持为null，系统使用默认图标
            }

            return info;
        }
        #endregion

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("URI not supported by the plug-in.");
            }
            return String.Equals(translationProviderUri.Scheme, DtranxTranslationProvider.DtranxTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription
        {
            get { return "数译机器翻译插件提供多语言翻译服务，支持安全的凭据存储"; }
        }

        public string TypeName
        {
            get { return "数译机器翻译"; }
        }

        #endregion

        #region "Helper Methods"

        /// <summary>
        /// 检查是否有有效的凭据配置
        /// </summary>
        private bool HasValidCredentials(ITranslationProviderCredentialStore credentialStore, Uri providerUri)
        {
            if (credentialStore == null) return false;

            try
            {
                DtranxCredentials credentials = DtranxCredentialManager.GetCredentials(credentialStore, providerUri);
                return credentials.IsValid();
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}