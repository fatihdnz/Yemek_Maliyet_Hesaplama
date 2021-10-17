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
using System.Data.OleDb;

namespace yemek_maliyet_hesaplama
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            AcceptButton = giris_btn;
            password_txtbx.PasswordChar = '*';
        }
        
        private void giris_btn_Click(object sender, EventArgs e)
        {
            SqlConnection foo;
            SqlCommand cmd;
            SqlDataReader dr;
            string username = username_txtbx.Text;
            string password = password_txtbx.Text;
            foo = new SqlConnection ("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Daily_Eating");
            cmd = new SqlCommand();
            foo.Open();
            cmd.Connection = foo;
            cmd.CommandText = "SELECT * FROM admin_info where username='" + username_txtbx.Text + "' AND password='" + password_txtbx.Text + "'";
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                MessageBox.Show("Tebrikler! Başarılı bir şekilde giriş yaptınız.");
                Form1 form1 = new Form1();
                form1.Show();  // form2 göster diyoruz
                this.Hide();   // bu yani form1 gizle diyoruz 
            }
            else
            {
                MessageBox.Show("Kullanıcı adını ve şifrenizi kontrol ediniz.");
            }
            foo.Close();
        }   
            private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                password_txtbx.PasswordChar = '\0';
            }
            else
            {
                password_txtbx.PasswordChar = '*';
            }
        }

        private void cıkıs_btn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            username_txtbx.Text = "";
        }

        private void textBox4_MouseClick(object sender, MouseEventArgs e)
        {
            password_txtbx.Text = "";
        }
    }
}
