// 在 DtranxProviderConfDialog.cs 中添加这些缺失的方法：

using System;
using System.Windows.Forms;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    public partial class DtranxProviderConfDialog : Form
    {
        private DtranxTranslationOptions _options;
        private DtranxCredentials _credentials;
        private ITranslationProviderCredentialStore _credentialStore;

        // 修改构造函数以支持凭据
        public DtranxProviderConfDialog(DtranxTranslationOptions options)
        {
            InitializeComponent();
            _options = options;
            _credentials = new DtranxCredentials();
            _credentialStore = null;
            LoadSettings();
        }

        public DtranxProviderConfDialog(DtranxTranslationOptions options, DtranxCredentials credentials, ITranslationProviderCredentialStore credentialStore)
        {
            InitializeComponent();
            _options = options;
            _credentials = credentials ?? new DtranxCredentials();
            _credentialStore = credentialStore;
            LoadSettings();
        }

        public DtranxTranslationOptions Options
        {
            get { return _options; }
        }

        private void LoadSettings()
        {
            if (_options != null)
            {
                combo_Domain.Text = _options.Domain ?? "general";

                // 优先从凭据对象加载，然后从全局设置加载
                if (_credentials != null && _credentials.IsValid())
                {
                    txt_AppKey.Text = _credentials.AppKey;
                    txt_SignKey.Text = _credentials.SignKey;
                    txt_Phone.Text = _credentials.Phone;
                    chk_SaveGlobally.Checked = true;
                }
                else if (DtranxSettingsManager.HasSavedSettings())
                {
                    DtranxSettingsManager.LoadFromRegistry(_options);
                    txt_AppKey.Text = _options.AppKey ?? "";
                    txt_SignKey.Text = _options.SignKey ?? "";
                    txt_Phone.Text = _options.Phone ?? "";
                    chk_SaveGlobally.Checked = true;
                }
                else
                {
                    txt_AppKey.Text = "";
                    txt_SignKey.Text = "";
                    txt_Phone.Text = "";
                    chk_SaveGlobally.Checked = true; // 默认保存
                }
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (string.IsNullOrEmpty(txt_AppKey.Text))
            {
                MessageBox.Show("请输入AppKey", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txt_SignKey.Text))
            {
                MessageBox.Show("请输入SignKey", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txt_Phone.Text))
            {
                MessageBox.Show("请输入手机号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 保存非敏感配置
                _options.Domain = combo_Domain.Text;

                // 更新凭据对象
                _credentials.AppKey = txt_AppKey.Text.Trim();
                _credentials.SignKey = txt_SignKey.Text.Trim();
                _credentials.Phone = txt_Phone.Text.Trim();

                // 保存到凭据存储（如果可用）
                if (_credentialStore != null && chk_SaveGlobally.Checked)
                {
                    DtranxCredentialManager.SaveCredentials(
                        _credentialStore,
                        _options.Uri,
                        _credentials.AppKey,
                        _credentials.SignKey,
                        _credentials.Phone
                    );
                }

                // 同时保存到全局设置（向后兼容）
                if (chk_SaveGlobally.Checked)
                {
                    _options.AppKey = _credentials.AppKey;
                    _options.SignKey = _credentials.SignKey;
                    _options.Phone = _credentials.Phone;
                    DtranxSettingsManager.SaveToRegistry(_options);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void linkLabel_GetKeys_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://openapi.dtranx.com/");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开网页：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_TestConnection_Click(object sender, EventArgs e)
        {
            // 先进行基本验证
            if (string.IsNullOrEmpty(txt_AppKey.Text) ||
                string.IsNullOrEmpty(txt_SignKey.Text) ||
                string.IsNullOrEmpty(txt_Phone.Text))
            {
                MessageBox.Show("请填写所有必填字段", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btn_TestConnection.Enabled = false;
                btn_TestConnection.Text = "测试中...";

                // 创建临时凭据对象进行测试
                var testCredentials = new DtranxCredentials
                {
                    AppKey = txt_AppKey.Text,
                    SignKey = txt_SignKey.Text,
                    Phone = txt_Phone.Text
                };

                // 测试API连接
                if (TestApiConnection(testCredentials, combo_Domain.Text))
                {
                    MessageBox.Show("连接成功！API凭据有效。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("连接失败！请检查API凭据。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试连接时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btn_TestConnection.Enabled = true;
                btn_TestConnection.Text = "测试连接";
            }
        }

        private bool TestApiConnection(DtranxCredentials credentials, string domain)
        {
            // 这里实现API连接测试逻辑
            // 参考之前的 TestApiConnection 方法实现
            try
            {
                string url = "https://openapi.dtranx.com/mt/yyq/translate";
                string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                string path = "/mt/yyq/translate";
                string version = "1.0.0";

                // 生成签名
                string strToSign = $"path{path}timestamp{timestamp}version{version}{credentials.SignKey}";
                string sign = GetMD5Hash(strToSign).ToUpper();

                // 准备测试请求体
                var requestBody = new
                {
                    src_lang = "zh",
                    tgt_lang = "en",
                    domain = string.IsNullOrEmpty(domain) ? "general" : domain,
                    data = new
                    {
                        src_text = new[] { "测试" }
                    }
                };

                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                // 创建请求
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("timestamp", timestamp);
                request.Headers.Add("path", path);
                request.Headers.Add("version", version);
                request.Headers.Add("sign", sign);
                request.Headers.Add("appKey", credentials.AppKey);
                request.Headers.Add("phone", credentials.Phone);
                request.Timeout = 10000;

                // 发送请求
                byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = data.Length;

                using (System.IO.Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                // 获取响应
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    string responseText = reader.ReadToEnd();
                    var result = Newtonsoft.Json.Linq.JObject.Parse(responseText);

                    string status = result["status"]?.ToString();
                    return status == "200";
                }
            }
            catch
            {
                return false;
            }
        }

        private string GetMD5Hash(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void btn_ClearConfiguration_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "确定要清除所有保存的配置信息吗？\n这将清除全局设置和凭据存储中的信息。",
                "确认清除配置",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 清除凭据存储
                    if (_credentialStore != null)
                    {
                        DtranxCredentialManager.ClearCredentials(_credentialStore, _options.Uri);
                    }

                    // 清除全局设置
                    DtranxSettingsManager.ClearSavedSettings();

                    // 清除表单
                    txt_AppKey.Text = "";
                    txt_SignKey.Text = "";
                    txt_Phone.Text = "";
                    combo_Domain.SelectedIndex = 0;
                    chk_SaveGlobally.Checked = false;

                    // 清除凭据对象
                    _credentials = new DtranxCredentials();

                    MessageBox.Show("配置信息已成功清除！", "清除完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"清除配置时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}