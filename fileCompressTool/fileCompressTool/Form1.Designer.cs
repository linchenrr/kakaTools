namespace fileCompressTool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_clear = new System.Windows.Forms.Button();
            this.lb_status = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_fileNames = new System.Windows.Forms.TextBox();
            this.btn_addFile = new System.Windows.Forms.Button();
            this.btn_compress = new System.Windows.Forms.Button();
            this.btn_uncompress = new System.Windows.Forms.Button();
            this.checkBox_cover = new System.Windows.Forms.CheckBox();
            this.cb_algorithm = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_clear
            // 
            this.btn_clear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_clear.Location = new System.Drawing.Point(29, 166);
            this.btn_clear.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(115, 64);
            this.btn_clear.TabIndex = 21;
            this.btn_clear.Text = "清除列表";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // lb_status
            // 
            this.lb_status.AutoSize = true;
            this.lb_status.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_status.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_status.Location = new System.Drawing.Point(243, 11);
            this.lb_status.Name = "lb_status";
            this.lb_status.Size = new System.Drawing.Size(42, 21);
            this.lb_status.TabIndex = 20;
            this.lb_status.Text = "状态";
            this.lb_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(26, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 19;
            this.label1.Text = "已选择以下文件：";
            // 
            // tb_fileNames
            // 
            this.tb_fileNames.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_fileNames.Location = new System.Drawing.Point(29, 282);
            this.tb_fileNames.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_fileNames.Multiline = true;
            this.tb_fileNames.Name = "tb_fileNames";
            this.tb_fileNames.ReadOnly = true;
            this.tb_fileNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_fileNames.Size = new System.Drawing.Size(353, 225);
            this.tb_fileNames.TabIndex = 18;
            // 
            // btn_addFile
            // 
            this.btn_addFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_addFile.Location = new System.Drawing.Point(29, 85);
            this.btn_addFile.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btn_addFile.Name = "btn_addFile";
            this.btn_addFile.Size = new System.Drawing.Size(115, 64);
            this.btn_addFile.TabIndex = 17;
            this.btn_addFile.Text = "添加文件";
            this.btn_addFile.UseVisualStyleBackColor = true;
            this.btn_addFile.Click += new System.EventHandler(this.btn_addFile_Click);
            // 
            // btn_compress
            // 
            this.btn_compress.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btn_compress.Location = new System.Drawing.Point(229, 85);
            this.btn_compress.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btn_compress.Name = "btn_compress";
            this.btn_compress.Size = new System.Drawing.Size(115, 64);
            this.btn_compress.TabIndex = 22;
            this.btn_compress.Text = "压缩";
            this.btn_compress.UseVisualStyleBackColor = true;
            this.btn_compress.Click += new System.EventHandler(this.btn_compress_Click);
            // 
            // btn_uncompress
            // 
            this.btn_uncompress.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btn_uncompress.Location = new System.Drawing.Point(229, 166);
            this.btn_uncompress.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btn_uncompress.Name = "btn_uncompress";
            this.btn_uncompress.Size = new System.Drawing.Size(115, 64);
            this.btn_uncompress.TabIndex = 23;
            this.btn_uncompress.Text = "解压缩";
            this.btn_uncompress.UseVisualStyleBackColor = true;
            this.btn_uncompress.Click += new System.EventHandler(this.btn_uncompress_Click);
            // 
            // checkBox_cover
            // 
            this.checkBox_cover.AutoSize = true;
            this.checkBox_cover.Checked = true;
            this.checkBox_cover.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_cover.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_cover.Location = new System.Drawing.Point(29, 14);
            this.checkBox_cover.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox_cover.Name = "checkBox_cover";
            this.checkBox_cover.Size = new System.Drawing.Size(87, 21);
            this.checkBox_cover.TabIndex = 24;
            this.checkBox_cover.Text = "覆盖原文件";
            this.checkBox_cover.UseVisualStyleBackColor = true;
            // 
            // cb_algorithm
            // 
            this.cb_algorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_algorithm.FormattingEnabled = true;
            this.cb_algorithm.Location = new System.Drawing.Point(59, 41);
            this.cb_algorithm.Name = "cb_algorithm";
            this.cb_algorithm.Size = new System.Drawing.Size(86, 25);
            this.cb_algorithm.TabIndex = 25;
            this.cb_algorithm.SelectedIndexChanged += new System.EventHandler(this.cb_algorithm_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 26;
            this.label2.Text = "算法:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 522);
            this.Controls.Add(this.cb_algorithm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox_cover);
            this.Controls.Add(this.btn_uncompress);
            this.Controls.Add(this.btn_compress);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.lb_status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_fileNames);
            this.Controls.Add(this.btn_addFile);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "ｋａｋａ —— 文件压缩工具  2011正式版";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Label lb_status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_fileNames;
        private System.Windows.Forms.Button btn_addFile;
        private System.Windows.Forms.Button btn_compress;
        private System.Windows.Forms.Button btn_uncompress;
        private System.Windows.Forms.CheckBox checkBox_cover;
        private System.Windows.Forms.ComboBox cb_algorithm;
        private System.Windows.Forms.Label label2;
    }
}

