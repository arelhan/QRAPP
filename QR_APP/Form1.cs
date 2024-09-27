using QRCoder;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QR_APP
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Form1()
        {
            InitializeComponent();
            // Formun boş alanlarına tıklama olayını ayarlayın
            this.MouseDown += new MouseEventHandler(Form_MouseDown);
            this.MouseMove += new MouseEventHandler(Form_MouseMove);
            this.MouseUp += new MouseEventHandler(Form_MouseUp);
        }


        private void btn_Create_Click(object sender, EventArgs e)
        {
            // QR koduna eklenmek istenen link
            string url = txt_url.Text;
            

            if (string.IsNullOrEmpty(url))
            {
                lbl_info.Text = "Lütfen bir URL girin."; // Hata mesajını Label üzerinde göster
                return;
            }

            // QR kodu oluşturucu
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                try
                {
                    // QR kod verilerini oluştur
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

                    // QR kodunu Bitmap olarak oluştur
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20)) // 20, modül başına piksel sayısı
                        {
                            // SaveFileDialog ile kaydedilecek yeri ve dosya adını sor
                            SaveFileDialog saveFileDialog = new SaveFileDialog
                            {
                                Filter = "PNG Dosyası|*.png",
                                Title = "QR Kodunu Kaydet",
                                FileName = "AREqrcode_"+DateTime.Now.ToString("yyyy_MM_dd_HHmmss")+"_.png",  // Varsayılan dosya adı
                                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)  // Varsayılan klasör Masaüstü
                            };

                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                // Dosyayı PNG olarak kaydet
                                qrCodeImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                lbl_info.Text = "QR kodu başarıyla kaydedildi: ";

                                // Dosyanın bulunduğu klasörü aç
                                string folderPath = Path.GetDirectoryName(saveFileDialog.FileName);
                                if (!string.IsNullOrEmpty(folderPath))
                                {
                                    Process.Start("explorer.exe", folderPath);
                                }
                            }
                            else
                            {
                                lbl_info.Text = "Dosya kaydedilmedi.";
                            }
                        }
                    }
                }
                catch
                {
                    lbl_info.Text = $"QR kod oluşturulamadı";  // Hata mesajını Label'a yazdır
                }
            }
        }
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // Eğer sol fare butonuna tıklanmışsa
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                // Tıklanan konumu ve formun konumunu kaydet
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            // Eğer form taşınıyorsa
            if (dragging)
            {
                // Formu yeni konuma taşı
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            // Fare tuşu bırakıldığında sürüklemeyi durdur
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
            }
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
