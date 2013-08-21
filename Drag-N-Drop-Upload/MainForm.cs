using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace Drag_N_Drop_Upload
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AllowDrop = true;
            DragEnter += new DragEventHandler(Form1_DragEnter);
            DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))// && IsImage((string)e.Data.GetData(DataFormats.FileDrop)))
            {
                /*string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool allowed = false;

                foreach (string file in files)
                    if (!IsImage(file))
                        allowed = false;

                if (allowed)*/
                    e.Effect = DragDropEffects.Copy;
            }
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string filename in files)
            {
                //! Looks like shit so just disable it. :)
                //System.Drawing.Image img = System.Drawing.Image.FromFile(filename);
                //pictureBox.Image = img;

                using (var w = new WebClient())
                {
                    var values = new NameValueCollection
                    {
                        {"image", Convert.ToBase64String(File.ReadAllBytes(filename))}
                    };

                    w.Headers.Add("Authorization", "Client-ID 3290b3fd5c28c93"); //! Client id I got from the API page when registrating the app...
                    byte[] response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);
                    string responseStr = Encoding.Default.GetString(response);

                    var xml = XElement.Parse(responseStr);

                    if (xml.Attribute("success").Value == "1")
                    {
                        string uploadedURL = xml.Element("link").Value;
                        Process.Start(uploadedURL);
                        listBoxUploads.Items.Add(uploadedURL);
                    }
                    else
                        MessageBox.Show("Uploading failed..", "Something went wrong!");
                }
            }
        }

        static bool IsImage(string filename)
        {
            try
            {
                return System.Drawing.Image.FromFile(filename).RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
