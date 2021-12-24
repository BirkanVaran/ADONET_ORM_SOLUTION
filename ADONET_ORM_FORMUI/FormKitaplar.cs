using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADONET_ORM_BLL;
using ADONET_ORM_Entites;
using ADONET_ORM_Entites.Entites;

namespace ADONET_ORM_FORMUI
{
    public partial class FormKitaplar : Form
    {
        public FormKitaplar()
        {
            InitializeComponent();
        }
        // GLOBAL ALAN
        YazarlarORM myYazarORM = new YazarlarORM();
        TurlerORM myTurlerORM = new TurlerORM();
        KitaplarORM myKitapORM = new KitaplarORM();

        private void btnKitapEkle_Click(object sender, EventArgs e)
        {

        }

        private void FormKitaplar_Load(object sender, EventArgs e)
        {
            TumKitaplariGridViewModelleGetir();
            TumYazarlariComboyaGetir();
            TumTurleriComboyaGetir();
            TumKitaplariSilComboyaGetir();
            TumKitaplariGuncelleComboyaGetir();

            // Comboboxların içine yazı yazılmasın diye, ComboBox'ların style'larını düzenleyeceğiz.
            // 1. yöntem: Tek tek ComboBox'ların isimlerinden, ilgili property'i düzenlemektir:
            comboBoxKitapGuncelle.DropDownStyle = ComboBoxStyle.DropDownList;
            // 2. yöntem:  freach döngüsüyle form controlleri taranarak comboBox bulursa bulduğu nesnein ilgili property'sini düzenlemektir.
            foreach (var item in this.Controls)
            {
                if (item is TabControl)
                {
                    foreach (var subItem in ((TabControl)item).Controls)
                    {
                        if (subItem is TabPage)
                        {
                            foreach (var subOfSubItem in ((TabPage)subItem).Controls)
                            {
                                //if ((subOfSubItem is ComboBox)) kullanmak istemezsek;
                                if (subOfSubItem.GetType()==typeof(ComboBox))
                                {
                                    ((ComboBox)subOfSubItem).DropDownStyle = ComboBoxStyle.DropDownList;
                                }
                            }
                        }
                    }
                }
            }

            //3. Yöntem - For ile daha kısası:
            for (int i = 0; i < this.Controls[0].Controls.Count; i++)
            {
                for (int k = 0; k < this.Controls[0].Controls[i].Controls.Count; k++)
                {
                    if (this.Controls[0].Controls[i].Controls[k] is ComboBox)
                    {
                        ((ComboBox)this.Controls[0].Controls[i].Controls[k]).DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                }

            }

            // 4. Yöntem En Kısa:

        }

        private void TumKitaplariGuncelleComboyaGetir()
        {
            comboBoxKitapGuncelle.DisplayMember = "KitapAdi";
            comboBoxKitapGuncelle.ValueMember = "KitapId";
            comboBoxKitapGuncelle.DataSource = myKitapORM.Select();
        }

        private void TumKitaplariGridViewModelleGetir()
        {
            dataGridViewKitaplar.DataSource = myKitapORM.KitaplariViewModelleGetir();

            dataGridViewKitaplar.Columns["SilindiMi"].Visible = false;
            dataGridViewKitaplar.Columns["TurId"].Visible = false;
            dataGridViewKitaplar.Columns["YazarId"].Visible = false;
            for (int i = 0; i < dataGridViewKitaplar.Columns.Count; i++)
            {
                dataGridViewKitaplar.Columns[i].Width = 120;
            }
        }

        private void TumTurleriComboyaGetir()
        {
            cmbBox_Ekle_Tur.DisplayMember = "TurAdi";
            cmbBox_Ekle_Tur.ValueMember = "TurId";
            cmbBox_Ekle_Tur.DataSource = myTurlerORM.TurleriGetir();
            cmbBox_Ekle_Tur.SelectedIndex = 0;

            cmbBox_Guncelle_Tur.DisplayMember = "TurAdi";
            cmbBox_Guncelle_Tur.ValueMember = "TurId";
            cmbBox_Guncelle_Tur.DataSource = myTurlerORM.TurleriGetir();
        }

        private void TumYazarlariComboyaGetir()
        {
            cmbBox_Ekle_Yazar.DisplayMember = "YazarAdSoyad";
            cmbBox_Ekle_Yazar.ValueMember = "YazarId";
            cmbBox_Ekle_Yazar.DataSource = myYazarORM.Select();

            cmbBox_Guncelle_Yazar.DisplayMember = "YazarAdSoyad";
            cmbBox_Guncelle_Yazar.ValueMember = "YazarId";
            cmbBox_Guncelle_Yazar.DataSource = myYazarORM.Select();

        }

        private void btnKitapEkle_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown_Ekle_SayfaSayisi.Value <= 0)
                {
                    MessageBox.Show("Sayfa sayısı 0'dan büüyk olmalıdır.");
                    return;
                }
                else if (numericUpDown_Ekle_Stok.Value <= 0)
                {
                    MessageBox.Show("Stok sıfırdan büyük olmalıdır.");
                    return;
                }
                if ((int)cmbBox_Ekle_Yazar.SelectedValue <= 0)
                {
                    MessageBox.Show("Kitabın bir yazarı olmalıdır.");
                    return;
                }
                Kitap yeniKitap = new Kitap()
                {
                    KayitTarihi = DateTime.Now,
                    KitapAdi = txtKitapEkle.Text.Trim(),
                    SayfaSayisi = (int)numericUpDown_Ekle_SayfaSayisi.Value,
                    Stok = (int)numericUpDown_Ekle_Stok.Value,
                    SilindiMi = false,
                    YazarId = (int)cmbBox_Ekle_Yazar.SelectedValue
                };
                if ((int)cmbBox_Ekle_Tur.SelectedValue == -1)
                {
                    yeniKitap.TurId = null;
                }
                else
                {
                    yeniKitap.TurId = (int)cmbBox_Ekle_Tur.SelectedValue;
                }

                if (myKitapORM.Insert(yeniKitap))
                {
                    MessageBox.Show($"{yeniKitap.KitapAdi.ToString()} isimli kitap başarıyla eklendi.");
                    TumKitaplariGridViewModelleGetir();
                    // Temizlik
                    EkleSayfasindakiKontrolleriTemizle();
                }
                // TumKitaplariGridViewModelleGetir()
                SilmeSayfasiKontrolleriTemizle();
                TumKitaplariSilComboyaGetir();


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void EkleSayfasindakiKontrolleriTemizle()
        {
            txtKitapEkle.Clear();
            cmbBox_Ekle_Yazar.SelectedIndex = -1;
            cmbBox_Ekle_Tur.SelectedIndex = -1;
            numericUpDown_Ekle_SayfaSayisi.Value = 0;
            numericUpDown_Ekle_Stok.Value = 0;
        }

        private void SilmeSayfasiKontrolleriTemizle()
        {
            cmbBox_Sil_Kitap.SelectedIndex = -1;
            richTextBoxKitap.Clear();
        }

        private void TumKitaplariSilComboyaGetir()
        {
            cmbBox_Sil_Kitap.DisplayMember = "KitapAdi";
            cmbBox_Sil_Kitap.ValueMember = "KitapId";
            // cmbBox_Sil_Kitap.DataSource = myKitapORM.Select();
            //Yukarıdaki gibi yapmak istemezsek yani
            // KitaplarORM class'ından instance almak istemezsek 
            // class içine tanımladığımız static property aracılığıyla o instance'a ulaşmış oluruz
            // aslında burada kendimize arka planda instance oluşturuyoruz ve static nesne aracılığıyla o nesneyi kullanıyoruz.

            cmbBox_Sil_Kitap.DataSource = KitaplarORM.Current.Select();

        }

        private void btnKitapSil_Click(object sender, EventArgs e)
        {
            try
            {

                if ((int)cmbBox_Sil_Kitap.SelectedValue <= 0)
                {
                    MessageBox.Show("Lütfen kitap seçimi yapınız.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;

                }
                Kitap kitabim = myKitapORM.SelectET((int)cmbBox_Sil_Kitap.SelectedValue);


                DialogResult cevap = MessageBox.Show($"Bu kitabı silmek yerine pasifleştirmek ister misiniz?", "SİLME ONAY", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (cevap == DialogResult.Yes)
                {
                    // Pasifleştirme. - UPDATE ile yapılmalı.
                    kitabim.SilindiMi = true;
                    switch (myKitapORM.Update(kitabim))
                    {
                        case true:
                            MessageBox.Show($"{kitabim.KitapAdi} kitabı pasifleştirildi.");
                            SilmeSayfasiKontrolleriTemizle();
                            TumKitaplariSilComboyaGetir();

                            break;
                        case false: throw new Exception($"HATA! - {kitabim.KitapAdi} kitabı pasifleştirilirken beklenmeyen bir hata meydana geldi."); break;
                    }

                }
                else if (cevap == DialogResult.No)
                {
                    // Silme
                    int kitapID = (int)cmbBox_Sil_Kitap.SelectedValue;
                    var oduncListe = OduncIslemlerORM.Current.Select().Where(x => x.KitapId == kitabim.KitapId).ToList();
                    if (oduncListe.Count > 0)
                    {
                        MessageBox.Show("Dikkat! Bu kitap ödünç alınmıştır, silinemez.");
                        return;
                    }
                    // Yukarıdaki if'e girmezse return olmaz.
                    // Return olmadığında kod aşağı doğru okunmaya devam eder.

                    if (myKitapORM.Delete(kitabim))
                    {
                        MessageBox.Show($"{kitabim.KitapAdi} adlı kitap silindi.");
                        SilmeSayfasiKontrolleriTemizle();
                        TumKitaplariSilComboyaGetir();
                    }
                    else
                    {
                        throw new Exception($"HATA {kitabim.KitapAdi} adlı kitap silinemedi.");
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void comboBoxKitapGuncelle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GuncelleSayfasiTemizle();
                if (comboBoxKitapGuncelle.SelectedIndex >= 0)
                {
                    Kitap secilenKitap = myKitapORM.SelectET((int)comboBoxKitapGuncelle.SelectedValue);
                    txt_GuncelleKitapAdi.Text = secilenKitap.KitapAdi;
                    numericUpDown_Guncelle_SayfaSayisi.Value = secilenKitap.SayfaSayisi;
                    numericUpDown_Guncelle_Stok.Value = secilenKitap.Stok;
                    cmbBox_Guncelle_Yazar.SelectedValue = secilenKitap.YazarId;

                    if (secilenKitap.TurId == null)
                    {
                        //cmbBox_Guncelle_Tur.SelectedIndex = 0;
                        //cmbBox_Guncelle_Tur.SelectedValue = -1
                        cmbBox_Guncelle_Tur.SelectedValue = Sabitler.TurYokSelectedValue;
                        // betul şuradaki -1 ile ilgili static classtan birşey göster hatırlatma notebook 
                    }
                    else
                    {
                        cmbBox_Guncelle_Tur.SelectedValue = secilenKitap.TurId;
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("HATA:" + ex.Message);
            }
        }

        private void GuncelleSayfasiTemizle()
        {
            txt_GuncelleKitapAdi.Text = string.Empty;
            numericUpDown_Guncelle_SayfaSayisi.Value = 0;
            numericUpDown_Guncelle_Stok.Value = 0;
            cmbBox_Guncelle_Tur.SelectedIndex = -1;
            cmbBox_Guncelle_Yazar.SelectedIndex = -1;

        }

        private void btnKitapGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxKitapGuncelle.SelectedIndex >= 0)
                {
                    if (numericUpDown_Guncelle_SayfaSayisi.Value <= 0)
                    {
                        throw new Exception("HATA");
                    }
                    if (numericUpDown_Guncelle_Stok.Value <= 0)
                    {
                        throw new Exception("HATA");
                    }
                    Kitap secilenKitap = myKitapORM.SelectET((int)comboBoxKitapGuncelle.SelectedValue);
                    if (secilenKitap == null)
                    {
                        throw new Exception("HATA: Kitap bulunamadı.");
                    }
                    else
                    {
                        secilenKitap.KitapAdi = txt_GuncelleKitapAdi.Text.Trim();
                        secilenKitap.Stok = (int)numericUpDown_Guncelle_Stok.Value;
                        secilenKitap.SilindiMi = false;
                        secilenKitap.YazarId = (int)cmbBox_Guncelle_Yazar.SelectedValue;
                        if ((int)cmbBox_Guncelle_Tur.SelectedValue == -1)
                        {
                            secilenKitap.TurId = null;
                        }
                        else
                        {
                            secilenKitap.TurId = (int)cmbBox_Guncelle_Tur.SelectedValue;
                        }
                        switch (myKitapORM.Update(secilenKitap))
                        {
                            case true:
                                MessageBox.Show("Başarıyla güncellendi");
                                TumKitaplariGuncelleComboyaGetir();
                                TumKitaplariGridViewModelleGetir();
                                TumKitaplariSilComboyaGetir();
                                break;
                            case false:
                                throw new Exception("HATA");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void cmbBox_Sil_Kitap_SelectedIndexChanged(object sender, EventArgs e)
        {
            // RichTextBox dolsun.
            if (cmbBox_Sil_Kitap.SelectedIndex >= 0)
            {
                Kitap secilenKitap = myKitapORM.SelectET((int)cmbBox_Sil_Kitap.SelectedValue);

                if (secilenKitap != null)
                {
                    string turu = secilenKitap.TurId == null ? "Türü yok" : myTurlerORM.Select().FirstOrDefault(x => x.TurId == secilenKitap.TurId)?.TurAdi;
                    richTextBoxKitap.Text = $"Kitap Adı: {secilenKitap.KitapAdi}\n"
                                            + $"Türü: {turu} \n"
                                            + $"Yazar:  {myYazarORM.Select().FirstOrDefault(x => x.YazarId == secilenKitap.YazarId).YazarAdSoyad}\n"
                                            + $"Sayfa Sayısı: {secilenKitap.SayfaSayisi}\n"
                                            + $"Stok'ta {secilenKitap.Stok} adet var.";
                }
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            // Tablar değiştikçe temizlik yapılsın.
            EkleSayfasindakiKontrolleriTemizle();
            GuncelleSayfasiTemizle();
            SilmeSayfasiKontrolleriTemizle();
            comboBoxKitapGuncelle.SelectedIndex = -1;
            
        }
    }
}
