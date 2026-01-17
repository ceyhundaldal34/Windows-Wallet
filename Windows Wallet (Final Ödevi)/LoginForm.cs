using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_Wallet__Final_Ödevi_
{
    public partial class LoginForm : Form
    {
        public string KullaniciAdi { get; set; }

        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        // GİRİŞ BUTONU
        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string kadi = txtUser.Text.Trim();
            string sifre = txtPass.Text.Trim();

            if (string.IsNullOrEmpty(kadi) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifrenizi girin.");
                return;
            }
            if (sifre.Length < 6)
            {
                MessageBox.Show("Şifreniz en az 6 karakter olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var baglanti = DatabaseHelper.GetConnection())
            {
                try
                {
                    baglanti.Open();
                    string sorgu = "SELECT Id FROM Users WHERE Username=@u AND Password=@p";

                    using (var komut = new System.Data.SQLite.SQLiteCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@u", kadi);
                        komut.Parameters.AddWithValue("@p", sifre);

                        object sonuc = komut.ExecuteScalar();

                        if (sonuc != null)
                        {
                            int userId = Convert.ToInt32(sonuc);

                            try
                            {
                                MainForm anaForm = new MainForm(kadi);
                                this.Hide();
                                anaForm.ShowDialog();
                                this.Show();
                            }
                            catch (Exception hata)
                            {
                                MessageBox.Show("HATA YAKALANDI:\n\n" + hata.ToString(), "Dedektif Modu");
                                this.Show();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        // --- İŞTE EKSİK OLAN VE HATAYI ÇÖZECEK PARÇA ---
        private void btnCikis_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Kayıt Ol Linki
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm kayit = new RegisterForm();
            this.Hide();
            kayit.ShowDialog();
            this.Show();
        }
    }
}