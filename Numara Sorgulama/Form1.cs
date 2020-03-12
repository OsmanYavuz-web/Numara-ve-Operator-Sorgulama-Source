using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;

namespace Numara_Sorgulama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string kaynakKod = null;
        string kaynakKod2 = null;

        #region FORM_LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
           webBrowser1.Navigate(textBox3.Text);
           webBrowser2.Navigate(textBox4.Text);
        }
        #endregion

        #region Captcha Alma
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                textBox2.Clear();

                kaynakKod = webBrowser1.Document.Body.InnerHtml.ToString(); // site kaynak kod

                IHTMLDocument2 doc = (IHTMLDocument2)webBrowser1.Document.DomDocument; // unmanaged document nesnesini alıyoruz
                IHTMLControlRange imgler = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange(); // controlRange ile Html nesne dizisi oluşturuyoruz

                foreach (IHTMLImgElement img in doc.images) // Tüm img elementleri için
                {
                    imgler.add((IHTMLControlElement)img); // Koleksiyona elementi ekliyoruz
                    imgler.execCommand("copy", false, null); // Koleksiyonu Clipboard a kopyalıyoruz

                    using (Bitmap bmp = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap)) // Clipboard daki resmi bitmap olarak alıyoruz
                    {
                        pictureBox1.Image = Image.FromHbitmap(bmp.GetHbitmap()); // Picturebox ta gösteriyoruz
                    }
                    break;
                }
            }
            catch { }
        }
        #endregion

        #region Sorgu gönderme
        private void button1_Click(object sender, EventArgs e)
        {
            label3.Text = "Bekleniyor..";
            label6.Text = "Bekleniyor..";

            label3.ForeColor = Color.Black;
            label6.ForeColor = Color.Black;

            try
            {
                #region Numara Sorgulama
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Tüm alanları doldurmalısınız!", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (textBox1.TextLength == 1 || textBox1.TextLength < 0)
                {
                    MessageBox.Show("Telefon numarası 11 haneli olmalıdır.", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    webBrowser1.Document.GetElementById("no").InnerText = textBox1.Text;
                    webBrowser1.Document.GetElementById("guvkod").InnerText = textBox2.Text;
                    HtmlElementCollection elc = this.webBrowser1.Document.GetElementsByTagName("input");
                    foreach (HtmlElement el in elc)
                    {
                        if (el.GetAttribute("value").Equals("Numarayı Sorgula"))
                        {
                            el.InvokeMember("Click");
                        }
                    }
                }
                #endregion

                #region Operatör sorgulama
                string numara = textBox1.Text;
                numara = numara.Remove(0, 1);
                webBrowser2.Document.GetElementById("txtMsisdn").InnerText = numara;
                HtmlElementCollection elc2 = this.webBrowser2.Document.GetElementsByTagName("input");
                foreach (HtmlElement el2 in elc2)
                {
                    if (el2.GetAttribute("value").Equals("Sorgula"))
                    {
                        el2.InvokeMember("Click");
                    }
                }
                #endregion
            }
            catch { }
        }
        #endregion

        #region Kod Yenileme
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            webBrowser1.Refresh();
        }
        #endregion

        #region Yeni Sorgu
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser1.Refresh();
                webBrowser2.Navigate(textBox4.Text);
                textBox1.Text = "0";
                textBox2.Clear();
                label3.Text = "Bekleniyor..";
                label6.Text = "Bekleniyor..";

                label3.ForeColor = Color.Black;
                label6.ForeColor = Color.Black;
            }
            catch { }
        }
        #endregion

        #region Kontrol İşlemleri
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
              #region Numara sorgulama
                var links = webBrowser1.Document.GetElementsByTagName("div");
                foreach (HtmlElement link in links)
                {
                    if (link.GetAttribute("id") == "sonuc")
                    {
                        label3.Text = link.InnerText;
                    }
                }

                if (label3.Text.IndexOf("Güvenlik Kodu Yanlış") != -1)
                {
                    webBrowser1.Refresh();
                    label3.ForeColor = Color.DarkRed;
                    label3.Text = "Güvenlik Kodu Yanlış";
                    textBox2.Clear();
                        
                }
                else if (label3.Text.IndexOf("Numara Bulunamadı") != -1)
                {
                    webBrowser1.Refresh();
                    label3.ForeColor = Color.DarkRed;
                    label3.Text = "Numara Bulunamadı";
                    textBox2.Clear();
                }
                else
                {
                    label3.ForeColor = Color.DarkBlue;
                }
                #endregion

              #region Operatör sorgulama
                if (kaynakKod2.IndexOf("hizmet aldığı işletmeci") != -1)
                {
                    label6.ForeColor = Color.DarkBlue;
                    string gelen = kaynakKod2;
                    int titleIndexBaslangici = gelen.IndexOf(textBox6.Text) + textBox6.TextLength;
                    int titleIndexBitisi = gelen.Substring(titleIndexBaslangici).IndexOf("</DIV>");
                    string cikti = gelen.Substring(titleIndexBaslangici, titleIndexBitisi).Remove(0, 11).Replace("numarası", "Numara");
                    label6.Text = cikti.Replace("hizmet aldığı işletmeci: ","");
                }
                else
                {
                    label6.Text = "Bekleniyor..";
                    label6.ForeColor = Color.Black;
                }

                if (kaynakKod2.IndexOf("numarası bir işletmeciye ait değildir.") != -1)
                {
                    label6.Text = "Bu numara herhangi bir işletmeciye ait değildir";
                }
              #endregion
            }
            catch { }
        }
        #endregion

        #region Operatör sorgulama site kaynak kod
        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                kaynakKod2 = webBrowser2.Document.Body.InnerHtml.ToString(); // site kaynak kod
                textBox5.Text = kaynakKod2;
            }
            catch { }
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void programıKapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void programHakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }

    }
}
