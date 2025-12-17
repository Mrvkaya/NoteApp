using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NotUygulamasi
{
    public partial class Form1 : Form
    {
        string notesFolder = @"C:\DENEMENOTLAR";
        string seciliDosyaYolu = "";

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(notesFolder))
                Directory.CreateDirectory(notesFolder);

            NotlariListele();
        }

        // NOTLARI LİSTELEME İŞLEMİ
        private void NotlariListele()
        {
            var liste = Directory.GetFiles(notesFolder, "*.txt")
                .Select(f =>
                {
                    string ilkSatir = File.ReadLines(f).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(ilkSatir))
                        ilkSatir = "(Başlıksız Not)";

                    return new NoteItem
                    {
                        Title = ilkSatir,
                        FilePath = f
                    };
                })
                .OrderByDescending(x => x.FilePath)
                .ToList();

            listBox1.DataSource = null;
            listBox1.DisplayMember = "Title";
            listBox1.ValueMember = "FilePath";
            listBox1.DataSource = liste;

            if (!string.IsNullOrEmpty(seciliDosyaYolu))
            {
                var secilecek = liste.FirstOrDefault(x => x.FilePath == seciliDosyaYolu);
                if (secilecek != null)
                    listBox1.SelectedItem = secilecek;
            }
        }

        // NOTLARI OKUMA İŞLEMİ
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            var item = (NoteItem)listBox1.SelectedItem;
            seciliDosyaYolu = item.FilePath;

            if (File.Exists(seciliDosyaYolu))
                textBox1.Text = File.ReadAllText(seciliDosyaYolu);
        }

        // YENİ NOT KAYDETME İŞLEMİ
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Not boş olamaz");
                return;
            }

            if (!string.IsNullOrEmpty(seciliDosyaYolu))
            {
                MessageBox.Show("Bu not zaten var. Güncelle kullan.");
                return;
            }

            string dosyaAdi = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            seciliDosyaYolu = Path.Combine(notesFolder, dosyaAdi);

            File.WriteAllText(seciliDosyaYolu, textBox1.Text);
            NotlariListele();
        }

        // GÜNCELLEME İŞLEMİ
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(seciliDosyaYolu))
            {
                MessageBox.Show("Güncellenecek not seçmedin");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Not boş olamaz");
                return;
            }

            File.WriteAllText(seciliDosyaYolu, textBox1.Text);
            NotlariListele();
        }

        // YENİ
        private void btnYeni_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            seciliDosyaYolu = "";
            listBox1.ClearSelected();
        }

        // SİLME İŞLEMİ
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(seciliDosyaYolu))
            {
                MessageBox.Show("Silinecek not seçmedin");
                return;
            }

            File.Delete(seciliDosyaYolu);

            textBox1.Clear();
            seciliDosyaYolu = "";
            NotlariListele();
        }
    }

    public class NoteItem
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
    }
}
