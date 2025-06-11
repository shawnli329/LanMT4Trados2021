using System;
using System.Drawing;
using System.IO;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    /// <summary>
    /// 管理嵌入的Base64图标
    /// </summary>
    public static class EmbeddedIconManager
    {
        // 16x16 蓝色"MT"图标的Base64数据
        private const string ICON_BASE64 = @"
AAABAAEAEBAAAAEAGABoAwAAFgAAACgAAAAQAAAAIAAAAAEAGAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/b4f/coj/coj/coj/coj/coj/coj/coj/coj/coj/cYcAAAAAAAAAAAAAAAD/b4//coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/b4oAAAAAAAD+cYf/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/b4oAAAD/coj/coj/coj/coj+nZz/coj+28j//Pf+79/+nZz/coj/coj/coj/coj/cogAAAD/coj/coj/coj/coj//////////////////////////Pf/coj/coj/coj/coj/cof/coj/coj/coj/coj////////////////////////////97NL/coj/coj/coj/coj/coj/coj/coj7s3/5znr93cz////////////////715n5z3X/coj/coj/coj/coj/coj/coj/coj52nn52nn52nn50XX6qWX6qWT6tGn5vm75yXL/coj/coj/coj/coj/coj/coj/coj6zXv52nn75qP//ff////////95MD5uGv5w2//coj/coj/coj/coj/coj/coj/coj9k4P////////////////////////////70pf/coj/coj/coj/coj/coj/coj/coj/coj//Pf////////////////////////+nZz/coj/coj/coj/coj/coj/coj/coj/coj/coj+9ef////////////////+nZz/coj/coj/coj/coj/c4f/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/cYgAAAD/cYf/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/cYgAAAAAAAAAAAD/cYj/coj/coj/coj/coj/coj/coj/coj/coj/coj/coj/cYcAAAAAAAAAAAAAAAAAAAD/b4r+coj/cof/cof/cof/cof/cof/cYf/b4cAAAAAAAAAAAAAAAAAAACADwAAAAMAAAABAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAMAAIAHAADAHwAA";

        private const string SYMBOL_BASE64 = @"
iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlz
AAAOwwAADsMBx2+oZAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4yMfEgaZUAAAFZSURB
VDhPpZOxTgJBEIb3EISAgBYkNhY2NjQmJiTGwsLGVyDxCXgF4xMYE2NjY2NjY2FBQmJCAgmFFRYW
QAAREBHu8P+d2YO74C7xxC/Z3Zn5Znb2ZtcTQvxLsizrIhaLXaXT6XMYt+PxeLTb7TZrtVqN0+nU
VBVm2LMzmUwuEonEKXA3Go1e+v3+G3yLoigyGAyMYDBoxGIxwzRNg2XZSqXSzGw2m4njBqxcLl+h
2BX8BfQFvu8bpVLJ4DhBIKPX693AuKvruFAsFg2OEwQyWq1WVtdxwXECj0eJdDp9AuNSURQB3W7X
6HQ6xnA4NCaTidFoNFgvEAiIOOvV63Wjn8/nV7lcToQlQ6FQyIjH48wwDCObzRpAjUYjLCqKUqhW
q5ZE4n9AZ7fbfQStKMoMr1wu97jf72dyAIfD4ScUPqvVagtksVhsA3K5XFvQD/ZdGb4BP8iwyGEr
iCYAAAAASUVORK5CYII=";

        /// <summary>
        /// 获取嵌入的主图标
        /// </summary>
        public static Icon GetMainIcon()
        {
            try
            {
                byte[] iconBytes = Convert.FromBase64String(ICON_BASE64.Replace("\r", "").Replace("\n", "").Replace(" ", ""));
                using (MemoryStream ms = new MemoryStream(iconBytes))
                {
                    return new Icon(ms);
                }
            }
            catch
            {
                // 如果Base64解码失败，返回默认图标
                return SystemIcons.Application;
            }
        }

        /// <summary>
        /// 获取嵌入的搜索结果图标
        /// </summary>
        public static Bitmap GetSearchResultImage()
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(SYMBOL_BASE64.Replace("\r", "").Replace("\n", "").Replace(" ", ""));
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return new Bitmap(ms);
                }
            }
            catch
            {
                // 如果Base64解码失败，创建一个简单的位图
                Bitmap bmp = new Bitmap(16, 16);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.LightBlue);
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    {
                        g.DrawString("MT", font, Brushes.DarkBlue, 0, 1);
                    }
                }
                return bmp;
            }
        }
    }
}