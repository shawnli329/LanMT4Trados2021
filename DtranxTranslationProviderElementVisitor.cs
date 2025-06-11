using System;
using System.Text;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    /// <summary>
    /// 元素访问器类，用于提取片段中的纯文本
    /// </summary>
    class DtranxTranslationProviderElementVisitor : ISegmentElementVisitor
    {
        private DtranxTranslationOptions _options;
        private string _plainText;

        public string PlainText
        {
            get
            {
                if (_plainText == null)
                {
                    _plainText = "";
                }
                return _plainText;
            }
            set
            {
                _plainText = value;
            }
        }

        public void Reset()
        {
            _plainText = "";
        }

        public DtranxTranslationProviderElementVisitor(DtranxTranslationOptions options)
        {
            _options = options;
        }

        #region ISegmentElementVisitor Members

        public void VisitDateTimeToken(Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken token)
        {
            _plainText += token.Text;
        }

        public void VisitMeasureToken(Sdl.LanguagePlatform.Core.Tokenization.MeasureToken token)
        {
            _plainText += token.Text;
        }

        public void VisitNumberToken(Sdl.LanguagePlatform.Core.Tokenization.NumberToken token)
        {
            _plainText += token.Text;
        }

        public void VisitSimpleToken(Sdl.LanguagePlatform.Core.Tokenization.SimpleToken token)
        {
            _plainText += token.Text;
        }

        public void VisitTag(Tag tag)
        {
            // 标签通常表示格式化或占位符，保留其文本等价物
            _plainText += tag.TextEquivalent;
        }

        public void VisitTagToken(Sdl.LanguagePlatform.Core.Tokenization.TagToken token)
        {
            _plainText += token.Text;
        }

        public void VisitText(Text text)
        {
            _plainText += text;
        }

        #endregion
    }
}