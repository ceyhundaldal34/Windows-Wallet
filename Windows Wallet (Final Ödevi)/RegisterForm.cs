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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnSignin_Click(object sender, EventArgs e)
        {
            string kadi = txtRegUsername.Text.Trim();
            string sifre = txtRegPassword.Text.Trim();

            // 1. Boş Alan Kontrolü
            if (string.IsNullOrEmpty(kadi) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Alanlar boş geçilemez!");
                return;
            }

            // --- YENİ EKLENEN GÜVENLİK ŞARTLARI ---

            // 2. Uzunluk Kontrolü (En az 6 karakter)
            if (sifre.Length < 6)
            {
                MessageBox.Show("Şifreniz en az 6 karakter olmalıdır.", "Güvenlik Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Veritabanına gitme, işlemi durdur.
            }

            // 3. Özel Karakter Kontrolü (En az 1 tane)
            // Şifrenin içindeki karakterlere tek tek bakar. Hepsi harf veya rakamsa (yani özel karakter yoksa) hata verir.
            if (!sifre.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                MessageBox.Show("Şifreniz en az 1 adet özel karakter (!, *, ?, # vb.) içermelidir.", "Güvenlik Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur.
            }
            // -------------------------------------

            using (var baglanti = DatabaseHelper.GetConnection())
            {
                try
                {
                    baglanti.Open();
                    string sorgu = "INSERT INTO Users (Username, Password) VALUES (@u, @p)";

                    using (var komut = new System.Data.SQLite.SQLiteCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@u", kadi);
                        komut.Parameters.AddWithValue("@p", sifre);
                        komut.ExecuteNonQuery();
                    }

                    MessageBox.Show("Kayıt Başarılı! Giriş ekranına yönlendiriliyorsunuz.");

                    // --- KÜÇÜK BİR İPUCU ---
                    // Login formundan buraya 'ShowDialog' ile geldiğimiz için,
                    // yeni bir LoginForm oluşturmana gerek yok. 
                    // Sadece bu formu kapatırsan, arkadaki Login formu zaten bekliyor olacak.
                    this.Close();
                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    if (ex.Message.Contains("UNIQUE"))
                        MessageBox.Show("Bu kullanıcı adı zaten var! Lütfen başka bir isim seçin.");
                    else
                        MessageBox.Show("Veritabanı Hatası: " + ex.Message);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Sadece kendini kapat. (Otomatik olarak LoginForm geri gelecek)
        }
    }
}
