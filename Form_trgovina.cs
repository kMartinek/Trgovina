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
//TODO 


namespace Trgovina_app
{
    public partial class Form_trgovina : Form
    {
        public Form_trgovina()
        {
            InitializeComponent();
        }
        public Form_trgovina(int ovlast, String user)
        {            
            InitializeComponent();
            this.Text = "Trgovac: "+ user; 
            tabControl1.Location = new Point(0, 0);
            tabControl1.Size = new Size(this.Width, this.Height);
            tabPage1.Text = "Izdavanje računa";
            tabPage2.Text = "Pretraživanje artikala";
            tabPage3.Text = "Rokovi trajanja i količine";
            dataGridView1.Width = this.Width;
            dataGridView1.Height = this.Height - 110;
            dataGridView2.Width = this.Width;
            dataGridView2.Height = this.Height - 80;

        }

        private void Form_trgovina_Resize(object sender, EventArgs e)
        {
            tabControl1.Size = new Size(this.Width, this.Height);
            dataGridView1.Width = this.Width-3;
            dataGridView1.Height = this.Height - 110;
            dataGridView2.Width = this.Width - 3;
            dataGridView2.Height = this.Height - 80;

        }


       
        private void Form_trgovina_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'baza2DataSet.Racun' table. You can move, or remove it, as needed.
            this.racunTableAdapter.Fill(this.baza2DataSet.Racun);



        }

        AutoCompleteStringCollection coll = new AutoCompleteStringCollection();
        //Bojana
        //SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Bojana\\Desktop\\faks\\rp3moji\\Trgovina_app\\Trgovina_app\\NovaBaza.mdf;Integrated Security=True;Connect Timeout=30");
        //Barbara
        //SqlConnection con = new SqlConnection(" Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\NovaBaza.mdf;Integrated Security = True; Connect Timeout = 30");
        //Karlo
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kac\Documents\BazaK.mdf;Integrated Security=True;Connect Timeout=30");

        SqlDataAdapter da;

        public void Auto()

        {
            da = new SqlDataAdapter("select Kod from Artikli order by Kod asc", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)

            {

                for (int i = 0; i < dt.Rows.Count; i++)

                {

                    coll.Add(dt.Rows[i]["Kod"].ToString());

                }

            }
            else

            {

                MessageBox.Show("Not found");

            }

            textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;

            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

            textBox1.AutoCompleteCustomSource = coll;

        }

        public void fillgridKod()

        {
            Console.WriteLine("Tu si"); 
            da = new SqlDataAdapter("select * from Artikli where Kod like %" + textBox1.Text + "'%'", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)

            {

                dataGridView1.DataSource = dt;


            }
            
            
        }

        public void fillgridKod2()

        {
            da = new SqlDataAdapter("select * from Artikli where Kod like '" + textBox2.Text + "%'", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

           if (dt.Rows.Count > 0)

            {

                dataGridView3.DataSource = dt;
                dataGridView3.Rows[0].Selected = true;

            }

        }






        public void fillgridKod3()

        {
            da = new SqlDataAdapter("select * from Racun", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            // if (dt.Rows.Count > 0)

            {

                dataGridView4.DataSource = dt;
                dataGridView4.Rows[0].Selected = true;
                textBox2.Clear();

            }

        }

        public void fillgridIme()

        {
            Console.WriteLine("Tu si");
            da = new SqlDataAdapter("select * from Artikli where Naziv like '%" + textBox1.Text + "%'", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)

            {

                dataGridView1.DataSource = dt;


            }

        }
        //*********************************************
        //ovo ne radi.... sutra cu pokusat opet al nikako iscupat iz baze br. dana do isteka roka... 
        public void fillgridIstek()

        {

            Console.WriteLine("Rok trajanja");
            DateTime today = DateTime.Today; 
            //SELECT * from Artikli where DATEDIFF(NOW(), Rok_trajanja) < 10 
            //DATEDIFF(Rok_trajanja, 'now', day)
            da = new SqlDataAdapter("SELECT * from Artikli ", con); // da nije prazno onda bezvezni upit

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)

            {

                dataGridView2.DataSource = dt;


            }

        }


        //*********************************************


        // selektira iz baze proizvode kojih nedostaje (ima ih manje od 20)
        public void fillgridFali()

        {
            Console.WriteLine("Fali");

            
            da = new SqlDataAdapter("SELECT * from Artikli where Kolicina < 20 ", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)

            {

                dataGridView2.DataSource = dt;


            }

        }



        private void textBox1_TextChanged(object sender, EventArgs e)

        {
            if (radioButton1.Checked)
                fillgridKod(); //when selecting the searched name then filling its data in datagrid.  
            else if (radioButton2.Checked)
                fillgridIme(); 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            fillgridIstek();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            fillgridFali();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            fillgridKod2(); 
            
        }

        bool plus_minus = false;

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {

            plus_minus = false;

            if (e.KeyCode == Keys.Enter)
            {
                dodaj();
                fillgridKod2();
                fillgridKod3();

            }
            if (e.KeyCode == Keys.Add)
            {
                plus_minus = true;
                ++numericUpDown1.Value;
            }
            if (e.KeyCode == Keys.Subtract)
            {
                plus_minus = true;
                if (numericUpDown1.Value > 1) --numericUpDown1.Value;
            }
        }

        //ne upisuje + i - u textbox
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (plus_minus == true)
            {
                e.Handled = true;
            }
        }

        //funkcija za dodavanje u račun
        public void dodaj()
        {
            int kod = 0;
            string naziv;
            decimal cijena;
            int popust;

            if (dataGridView1.Rows.Count > 0)
            {
                //iz selectanog reda vadi podatke u varijablu red
                DataGridViewRow red = dataGridView3.SelectedRows[0];
                try
                {
                    kod = Int32.Parse(red.Cells[0].Value.ToString());
                    cijena = Decimal.Parse(red.Cells[2].Value.ToString());
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                
                naziv = red.Cells[1].Value.ToString();
                cijena = (decimal)red.Cells[2].Value;
                popust = 0;
                int kolicina = (int)numericUpDown1.Value;


                //iz baze traži redak koji ima isti kod jer su tamo spremljeni podaci o popustima, a ne u gridviewu
                da = new SqlDataAdapter("SELECT * from Artikli where Kod =  " + kod, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                DataRow red_baza = dt.Rows[0];

                //provjera dal se pokušava na račun dodati više od količine artikala u ponudi
                if (kolicina > (int)red_baza[5])
                {
                    MessageBox.Show("Odabrana je veća količina artikala nego što ih je u ponudi.");
                    return;
                }


                //e tu kreće sranje, poc mi je samo za ispis da vidim šta se točno dogođalo
                if (red_baza.IsNull(8)==false && red_baza.IsNull(9)==false)
                {

                    
                    string poc="";

                    /*poc += red_baza[7].ToString();
                    MessageBox.Show(poc);
                    */

                    //test ubacivanja dinamički datuma početka popusta
                    using (con)
                    {
                        con.Open();
                        DateTime myDateTime = DateTime.Now;
                        string sqlFormattedDate = myDateTime.ToString();

                        string insStmt3 = "UPDATE artikli SET PopustPocetak = '12.01.2012 00:00:00'  WHERE Kod = " + kod;
                        SqlCommand insCmd3 = new SqlCommand(insStmt3, con);
                        int affectedRows = insCmd3.ExecuteNonQuery();
                        MessageBox.Show(affectedRows + " rows inserted!");


                    }
                    con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kac\Documents\BazaK.mdf;Integrated Security=True;Connect Timeout=30");
                    da = new SqlDataAdapter("SELECT * from Artikli where Kod =  " + kod, con);
                    dt = new DataTable();

                    da.Fill(dt);

                   red_baza = dt.Rows[0];

                    //provjera dal je ostalo null , to nam neće trebati
                    for (int i = 0; i < 10; ++i)
                    {
                        if (red_baza.IsNull(8)) { poc = "jebeni null "; poc += red_baza[8].ToString(); }
                    }
                    MessageBox.Show(poc);
                    string kra = red_baza[9].ToString();

                    //ovaj dio je kriv jer sam mislio da parsira datatime drugačije ugl string je oblika '12.01.2012 00:00:00' pa tako treba nekako splitat string da se creataju datetimeovi

                    List<string> datum_p = poc.Split('.').ToList<string>(); 
                    List<string> datum_k = kra.Split('.').ToList<string>();
                    DateTime datum_pocetka = new DateTime(Int32.Parse(datum_p[2]),Int32.Parse(datum_p[1]),Int32.Parse(datum_p[0]));
                    DateTime datum_kraja = new DateTime(Int32.Parse(datum_k[2]), Int32.Parse(datum_k[1]), Int32.Parse(datum_k[0]));

                    //provjera da je današnji datum stvarno u roku akcije
                    if (provjeri_datum(datum_pocetka, datum_kraja) == true) popust = (int)red_baza[4];

                }

                //cijena s popustom
                cijena = cijena * (1 - (popust / 100));


                //e i tu sad izbacuje error zbog fantomskog columna [Vrsta], al mislim da ako to proradi da smo na konju 
                string insStmt = "insert into Racun ([Kod], [Naziv], [Cijena], [Kolicina]) values (@kod,@naziv, @cijena, @kolicina)";
                con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kac\Documents\BazaK.mdf;Integrated Security=True;Connect Timeout=30");
                using (con)
                {
                    con.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, con);
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@kod", kod);
                    insCmd.Parameters.AddWithValue("@naziv", naziv);
                    insCmd.Parameters.AddWithValue("@cijena", cijena);
                    insCmd.Parameters.AddWithValue("@kolicina", kolicina);
                    int affectedRows = insCmd.ExecuteNonQuery();
                    MessageBox.Show(affectedRows + " rows inserted!");

                    string insStmt2 = "UPDATE artikli SET kolicina = " + ((int)red_baza[5] - kolicina) + " WHERE Kod = " + kod;
                    SqlCommand insCmd2 = new SqlCommand(insStmt2, con);
                    affectedRows = insCmd.ExecuteNonQuery();
                    MessageBox.Show(affectedRows + " rows inserted!");
                   

                }
            }

        }

        public bool provjeri_datum(DateTime pocetak, DateTime kraj)
        {
            

           

            if (DateTime.Compare(pocetak, DateTime.Today) < 0 && DateTime.Compare(DateTime.Today, kraj) < 0) return true;
            else return false;


        }

    }
}
