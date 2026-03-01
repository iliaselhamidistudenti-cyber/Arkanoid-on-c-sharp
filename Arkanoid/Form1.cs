using Arkanoid.Properties;

namespace Arkanoid
{
    public partial class Form1 : Form
    {
        /*
         * blocco blu vale 13 punti
         * blocco giallo vale 17 punti
         * blocco rosso vale 20 punti
         * blocco verde vale 25 punti
         */
        //carlo stupido
        public int speed = 3;
        Keys keys;
        PictureBox logo;
        public bool collisione = false;
        List<PictureBox> Blocchi;
        Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
        }
        public void InizializzaBLocchi(bool stato)
        {
            foreach (PictureBox pictureBox in Blocchi)
            {
                pictureBox.Visible = stato;
            }
        }
        public void RandomColor()
        {
            int Colore;
            foreach (PictureBox pictureBox in Blocchi)
            {
                Colore = rnd.Next(1, 23);
                if (Colore >= 1 && Colore <= 9) pictureBox.Image = Resources.b_blu;
                else if (Colore > 9 && Colore <= 15) pictureBox.Image = Resources.b_giallo;
                else if (Colore > 15 && Colore <= 19) pictureBox.Image = Resources.b_rosso;
                else if (Colore > 19) pictureBox.Image = Resources.b_verde;
            }
        }
        public int PuntiBlocco(PictureBox pictureBox)
        {
            int punti = 0;
            if (pictureBox.Image == Resources.b_blu) punti = 13;
            if (pictureBox.Image == Resources.b_giallo) punti = 17;
            if (pictureBox.Image == Resources.b_rosso) punti = 20;
            if (pictureBox.Image == Resources.b_verde) punti = 25;

            return punti;
        }

        private void SchermataIniziale()
        {
            logo = new PictureBox();
            logo.Image = Resources.logo95;
            logo.BackColor = System.Drawing.Color.Transparent;
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.Height = this.ClientSize.Height / 3;
            logo.Width = this.ClientSize.Width / 2;
            logo.Top = this.ClientSize.Height / 4;
            logo.Left = (this.ClientSize.Width - logo.Width) / 2;
            this.Controls.Add(logo);
            Barra.Visible = false;
            palla.Visible = false;
        }
        public record Movimento(int left, int right, int up, int down);
        private void Barra_Movimento()
        {

            Barra.Left = (this.ClientSize.Width - Barra.Width) / 2;
            Barra.Visible = true;
            IProgress<Movimento> progress = new Progress<Movimento>(
                valore =>
                {
                    if (keys == Keys.Left && Barra.Left > 0)
                    {
                        Barra.Left -= speed;
                    }
                    else if (keys == Keys.Right && Barra.Right < this.ClientSize.Width)
                    {
                        Barra.Left += speed;
                    }
                }
            );
            var task = Task.Run(() =>
            {
                while (true)
                {
                    progress.Report(new Movimento(speed, speed, 0, 0));
                    Task.Delay(10).Wait();
                }
            });
        }
        private void Pallina()
        {
            palla.Left = (this.ClientSize.Width - Barra.Width) / 2;
            palla.Visible = true;
            IProgress<Movimento> progress = new Progress<Movimento>(
                valore =>
                {
                    palla.Left -= speed;
                }
            );
            var task = Task.Run(() =>
            {
                while (true)
                {
                    progress.Report(new Movimento(speed, speed, 0, 0));
                    Task.Delay(10).Wait();
                }
            });
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Blocchi = new List<PictureBox>()
            {
                b1,b2,b3,b4,b5,b6,b7,b8,b9,b10,b11,
                bd1,bd2,bd3,bd4,bd5,bd6,bd7,bd8,bd9,bd10,bd11,
                bt1,bt2,bt3,bt4,bt5,bt6,bt7,bt8,bt9,bt10,bt11,
                bq1,bq2,bq3,bq4,bq5,bq6,bq7,bq8,bq9,bq10,bq11,
                bc1,bc2,bc3,bc4,bc5,bc6,bc7,bc8,bc9,bc10,bc11,
                bs1,bs2, bs3, bs4, bs5, bs6, bs7, bs8, bs9,bs10,bs11,
                bo1, bo2, bo3, bo4, bo5, bo6, bo7, bo8, bo9, bo10, bo11,
                bn1, bn2, bn3, bn4, bn5, bn6, bn7, bn8, bn9, bn10, bn11
            };
            InizializzaBLocchi(false);
            SchermataIniziale();



        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (logo != null)
            {
                this.Controls.Remove(logo);
                single_p.Visible = false;
                Barra_Movimento();
                Pallina();
                RandomColor();
                InizializzaBLocchi(true);

            }
        }
        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            keys = e.KeyCode;
        }
        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            keys = Keys.None;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox84_Click(object sender, EventArgs e)
        {

        }
    }
}
