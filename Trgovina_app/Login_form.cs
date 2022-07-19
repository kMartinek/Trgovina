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
    public partial class Login_form : Form
    {
        static string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        //Barbara (ovdje jednom otvorim konekciju i to je to)
        //static string ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\barbara\Desktop\projekt subota ujutro 16 02\Trgovina_app\Trgovina_app\BazaK.mdf; Integrated Security = True; Connect Timeout = 30";
        //Bojana
        static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + path + @"\BazaK.mdf;Integrated Security=True;Connect Timeout=30";

        string ime = "";
        string prezime = "";


        SqlConnection con = new SqlConnection(ConnectionString);

        public Login_form()
        {
            InitializeComponent();

        }

        private void Login_button_Click(object sender, EventArgs e)
        {
            //sljedece naredbe pociste textBoxove nakon klika
           
            //pozovi funkciju za provjeru logina
            provjera(Username_textBox.Text.ToString(), Password_textBox.Text.ToString());
            //iz baze uzmi i ime korisnika
            String username = Username_textBox.Text ;
            if (provjera(Username_textBox.Text.ToString(), Password_textBox.Text.ToString()) == 0)
                MessageBox.Show("Krivi uneseni podaci, pokušajte ponovo!");
            else if (provjera(Username_textBox.Text.ToString(), Password_textBox.Text.ToString()) == 1)
            {          
                this.Hide();
                string podaci = ime + " " + prezime;
                Form_trgovina f = new Form_trgovina(1, podaci, this);
                f.Closed += (s, args) => this.Close();
                f.Show();
            }
            else
            {
                this.Hide();
                string podaci = ime + " " + prezime;
                Form_direktor f = new Form_direktor(2, podaci);
                f.Closed += (s, args) => this.Close();
                f.Show();

            }
            Username_textBox.Clear();
            Password_textBox.Clear();
        }

        private int provjera(String user_text, String pass_text)
        {
            con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("select Username, IsDirector, Ime, Prezime  from [dbo].[Korisnici] where Username= '"+user_text+"' and Password='"+pass_text+"'" , con);

            cmd.Connection.Open();
            SqlDataReader r = cmd.ExecuteReader();
            
            while (r.Read())
            {
                Console.WriteLine(r[0].ToString() + " " + r[1].ToString());
                ime = r[2].ToString();
                ime = ime.Trim();
                prezime = r[3].ToString();
                prezime.Replace(" ", string.Empty);
                if (r[1].ToString() == "0") return 1;
                else if (r[1].ToString() == "1") return 2;
                
            }
            //if(provjera nije prosla) vraća 0
            //if(provjera prosla i nije admin) vraća 1
            //if (provjera prosla i admin) vraća 2
            return 0; 
        }

        //nakon upisanog passworda, često imamo refleks kliknuti enter, to nam ova fja omogucuje
        private void Password_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login_button_Click(this, new EventArgs());
            }
        }

        //zatvaramo konekciju pri zatvaranju forme
        private void Login_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.Close(); 
        }

        private void Login_form_Load(object sender, EventArgs e)
        {

        }
    }
}
