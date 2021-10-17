using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;

namespace yemek_maliyet_hesaplama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection bgl = new SqlConnection(ConfigurationManager.ConnectionStrings["bgl"].ConnectionString);
        //SqlConnection bgl = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
        public void kayitlari_getir()
        {
            
            string getir = "select * from personel_info where state=1";
            SqlCommand komut = new SqlCommand(getir, bgl);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            personelkayıt_dt.DataSource = dt;
            personelkayıt_dt.Columns["id"].Visible = false;
            personelkayıt_dt.Columns["state"].Visible = false;
            bgl.Close();
        }
        public void kayitlari_getir2()
        {
            string getir = "select * from personel_info where state=1";
            SqlCommand komut = new SqlCommand(getir, bgl);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            gunlukgiris_dt.DataSource = dt;
            gunlukgiris_dt.Columns["state"].Visible = false;
            bgl.Close();
        }
        public void kayitlari_getir4()
        {
            string getir = "select * from personal_eat_food where date between @tarih1 and @tarih2";
            SqlCommand komut = new SqlCommand(getir, bgl);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            ad.SelectCommand.Parameters.AddWithValue("@tarih1", dateTimePicker4.Value.AddDays(-1));
            ad.SelectCommand.Parameters.AddWithValue("@tarih2", dateTimePicker4.Value);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            dataGridView4.DataSource = dt;
            //dataGridView4.Columns["id"].Visible = false;
            bgl.Close();
        }
        public void kayitlari_getir3()
        {
            int sayipersoneltopla = 0;
            int sayimisafirtopla = 0;
            int sifirEkGosterge = 0;
            int binyüzeKadarEkGosterge = 0;
            int binyüz_ikibinikiyüz_EkGosterge = 0;
            int ikibinikiyüzfazlası_EkGosterge = 0;
            int flag = 0;
            if (bgl.State == ConnectionState.Open)
            {
                bgl.Close();
            }
            bgl.Open();
            try
            {
                SqlCommand topla = new SqlCommand("select eat_piece , guest , ek_gosterge from personal_eat_food where date between @tarih1 and @tarih2 ", bgl);
                SqlDataAdapter ado = new SqlDataAdapter(topla);
                ado.SelectCommand.Parameters.AddWithValue("@tarih1", dateTimePicker2.Value.AddDays(-1));
                ado.SelectCommand.Parameters.AddWithValue("@tarih2", dateTimePicker3.Value);
                SqlDataReader verioku = topla.ExecuteReader();
                while (verioku.Read())
                {
                    sayipersoneltopla += Convert.ToInt32(verioku["eat_piece"]);
                    sayimisafirtopla += Convert.ToInt32(verioku["guest"]);
                    flag = Convert.ToInt32(verioku["ek_gosterge"]);
                    if (flag == 0)
                    {
                        sifirEkGosterge++;
                    }
                    else if (flag > 0 && flag <= 1100)
                    {
                        binyüzeKadarEkGosterge++;
                    }
                    else if (flag > 1100 && flag <= 2200)
                    {
                        binyüz_ikibinikiyüz_EkGosterge++;
                    }
                    else
                    {
                        ikibinikiyüzfazlası_EkGosterge++;
                    }
                }
                topla.Dispose();
                label31.Text = sifirEkGosterge.ToString();
                label32.Text = binyüzeKadarEkGosterge.ToString();
                label37.Text = binyüz_ikibinikiyüz_EkGosterge.ToString();
                label39.Text = ikibinikiyüzfazlası_EkGosterge.ToString();

                double personel_tutar = double.Parse(topmaliyet__txtbx.Text) - double.Parse(devletyardım__txtbx.Text);
                label24.Text = personel_tutar.ToString("0.##");
                label23.Text = (sayimisafirtopla + sayipersoneltopla).ToString();
                label15.Text = (sayimisafirtopla).ToString();
                label16.Text = (sayipersoneltopla).ToString();
                double toplammaliyet = double.Parse(topmaliyet__txtbx.Text);
                double toplamyemeksayisi = Convert.ToDouble(label23.Text);
                label22.Text = (toplammaliyet / toplamyemeksayisi).ToString("0.##");

                double toplaEkgostergesıfır = 0;
                double toplaEkgostergebir = 0;
                double toplaEkgostergeiki = 0;
                double toplaEkgostergeuc = 0;
                toplaEkgostergesıfır = (sifirEkGosterge) * (0.1);
                toplaEkgostergebir = (binyüzeKadarEkGosterge) * (0.2);
                toplaEkgostergeiki = (binyüz_ikibinikiyüz_EkGosterge) * (0.3);
                toplaEkgostergeuc = (ikibinikiyüzfazlası_EkGosterge) * (0.4);
                double toplananMiktar = toplaEkgostergesıfır + toplaEkgostergebir + toplaEkgostergeiki + toplaEkgostergeuc;
                double ortakÖdenecek = personel_tutar - toplananMiktar - ((toplammaliyet / toplamyemeksayisi) * sayimisafirtopla);
                double öde = (ortakÖdenecek / (sifirEkGosterge + binyüzeKadarEkGosterge + binyüz_ikibinikiyüz_EkGosterge + ikibinikiyüzfazlası_EkGosterge));
                label44.Text = (öde + 0.1).ToString("0.##");
                label45.Text = (öde + 0.2).ToString("0.##");
                label46.Text = (öde + 0.3).ToString("0.##");
                label47.Text = (öde + 0.4).ToString("0.##");
                verioku.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
            }
            finally
            {
                bgl.Close();
            }
            ////////////////////////////////////////////////////////////////
            string getir = "select name, surname, add_ind, note, state from personel_info where state=1";
            SqlCommand komut = new SqlCommand(getir, bgl);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            maliyet_dt.DataSource = dt;
            maliyet_dt.Columns["state"].Visible = false;
            bgl.Close();
            DateTime baslamaTarihi = new DateTime();
            DateTime bitisTarihi = new DateTime();
            baslamaTarihi = dateTimePicker2.Value.AddDays(-1);
            bitisTarihi = dateTimePicker3.Value;
            TimeSpan gunsayısı = bitisTarihi - baslamaTarihi;//Sonucu zaman olarak döndürür
            double toplamGun = gunsayısı.TotalDays;//toplamgun iki tarih arasındaki gün sayısını tutuyor
            string[] S_Dates = new string[Convert.ToInt32(toplamGun)];
            DateTime[] D_Dates = new DateTime[Convert.ToInt32(toplamGun)];

            for (int i = 0; i < Convert.ToInt32(toplamGun); i++)
            {
                S_Dates[i] = baslamaTarihi.AddDays(i + 1).ToString("dd\nMM\nyy");
                D_Dates[i] = baslamaTarihi.AddDays(i);
                dt.Columns.Add(S_Dates[i]);
                DataGridViewColumn column = maliyet_dt.Columns[i + 5];
                //maliyet_dt.Columns["reg_number"].Visible = false;
                //maliyet_dt.Columns["job_id"].Visible = false;
                //dataGridView3.Columns["add_ind"].Visible = false;
                maliyet_dt.Columns["note"].Visible = false;
                maliyet_dt.Columns["state"].Visible = false;
                column.Width = 30;
            }
            dt.Columns.Add("TOPLAM\nYEMEK");
            dt.Columns.Add("YARDIM\nALINAN\nYEMEK");
            dt.Columns.Add("TOLAM\nYEMEK\nMALİYET\nTUTARI");
            dt.Columns.Add("PERSONELİN\nKENDİ YEMEĞİ\nKARŞILIĞI\nÖDEYECEĞİ");
            dt.Columns.Add("PERSONELİN\nFAZLA YEDİĞİ\nYEMEK İÇİN\nÖDEYECEĞİ");
            dt.Columns.Add("PERSONELİN\nÖDEYECEĞİ\nTOPLAM\nMİKTAR");
            dt.Columns.Add("YEMEK\nYARDIMINDAN\nKARŞILANAN");
            int[] person_id = new int[personelkayıt_dt.Rows.Count];
            int[] ek_gosterge = new int[personelkayıt_dt.Rows.Count];
            for (int i = 0; i < personelkayıt_dt.Rows.Count; i++)
            {
                person_id[i] = Convert.ToInt32(personelkayıt_dt.Rows[i].Cells[0].Value);
                ek_gosterge[i] = Convert.ToInt32(personelkayıt_dt.Rows[i].Cells[5].Value);
            }

            int kendiYediği = 0;
            int misafir = 0;
            DateTime NowDate = new DateTime();
            int NowID = new int();
            for (int i = 0; i < personelkayıt_dt.Rows.Count; i++)
            {
                int toplamKendiYedigi = 0;
                int toplamMisafirininYedigi = 0;
                for (int j = 8; j < Convert.ToInt32(toplamGun) + 8; j++)
                {
                    bgl.Close();
                    bgl.Open();
                    NowDate = D_Dates[j - 8];
                    NowID = person_id[i];
                    SqlCommand topla = new SqlCommand("select eat_piece , guest from personal_eat_food where date between @NowDate and @NowDate1 and personal_id = @NowID ", bgl);
                    SqlDataAdapter ads = new SqlDataAdapter(topla);
                    ads.SelectCommand.Parameters.AddWithValue("@NowDate", D_Dates[j - 8]);
                    ads.SelectCommand.Parameters.AddWithValue("@NowDate1", NowDate.AddDays(1));
                    ads.SelectCommand.Parameters.AddWithValue("@NowID", person_id[i]);
                    SqlDataReader verioku = topla.ExecuteReader();
                    while (verioku.Read())
                    {
                        if (Convert.ToInt32(verioku["eat_piece"]) == 1 && Convert.ToInt32(verioku["guest"]) == 0)
                        {
                            kendiYediği = Convert.ToInt32(verioku["eat_piece"]);
                            misafir = 0;
                            maliyet_dt.Rows[i].Cells[j].Value = (kendiYediği + misafir);
                            toplamKendiYedigi += kendiYediği;
                        }
                        else if (Convert.ToInt32(verioku["eat_piece"]) == 1 && Convert.ToInt32(verioku["guest"]) >= 1)
                        {
                            kendiYediği = Convert.ToInt32(verioku["eat_piece"]);
                            misafir = Convert.ToInt32(verioku["guest"]);
                            maliyet_dt.Rows[i].Cells[j].Value = (kendiYediği + misafir);
                            toplamKendiYedigi += kendiYediği;
                            toplamMisafirininYedigi += misafir;
                        }
                    }
                }

                for (int k = 5 + Convert.ToInt32(toplamGun); k < 6 + Convert.ToInt32(toplamGun); k++)
                {
                    maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi + toplamMisafirininYedigi);
                }
                for (int k = 6 + Convert.ToInt32(toplamGun); k < 7 + Convert.ToInt32(toplamGun); k++)
                {
                    maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi);
                }
                for (int k = 7 + Convert.ToInt32(toplamGun); k < 8 + Convert.ToInt32(toplamGun); k++)
                {
                    maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi + toplamMisafirininYedigi) * Convert.ToDouble(label22.Text);
                    double sayi;
                    sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                    maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                }
                for (int k = 8 + Convert.ToInt32(toplamGun); k < 9 + Convert.ToInt32(toplamGun); k++)
                {
                    if (ek_gosterge[i] == 0)
                    {
                        maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi) * Convert.ToDouble(label44.Text);
                        double sayi;
                        sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                        maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                    }
                    else if (ek_gosterge[i] > 0 && ek_gosterge[i] <= 1100)
                    {
                        maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi) * Convert.ToDouble(label45.Text);
                        double sayi;
                        sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                        maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                    }
                    else if (ek_gosterge[i] > 1100 && ek_gosterge[i] <= 2200)
                    {
                        maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi) * Convert.ToDouble(label46.Text);
                        double sayi;
                        sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                        maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                    }
                    else
                    {
                        maliyet_dt.Rows[i].Cells[k].Value = (toplamKendiYedigi) * Convert.ToDouble(label47.Text);
                        double sayi;
                        sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                        maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                    }
                }
                for (int k = 9 + Convert.ToInt32(toplamGun); k < 10 + Convert.ToInt32(toplamGun); k++)
                {
                    maliyet_dt.Rows[i].Cells[k].Value = (toplamMisafirininYedigi) * Convert.ToDouble(label22.Text);
                    double sayi;
                    sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                    maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                }
                for (int k = 10 + Convert.ToInt32(toplamGun); k < 11 + Convert.ToInt32(toplamGun); k++)
                {
                    maliyet_dt.Rows[i].Cells[k].Value = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k - 2].Value) + Convert.ToDouble(maliyet_dt.Rows[i].Cells[k - 1].Value);
                    double sayi;
                    sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                    maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                }
                for (int k = 11 + Convert.ToInt32(toplamGun); k < 12 + Convert.ToInt32(toplamGun); k++)
                {
                    maliyet_dt.Rows[i].Cells[k].Value = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k - 4].Value) - Convert.ToDouble(maliyet_dt.Rows[i].Cells[k - 1].Value);
                    double sayi;
                    sayi = Convert.ToDouble(maliyet_dt.Rows[i].Cells[k].Value);
                    maliyet_dt.Rows[i].Cells[k].Value = sayi.ToString("0.##");
                }
            }
            bgl.Close();
        }
        public void gunluk_giris()
        {
            string getir = "select * from personel_info where state=1";
            SqlCommand komut = new SqlCommand(getir, bgl);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            bgl.Close();
        }
        void temizle()
        {
            ad_txtbx.Clear();
            soyad_txtbx.Clear();
            sicilno_txtbx.Clear();
            ekgostege_txtbx.Clear();
            not_txtbx.Clear();
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            DataTable tablo = new DataTable();
            SqlCommand komut = new SqlCommand("select * from job_status", bgl);
            SqlDataAdapter da = new SqlDataAdapter(komut);
            DataSet ds = new DataSet();
            da.Fill(tablo);
            statu_chckbx.ValueMember = "id";
            statu_chckbx.DisplayMember = "status";
            statu_chckbx.DataSource = tablo;
            int jobid = Convert.ToInt32(statu_chckbx.SelectedValue);
            bgl.Close();
            kayitlari_getir();
            kayitlari_getir2();
            gunluk_giris();
            password1_txtbx.PasswordChar = '*';
            password2_txtbx.PasswordChar = '*';

        }

        private void gunlukkaydet_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (bgl.State == ConnectionState.Closed)
                {

                    if ((miktar_nupdown.Value) < 2)
                    {
                        foreach (DataGridViewRow row in gunlukgiris_dt.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells["Selected"].Value) == true)
                            {
                                string kayit = "insert into personal_eat_food (personal_id,per_name,per_surname,date,eat_piece,guest,ek_gosterge)" +
                                    " values(@personal_id,@per_name,@per_surname,@date,@eat_piece,@guest,@ek_gosterge)";
                                SqlCommand komut = new SqlCommand(kayit, bgl);
                                komut.Parameters.Clear();
                                komut.Parameters.AddWithValue("@personal_id", Convert.ToInt32(row.Cells["id"].Value));
                                komut.Parameters.AddWithValue("@per_name", row.Cells["name"].Value);
                                komut.Parameters.AddWithValue("@per_surname", row.Cells["surname"].Value);
                                komut.Parameters.AddWithValue("@ek_gosterge", row.Cells["add_ind"].Value);
                                komut.Parameters.AddWithValue("@date", dateTimePicker1.Value);
                                komut.Parameters.AddWithValue("@eat_piece", Convert.ToInt32(miktar_nupdown.Value));
                                komut.Parameters.AddWithValue("@guest", 0);

                                bgl.Open();
                                komut.ExecuteNonQuery();
                                bgl.Close();
                            }
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in gunlukgiris_dt.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells["Selected"].Value) == true)
                            {
                                string kayit = "insert into personal_eat_food (personal_id,per_name,per_surname,date,eat_piece,guest,ek_gosterge)" +
                                    " values(@personal_id,@per_name,@per_surname,@date,@eat_piece,@guest,@ek_gosterge)";
                                SqlCommand komut = new SqlCommand(kayit, bgl);
                                komut.Parameters.Clear();
                                komut.Parameters.AddWithValue("@personal_id", Convert.ToInt32(row.Cells["id"].Value));
                                komut.Parameters.AddWithValue("@per_name", row.Cells["name"].Value);
                                komut.Parameters.AddWithValue("@per_surname", row.Cells["surname"].Value);
                                komut.Parameters.AddWithValue("@ek_gosterge", row.Cells["add_ind"].Value);
                                komut.Parameters.AddWithValue("@date", dateTimePicker1.Value);
                                komut.Parameters.AddWithValue("@eat_piece", 1);
                                komut.Parameters.AddWithValue("@guest", Convert.ToInt32(miktar_nupdown.Value) - 1);

                                bgl.Open();
                                komut.ExecuteNonQuery();
                                bgl.Close();
                            }
                        }
                    }
                }
                MessageBox.Show("Kayıt başarılı!");
            }
            catch (Exception hata)
            {
                MessageBox.Show("Bir hata var!!!\n" + hata.Message);
            }
            foreach (DataGridViewRow row in gunlukgiris_dt.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Selected"].Value) == true)
                {
                    row.Cells["Selected"].Value = false;
                }
            }
            miktar_nupdown.Value = 1;
        }

        private void ekle_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (bgl.State == ConnectionState.Closed)
                {
                    bgl.Open();
                    string kayit = "insert into personel_info (name,surname,reg_number,job_id,add_ind,note) values(@name,@surname,@reg_number,@job_id,@add_ind,@note)";
                    SqlCommand komut = new SqlCommand(kayit, bgl);
                    komut.Parameters.AddWithValue("@name", ad_txtbx.Text);
                    komut.Parameters.AddWithValue("@surname", soyad_txtbx.Text);
                    komut.Parameters.AddWithValue("@reg_number", sicilno_txtbx.Text);
                    komut.Parameters.AddWithValue("@job_id", statu_chckbx.SelectedValue);
                    komut.Parameters.AddWithValue("@add_ind", ekgostege_txtbx.Text);
                    komut.Parameters.AddWithValue("@note", not_txtbx.Text);
                    //komut.Parameters.AddWithValue("@state", 1);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Kayıt başarılı!");
                    temizle();

                    kayitlari_getir();
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show("Bir hata var!!!\n" + hata.Message);
            }
        }
        private void guncelle_btn_Click(object sender, EventArgs e)
        {
            bgl.Open();
            //SqlConnection connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
            //connection.Open();
            SqlCommand cmd = new SqlCommand("UPDATE personel_info SET name='" + ad_txtbx.Text + "', surname='" + soyad_txtbx.Text + "',reg_number='" + sicilno_txtbx.Text + "',job_id='" + statu_chckbx.SelectedValue + "',add_ind='" + ekgostege_txtbx.Text + "',note='" + not_txtbx.Text + "' WHERE reg_number='" + personelkayıt_dt.CurrentRow.Cells[3].Value.ToString() + "' ", bgl);//update işlemleri
            cmd.ExecuteNonQuery();
            MessageBox.Show("Güncellendi", "Bilgi");
            kayitlari_getir();
            temizle();
        }
        private void sil_btn_Click(object sender, EventArgs e)
        {
            DialogResult cevap;
            cevap = MessageBox.Show("Kaydı silmek istediğinizden emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                //SqlConnection baglanti1 = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
                bgl.Open();
                SqlCommand cmd = new SqlCommand("UPDATE personel_info SET name='" + ad_txtbx.Text + "', surname='" + soyad_txtbx.Text + "',reg_number='" + sicilno_txtbx.Text + "',job_id='" + statu_chckbx.SelectedValue + "',add_ind='" + ekgostege_txtbx.Text + "',state='" + false + "',note='" + not_txtbx.Text + "' WHERE reg_number='" + personelkayıt_dt.CurrentRow.Cells[3].Value.ToString() + "' ", bgl);//update işlemleri
                cmd.ExecuteNonQuery();
                kayitlari_getir();
                temizle();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ad_txtbx.Text = personelkayıt_dt.CurrentRow.Cells[1].Value.ToString();
            soyad_txtbx.Text = personelkayıt_dt.CurrentRow.Cells[2].Value.ToString();
            sicilno_txtbx.Text = personelkayıt_dt.CurrentRow.Cells[3].Value.ToString();
            statu_chckbx.SelectedValue = personelkayıt_dt.CurrentRow.Cells[4].Value.ToString();
            ekgostege_txtbx.Text = personelkayıt_dt.CurrentRow.Cells[5].Value.ToString();
            not_txtbx.Text = personelkayıt_dt.CurrentRow.Cells[6].Value.ToString();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void maliyethesapla_btn_Click(object sender, EventArgs e)
        {
            kayitlari_getir3();

        }
        private void temizle_btn_Click(object sender, EventArgs e)
        {
            temizle();
        }
        //SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr;
        private void button9_Click(object sender, EventArgs e)
        {
            //con = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
            cmd = new SqlCommand();
            bgl.Open();
            cmd.Connection = bgl;
            cmd.CommandText = "SELECT * FROM personel_info";
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //listBox1.Items.Add((dr["name"] + " " + dr["surname"]));
            }
            bgl.Close();
        }
        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gunlukkaydet_btn_Click(this, new EventArgs());
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult dialog = new DialogResult();
                dialog = MessageBox.Show("Bu işlem, veri yoğunluğuna göre uzun sürebilir. Devam etmek istiyor musunuz?", "EXCEL'E AKTARMA", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    Microsoft.Office.Interop.Excel.Application uyg = new Microsoft.Office.Interop.Excel.Application();
                    uyg.Visible = true;
                    Microsoft.Office.Interop.Excel.Workbook kitap = uyg.Workbooks.Add(System.Reflection.Missing.Value);
                    Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)kitap.Sheets[1];
                    for (int i = 0; i < maliyet_dt.Columns.Count; i++)
                    {
                        Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[1, i + 1];
                        myRange.Value2 = maliyet_dt.Columns[i].HeaderText;
                    }

                    for (int i = 0; i < maliyet_dt.Columns.Count; i++)
                    {
                        for (int j = 0; j < maliyet_dt.Rows.Count; j++)
                        {
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                            myRange.Value2 = maliyet_dt[i, j].Value;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("İŞLEM İPTAL EDİLDİ.", "İşlem Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {

                MessageBox.Show("İŞLEM TAMAMLANMADAN EXCEL PENCERESİNİ KAPATTINIZ.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void kayıtgetir_btn_Click_1(object sender, EventArgs e)
        {
            kayitlari_getir4();
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int eskiYemekSayisi = 0;
            eskiYemekSayisi = Convert.ToInt32(dataGridView4.CurrentRow.Cells[3].Value) + Convert.ToInt32(dataGridView4.CurrentRow.Cells[4].Value);
            miktar2_nupdown.Value = eskiYemekSayisi;
            label52.Text = dataGridView4.CurrentRow.Cells[5].Value.ToString();
            label53.Text = dataGridView4.CurrentRow.Cells[6].Value.ToString();
            label54.Text = dataGridView4.CurrentRow.Cells[1].Value.ToString();
            label55.Text = dataGridView4.CurrentRow.Cells[7].Value.ToString();
        }

        private void yemekkayıtguncelle_btn_Click(object sender, EventArgs e)
        {
            if (miktar2_nupdown.Value < 1)
            {
                yemekkayıtsil_btn_Click(this, new EventArgs());
            }
            else
            {
                bgl.Open();
                //SqlConnection connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
                bgl.Open();
                SqlCommand cmd = new SqlCommand("UPDATE personal_eat_food SET eat_piece='" + 1 + "', guest='" + (Convert.ToInt32(miktar2_nupdown.Value) - 1) + "' WHERE id='" + dataGridView4.CurrentRow.Cells[0].Value.ToString() + "' ", bgl);//update işlemleri
                cmd.ExecuteNonQuery();
                MessageBox.Show("Güncellendi", "Bilgi");
                kayitlari_getir4();
            }
        }

        private void yemekkayıtsil_btn_Click(object sender, EventArgs e)
        {
            bgl.Open();
            //SqlConnection connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
            bgl.Open();
            SqlCommand cmd = new SqlCommand("DELETE personal_eat_food WHERE id='" + dataGridView4.CurrentRow.Cells[0].Value.ToString() + "' ", bgl);//delete işlemi
            cmd.ExecuteNonQuery();
            MessageBox.Show("Kayıt Silindi", "Bilgi");
            kayitlari_getir4();
        }
        public void temizle4()
        {
            username_txtbx.Clear();
            password1_txtbx.Clear();
            password2_txtbx.Clear();
        }
        private void userkaydet_btn_Click(object sender, EventArgs e)
        {

            if (password1_txtbx.Text == password2_txtbx.Text)
            {
                bgl.Open();
                string kayit = "insert into admin_info (username,password) values(@username,@password)";
                SqlCommand komut = new SqlCommand(kayit, bgl);
                komut.Parameters.AddWithValue("@username", username_txtbx.Text);
                komut.Parameters.AddWithValue("@password", password1_txtbx.Text);
                komut.ExecuteNonQuery();
                MessageBox.Show("Kayıt başarılı!");

            }
            else
            {
                MessageBox.Show("Şfireler Uyuşmamaktadır!", "Error");
            }
            temizle4();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                password1_txtbx.PasswordChar = '\0';
            }
            else
            {
                password1_txtbx.PasswordChar = '*';
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                password2_txtbx.PasswordChar = '\0';
            }
            else
            {
                password2_txtbx.PasswordChar = '*';
            }
        }

        private void usersil_btn_Click(object sender, EventArgs e)
        {
            temizle4();
            bgl.Close();
            bgl.Open();
            string secmeSorgusu = "SELECT * from admin_info";
            SqlCommand secmeKomutu = new SqlCommand(secmeSorgusu, bgl);
            secmeKomutu.Parameters.AddWithValue("@username", username_txtbx.Text);
            SqlDataAdapter da = new SqlDataAdapter(secmeKomutu);
            SqlDataReader dr = secmeKomutu.ExecuteReader();
            //DataReader ile müşteri verilerini veritabanından belleğe aktardık.
            if (dr.Read()) //Datareader herhangi bir okuma yapabiliyorsa aşağıdaki kodlar çalışır.
            {
                //string username = dr["username"].ToString();
                dr.Close();
                string username = username_txtbx.Text;
                //Datareader ile okunan müşteri ad ve soyadını isim değişkenine atadım.
                //Datareader açık olduğu sürece başka bir sorgu çalıştıramayacağımız için dr nesnesini kapatıyoruz.
                DialogResult durum = MessageBox.Show(username + " kaydını silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo);
                //Kullanıcıya silme onayı penceresi açıp, verdiği cevabı durum değişkenine aktardık.
                if (DialogResult.Yes == durum) // Eğer kullanıcı Evet seçeneğini seçmişse, veritabanından kaydı silecek kodlar çalışır.
                {
                    string silmeSorgusu = "DELETE from admin_info where username=@username";
                    //musterino parametresine bağlı olarak müşteri kaydını silen sql sorgusu
                    SqlCommand silKomutu = new SqlCommand(silmeSorgusu, bgl);
                    silKomutu.Parameters.AddWithValue("@username", username_txtbx.Text);
                    silKomutu.ExecuteNonQuery();
                    MessageBox.Show("Kayıt Silindi...");
                    temizle4();
                    //Silme işlemini gerçekleştirdikten sonra kullanıcıya mesaj verdik.
                }
            }
            else
                MessageBox.Show("Kullanıcı Adı Bulunamadı.");
            bgl.Close();
        }

        private void passwordyenile_btn_Click(object sender, EventArgs e)
        {
            bgl.Close();
            if (password1_txtbx.Text == password2_txtbx.Text)
            {
                //SqlConnection connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
                bgl.Open();
                SqlCommand cmd = new SqlCommand("UPDATE admin_info SET username='" + username_txtbx.Text + "', password='" + password1_txtbx.Text + "' WHERE username='" + username_txtbx.Text+"' ", bgl);//update işlemleri
                cmd.ExecuteNonQuery();
                MessageBox.Show("Şfire Güncellendi!", "Bilgi");

                temizle4();
            }
            else
            {
                MessageBox.Show("Şifreler Uyuşmamaktadır!", "Error");
            }
        }
    }
}