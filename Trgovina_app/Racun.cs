using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Trgovina_app
{
    public partial class Racun : Form
    {
        static string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        //Barbara (ovdje jednom otvorim konekciju i to je to)
        //static string ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\barbara\Desktop\projekt subota ujutro 16 02\Trgovina_app\Trgovina_app\BazaK.mdf; Integrated Security = True; Connect Timeout = 30";
        //Bojana
        static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + path + @"\BazaK.mdf;Integrated Security=True;Connect Timeout=30";
        SqlConnection con = new SqlConnection(ConnectionString);
        SqlDataAdapter da;

        public Racun()
        {
            InitializeComponent();
        }
        decimal uku, vratiti, placeno, sbonom; 
        public Racun(String trgovac, String placanje1, decimal iznos1, String placanje2, decimal iznos2, decimal ukupno)
        {
            uku = ukupno;
            placeno = iznos1;
            if (ukupno > iznos2) sbonom = ukupno - iznos2;
            else sbonom = 0;
            
            InitializeComponent();
            label14.Text = uku.ToString() + " kn";
            this.label2.Text = trgovac;
            if (iznos1 == 0 || placanje1 == "")
            {
                label4.Text = placanje2;
                label10.Text = iznos1.ToString() + " kn + " + iznos2.ToString() + " kn";
                label12.Text = "0.00 kn";

            }
            else if (iznos2 == 0 || placanje2 == "")
            {
                label4.Text = placanje1;
                label10.Text = iznos1.ToString() + " kn";
                vratiti = iznos1 - ukupno;
                label12.Text = vratiti.ToString()+ " kn";
            }
            else
            {
                label4.Text = placanje1 + ", " + placanje2;
                if (placeno >= sbonom)
                {
                    label10.Text = iznos1.ToString() + " kn + " + iznos2.ToString() + " kn";
                    vratiti = placeno - sbonom;
                    label12.Text = vratiti.ToString() + " kn";
                }
            }

            da = new SqlDataAdapter("select * from Racun", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
            }  
        }
        int t = 0;
        decimal poreza_ukupno = 0; 
        
        private void Racun_Load(object sender, EventArgs e)
        {
            this.racunTableAdapter.Fill(this.bazaKDataSet.Racun);
            if (t == 0)
            {
            
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {

                int porez = 25;
                String kod = dataGridView1.Rows[i].Cells[0].Value.ToString();
                decimal cijena =Convert.ToDecimal( dataGridView1.Rows[i].Cells[2].Value.ToString());
                decimal popust = Convert.ToDecimal(dataGridView1.Rows[i].Cells[4].Value.ToString());

                    da = new SqlDataAdapter("select Vrsta from Artikli where Kod='" + kod + "'", con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    String pom = dt.Rows[0]["Vrsta"].ToString();
                        if (pom.Contains("osnovno"))
                        porez = 13;
                }
                else
                {
                    MessageBox.Show("Not found");
                }

                    decimal iz = (decimal)1.0 - (decimal)((decimal)popust / 100);

                    poreza_ukupno += (decimal)cijena*iz * (decimal)((decimal)porez / 100);
                    label5.Text = poreza_ukupno.ToString("0.##") + " kn";
                    decimal osnovica = uku - poreza_ukupno;
                    label8.Text = osnovica.ToString("0.##") + " kn"; 
                    dataGridView1.Rows[i].Cells[5].Value = porez.ToString("0.##") + "%";
            }
            t = 1;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            da = new SqlDataAdapter("delete from Racun", con);

            DataTable dt = new DataTable();

            da.Fill(dt);
            Close(); 
        }

        private void Racun_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.Close();
        }
    }
}
