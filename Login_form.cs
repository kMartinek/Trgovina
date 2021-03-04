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

namespace Trgovina_app
{
    public partial class Login_form : Form
    {
       
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
                Form_trgovina f = new Form_trgovina(1, username);
                f.Closed += (s, args) => this.Close();
                f.Show();

            }
            else
            {
                this.Hide();
                Form_trgovina f = new Form_trgovina(2, username);
                f.Closed += (s, args) => this.Close();
                f.Show();

            }
            Username_textBox.Clear();
            Password_textBox.Clear();
        }

        private int provjera(String user_text, String pass_text)
        {
            //Bojana
            //SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Bojana\\Desktop\\faks\\rp3moji\\Trgovina_app\\Trgovina_app\\NovaBaza.mdf;Integrated Security=True;Connect Timeout=30");
            //Barbara
            SqlConnection con = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Kac\Documents\BazaK.mdf; Integrated Security = True; Connect Timeout = 30");
            SqlCommand cmd = new SqlCommand("select Username, IsDirector from [dbo].[Korisnici] where Username= '"+user_text+"' and Password='"+pass_text+"'" , con);

            cmd.Connection.Open();
            SqlDataReader r = cmd.ExecuteReader();
            /*
                        while (r.Read())
                            Console.WriteLine(r[0].ToString() + " " + r[1].ToString() );
                        r.Close();
                        */
            while (r.Read())
            {
                Console.WriteLine(r[0].ToString() + " " + r[1].ToString());
                if (r[1].ToString() == "0") return 1;
                else if (r[1].ToString() == "1") return 2;
                
            }
            //if(provjera nije prosla) vraća 0
            //if(provjera prosla i nije admin) vraća 1
            //if (provjera prosla i admin) vraća 2
            return 0; 
        }

        private void Login_form_Load(object sender, EventArgs e)
        {

        }
    }
}
