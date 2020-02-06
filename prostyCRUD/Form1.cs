using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace prostyCRUD
{
    public partial class Form1 : Form
    {
        // dane typu string potrzebne przy logowaniu do bazy danych
        string connectionString = @"Server=localhost;Database=samochody;Uid=root;Pwd=123asd123asd";
        // zmienna pomocnicza z numerem id samochodu
        int idSamochód;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnZapisz_Click(object sender, EventArgs e)
        {
            //ustanawianie połączenia z bazą mysql
            using(MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                
                mysqlCon.Open();
                //wywołanie procedury bazodanowej, która dodaje lub nadpisuje dane w bazie
                MySqlCommand mySqlCmd = new MySqlCommand("SamochódDodajLubEdytuj", mysqlCon);
                mySqlCmd.CommandType = CommandType.StoredProcedure;
                mySqlCmd.Parameters.AddWithValue("_idSamochód", idSamochód);
                mySqlCmd.Parameters.AddWithValue("_SamochódModel", txtModel.Text.Trim());
                mySqlCmd.Parameters.AddWithValue("_SamochódMarka", txtMarka.Text.Trim());
                mySqlCmd.Parameters.AddWithValue("_SamochódOpis", txtOpis.Text.Trim());
                mySqlCmd.ExecuteNonQuery();
                MessageBox.Show("Operacja powiodła się!");
                Clear();
                GridFill();

            }

        }

        void GridFill()
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                //wyswietlanie rekordów w bazie w kontrolce DataGridView
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("SamochódPokażWszystkie", mysqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtblSamochod = new DataTable();
                sqlDa.Fill(dtblSamochod);
                dgvSamochody.DataSource = dtblSamochod;
                dgvSamochody.Columns[0].Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            GridFill();
        }

        void Clear()
        {
            //czyszczenie interfejsu
            txtMarka.Text = txtModel.Text = txtOpis.Text = txtSzukaj.Text = "";
            idSamochód = 0;
            btnZapisz.Text = "Zapisz";
            btnUsun.Enabled = false;
        }

        private void dgvSamochody_DoubleClick(object sender, EventArgs e)
        {
            //po dwukrotnym kliknięciu program wczytuje dane z wybranego rekordu umożliwiając ich edycję,
            // bądź usunięcie rekordu
            if (dgvSamochody.CurrentRow.Index != -1)
            {
                txtMarka.Text = dgvSamochody.CurrentRow.Cells[2].Value.ToString();
                txtModel.Text = dgvSamochody.CurrentRow.Cells[1].Value.ToString();
                txtOpis.Text = dgvSamochody.CurrentRow.Cells[3].Value.ToString();
                idSamochód = Convert.ToInt32(dgvSamochody.CurrentRow.Cells[0].Value.ToString());
                btnZapisz.Text = "Aktualizuj";
                btnUsun.Enabled = true;
            }
        }

        private void btnSzukaj_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                // wywołuje procedure z bazy danych i wyświetla rekordy zawierające szukaną frazę
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("SamochódSzukajPoWartości", mysqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("_SzukanaWartość", txtSzukaj.Text);
                DataTable dtblSamochod = new DataTable();
                sqlDa.Fill(dtblSamochod);
                dgvSamochody.DataSource = dtblSamochod;
                dgvSamochody.Columns[0].Visible = false;
            }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            //przycisk anuluj czyści interfejs
            Clear();
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                //wykonuje procedurę w bazie danych i usuwa edytowany rekord z bazy
                MySqlCommand mySqlCmd = new MySqlCommand("SamochódUsuńPoId", mysqlCon);
                mySqlCmd.CommandType = CommandType.StoredProcedure;
                mySqlCmd.Parameters.AddWithValue("_idSamochód", idSamochód);
                mySqlCmd.ExecuteNonQuery();
                MessageBox.Show("Operacja powiodła się!");
                Clear();
                GridFill();

            }
        }
    }
}
