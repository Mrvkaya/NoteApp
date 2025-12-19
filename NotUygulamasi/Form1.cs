using System;
using System.IO;
using System.Windows.Forms;

namespace NotUygulamasi
{
    public partial class Form1 : Form
    {
        static int sonDosyalarSayisi = 20;
        string notesFolder = @"C:\DENEMENOTLAR";
        string seciliDosyaYolu = "";
        string[,] sonDosyalar = new string[sonDosyalarSayisi, 2];
        string sonDosyalarDosyasi = @"C:\DENEMENOTLAR\sondosyalar.txt"; //SON DOSYALAR DİZİSİNİN KALICI OLARAK KAYDEDİLECEĞİ DOSYA

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            lstDosyalar.SelectedIndexChanged += lstDosyalar_SelectedIndexChanged;
        }

        private void Form1_Load(object sender, EventArgs e)  // FORMLOAD DA DOSYAYI OKUYUP DİZİYE KAYDETTİM
        {
            try
            {
                SonDosyalariYukle();
                NotlariListele();
                SecilenButonlariAyarla();
            }
            catch
            {
                MessageBox.Show("Notlar yüklenemedi.");
            }
        }

        private void SecilenButonlariAyarla()
        {
            bool seciliVar = !string.IsNullOrEmpty(seciliDosyaYolu);
            btnSil.Enabled = seciliVar;
            btnGuncelle.Enabled = seciliVar;
            btnYeni.Enabled = seciliVar;
            btnKaydet.Enabled = !seciliVar;
            btnVazgec.Enabled = !false;
        }

        // NOTLARI LİSTELE

        private void NotlariListele()
        {
            lstDosyalar.Items.Clear();

            for (int i = 0; i < sonDosyalarSayisi; i++)
            {
                if (!string.IsNullOrEmpty(sonDosyalar[i, 0]))
                {
                    lstDosyalar.Items.Add(sonDosyalar[i, 0]);
                }
            }
        }


        // LİSTEDEN SEÇ OKU

        private void lstDosyalar_SelectedIndexChanged(object sender, EventArgs e) //SEÇİLİ OLAN DOSYA ADINI DİZİDE ARATIP ONUN YOLUNU SEÇİLİ DOSYA YOLUNA ATADIM
        {
            if (lstDosyalar.SelectedIndex == -1)
                return;

            int index = lstDosyalar.SelectedIndex;
            seciliDosyaYolu = sonDosyalar[index, 1];

            if (File.Exists(seciliDosyaYolu))
                txtNotlar.Text = File.ReadAllText(seciliDosyaYolu);
            else
                txtNotlar.Clear();

            SecilenButonlariAyarla();
        }


        // KAYDET

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNotlar.Text))
            {
                MessageBox.Show("Not boş olamaz");
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = notesFolder;
            dlg.Filter = "Text Files (*.txt)|*.txt";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            seciliDosyaYolu = dlg.FileName;
            File.WriteAllText(seciliDosyaYolu, txtNotlar.Text);

            DosyayiUsteEkle(seciliDosyaYolu);

            SonDosyalariKaydet();
            //oncekiIcerik = txtNotlar.Text;
            NotlariListele();
            lstDosyalar.SelectedIndex = 0;
            SecilenButonlariAyarla();
        }


        // GÜNCELLE

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(seciliDosyaYolu))
                return;

            File.WriteAllText(seciliDosyaYolu, txtNotlar.Text);
            // oncekiIcerik = txtNotlar.Text;
            MessageBox.Show("Not güncellendi");
        }


        // YENİ

        private void btnYeni_Click(object sender, EventArgs e)
        {
            txtNotlar.Clear();
            seciliDosyaYolu = "";
            lstDosyalar.ClearSelected();
            SecilenButonlariAyarla();
        }


        // SİLME İŞLEMİ

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (lstDosyalar.SelectedIndex == -1)
                return;

            int index = lstDosyalar.SelectedIndex;

            if (File.Exists(sonDosyalar[index, 1]))
                File.Delete(sonDosyalar[index, 1]);

            for (int i = index; i < sonDosyalarSayisi; i++)
            {
                sonDosyalar[i, 0] = sonDosyalar[i + 1, 0];
                sonDosyalar[i, 1] = sonDosyalar[i + 1, 1];
            }

            sonDosyalar[sonDosyalarSayisi - 1, 0] = "";
            sonDosyalar[sonDosyalarSayisi - 1, 1] = "";

            SonDosyalariKaydet();
            NotlariListele();
            txtNotlar.Clear();
            seciliDosyaYolu = "";
            SecilenButonlariAyarla();
        }


        // AÇ (HARİCİ TXT)

        private void btnAc_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text Files (*.txt)|*.txt";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            seciliDosyaYolu = dlg.FileName;
            txtNotlar.Text = File.ReadAllText(seciliDosyaYolu);


            DosyayiUsteEkle(seciliDosyaYolu);

            SonDosyalariKaydet();
            NotlariListele();
            lstDosyalar.SelectedIndex = 0;
            SecilenButonlariAyarla();
        }


        // YARDIMCI METOTLAR

        private void DosyayiUsteEkle(string dosyaYolu)
        {
            // varsa sil
            for (int i = 0; i < sonDosyalarSayisi; i++)
            {
                if (sonDosyalar[i, 1] == dosyaYolu)
                {
                    for (int j = i; j < sonDosyalarSayisi - 1; j++)
                    {
                        sonDosyalar[j, 0] = sonDosyalar[j + 1, 0];
                        sonDosyalar[j, 1] = sonDosyalar[j + 1, 1];
                    }
                    sonDosyalar[sonDosyalarSayisi - 1, 0] = "";
                    sonDosyalar[sonDosyalarSayisi - 1, 1] = "";
                    break;
                }
            }

            // KAYDIR
            for (int i = sonDosyalarSayisi-1; i > 0; i--)
            {
                if (sonDosyalar[i - 1, 0] != null)
                {
                    sonDosyalar[i, 0] = sonDosyalar[i - 1, 0];
                    sonDosyalar[i, 1] = sonDosyalar[i - 1, 1];
                }
            }

            sonDosyalar[0, 0] = Path.GetFileNameWithoutExtension(dosyaYolu);
            sonDosyalar[0, 1] = dosyaYolu;
        }

        private void SonDosyalariKaydet() //SON DOSYALAR DİZİSİNİ BİR DOSYAYA KAYDETTİM
        {
            using (StreamWriter sw = new StreamWriter(sonDosyalarDosyasi))
            {
                for (int i = 0; i < sonDosyalarSayisi; i++)
                {
                    if (!string.IsNullOrEmpty(sonDosyalar[i, 0]))
                        sw.WriteLine($"{sonDosyalar[i, 0]}|{sonDosyalar[i, 1]}");
                }
            }
        }

        private void SonDosyalariYukle()
        {
            if (!File.Exists(sonDosyalarDosyasi))
                return;

            var satirlar = File.ReadAllLines(sonDosyalarDosyasi);
            int donguSayisi = Math.Min(satirlar.Length, sonDosyalarSayisi);

            for (int i = 0; i < donguSayisi; i++)
            {
                string satir = satirlar[i];
                string[] p = satir.Split('|');

                if (p.Length == 2)
                {
                    sonDosyalar[i, 0] = p[0];
                    sonDosyalar[i, 1] = p[1];
                }
            }
        }

        //VAZGEÇ
        private void btnVazgec_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(seciliDosyaYolu))
            {
                txtNotlar.Clear();
                return;
            }
            txtNotlar.Text = File.ReadAllText(seciliDosyaYolu);
        }
    }
}
