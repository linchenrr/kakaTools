namespace excelExport
{
    partial class Form_ExcelExport
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ExcelExport));
            this.button1 = new System.Windows.Forms.Button();
            this.tb_fileNames = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_export = new System.Windows.Forms.Button();
            this.tb_outPutPath = new System.Windows.Forms.TextBox();
            this.btn_selectOutPutFolder = new System.Windows.Forms.Button();
            this.lb_status = new System.Windows.Forms.Label();
            this.checkBox_ignore = new System.Windows.Forms.CheckBox();
            this.tb_ignoreLine = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_ignoreColumn = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_ignoreSheet = new System.Windows.Forms.TextBox();
            this.tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.item_show = new System.Windows.Forms.ToolStripMenuItem();
            this.item_hide = new System.Windows.Forms.ToolStripMenuItem();
            this.item_close = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_clear = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_primaryKey = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox_merge = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cb_compressOP = new System.Windows.Forms.ComboBox();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(16, 195);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 64);
            this.button1.TabIndex = 1;
            this.button1.Text = "添加文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_fileNames
            // 
            this.tb_fileNames.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_fileNames.Location = new System.Drawing.Point(14, 452);
            this.tb_fileNames.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_fileNames.Multiline = true;
            this.tb_fileNames.Name = "tb_fileNames";
            this.tb_fileNames.ReadOnly = true;
            this.tb_fileNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_fileNames.Size = new System.Drawing.Size(374, 263);
            this.tb_fileNames.TabIndex = 2;
            this.tb_fileNames.DragDrop += new System.Windows.Forms.DragEventHandler(this.tb_fileNames_DragDrop);
            this.tb_fileNames.DragEnter += new System.Windows.Forms.DragEventHandler(this.tb_fileNames_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(14, 431);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "当前已选择的文件或目录(将会导出目录下的所有文件)";
            // 
            // btn_export
            // 
            this.btn_export.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_export.Location = new System.Drawing.Point(214, 195);
            this.btn_export.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(114, 64);
            this.btn_export.TabIndex = 4;
            this.btn_export.Text = "导出";
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // tb_outPutPath
            // 
            this.tb_outPutPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_outPutPath.Location = new System.Drawing.Point(14, 374);
            this.tb_outPutPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_outPutPath.Multiline = true;
            this.tb_outPutPath.Name = "tb_outPutPath";
            this.tb_outPutPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_outPutPath.Size = new System.Drawing.Size(371, 45);
            this.tb_outPutPath.TabIndex = 5;
            this.tb_outPutPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.tb_outPutPath_DragDrop);
            this.tb_outPutPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.tb_outPutPath_DragEnter);
            // 
            // btn_selectOutPutFolder
            // 
            this.btn_selectOutPutFolder.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_selectOutPutFolder.Location = new System.Drawing.Point(16, 275);
            this.btn_selectOutPutFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_selectOutPutFolder.Name = "btn_selectOutPutFolder";
            this.btn_selectOutPutFolder.Size = new System.Drawing.Size(115, 61);
            this.btn_selectOutPutFolder.TabIndex = 6;
            this.btn_selectOutPutFolder.Text = "选择导出目录";
            this.btn_selectOutPutFolder.UseVisualStyleBackColor = true;
            this.btn_selectOutPutFolder.Click += new System.EventHandler(this.btn_selectOutPutFolder_Click);
            // 
            // lb_status
            // 
            this.lb_status.AutoSize = true;
            this.lb_status.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_status.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_status.Location = new System.Drawing.Point(31, 150);
            this.lb_status.Name = "lb_status";
            this.lb_status.Size = new System.Drawing.Size(42, 21);
            this.lb_status.TabIndex = 7;
            this.lb_status.Text = "状态";
            this.lb_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox_ignore
            // 
            this.checkBox_ignore.AutoSize = true;
            this.checkBox_ignore.Checked = true;
            this.checkBox_ignore.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ignore.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_ignore.Location = new System.Drawing.Point(214, 58);
            this.checkBox_ignore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox_ignore.Name = "checkBox_ignore";
            this.checkBox_ignore.Size = new System.Drawing.Size(111, 21);
            this.checkBox_ignore.TabIndex = 8;
            this.checkBox_ignore.Text = "忽略空白行、列";
            this.checkBox_ignore.UseVisualStyleBackColor = true;
            // 
            // tb_ignoreLine
            // 
            this.tb_ignoreLine.Location = new System.Drawing.Point(103, 56);
            this.tb_ignoreLine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_ignoreLine.Name = "tb_ignoreLine";
            this.tb_ignoreLine.Size = new System.Drawing.Size(48, 23);
            this.tb_ignoreLine.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(14, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "忽略的行前缀:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(14, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "忽略的列前缀:";
            // 
            // tb_ignoreColumn
            // 
            this.tb_ignoreColumn.Location = new System.Drawing.Point(103, 94);
            this.tb_ignoreColumn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_ignoreColumn.Name = "tb_ignoreColumn";
            this.tb_ignoreColumn.Size = new System.Drawing.Size(48, 23);
            this.tb_ignoreColumn.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(14, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "忽略的表前缀:";
            // 
            // tb_ignoreSheet
            // 
            this.tb_ignoreSheet.Location = new System.Drawing.Point(103, 18);
            this.tb_ignoreSheet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_ignoreSheet.Name = "tb_ignoreSheet";
            this.tb_ignoreSheet.Size = new System.Drawing.Size(48, 23);
            this.tb_ignoreSheet.TabIndex = 15;
            // 
            // tray
            // 
            this.tray.ContextMenuStrip = this.contextMenu;
            this.tray.Icon = ((System.Drawing.Icon)(resources.GetObject("tray.Icon")));
            this.tray.Visible = true;
            this.tray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tray_MouseClick);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.item_show,
            this.item_hide,
            this.item_close});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(101, 70);
            // 
            // item_show
            // 
            this.item_show.Name = "item_show";
            this.item_show.Size = new System.Drawing.Size(100, 22);
            this.item_show.Text = "显示";
            this.item_show.Click += new System.EventHandler(this.item_show_Click);
            // 
            // item_hide
            // 
            this.item_hide.Name = "item_hide";
            this.item_hide.Size = new System.Drawing.Size(100, 22);
            this.item_hide.Text = "隐藏";
            this.item_hide.Click += new System.EventHandler(this.item_hide_Click);
            // 
            // item_close
            // 
            this.item_close.Name = "item_close";
            this.item_close.Size = new System.Drawing.Size(100, 22);
            this.item_close.Text = "退出";
            this.item_close.Click += new System.EventHandler(this.item_close_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_clear.Location = new System.Drawing.Point(214, 272);
            this.btn_clear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(115, 64);
            this.btn_clear.TabIndex = 16;
            this.btn_clear.Text = "清除列表";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(209, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "主键前缀:";
            // 
            // tb_primaryKey
            // 
            this.tb_primaryKey.Location = new System.Drawing.Point(279, 126);
            this.tb_primaryKey.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_primaryKey.Name = "tb_primaryKey";
            this.tb_primaryKey.Size = new System.Drawing.Size(57, 23);
            this.tb_primaryKey.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(14, 353);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(312, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "导出目录，可以拖拽至此  (如果留空则导出到源文件目录)";
            // 
            // checkBox_merge
            // 
            this.checkBox_merge.AutoSize = true;
            this.checkBox_merge.Checked = true;
            this.checkBox_merge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_merge.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_merge.Location = new System.Drawing.Point(214, 97);
            this.checkBox_merge.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox_merge.Name = "checkBox_merge";
            this.checkBox_merge.Size = new System.Drawing.Size(75, 21);
            this.checkBox_merge.TabIndex = 20;
            this.checkBox_merge.Text = "合并子表";
            this.checkBox_merge.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(209, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(164, 34);
            this.label7.TabIndex = 21;
            this.label7.Text = "留空或未找到的情况下\r\n默认使用第一个字段作为主键";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(211, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 17);
            this.label8.TabIndex = 22;
            this.label8.Text = "压缩:";
            // 
            // cb_compressOP
            // 
            this.cb_compressOP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_compressOP.FormattingEnabled = true;
            this.cb_compressOP.Location = new System.Drawing.Point(245, 18);
            this.cb_compressOP.Name = "cb_compressOP";
            this.cb_compressOP.Size = new System.Drawing.Size(84, 25);
            this.cb_compressOP.TabIndex = 23;
            // 
            // Form_ExcelExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 728);
            this.Controls.Add(this.cb_compressOP);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tb_primaryKey);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBox_merge);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.tb_ignoreSheet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_ignoreColumn);
            this.Controls.Add(this.tb_ignoreLine);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox_ignore);
            this.Controls.Add(this.btn_selectOutPutFolder);
            this.Controls.Add(this.lb_status);
            this.Controls.Add(this.btn_export);
            this.Controls.Add(this.tb_outPutPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_fileNames);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form_ExcelExport";
            this.Text = "ｋａｋａ —— excel数据导出工具  2012beta1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb_fileNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.TextBox tb_outPutPath;
        private System.Windows.Forms.Button btn_selectOutPutFolder;
        private System.Windows.Forms.Label lb_status;
        private System.Windows.Forms.CheckBox checkBox_ignore;
        private System.Windows.Forms.TextBox tb_ignoreLine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_ignoreColumn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_ignoreSheet;
        private System.Windows.Forms.NotifyIcon tray;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem item_close;
        private System.Windows.Forms.ToolStripMenuItem item_show;
        private System.Windows.Forms.ToolStripMenuItem item_hide;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_primaryKey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox_merge;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cb_compressOP;
    }
}

