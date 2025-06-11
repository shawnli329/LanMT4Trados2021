using System;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    /// <summary>
    /// 凭据数据类
    /// </summary>
    public class DtranxCredentials
    {
        public string AppKey { get; set; } = "";
        public string SignKey { get; set; } = "";
        public string Phone { get; set; } = "";

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AppKey) &&
                   !string.IsNullOrEmpty(SignKey) &&
                   !string.IsNullOrEmpty(Phone);
        }
    }
}