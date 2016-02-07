namespace AmiigoPC
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_dec = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAmiigoDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.save = new System.Windows.Forms.ToolStripMenuItem();
            this.saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAmiiboBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportDecBin = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportEncBin = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAmiiboBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDecBin = new System.Windows.Forms.ToolStripMenuItem();
            this.exportEncbin = new System.Windows.Forms.ToolStripMenuItem();
            this.lbl_uid = new System.Windows.Forms.Label();
            this.lbl_crc = new System.Windows.Forms.Label();
            this.lbl_key = new System.Windows.Forms.Label();
            this.button_enc = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_dec
            // 
            this.button_dec.Enabled = false;
            this.button_dec.Location = new System.Drawing.Point(12, 119);
            this.button_dec.Name = "button_dec";
            this.button_dec.Size = new System.Drawing.Size(149, 37);
            this.button_dec.TabIndex = 0;
            this.button_dec.Text = "Decrypt and export dump\r\n(to edit the save)";
            this.button_dec.UseVisualStyleBackColor = true;
            this.button_dec.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.binDumpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(354, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadAmiigoDumpToolStripMenuItem,
            this.toolStripSeparator1,
            this.save,
            this.saveas,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadAmiigoDumpToolStripMenuItem
            // 
            this.loadAmiigoDumpToolStripMenuItem.Name = "loadAmiigoDumpToolStripMenuItem";
            this.loadAmiigoDumpToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.loadAmiigoDumpToolStripMenuItem.Text = "Load amiigo dump";
            this.loadAmiigoDumpToolStripMenuItem.Click += new System.EventHandler(this.loadAmiigoDumpToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);
            // 
            // save
            // 
            this.save.Enabled = false;
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(175, 22);
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // saveas
            // 
            this.saveas.Enabled = false;
            this.saveas.Name = "saveas";
            this.saveas.Size = new System.Drawing.Size(175, 22);
            this.saveas.Text = "Save as...";
            this.saveas.Click += new System.EventHandler(this.saveas_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(172, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // binDumpToolStripMenuItem
            // 
            this.binDumpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importAmiiboBinToolStripMenuItem,
            this.exportAmiiboBinToolStripMenuItem});
            this.binDumpToolStripMenuItem.Name = "binDumpToolStripMenuItem";
            this.binDumpToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.binDumpToolStripMenuItem.Text = "Bin dump";
            // 
            // importAmiiboBinToolStripMenuItem
            // 
            this.importAmiiboBinToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImportDecBin,
            this.ImportEncBin});
            this.importAmiiboBinToolStripMenuItem.Name = "importAmiiboBinToolStripMenuItem";
            this.importAmiiboBinToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.importAmiiboBinToolStripMenuItem.Text = "Import amiibo bin";
            // 
            // ImportDecBin
            // 
            this.ImportDecBin.Enabled = false;
            this.ImportDecBin.Name = "ImportDecBin";
            this.ImportDecBin.Size = new System.Drawing.Size(186, 22);
            this.ImportDecBin.Text = "Import decrypted bin";
            this.ImportDecBin.Click += new System.EventHandler(this.ImportDecBin_Click);
            // 
            // ImportEncBin
            // 
            this.ImportEncBin.Enabled = false;
            this.ImportEncBin.Name = "ImportEncBin";
            this.ImportEncBin.Size = new System.Drawing.Size(186, 22);
            this.ImportEncBin.Text = "Import encrypted bin";
            this.ImportEncBin.Click += new System.EventHandler(this.ImportEncBin_Click);
            // 
            // exportAmiiboBinToolStripMenuItem
            // 
            this.exportAmiiboBinToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDecBin,
            this.exportEncbin});
            this.exportAmiiboBinToolStripMenuItem.Name = "exportAmiiboBinToolStripMenuItem";
            this.exportAmiiboBinToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.exportAmiiboBinToolStripMenuItem.Text = "Export amiibo bin";
            // 
            // exportDecBin
            // 
            this.exportDecBin.Enabled = false;
            this.exportDecBin.Name = "exportDecBin";
            this.exportDecBin.Size = new System.Drawing.Size(183, 22);
            this.exportDecBin.Text = "Export decrypted bin";
            this.exportDecBin.Click += new System.EventHandler(this.exportDecBin_Click);
            // 
            // exportEncbin
            // 
            this.exportEncbin.Enabled = false;
            this.exportEncbin.Name = "exportEncbin";
            this.exportEncbin.Size = new System.Drawing.Size(183, 22);
            this.exportEncbin.Text = "Export encrypted bin";
            this.exportEncbin.Click += new System.EventHandler(this.exportEncbin_Click);
            // 
            // lbl_uid
            // 
            this.lbl_uid.AutoSize = true;
            this.lbl_uid.Location = new System.Drawing.Point(12, 36);
            this.lbl_uid.Name = "lbl_uid";
            this.lbl_uid.Size = new System.Drawing.Size(60, 13);
            this.lbl_uid.TabIndex = 2;
            this.lbl_uid.Text = "Dump UID:";
            // 
            // lbl_crc
            // 
            this.lbl_crc.AutoSize = true;
            this.lbl_crc.Location = new System.Drawing.Point(12, 62);
            this.lbl_crc.Name = "lbl_crc";
            this.lbl_crc.Size = new System.Drawing.Size(64, 13);
            this.lbl_crc.TabIndex = 3;
            this.lbl_crc.Text = "Dump MD5:";
            // 
            // lbl_key
            // 
            this.lbl_key.AutoSize = true;
            this.lbl_key.Location = new System.Drawing.Point(12, 86);
            this.lbl_key.Name = "lbl_key";
            this.lbl_key.Size = new System.Drawing.Size(161, 13);
            this.lbl_key.TabIndex = 4;
            this.lbl_key.Text = "Amiibo write key (based on UID):";
            // 
            // button_enc
            // 
            this.button_enc.Enabled = false;
            this.button_enc.Location = new System.Drawing.Point(189, 119);
            this.button_enc.Name = "button_enc";
            this.button_enc.Size = new System.Drawing.Size(153, 37);
            this.button_enc.TabIndex = 5;
            this.button_enc.Text = "Encrypt and import dump\r\n(to restore it on the amiibo)";
            this.button_enc.UseVisualStyleBackColor = true;
            this.button_enc.Click += new System.EventHandler(this.button_enc_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 26);
            this.label4.TabIndex = 6;
            this.label4.Text = "Amiigo and AmiigoPC by exelix11\r\nAmiibo decrypt/encrypt service by Socram8888";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(50, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 72);
            this.label1.TabIndex = 7;
            this.label1.Text = "Drag an amiigo dump here";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 199);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_enc);
            this.Controls.Add(this.lbl_key);
            this.Controls.Add(this.lbl_crc);
            this.Controls.Add(this.lbl_uid);
            this.Controls.Add(this.button_dec);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Amiigo pc 1.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Frm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Frm_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_dec;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAmiigoDumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem save;
        private System.Windows.Forms.ToolStripMenuItem saveas;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binDumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importAmiiboBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAmiiboBinToolStripMenuItem;
        private System.Windows.Forms.Label lbl_uid;
        private System.Windows.Forms.ToolStripMenuItem ImportDecBin;
        private System.Windows.Forms.ToolStripMenuItem ImportEncBin;
        private System.Windows.Forms.ToolStripMenuItem exportDecBin;
        private System.Windows.Forms.ToolStripMenuItem exportEncbin;
        private System.Windows.Forms.Label lbl_crc;
        private System.Windows.Forms.Label lbl_key;
        private System.Windows.Forms.Button button_enc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
    }
}

