using System;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    #region "Declaration"
    [TranslationProviderFactory(
        Id = "DtranxTranslationProviderFactory",
        Name = "DtranxTranslationProviderFactory",
        Description = "Factory for Dtranx Translation Provider.")]
    #endregion
    public class DtranxTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        #region "CreateTranslationProvider"
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new Exception("Cannot handle URI.");
            }

            // 重要：需要传递凭据存储
            DtranxTranslationProvider tp = new DtranxTranslationProvider(
                new DtranxTranslationOptions(translationProviderUri),
                credentialStore);

            return tp;
        }
        #endregion

        #region "SupportsTranslationProviderUri"
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("Translation provider URI not supported.");
            }
            return String.Equals(translationProviderUri.Scheme, DtranxTranslationProvider.DtranxTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region "GetTranslationProviderInfo"
        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo info = new TranslationProviderInfo();

            #region "TranslationMethod"
            info.TranslationMethod = DtranxTranslationOptions.ProviderTranslationMethod;
            #endregion

            #region "Name"
            info.Name = PluginResources.Plugin_NiceName;
            #endregion

            return info;
        }
        #endregion


        #endregion
    }
}