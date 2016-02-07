using System.IO;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace AmiigoPC
{
    public partial class Form1 : Form
    {
        byte[] FullDump;
        byte[] AmiiboDump;
        byte[] AmiiboUID;
        string LoadedFile; 

        public Form1()
        {
            InitializeComponent();
        }

        void OpenFile(string file)
        {
            try
            {
                label1.Visible = false;
                AmiiboDump = new byte[0x214];
                AmiiboUID = new byte[0x7];
                FullDump = File.ReadAllBytes(file);
                if (FullDump.Length != 0x22D)
                {
                    MessageBox.Show("This file is not an amiigo dump, wrong size");
                    FullDump = new byte[0];
                    return;
                }
                byte[] magic = new byte[0xA];
                System.Buffer.BlockCopy(FullDump, 0x00, magic, 0, 0xA);
                if (!ByteArrayEqual(magic, new byte[] { 0x41, 0x6D, 0x69, 0x69, 0x67, 0x6F, 0x44, 0x75, 0x6D, 0x70 }))
                {
                    MessageBox.Show("This file is not an amiigo dump");
                    FullDump = new byte[0];
                    return;
                }
                System.Buffer.BlockCopy(FullDump, 0x0E, AmiiboUID, 0, 0x7);
                System.Buffer.BlockCopy(FullDump, 0x19, AmiiboDump, 0, 0x214);
                lbl_uid.Text = "Dump UID: " + BitConverter.ToString(AmiiboUID).Replace("-"," ");
                MD5 hash = MD5.Create();
                lbl_crc.Text = "Dump MD5: " + BitConverter.ToString(hash.ComputeHash(AmiiboDump)).Replace("-", " ");
                lbl_key.Text = "Amiibo write key (based on UID): " + BitConverter.ToString(GetPWD(AmiiboUID)).Replace("-", " "); 
                UnlockGui();
                LoadedFile = file;
            }
            catch (Exception ex)
            {
                FullDump = new byte[0];
                AmiiboDump = new byte[0x214];
                AmiiboUID = new byte[0x7];
                label1.Visible = true;
                MessageBox.Show("Error on loading file: " + ex.Message);
                LockGUI();
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = "Bin file|*.bin";
            if (sav.ShowDialog() == DialogResult.OK)
            {
                byte[] data = DecryptDump(AmiiboDump);
                if (data == null)
                {
                    MessageBox.Show("Error: returned byte[] == null\r\nYou must be connected to the Internet to decrypt a dump");
                    return;
                }
                File.WriteAllBytes(sav.FileName,data);
                MessageBox.Show("Done !");
            }
        }

        byte[] DecryptDump(byte[] encBin)
        {
            RestClient client = new RestClient("https://www.socram.ovh/amiibo/");
            RestRequest request = new RestRequest("api.php", Method.POST);
            request.AddParameter("MAX_FILE_SIZE", 540);
            request.AddParameter("operation", "decrypt");
            request.AddFileBytes("dump", encBin, @"Dump.bin");
            return client.DownloadData(request);
        }

        byte[] EncryptDump(byte[] decBin)
        {
            RestClient client = new RestClient("https://www.socram.ovh/amiibo/");
            RestRequest request = new RestRequest("api.php", Method.POST);
            request.AddParameter("MAX_FILE_SIZE", 540);
            request.AddParameter("operation", "encrypt");
            request.AddFileBytes("dump", decBin, @"Dump.bin");
            return client.DownloadData(request);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LockGUI();
        }

        void UnlockGui()
        {
            button_dec.Enabled = true;
            button_enc.Enabled = true;
            save.Enabled = true;
            saveas.Enabled = true;
            ImportDecBin.Enabled = true;
            ImportEncBin.Enabled = true;
            exportDecBin.Enabled = true;
            exportEncbin.Enabled = true;
        }

        void LockGUI()
        {
            button_dec.Enabled = false;
            button_enc.Enabled = false;
            save.Enabled = false;
            saveas.Enabled = false;
            ImportDecBin.Enabled = false;
            ImportEncBin.Enabled = false;
            exportDecBin.Enabled = false;
            exportEncbin.Enabled = false;
        }

        private byte[] GetPWD(byte[] Uid)
        {
            List<byte> Res = new List<byte>();
            int XorVal = 0xaa;
            Res.Add((byte)((Uid[0x1] ^ Uid[0x3]) ^ XorVal));
            Res.Add((byte)((Uid[0x2] ^ Uid[0x4]) ^ XorVal/2));
            Res.Add((byte)((Uid[0x3] ^ Uid[0x5]) ^ XorVal));
            Res.Add((byte)((Uid[0x4] ^ Uid[0x6]) ^ XorVal/2));
            return Res.ToArray();
        }

        bool ByteArrayEqual(byte[] input1, byte[] input2)
        {
            try
            {
                if (input1.Length != input2.Length) return false;
                for (int i = 0; i < input1.Length; i++)
                {
                    if (input1[i] != input2[i]) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void loadAmiigoDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opn = new OpenFileDialog();
            opn.Filter = "Bin file|*.bin|*.*|*.*";
            if (opn.ShowDialog() == DialogResult.OK) OpenFile(opn.FileName);
        }

        private void button_enc_Click(object sender, EventArgs e)
        {
            OpenFileDialog opn = new OpenFileDialog();
            opn.Filter = "Bin file|*.bin|*.*|*.*";
            if (opn.ShowDialog() == DialogResult.OK)
            {
                byte[] data = EncryptDump(File.ReadAllBytes(opn.FileName));
                if (data == null)
                {
                    MessageBox.Show("Error: returned byte[] == nullf\r\nYou must be connected to the Internet to encrypt a dump");
                    return;
                }
                AmiiboDump = data;
                MD5 hash = MD5.Create();
                lbl_crc.Text = "Dump MD5: " + BitConverter.ToString(hash.ComputeHash(AmiiboDump)).Replace("-", " ");
                MessageBox.Show("Done !");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void save_Click(object sender, EventArgs e)
        {
            List<byte> data = new List<byte>();
            data.AddRange(new byte[] { 0x41, 0x6D, 0x69, 0x69, 0x67, 0x6F, 0x44, 0x75, 0x6D, 0x70 });
            data.AddRange(new byte[4]);
            data.AddRange(AmiiboUID);
            data.AddRange(new byte[4]);
            data.AddRange(AmiiboDump);
            File.Delete(LoadedFile);
            File.WriteAllBytes(LoadedFile, data.ToArray());
        }

        private void saveas_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = "Amiigo dump|*.amiigo";
            if (sav.ShowDialog() == DialogResult.OK)
            {
                List<byte> data = new List<byte>();
                data.AddRange(new byte[] { 0x41, 0x6D, 0x69, 0x69, 0x67, 0x6F, 0x44, 0x75, 0x6D, 0x70 });
                data.AddRange(new byte[4]);
                data.AddRange(AmiiboUID);
                data.AddRange(new byte[4]);
                data.AddRange(AmiiboDump);               
                File.WriteAllBytes(sav.FileName, data.ToArray());
            }
        }

        private void ImportEncBin_Click(object sender, EventArgs e)
        {
            OpenFileDialog opn = new OpenFileDialog();
            opn.Filter = "Bin file|*.bin|*.*|*.*";
            if (opn.ShowDialog() == DialogResult.OK)
            {
                AmiiboDump = File.ReadAllBytes(opn.FileName);
                MD5 hash = MD5.Create();
                lbl_crc.Text = "Dump MD5: " + BitConverter.ToString(hash.ComputeHash(AmiiboDump)).Replace("-", " ");
                MessageBox.Show("Done !");
            }
        }

        private void ImportDecBin_Click(object sender, EventArgs e)
        {
            button_enc_Click(null, null);
        }

        private void exportEncbin_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = "Bin file|*.bin";
            if (sav.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sav.FileName, AmiiboDump);
                MessageBox.Show("Done !");
            }
        }

        private void exportDecBin_Click(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void Frm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Frm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenFile(files[0]);
        }
    }

}
