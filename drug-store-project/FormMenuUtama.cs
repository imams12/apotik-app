using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics.Metrics;
using System.Windows.Forms;
using System.Data.SqlClient;
using apotik_project.model;
using System.Transactions;
using drug_store_project.model;
using System.Text.RegularExpressions;

namespace apotik_project
{
    public partial class FormMenuUtama : Form
    {

        SqlCommand cmd;
        SqlDataReader reader;
        ConnectSql Connect = new ConnectSql();
        int price = 0;
        int total = 0;
        int quantity = 0;
        string idProduct = "";
        string productName = "";

        int counter = 1;
        int result = 0;

        TransactionDetail transactionDetail = new TransactionDetail();

        List<TransactionDetail> transactionDetailList = new List<TransactionDetail>();

        public FormMenuUtama(String username)
        {
            InitializeComponent();

            textBox1.Text = getInvoiceNumber();
            counter++;
            textBox2.Text = DateTime.Now.ToString("D");
            textBox3.Text = username;


            comboBox1_Load();

            DisplayTransactionDetails();
        }

        private string getInvoiceNumber()
        {
            SqlConnection conn = Connect.GetConnect();
            conn.Open();
            cmd = new SqlCommand("SELECT MAX(CAST(SUBSTRING(invoice_number, 2, LEN(invoice_number) - 1) AS INT )) FROM m_transaction", conn);
            cmd.ExecuteNonQuery();

            object result = cmd.ExecuteScalar();
            int invoiceNumber = (result == DBNull.Value) ? 0 : Convert.ToInt32(result);

            int countInvoicenumber = invoiceNumber + 1;
            string invoiceFormat = "F" + countInvoicenumber.ToString().PadLeft(3, '0');
            conn.Close();

            return invoiceFormat;
        }

        private void comboBox1_Load()
        {
            SqlConnection conn = Connect.GetConnect();
            string sql = " select * from m_product";
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["code"]);
            }
            conn.Close();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection conn = Connect.GetConnect();
            string sql = "SELECT * FROM m_product WHERE code = @selectedCode";
            cmd = new SqlCommand(sql, conn);
            idProduct = comboBox1.Text;
            cmd.Parameters.AddWithValue("@selectedCode", idProduct);
            conn.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productName = reader["name"].ToString();
                string unitPrice = reader["unit_price"].ToString();
                price = int.Parse(unitPrice.Replace(",00", ""));

                textBox4.Text = productName;
                textBox7.Text = price.ToString("#,##0", new System.Globalization.CultureInfo("id-ID"));
            }
            conn.Close();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                quantity = 0;
            }
            else if(Regex.IsMatch(textBox6.Text, "^[1-9]\\d*$"))
            {
                quantity = int.Parse(textBox6.Text);
            }
            else
            {
                MessageBox.Show("Masukkan input yang benar");
            }

            total = quantity * price;
            textBox5.Text = (total).ToString("#,##0", new System.Globalization.CultureInfo("id-ID"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1 && !textBox4.Text.Equals("") && !textBox7.Text.Equals("") && !textBox6.Text.Equals("") && !textBox5.Equals(""))
            {
                transactionDetail.Id = transactionDetailList.Count + 1;
                transactionDetail.Id_Product = idProduct;
                transactionDetail.ProductName = productName;
                transactionDetail.Quantity = quantity;
                transactionDetail.Sub_total = total;
                transactionDetailList.Add(transactionDetail);
                result += total;
                textBox8.Text = (result).ToString("#,##0", new System.Globalization.CultureInfo("id-ID"));

                dataGridView1.Rows.Add(transactionDetail.Id, transactionDetail.Id_Product, transactionDetail.ProductName, transactionDetail.Quantity, transactionDetail.Sub_total);

                handleReset();
            }
            else
            {
                MessageBox.Show("Semua data produk harus diisi!");
            }
        }

        private void handleReset()
        {
            comboBox1.SelectedIndex = -1;
            textBox4.Text = "";
            textBox7.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
        }

        private void DisplayTransactionDetails()
        {

            dataGridView1.Columns.Add("Id", "No");
            dataGridView1.Columns.Add("idProduct", "BarangID");
            dataGridView1.Columns.Add("productName", "Nama");
            dataGridView1.Columns.Add("quantity", "JmlBeli");
            dataGridView1.Columns.Add("total", "SubTotal");

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            int myMoney = 0;
            if (string.IsNullOrEmpty(textBox9.Text))
            {
                myMoney = 0;
            }
            else if (Regex.IsMatch(textBox9.Text, "^[1-9]\\d*$"))
            {
                myMoney = int.Parse(textBox9.Text);
            }
            else
            {
                MessageBox.Show("Masukkan input yang benar");
            }

            int moneyBack = myMoney - result;
            textBox10.Text = moneyBack <= 0 ? "0" : moneyBack.ToString("#,##0", new System.Globalization.CultureInfo("id-ID"));
        }

        public int getIdOperator(String username)
        {
            int idOperator = 0;
            SqlConnection conn = Connect.GetConnect();
            string queryIdOperator = "SELECT id FROM m_operator WHERE username = @username";
            SqlCommand cmd = new SqlCommand(queryIdOperator, conn);
            cmd.Parameters.AddWithValue("@username", username);
            conn.Open();
            object result = cmd.ExecuteScalar();
            if (result != DBNull.Value)
            {
                idOperator = Convert.ToInt32(result);

            }
            conn.Close();
            return idOperator;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(transactionDetailList.Count > 0)
            {
                if (!textBox10.Text.Equals("") && !textBox9.Text.Equals(""))
                {
                    try
                    {
                        TransactionProduct transactionProduct = new TransactionProduct();
                        transactionProduct.InvoiceNumber = textBox1.Text;
                        transactionProduct.TransactionDate = DateTime.Parse(textBox2.Text);
                        transactionProduct.IdOperator = getIdOperator(textBox3.Text);

                        DialogResult result = MessageBox.Show("Apakah Anda yakin?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            AddTransaction(transactionProduct, transactionDetailList);
                            MessageBox.Show("Successful Transaction ;)");
                            this.Hide();
                            FormMenuUtama frmUtama = new FormMenuUtama(textBox1.Text);
                            frmUtama.ShowDialog();
                        }
                        else
                        {
                            this.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Harap input Jumlah uang!");
                }
            }
            else
            {
                MessageBox.Show("Harap add product!");
            }

        }

        public int getIdProductForDb(String codeProduct)
        {
            int idProduct = 0;
            SqlConnection conn = Connect.GetConnect();
            string queryIdProduct = "SELECT id FROM m_product WHERE code = @codeProduct";
            SqlCommand cmd = new SqlCommand(queryIdProduct, conn);
            cmd.Parameters.AddWithValue("@codeProduct", codeProduct);
            conn.Open();
            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                idProduct = Convert.ToInt32(result);
            }
            conn.Close();

            return idProduct;
        }

        public void AddTransaction(TransactionProduct transactionProduct, List<TransactionDetail> transactionDetailList)
        {
            try
            {
                SqlConnection conn = Connect.GetConnect();
                conn.Open();

                using (SqlTransaction sqlTransaction = conn.BeginTransaction())
                {
                    try
                    {
                        string queryTransaction = "INSERT INTO m_transaction (invoice_number, transaction_date, id_operator) VALUES (@invoiceNumber, @transactionDate, @idOperator); SELECT SCOPE_IDENTITY();";
                        SqlCommand cmd = new SqlCommand(queryTransaction, conn, sqlTransaction);
                        cmd.Parameters.AddWithValue("@invoiceNumber", transactionProduct.InvoiceNumber);
                        cmd.Parameters.AddWithValue("@transactionDate", transactionProduct.TransactionDate);
                        cmd.Parameters.AddWithValue("@idOperator", transactionProduct.IdOperator);
                        int transactionId = Convert.ToInt32(cmd.ExecuteScalar());

                        foreach (TransactionDetail item in transactionDetailList)
                        {
                            int idProductDb = getIdProductForDb(item.Id_Product);

                            string queryTransactionDetail = "INSERT INTO t_transaction_detail (id_transaction, id_product, quantity, subtotal) VALUES (@idTransaction, @idProduct, @Quantity, @Subtotal)";
                            SqlCommand cmd3 = new SqlCommand(queryTransactionDetail, conn, sqlTransaction);
                            cmd3.Parameters.AddWithValue("@idTransaction", transactionId);
                            cmd3.Parameters.AddWithValue("@idProduct", idProductDb);
                            cmd3.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd3.Parameters.AddWithValue("@Subtotal", item.Sub_total);
                            cmd3.ExecuteNonQuery();
                        }


                        sqlTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            handleReset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                FormLogin formLogin = new FormLogin();
                formLogin.ShowDialog();
            }
            else
            {
                this.Show();
            }

        }
    }
}
