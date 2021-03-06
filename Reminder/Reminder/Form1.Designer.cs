﻿
namespace Reminder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.iconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.item_open = new System.Windows.Forms.ToolStripMenuItem();
            this.item_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_autoRun = new System.Windows.Forms.CheckBox();
            this.iconMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "定时提醒工具";
            this.notifyIcon.ContextMenuStrip = this.iconMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "定时提醒工具";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // iconMenu
            // 
            this.iconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.item_open,
            this.item_exit});
            this.iconMenu.Name = "contextMenuStrip1";
            this.iconMenu.Size = new System.Drawing.Size(137, 48);
            this.iconMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.iconMenu_ItemClicked);
            // 
            // item_open
            // 
            this.item_open.Name = "item_open";
            this.item_open.Size = new System.Drawing.Size(136, 22);
            this.item_open.Text = "打开主界面";
            // 
            // item_exit
            // 
            this.item_exit.Name = "item_exit";
            this.item_exit.Size = new System.Drawing.Size(136, 22);
            this.item_exit.Text = "退出";
            // 
            // cb_autoRun
            // 
            this.cb_autoRun.AutoSize = true;
            this.cb_autoRun.Location = new System.Drawing.Point(445, 12);
            this.cb_autoRun.Name = "cb_autoRun";
            this.cb_autoRun.Size = new System.Drawing.Size(75, 21);
            this.cb_autoRun.TabIndex = 1;
            this.cb_autoRun.Text = "开机自启";
            this.cb_autoRun.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 353);
            this.Controls.Add(this.cb_autoRun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.iconMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip iconMenu;
        private System.Windows.Forms.ToolStripMenuItem item_open;
        private System.Windows.Forms.ToolStripMenuItem item_exit;
        private System.Windows.Forms.CheckBox cb_autoRun;
    }
}

