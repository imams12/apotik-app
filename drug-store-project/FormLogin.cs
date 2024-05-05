using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apotik_project
{
    public partial class FormLogin : Form
    {
        private SqlCommand cmd, cmd2;
        private DataSet ds;
        private SqlDataAdapter da;
        private SqlDataReader rd;

        ConnectSql Connect = new ConnectSql();

        public FormLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlDataReader rd = null;
            SqlConnection conn = Connect.GetConnect();
            try
            {
                conn.Open();
                cmd = new SqlCommand("select * from m_operator where username='" + textBox1.Text.ToUpper() + "' and password= '" + textBox2.Text + "'", conn);
                cmd.ExecuteNonQuery();




                

                rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    this.Hide();

                    FormMenuUtama frmUtama = new FormMenuUtama(textBox1.Text.ToUpper());
                    frmUtama.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Username dan password salah");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
