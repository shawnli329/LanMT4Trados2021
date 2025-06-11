namespace Sdl.Sdk.LanguagePlatform.Samples.DtranxProvider
{
    partial class DtranxProviderConfDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_AppKey = new System.Windows.Forms.TextBox();
            this.txt_SignKey = new System.Windows.Forms.TextBox();
            this.txt_Phone = new System.Windows.Forms.TextBox();
            this.combo_Domain = new System.Windows.Forms.ComboBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel_GetKeys = new System.Windows.Forms.LinkLabel();
            this.btn_TestConnection = new System.Windows.Forms.Button();
            this.chk_SaveGlobally = new System.Windows.Forms.CheckBox();
            this.btn_ClearConfiguration = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AppKey:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "SignKey:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "手机号:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "翻译领域:";
            // 
            // txt_AppKey
            // 
            this.txt_AppKey.Location = new System.Drawing.Point(84, 32);
            this.txt_AppKey.Name = "txt_AppKey";
            this.txt_AppKey.Size = new System.Drawing.Size(270, 21);
            this.txt_AppKey.TabIndex = 4;
            // 
            // txt_SignKey
            // 
            this.txt_SignKey.Location = new System.Drawing.Point(84, 67);
            this.txt_SignKey.Name = "txt_SignKey";
            this.txt_SignKey.PasswordChar = '*';
            this.txt_SignKey.Size = new System.Drawing.Size(270, 21);
            this.txt_SignKey.TabIndex = 5;
            // 
            // txt_Phone
            // 
            this.txt_Phone.Location = new System.Drawing.Point(84, 102);
            this.txt_Phone.Name = "txt_Phone";
            this.txt_Phone.Size = new System.Drawing.Size(270, 21);
            this.txt_Phone.TabIndex = 6;
            // 
            // combo_Domain
            // 
            this.combo_Domain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Domain.FormattingEnabled = true;
            this.combo_Domain.Items.AddRange(new object[] {
            "general"});
            this.combo_Domain.Location = new System.Drawing.Point(84, 137);
            this.combo_Domain.Name = "combo_Domain";
            this.combo_Domain.Size = new System.Drawing.Size(270, 20);
            this.combo_Domain.TabIndex = 7;
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(223, 240);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 8;
            this.btn_OK.Text = "确定";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(305, 240);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "取消";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chk_SaveGlobally);
            this.groupBox1.Controls.Add(this.linkLabel_GetKeys);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.combo_Domain);
            this.groupBox1.Controls.Add(this.txt_AppKey);
            this.groupBox1.Controls.Add(this.txt_Phone);
            this.groupBox1.Controls.Add(this.txt_SignKey);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(373, 210);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数译API设置";
            // 
            // linkLabel_GetKeys
            // 
            this.linkLabel_GetKeys.AutoSize = true;
            this.linkLabel_GetKeys.Location = new System.Drawing.Point(211, 180);
            this.linkLabel_GetKeys.Name = "linkLabel_GetKeys";
            this.linkLabel_GetKeys.Size = new System.Drawing.Size(143, 12);
            this.linkLabel_GetKeys.TabIndex = 8;
            this.linkLabel_GetKeys.TabStop = true;
            this.linkLabel_GetKeys.Text = "点击获取AppKey和SignKey";
            this.linkLabel_GetKeys.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_GetKeys_LinkClicked);
            // 
            // btn_TestConnection
            // 
            this.btn_TestConnection.Location = new System.Drawing.Point(12, 240);
            this.btn_TestConnection.Name = "btn_TestConnection";
            this.btn_TestConnection.Size = new System.Drawing.Size(75, 23);
            this.btn_TestConnection.TabIndex = 10;
            this.btn_TestConnection.Text = "测试连接";
            this.btn_TestConnection.UseVisualStyleBackColor = true;
            this.btn_TestConnection.Click += new System.EventHandler(this.btn_TestConnection_Click);
            // 
            // chk_SaveGlobally
            // 
            this.chk_SaveGlobally.AutoSize = true;
            this.chk_SaveGlobally.Checked = true;
            this.chk_SaveGlobally.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveGlobally.Enabled = false;
            this.chk_SaveGlobally.Location = new System.Drawing.Point(21, 178);
            this.chk_SaveGlobally.Name = "chk_SaveGlobally";
            this.chk_SaveGlobally.Size = new System.Drawing.Size(156, 16);
            this.chk_SaveGlobally.TabIndex = 11;
            this.chk_SaveGlobally.Text = "在Trados里保存以上信息";
            this.chk_SaveGlobally.UseVisualStyleBackColor = true;
            // 
            // btn_ClearConfiguration
            // 
            this.btn_ClearConfiguration.Location = new System.Drawing.Point(96, 240);
            this.btn_ClearConfiguration.Name = "btn_ClearConfiguration";
            this.btn_ClearConfiguration.Size = new System.Drawing.Size(75, 23);
            this.btn_ClearConfiguration.TabIndex = 11;
            this.btn_ClearConfiguration.Text = "清除设置";
            this.btn_ClearConfiguration.UseVisualStyleBackColor = true;
            this.btn_ClearConfiguration.Click += new System.EventHandler(this.btn_ClearConfiguration_Click);

            this.Controls.Add(this.btn_ClearConfiguration);
            // 
            // DtranxProviderConfDialog
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(397, 284);
            this.Controls.Add(this.btn_ClearConfiguration);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_TestConnection);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DtranxProviderConfDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数译机器翻译设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_AppKey;
        private System.Windows.Forms.TextBox txt_SignKey;
        private System.Windows.Forms.TextBox txt_Phone;
        private System.Windows.Forms.ComboBox combo_Domain;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_TestConnection;
        private System.Windows.Forms.CheckBox chk_SaveGlobally;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel linkLabel_GetKeys;
        private System.Windows.Forms.Button btn_ClearConfiguration;
    }
}