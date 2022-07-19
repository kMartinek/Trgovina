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
using System.Globalization;
using System.IO;

//TODO 


namespace Trgovina_app
{
    public partial class Form_trgovina : Form
    {
        static string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        //Barbara (ovdje jednom otvorim konekciju i to je to)
        //static string ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\barbara\Desktop\projekt subota ujutro 16 02\Trgovina_app\Trgovina_app\BazaK.mdf; Integrated Security = True; Connect Timeout = 30";
        //Bojana
        static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + path + @"\BazaK.mdf;Integrated Security=True;Connect Timeout=30";
        SqlConnection con = new SqlConnection(ConnectionString);
        SqlDataAdapter da;


        public Form_trgovina()
        {
            InitializeComponent();
         
        }

        String trgovac = "";
        int pristup = 0;
        private Form_direktor parent=null; 
        public Form_trgovina(int ovlast, String user, Form p)
        {  
            InitializeComponent();
            this.Text = "Trgovac: " + user;

            tabControl1.Location = new Point(0, 0);
            tabControl1.Size = new Size(this.Width, this.Height);
            tabPage1.Text = "Izdavanje računa";
            tabPage2.Text = "Pretraživanje artikala";
            tabPage3.Text = "Rokovi trajanja i količine";
            dataGridView1.Width = this.Width;
            dataGridView1.Height = this.Height - 110;
            dataGridView2.Width = this.Width;
            dataGridView2.Location =new Point(0, tabControl1.Height+2);
            trgovac = user;

            if (ovlast == 2)
            {
                pristup = ovlast;
                parent = p as Form_direktor; 
            }


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

            //želimo da iskoči obavijest ako nekog artikla ima manje od 20 komada, i još jedna ako ima artikala kojima rok ističe za 15 dana
            da = new SqlDataAdapter("delete from Racun", con);

            DataTable dt = new DataTable();

            da.Fill(dt);
            
            fillgridIstek(15, true);
            fillgridFali(20, true); 


        }

        AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

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

        //ovaj je za ispunjavanje kod pretraživanja kodova
        public void fillgridKod()
        {
            con = new SqlConnection(ConnectionString);
            da = new SqlDataAdapter("select * from Artikli where Kod like '" + textBox1.Text + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Rows[0].Selected = true;
            }
        }

        //ovaj je ispis svih mogućih kodova pri izdavanju računa
        public void fillgridKod2()
        {
            con = new SqlConnection(ConnectionString);
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
            con = new SqlConnection(ConnectionString);

            
            da = new SqlDataAdapter("select * from Racun", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

             if (dt.Rows.Count > 0)
            {
                dataGridView4.DataSource = dt;
                dataGridView4.Rows[0].Selected = true;
                textBox2.Clear();
            }

        }

        public void fillgridIme()
        {
            da = new SqlDataAdapter("select * from Artikli where Naziv like '%" + textBox1.Text + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Rows[0].Selected = true;
            }

        }
        // ispunjavamo grid sa artiklima kojima rok ističe kroz manje od x dana
        public void fillgridIstek(int x, bool obavjestavam)

        {
            //od danas uzmem x dana, to je duration ovdje, i kao answer vraća datum koji je x dana od danas
            DateTime today = DateTime.Today;
            string tgod = today.ToString("yyyy");
            string tmj = today.ToString("MM");
            string tdan = today.ToString("dd");
            string stoday = tgod + "/" + tmj + "/" + tdan;
            System.TimeSpan duration = new System.TimeSpan(x+1, 0, 0, 0);
            System.DateTime answer = today.Add(duration);
            //iz nekih nepoznatih razloga, traži da datum bude u ovom šugavom obliku yyyy/MM/dd, pa sam namjestila
            string god = answer.ToString("yyyy");
            string mj = answer.ToString("MM");
            string dan = answer.ToString("dd"); 
            string date = god + "/" + mj + "/" + dan; 
            //sql upit vraća one datume koji su udaljeni x dana od danas.           
            da = new SqlDataAdapter("SELECT * from Artikli where RokTrajanja<'"+date+"' and RokTrajanja>'"+stoday+"'", con); 

            DataTable dt = new DataTable();
            //i s njima ispunimo tablicu.
            da.Fill(dt);

            if (dt.Rows.Count > 0 && obavjestavam==false)
            {
                dataGridView2.DataSource = dt;
                dataGridView2.Rows[0].Selected = true;
            }
            if(dt.Rows.Count > 0 && obavjestavam ==true)
            {
                MessageBox.Show("Postoje artikli na skladištu kojima uskoro ističe rok trajanja!"); 
            }
        }


        //*********************************************


        // selektira iz baze proizvode kojih nedostaje (ima ih manje od x)
        public void fillgridFali(int x, bool obavjestavam)
        {
            da = new SqlDataAdapter("SELECT * from Artikli where Kolicina < "+x+" and Kolicina >= 0 ", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0 && obavjestavam==false)
            {
                dataGridView2.DataSource = dt;
            }
            else if(dt.Rows.Count > 0 && obavjestavam ==true)
            {
                MessageBox.Show("Postoje artikli kojih na skladištu ima manje od 20!"); 
            }

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                fillgridKod(); //when selecting the searched name then filling its data in datagrid.  
            else if (radioButton2.Checked)
                fillgridIme(); 
        }

        

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            fillgridIstek((int)numericUpDown2.Value, false);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            fillgridFali((int)numericUpDown3.Value, false);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            fillgridKod2();
        }

        bool plus_minus = false;
        bool up_down = false;

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            plus_minus = false;
            up_down = false;


            if (e.KeyCode == Keys.Enter)
            {
                dodaj();
                numericUpDown1.Value = 1;
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

            if (e.KeyCode == Keys.Up)
            {
                up_down = true;
                int trenutni = 0;
                if (dataGridView3.SelectedRows.Count!=0) trenutni = dataGridView3.SelectedRows[0].Index;
                if (trenutni > 0)
                {
                    dataGridView3.Rows[trenutni - 1].Selected = true;
                    dataGridView3.FirstDisplayedScrollingRowIndex = trenutni -1;
                }
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                up_down = true;
                if (dataGridView3.SelectedRows.Count != 0)
                {
                    int trenutni = dataGridView3.SelectedRows[0].Index;
                    if (trenutni < dataGridView3.Rows.Count - 1) dataGridView3.Rows[trenutni + 1].Selected = true;
                    dataGridView3.FirstDisplayedScrollingRowIndex = trenutni;
                }
                e.Handled = true;
            }



        }

        //ne upisuje + i - u textbox
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((plus_minus == true) || (up_down == true))
            {
                e.Handled = true;
            }
        }



        private void dataGridView4_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
            {
                if(dataGridView4.SelectedRows.Count>0)
                {
                    izbaci();
                    fillgridKod2();
                    fillgridKod3();
                    e.Handled = true;
                }
            }
        }

        public void izbaci()
        {
            DataGridViewRow red = dataGridView4.SelectedRows[0];

            string kod = "0";
            string id ="";
            decimal cijena = 0;
            int popust=0;
            int kolicina =0;
            
            try
            {
                kod = red.Cells[1].Value.ToString();
                id = red.Cells[0].Value.ToString();
                cijena = (Decimal)red.Cells[3].Value;
                popust = Int32.Parse(red.Cells[4].Value.ToString());
                kolicina = Int32.Parse(red.Cells[5].Value.ToString());
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            if (dataGridView4.Rows.Count == 1)
            {
                con = new SqlConnection(ConnectionString);
                da = new SqlDataAdapter("delete from Racun", con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataGridView4.DataSource = null;
                dataGridView4.Refresh();
                label6.Text = "0";

                con = new SqlConnection(ConnectionString);
                using (con)
                {
                    con.Open();
                    string insStmt2 = "UPDATE artikli SET kolicina = kolicina + " + kolicina + " WHERE Kod = " + kod;
                    SqlCommand insCmd2 = new SqlCommand(insStmt2, con);
                    insCmd2.ExecuteNonQuery();
                }
            }
            else
            {
                con = new SqlConnection(ConnectionString);
                using (con)
                {
                    con.Open();
                    string del = "DELETE from Racun WHERE id = " + id;
                    SqlCommand delCmd = new SqlCommand(del, con);
                    delCmd.ExecuteNonQuery();

                    decimal iz = (decimal)1.0 - (decimal)((decimal)popust / 100);

                    decimal trenutni_iznos = Convert.ToDecimal(label6.Text);
                    trenutni_iznos -= cijena * iz * kolicina;
                    string sa = trenutni_iznos.ToString("0.##");
                    label6.Text = sa;

                    string insStmt2 = "UPDATE artikli SET kolicina = kolicina + " + kolicina + " WHERE Kod = " + kod;
                    SqlCommand insCmd2 = new SqlCommand(insStmt2, con);
                    insCmd2.ExecuteNonQuery();


                }
            }

            

        }




        //funkcija za dodavanje u račun
        public void dodaj()
        {
            string kod="0";
            string naziv;
            decimal cijena;
            int popust;

            if (dataGridView3.Rows.Count > 0)
            {
                //iz selectanog reda vadi podatke u varijablu red
                DataGridViewRow red = dataGridView3.SelectedRows[0];
                try
                {
                    kod = red.Cells[0].Value.ToString();
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

                con = new SqlConnection(ConnectionString);
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
                if (red_baza[8].ToString() != "" && red_baza[9].ToString() != "")
                {
                    DateTime pocetak = (DateTime)red_baza[8];
                    DateTime kraj = (DateTime)red_baza[9];
                    DateTime today = DateTime.Today;
                    if (pocetak <= today && today <= kraj)
                        popust = Convert.ToInt32(red_baza[4]);
                    else
                        popust = 0;
                }
                else
                    popust = 0;

                //cijena s popustom
                cijena = cijena * (1 - (popust / 100));


                string insStmt = "insert into Racun ([Kod], [Naziv], [Cijena], [Kolicina], [Popust]) values (@kod,@naziv, @cijena, @kolicina, @popust)";
                
                using (con)
                {
                    con.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, con);
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@kod", kod);
                    insCmd.Parameters.AddWithValue("@naziv", naziv);
                    insCmd.Parameters.AddWithValue("@cijena", cijena);
                    insCmd.Parameters.AddWithValue("@kolicina", kolicina);
                    insCmd.Parameters.AddWithValue("@popust", popust); 
                    int affectedRows = insCmd.ExecuteNonQuery();

                    //kada smo dodali artikl na račun, trebamo ga skinuti sa skladišta

                    string insStmt2 = "UPDATE artikli SET kolicina = " + ((int)red_baza[5] - kolicina) + " WHERE Kod = " + kod;
                    SqlCommand insCmd2 = new SqlCommand(insStmt2, con);
                    affectedRows = insCmd2.ExecuteNonQuery();                   

                }
                con = new SqlConnection(ConnectionString);

                //idemo potrpati ovo što imamo u datagridview za prikaz artikala na računu
                da = new SqlDataAdapter("select * from Racun", con);
                DataTable rc = new DataTable();
                da.Fill(rc);

                if (rc.Rows.Count > 0)
                {
                    dataGridView4.DataSource = rc;
                    dataGridView4.Rows[0].Selected = true;
                }
                //i želimo dodati cijenu u labelu6, koja nam kaze ukupni iznos:
                decimal iz = (decimal)1.0 - (decimal)((decimal)popust / 100);
                
                decimal trenutni_iznos = Convert.ToDecimal(label6.Text);
                trenutni_iznos += cijena*iz * kolicina;
                string sa = trenutni_iznos.ToString("0.##");
                label6.Text = sa;
            }

        }

        
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if(radioButton3.Checked)
                fillgridIstek((int)numericUpDown2.Value, false);

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                fillgridFali((int)numericUpDown3.Value, false); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dodaj();
            textBox2.Clear();
            numericUpDown1.Value = 1;
            fillgridKod2(); 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //ako plaćamo gotovinom, onda ne plaćamo čekovima ni karticom, ali može se plaćati bonom
            if (checkBox1.Checked)
            {

                checkBox2.Checked = false;
                checkBox3.Checked = false;
                textBox3.Visible = true;
                textBox3.Text = label6.Text; 
            }
            else
            {
                textBox3.Visible = false;
                textBox3.Text = "0"; 
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {//plaćanje karticom
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                

            }
          
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {//plaćanje čekom
            if (checkBox3.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                

            }
            else
            {
             

            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                textBox6.Visible = true;
            }
            else
            {
                textBox6.Visible = false;
                textBox6.Text = "0";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //prvo treba provjeriti ima li ista na računu, ako nema, ne idemo dalje.
            if (dataGridView4.Rows.Count == 0)
                MessageBox.Show("Ne može se izdati prazan račun!");
            else
            {
                //drugo moramo provjeriti da je sve u redu s plaćanjem
                //ukupni iznos checkiranih kućica mora biti jednak ukupnom iznosu koji se nalazi u label6
                decimal ukupno = Convert.ToDecimal(label6.Text);
                decimal gotovina = 0;
                decimal bon = 0;
                decimal kartica = 0;
                decimal cek = 0;
                String placanje1 = "", placanje2 = "";
                decimal iznos1 = 0, iznos2 = 0;
                if (checkBox4.Checked)//znači da imamo plaćanje bonom
                {
                    bon = Convert.ToDecimal(textBox6.Text);
                    placanje2 = "Bon";
                    iznos2 = bon;

                }
                if (checkBox4.Checked == false)
                {
                    placanje2 = "";
                    iznos2 = 0;
                }
                if (checkBox3.Checked)//placanje čekom
                {
                    
                    placanje1 = "Cek";
                    iznos1 = ukupno-iznos2;
                }
               
                if (checkBox2.Checked)//plaćanje karticom
                {
                   
                    placanje1 = "Kartica";
                    iznos1 = ukupno-iznos2;
                }
             
                if (checkBox1.Checked)//plaćanje gotovinom
                {
                    gotovina = Convert.ToDecimal(textBox3.Text);
                    placanje1 = "Gotovina";
                    iznos1 = gotovina;
                }
                

                if (iznos1 < ukupno - bon)
                    MessageBox.Show("Ukupni plaćeni iznos je premali!");
                else
                {//ako je sve ok s plaćanjem, idemo samo na novu formu koja će nam lijepo u listi prikazati sve što treba
                 //trebamo poslati
                 //-trgovca
                 //-nacin plaćanja i iznose
                 //kupljene artikle ćemo dobiti iz tablice racun koju na kraju dropamo
                    Racun f = new Racun(trgovac, placanje1, iznos1, placanje2, iznos2, ukupno);
                    f.ShowDialog();

                    con = new SqlConnection(ConnectionString);
                    da = new SqlDataAdapter("SELECT * from Racun", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        dataGridView4.DataSource = null;
                        dataGridView4.Refresh();
                        textBox2.Clear();
                        checkBox1.Checked = false;
                        checkBox2.Checked = false;
                        checkBox3.Checked = false;
                        checkBox4.Checked = false;
                        label6.Text = "0";
                    }
                }
            }
        }

        private void Form_trgovina_FormClosing(object sender, FormClosingEventArgs e)
        {
            while (dataGridView4.Rows.Count > 0) izbaci();
            if (pristup==2)
            this.parent.button5.Visible = true;
            con.Close(); 
        }

        private void dataGridView3_KeyDown(object sender, KeyEventArgs e)
        {
            plus_minus = false;
            up_down = false;


            if (e.KeyCode == Keys.Enter)
            {
                dodaj();
                numericUpDown1.Value = 1;
                fillgridKod2();
                fillgridKod3();
                e.Handled = true;
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
    }
}
