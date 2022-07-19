using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;


namespace Trgovina_app
{
    public partial class Form_direktor : Form
    {
        static string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        //Barbara (ovdje jednom otvorim konekciju i to je to)
        static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + path + @"\BazaK.mdf;Integrated Security=True;Connect Timeout=30";
        SqlConnection con = new SqlConnection(ConnectionString);
        SqlDataAdapter da;

        // mozda cu trebat kasnije
        String direktor = "";
        String ime = ""; 


        public Form_direktor()
        {
            InitializeComponent();
            
        }
 
        public Form_direktor(int ovlast, String user)
        {
            InitializeComponent();
            con.Open();
            this.Text = "Direktor: " + user;
            this.direktor = user;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Size = new Size(this.Width, this.Height);
            ime = user; 

            //tab zaposlenici
            tabPage1.Text = "Zaposlenici";
            radioButton5.Checked = true;
            popunjavanje_combo_status_zaposlenici();
            fillgridIme_zaposlenici();
            dataGridView4.ClearSelection();
            //******************************

            //tab artikli
            tabPage2.Text = "Artikli";
            popunjavanje_combo_kategorije_artikli();
            radioButton3.Checked = true;
            dataGridView2.ClearSelection();
            //******************************

            //tab popusti 
            tabPage3.Text = "Popusti";
            popunjavanje_combo_kategorije_popusti();
            radioButton1.Checked = true;
            //******************************

            //tab nabava 
            tabPage4.Text = "Nabava";
            //******************************

            //tab Trgovina  
            tabPage5.Text = "Trgovina";
            //******************************
        }

        private void Form_direktor_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bazaKDataSet.Artikli' table. You can move, or remove it, as needed.
            this.artikliTableAdapter.Fill(this.bazaKDataSet.Artikli);
            // TODO: This line of code loads data into the 'bazaKDataSet.Korisnici' table. You can move, or remove it, as needed.
            this.korisniciTableAdapter.Fill(this.bazaKDataSet.Korisnici);

        }

        //TODO ovo jos treba sredit al nije mi bilo prioritet
        private void Form_direktor_Resize(object sender, EventArgs e)
        {
            tabControl1.Size = new Size(this.Width, this.Height);
        }


        //tab ZAPOSLENICI


        //UREDI postojeceg zaposlenika
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            //Gumb je Uredi
            button4.Text = "Uredi";
            dataGridView4.ClearSelection();
            textBox12.Enabled = true;
            dataGridView4.Enabled = true;
            textBox8.Enabled = true;
            textBox9.Enabled = true;
            textBox10.Enabled = true;
            textBox11.Enabled = true;
            
        }
       
        //DODAJ novog zaposlenika
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            //Gumb je Dodaj
            button4.Text = "Dodaj";
            dataGridView4.ClearSelection();
            //DataGrid nam ne treba pa ga onesposobimo
            dataGridView4.Enabled = false;
            textBox12.Clear();
            textBox12.Enabled = false;
            //pa sve lijepo pocistimo  
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            comboBox3.SelectedIndex = 0;
            
        }

        //Klikom na zaposlenika popunjavaju se trenutni podatci
        private void dataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            
            string username1 = dataGridView4.CurrentRow.Cells[0].FormattedValue.ToString();
            string ime1 = dataGridView4.CurrentRow.Cells[1].FormattedValue.ToString();
            string prezime1 = dataGridView4.CurrentRow.Cells[2].FormattedValue.ToString();
            string status1 = dataGridView4.CurrentRow.Cells[3].FormattedValue.ToString();
            if (radioButton5.Checked)
            {
                textBox8.Text = username1;
                textBox9.Text = ime1;
                textBox10.Text = prezime1;
                textBox11.Clear();
                comboBox3.SelectedIndex = comboBox3.FindStringExact(status1);
            }

        }

        //popunjava datagrid zaposlenicima koje trazimo po prezimenu! 
        public void fillgridIme_zaposlenici()
        {
            // Izvadi sve korisnike iz baze
            da = new SqlDataAdapter("select * from Korisnici where Prezime like '%" + textBox12.Text + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dataGridView4.DataSource = dt;
                // dataGridView2.Rows[0].Selected = true;
            }
        }

        //DataGrid je inicijalno popunjen svim zaposlenicima ali kad se krene tipkat onda pretrazuje po pretimenu
        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            fillgridIme_zaposlenici();
            //moram vratit fokus jer zbog comboBoxa sam onu magiju morala gatat pa tu malo zeza
            textBox12.Focus();
        }

        // popunjavanje combobox-a ststusom zaposlenika iz baze  (Direktor/Trgovac)
        private void popunjavanje_combo_status_zaposlenici()
        {
            // Izvadi sve razlicite vrste proizvoda iz baze
            da = new SqlDataAdapter("Select Distinct Status from Korisnici", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Dictionary<string, string> combo_values = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; ++i)
                combo_values.Add(i.ToString(), dt.Rows[i][0].ToString());

                comboBox3.DataSource = new BindingSource(combo_values, null);
                comboBox3.DisplayMember = "Value";
                comboBox3.ValueMember = "Key";
            }
        }

        //Makne focus s comboboxa jer je inace izabrani tekst selektiran i ne vidi se
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label19.Focus();

        }

        //vraca 0 ako je odabran Trgovac i 1 ako je odabran Direktor
        private int vrati_status_zaposlenika()
        {
            //e ubilo me ovo!!!
            string str1 = "Direktor  ";
            string str2 = ((KeyValuePair<string, string>)comboBox3.SelectedItem).Value;
            int usporedi = string.Compare(str1, str2, false);
            if (usporedi == 0)
            {
                //Console.WriteLine("direktor");
                return 1;
            } 
            else
            {
                //Console.WriteLine("trgovac");
                return 0;
            }
        }

        //generira novi id tako da nađe max u bazi i doda 1
        private int generiraj_id_zaposlenika()
        {
            // Izvadi MAX id iz baze
            da = new SqlDataAdapter("Select MAX(Id) from Korisnici", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

             if (dt.Rows.Count == 1)
            {
                string id1 = dt.Rows[0][0].ToString();
                int id = System.Convert.ToInt32(id1);
                id += 1;
                return id;
            }
            return 0;

        }
     
        //provjeri je li unesen password 
        private Boolean provjeri_pass()
        {
            string password1 = textBox11.Text;
            //IsNullOrEmpty vraca true ako je string prazan, a false inace
            if (String.IsNullOrEmpty(password1))
            {
                MessageBox.Show("Morate unjeti Password!");
                textBox11.Focus();
                return false;
            }
            else
            {
                return true;
            }

        }

        //resetira sva polja
        private void ocisti_formu_zaposlenici()
        {
            fillgridIme_zaposlenici();
            dataGridView4.ClearSelection();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            comboBox3.SelectedIndex = 0;
            button4.Focus();
        }

        //uređuje/dodaje zaposlenika
        private void button4_Click(object sender, EventArgs e)
        {
            string username1 = textBox8.Text;
            string ime1 = textBox9.Text;
            string prezime1 = textBox10.Text;
            string password1 = textBox11.Text;
            string status1 = ((KeyValuePair<string, string>)comboBox3.SelectedItem).Value;
            int dir = vrati_status_zaposlenika();
            int affectedRows = 0;

            //ako je odabrano UREĐIVANJE postojeceg zaposlenika
            //ako je unesen pass (treba jer ga ne prikazujemo pa uvijek mora biti ponovo upisan)
            if (radioButton5.Checked && provjeri_pass())
            {
                string id = dataGridView4.CurrentRow.Cells[4].FormattedValue.ToString();
                int id1 = System.Convert.ToInt32(id);
                //UPDATE baze :)
                SqlCommand Cmd = new SqlCommand("UPDATE Korisnici SET Username=@username, Ime=@ime, Prezime=@prezime, Password=@password, IsDirector=@isdirector, Status=@status WHERE Id=@id", con);
                Cmd.Parameters.AddWithValue("@id", id1);
                Cmd.Parameters.AddWithValue("@username", username1);
                Cmd.Parameters.AddWithValue("@ime", ime1);
                Cmd.Parameters.AddWithValue("@prezime", prezime1);
                Cmd.Parameters.AddWithValue("@password", password1);
                Cmd.Parameters.AddWithValue("@isdirector", dir);
                Cmd.Parameters.AddWithValue("@status", status1);
                affectedRows = Cmd.ExecuteNonQuery();
                Console.WriteLine("promjena");
                ocisti_formu_zaposlenici();
                if (affectedRows == 1)
                {
                    MessageBox.Show("Uspješno uređen korisnik " + username1 + ".");
                }
                
            }
            //ako je odabrano DODAJ novog zaposlenika
            else if (radioButton6.Checked && provjeri_pass())
            {
                //generiramo mu jedinstveni ID
                int id1 = generiraj_id_zaposlenika();

                //INSERT u bazu :)
                SqlCommand Cmd = new SqlCommand("INSERT INTO dbo.Korisnici(Id, Username, Password, IsDirector, Ime, Prezime, Status) VALUES(@id, @username, @password, @isdirector, @ime, @prezime, @status)", con);
                Cmd.Parameters.AddWithValue("@id", id1);
                Cmd.Parameters.AddWithValue("@username", username1);
                Cmd.Parameters.AddWithValue("@ime", ime1);
                Cmd.Parameters.AddWithValue("@prezime", prezime1);
                Cmd.Parameters.AddWithValue("@password", password1);
                Cmd.Parameters.AddWithValue("@isdirector", dir);
                Cmd.Parameters.AddWithValue("@status", status1);
                affectedRows = Cmd.ExecuteNonQuery();
                //Console.WriteLine(affectedRows);
                ocisti_formu_zaposlenici();
                if (affectedRows == 1)
                {
                    MessageBox.Show("Uspješno dodan korisnik " + username1 + ".");
                }
                

            }
        }

        //**************************************************************



        //tab ARTIKLI


        //UREDI postojeci artikl
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //Gumb je Uredi
            button2.Text = "Uredi";
            //Kod artikla ne možemo mijenjati
            textBox2.Enabled = false;
            //DataGrid nam sad treba za izbor artikla kojeg mijenjamo
            dataGridView2.Enabled = true;
            //ubacit cemo i pretrazivanje artikla
            textBox5.Enabled = true;
            dataGridView2.ClearSelection();

        }

        //DODAJ novi artikl
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            //Gumb je Dodaj
            button2.Text = "Dodaj";
            //DataGrid nam ne treba pa ga onesposobimo
            dataGridView2.Enabled = false;
            textBox5.Enabled = false;
            //pa sve lijepo pocistimo
            textBox2.Enabled = true;
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox2.SelectedIndex = 0;
            dateTimePicker3.ResetText();
            dataGridView2.ClearSelection();


        }

        //Klikom na artikl popunjavaju se sadašnji podatci o artiklu
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            string kod1 = "";
            kod1 = dataGridView2.CurrentRow.Cells[0].FormattedValue.ToString();
            string naziv1 = dataGridView2.CurrentRow.Cells[1].FormattedValue.ToString();
            string vrsta1 = dataGridView2.CurrentRow.Cells[2].FormattedValue.ToString();
            string cijena1 = dataGridView2.CurrentRow.Cells[3].FormattedValue.ToString();
            //Ovo radi Barbari, ali ne Bojani
            /*
            string rok1 = dataGridView2.CurrentRow.Cells[4].FormattedValue.ToString();
            dateTimePicker3.Value = DateTime.ParseExact( rok1 , "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //do tu
            */
            //Ovako radi Bojani, Barbari ne radi
            
            string rok = dataGridView2.CurrentRow.Cells[4].FormattedValue.ToString();

            string[] pom = rok.Split('.');
            string rok1 = pom[0] + "/" + pom[1] + "/" + pom[2];
            Console.WriteLine(rok1); 
            DateTime enter_date = Convert.ToDateTime(rok1);
            dateTimePicker3.Value = enter_date;
            
            //do tu

            textBox2.Text = kod1;
            textBox3.Text = naziv1;
            textBox4.Text = cijena1;
            comboBox2.SelectedIndex = comboBox2.FindStringExact(vrsta1);

        }

        //popunjava datagrid s artiklima koje trazimo po imenu! (kopirano iz Form_trgovina) 
        //opet dupli kod al time cu se bavit kad sve bude gotovo
        public void fillgridIme_artikli_artikli()
        {
            // Izvadi sve razlicite vrste proizvoda iz baze
            da = new SqlDataAdapter("select * from Artikli where Naziv like '%" + textBox5.Text + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dataGridView2.DataSource = dt;
               // dataGridView2.Rows[0].Selected = true;
            }
        }

        //DataGrid je inicijalno popunjen svim artiklima ali kad se krene tipkat onda pretrazuje po imenu
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            fillgridIme_artikli_artikli();
            //moram vratit fokus jer zbog comboBoxa sam onu magiju morala gatat pa tu malo zeza
            textBox5.Focus();

        }

        // popunjavanje combobox-a kategorijama (vrstama) iz baze 
        //ako stignem napravit cu f-ju koja prima comboBox pa ga popuni jer je ovo dupli kod :(((
        private void popunjavanje_combo_kategorije_artikli()
        {
            // Izvadi sve razlicite vrste proizvoda iz baze
            da = new SqlDataAdapter("Select Distinct Vrsta from Artikli", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Dictionary<string, string> combo_values = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; ++i)
                    combo_values.Add(i.ToString(), dt.Rows[i][0].ToString());
                comboBox2.DataSource = new BindingSource(combo_values, null);
                comboBox2.DisplayMember = "Value";
                comboBox2.ValueMember = "Key";
            }

        }

        //Makne focus s comboboxa jer je inace izabrani tekst selektiran i ne vidi se
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label9.Focus();
        }

        //provjerava jel kod novog artikla jedinstven, tj. postoji li vec u bazi
        private Boolean provjeri_valjanost_koda()
        {
            string potencijalni_kod = textBox2.Text;
            da = new SqlDataAdapter("SELECT * from Artikli where Kod like '%" + potencijalni_kod  + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                //ako nije valjan upozori korisnika i ocisti polje
                textBox2.Clear();
                MessageBox.Show("Ne mogu postojati artikli s istim kodom! \n Molimo unesite jedinstveni kod!");
                return false;
            }

            return true;
        }

        //provjerava je li unesena cijena fakat broj
        private Boolean provjeri_valjanost_cijene()
        {
            decimal d;
            if (decimal.TryParse(textBox4.Text, out d))
            {
                return true;
            }
            else
            {
                //ako nije broj upozori korisnika i ocisti polje
                textBox4.Clear();
                MessageBox.Show("Unesite ispravnu cijenu!"); 
                return false;
            }
        }

        //resetira sva polja
        private void ocisti_formu_artikli()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            dateTimePicker3.ResetText();
            comboBox2.SelectedIndex = 0;
            fillgridIme_artikli_artikli();
            dataGridView2.ClearSelection();
        }

        //dodaje/uređuje  artikl
        private void button2_Click(object sender, EventArgs e)
        {
            string naziv1 = textBox3.Text;
            string kategorija = ((KeyValuePair<string, string>)comboBox2.SelectedItem).Value;
            System.DateTime datum1 = dateTimePicker3.Value.Date;
            int affectedRows = 0;

            //ako je odabrano UREĐIVANJE postojeceg artikla
            if (radioButton3.Checked && provjeri_valjanost_cijene())
            {
                 //ne pitajte zasto ovo prvo ide u string pa u int.. tako radi XD
                string kod = dataGridView2.CurrentRow.Cells[0].FormattedValue.ToString();
                int kod1 = System.Convert.ToInt32(kod);
                string cijena = textBox4.Text;
                float cijena1 = float.Parse(cijena, CultureInfo.InvariantCulture);

                //UPDATE baze :)
                SqlCommand Cmd = new SqlCommand("UPDATE Artikli SET Naziv=@naziv, Cijena=@cijena, Vrsta=@vrsta, RokTrajanja=@roktrajanja WHERE Kod=@kod", con);
                Cmd.Parameters.AddWithValue("@kod", kod1);
                Cmd.Parameters.AddWithValue("@naziv", naziv1);
                Cmd.Parameters.AddWithValue("@cijena", cijena1);
                Cmd.Parameters.AddWithValue("@vrsta", kategorija);
                Cmd.Parameters.AddWithValue("@roktrajanja", datum1);
                affectedRows = Cmd.ExecuteNonQuery();
                //Console.WriteLine(affectedRows);
                if (affectedRows == 1)
                {
                    MessageBox.Show("Uspješno uređen artikl "+ naziv1 + "." );
                }
                ocisti_formu_artikli();
            }
            //odabrano je DODAVANJE novog artikla 
            //ako je upisan jedinstveni kod dodajemo novi artikl u bazu
            else if (radioButton4.Checked && provjeri_valjanost_koda() && provjeri_valjanost_cijene())
            {
                //popust i kolicina su 0 za nove proizvode i dozvolila sam da datum nabave bude NULL
                int popust1 = 0;
                int kolicina1 = 0;
                string kod = textBox2.Text;
                int kod1 = System.Convert.ToInt32(kod);
                string cijena = textBox4.Text;
                float cijena1 = float.Parse(cijena, CultureInfo.InvariantCulture);
                System.DateTime danas = DateTime.Now;
                //maknuti time dio inace ce uvijek danas biti manje od danasnjeg datuma
                danas = danas.Date;

                //INSERT u bazu :)
                SqlCommand Cmd = new SqlCommand("INSERT INTO dbo.Artikli(Kod, Naziv, Vrsta, Cijena, Popust, Kolicina, DatumNabave, RokTrajanja) VALUES(@kod, @naziv, @vrsta, @cijena, @popust, @kolicina, @datumnabave, @roktrajanja)", con);
                Cmd.Parameters.AddWithValue("@kod", kod1);
                Cmd.Parameters.AddWithValue("@naziv", naziv1);
                Cmd.Parameters.AddWithValue("@vrsta", kategorija);
                Cmd.Parameters.AddWithValue("@cijena", cijena1);
                Cmd.Parameters.AddWithValue("@popust", popust1);
                Cmd.Parameters.AddWithValue("@kolicina", kolicina1);
                Cmd.Parameters.AddWithValue("@datumnabave", danas);
                Cmd.Parameters.AddWithValue("@roktrajanja", datum1);
                affectedRows = Cmd.ExecuteNonQuery();
                //Console.WriteLine(affectedRows); 
                if(affectedRows == 1)
                {
                    MessageBox.Show("Uspješno dodan artikl " + naziv1 + ".");
                }
                ocisti_formu_artikli();
            }

           
        }

        //**************************************************************



        //tab POPUST

        //Ako biramo popust za ARTIKL onda cemo onesposobit sve opcije vezane za kategoriju
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            textBox1.Enabled = true;
            dataGridView1.Enabled = true;
        }

        //Ako biramo popust za KATEGORIJU onesposobimo sve opcije vezane za artikle
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            textBox1.Enabled = false;
            dataGridView1.Enabled = false;
            dataGridView1.ClearSelection();
        }

        //provjera je li datum pocetka prije datuma kraja i je li >= od današnjeg datuma
        private void provjeraDatuma_popusti()
        {
            System.DateTime datum1 = dateTimePicker1.Value.Date;
            System.DateTime datum2 = dateTimePicker2.Value.Date;
            System.DateTime danas = DateTime.Now;
            //maknuti time dio inace ce uvijek danas biti manje od danasnjeg datuma
            danas = danas.Date;
            if (datum1 < danas)
            {
                MessageBox.Show("Ne možete izabrati datum početka koji je ranije od današnjeg datuma!");
            }

            if (datum1 > datum2)
            {
                MessageBox.Show("Morate izabrati datum početka koji je prije datuma završetka!");
            }
        }

        //poziv funkcije koja se brine da su izabrani logicni datumi
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            provjeraDatuma_popusti();
        }

        //poziv funkcije koja se brine da su izabrani logicni datumi
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            provjeraDatuma_popusti();
        }

        //popunjava datagrid s artiklima koje trazimo po imenu! (kopirano iz Form_trgovina)
        public void fillgridIme_artikli_popusti()
        {
            // Izvadi sve razlicite vrste proizvoda iz baze
            da = new SqlDataAdapter("SELECT * from Artikli where Naziv like '%" + textBox1.Text + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
               // dataGridView1.Rows[0].Selected = true;
            }
        }

        //DataGrid je inicijalno popunjen svim artiklima ali kad se krene tipkat onda pretrazuje po imenu
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            fillgridIme_artikli_popusti();
        }

        // popunjavanje combobox-a kategorijama (vrstama) iz baze 
        private void popunjavanje_combo_kategorije_popusti()
        {
            // Izvadi sve razlicite vrste proizvoda iz baze
            da = new SqlDataAdapter("Select Distinct Vrsta from Artikli", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Dictionary<string, string> combo_values = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; ++i)
                    combo_values.Add(i.ToString(), dt.Rows[i][0].ToString());
                comboBox1.DataSource = new BindingSource(combo_values, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
            }
        }

        //Makne focus s comboboxa jer je inace izabrani tekst selektiran i ne vidi se
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label6.Focus();
        }

        //dodavanje odabranog popusta
        private void button1_Click(object sender, EventArgs e)
        {
            // dohvacanje iznosa popusta
            int iznos = Convert.ToInt32(numericUpDown1.Value);
            // dohvacanje datuma pocetka
            System.DateTime datum1 = dateTimePicker1.Value.Date;
            // dohvacanje datuma kraja
            System.DateTime datum2 = dateTimePicker2.Value.Date;
            //ne košta nas ništa provjerit i ovdje jednom izabrane datume
            provjeraDatuma_popusti();
            
            //ako je odabran popust za kategoriju
             if (radioButton2.Checked)
             {
                //dohvati odabranu kategoriju
                 string kategorija = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Value;      
                 int affectedRows = 0;

                //UPDATE baze :)
                 SqlCommand Cmd = new SqlCommand("UPDATE Artikli SET Popust=@popust, PopustPocetak=@popustpocetak, PopustKraj=@popustkraj WHERE Vrsta=@vrsta", con);
                 Cmd.Parameters.AddWithValue("@popust", iznos);
                 Cmd.Parameters.AddWithValue("@popustpocetak", datum1);
                 Cmd.Parameters.AddWithValue("@popustkraj", datum2);
                 Cmd.Parameters.AddWithValue("@vrsta", kategorija);
                 affectedRows = Cmd.ExecuteNonQuery();
                 fillgridIme_artikli_popusti(); 
                 // Console.WriteLine("mijenjamo popust za kategoriju....");
                 //Console.WriteLine(affectedRows);
            }
             //odabran je popust za konkretni artikl
            else
            {
                //dohvati odabrani artikl
                //ne pitajte zasto ovo prvo ide u string pa u int.. tako radi XD
                string kod1 = dataGridView1.CurrentRow.Cells[0].FormattedValue.ToString();
                int kod2 = System.Convert.ToInt32(kod1);
                int affectedRows = 0;

                //UPDATE baze :)
                SqlCommand Cmd = new SqlCommand("UPDATE Artikli SET Popust=@popust, PopustPocetak=@popustpocetak, PopustKraj=@popustkraj WHERE Kod=@kod", con);
                Cmd.Parameters.AddWithValue("@popust", iznos);
                Cmd.Parameters.AddWithValue("@popustpocetak", datum1);
                Cmd.Parameters.AddWithValue("@popustkraj", datum2);
                Cmd.Parameters.AddWithValue("@kod", kod2);
                affectedRows = Cmd.ExecuteNonQuery();

              // Console.WriteLine(affectedRows);
              // Console.WriteLine(kod1);
            }

            //vratimo sve vrijednosti na pocetak da izgleda ljepse :)
            textBox1.Clear();
            dateTimePicker1.ResetText();
            dateTimePicker2.ResetText();
            comboBox1.SelectedIndex = 0;
            numericUpDown1.Value = 0;
            fillgridIme_artikli_popusti(); 
            //Console.WriteLine(kategorija);
            //Console.WriteLine(iznos);
            //Console.WriteLine(datum1);
            //Console.WriteLine(datum2);
        }

        //**************************************************************



        //tab NABAVA

        //ako je checkBoks oznacen onda onesposobi druge opcije na tabu
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                dataGridView3.ClearSelection();
                dataGridView3.Enabled = false;
            }
            if(checkBox1.Checked == false)
            {
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                dataGridView3.ClearSelection();
                dataGridView3.Enabled = true;
            }
         }

        //popunjava datagrid s artiklima koje trazimo po imenu! (kopirano iz Form_trgovina) 
        //opet dupli kod al time cu se bavit kad sve bude gotovo
        public void fillgridIme_artikli_nabava()
        {
            // Izvadi sve razlicite vrste proizvoda iz baze
            da = new SqlDataAdapter("select * from Artikli where Naziv like '%" + textBox6.Text + "%'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dataGridView3.DataSource = dt;
                //dataGridView3.Rows[0].Selected = true;
            }
        }
        
        //DataGrid je inicijalno popunjen svim artiklima ali kad se krene tipkat onda pretrazuje po imenu
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            fillgridIme_artikli_nabava();
        }

        //provjerava je li unesena količina broj 
        public Boolean provjera_valjanosti_kolicine()
        {
            int z;
            if (int.TryParse(textBox7.Text, out z))
            {
                return true;
            }
            else
            {
                textBox7.Clear();
                MessageBox.Show("Količina mora biti broj!");
                return false;
            }
        }

        //resetira sva polja
        private void ocisti_formu_nabava()
        {
            textBox6.Enabled = true;
            textBox7.Enabled = true;
            dataGridView3.Enabled = true;
            textBox6.Clear();
            textBox7.Clear();
            checkBox1.Checked = false;
            fillgridIme_artikli_nabava();
            dataGridView3.ClearSelection();
        }

        //azurira kolicinu artkala ->dva slucaja!
        private void button3_Click(object sender, EventArgs e)
        {
            //danasnji datum
            System.DateTime danas = DateTime.Now;
            //maknuti time dio inace ce uvijek danas biti manje od danasnjeg datuma
            danas = danas.Date;
            int affectedRows = 0;

            //ako je upisana ispravna kolicina obbavlja se update 
            if (checkBox1.Checked == false && provjera_valjanosti_kolicine() == true )
            {
                //ne pitajte zasto ovo prvo ide u string pa u int.. tako radi XD
                string kod = dataGridView3.CurrentRow.Cells[0].FormattedValue.ToString();
                int kod1 = System.Convert.ToInt32(kod);
                //trenutna kolicina 
                string stanje = dataGridView3.CurrentRow.Cells[3].FormattedValue.ToString();
                int stanje1 = System.Convert.ToInt32(stanje);
                string kolicina = textBox7.Text;
                int kolicina1 = System.Convert.ToInt32(kolicina) ;
                //nova kolicina (trenutna + novi artikli)
                kolicina1 += stanje1;

                //UPDATE baze :)
                SqlCommand Cmd = new SqlCommand("UPDATE Artikli SET Kolicina=@kolicina, DatumNabave=@datumnabave WHERE Kod=@kod", con);
                Cmd.Parameters.AddWithValue("@kod", kod1);
                Cmd.Parameters.AddWithValue("@kolicina", kolicina1);
                Cmd.Parameters.AddWithValue("@datumnabave", danas);
                affectedRows = Cmd.ExecuteNonQuery();
                //Console.WriteLine(affectedRows);

                //ako je sve ok proslo ispisi poruku
                if (affectedRows == 1)
                {
                    ocisti_formu_nabava();
                    //izbacimo poruku da korisnik ima osjecaj da se nesto dogodilo
                    //mozda se moze mijenjat neka labela :/ 
                    MessageBox.Show("Uspješno ste dodali "+ kolicina + " artikala.");
                }
            }
            //otpis artikala kojima je istekao rok trajanja (kolicina se postavlja na 0)
            if (checkBox1.Checked == true)
            {

                int kol = 0;
                //UPDATE baze :)
                SqlCommand Cmd = new SqlCommand("UPDATE Artikli SET Kolicina=@kolicina WHERE RokTrajanja <= @roktrajanja AND Kolicina > 0  ", con);
                Cmd.Parameters.AddWithValue("@kolicina", kol);
                Cmd.Parameters.AddWithValue("@roktrajanja", danas);
                affectedRows = Cmd.ExecuteNonQuery();
                ocisti_formu_nabava();
                //izbacimo poruku da korisnik ima osjecaj da se nesto dogodilo
                //mozda se moze mijenjat neka labela :/ 
                if(affectedRows > 0)
                {
                    MessageBox.Show("Otpisano je " + affectedRows + " različitih artikala.");

                }
                else
                {
                    MessageBox.Show("Nema artikala za otpis.");
                }


                //Console.WriteLine(affectedRows);

            }

        }

        //**************************************************************


        //tab TRGOVINA

        //Otvara formu Trgovina
        private void button5_Click(object sender, EventArgs e)
        {
            button5.Visible = false;
            Form_trgovina f = new Form_trgovina(2, ime, this);
            f.Show();
        }

        //**************************************************************


        // zatvorit konekciju na bazu kad se forma zatvara
        private void Form_direktor_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.Close();
        }


    }
}
