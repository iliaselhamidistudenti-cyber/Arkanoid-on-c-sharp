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
        public int punteggio_totale = 0;
        public int palla_velY = 2;
        public int palla_velX = 2;
        public int speed = 3;
        public bool game_over = false;
        Keys keys;
        PictureBox logo;
        List<PictureBox> Blocchi;
        Random rnd = new Random(Environment.TickCount);
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
        static Image img_blu = Resources.b_blu;
        static Image img_giallo = Resources.b_giallo;
        static Image img_rosso = Resources.b_rosso;
        static Image img_verde = Resources.b_verde;
        public void RandomColor()
        {
            int Colore;
            foreach (PictureBox pictureBox in Blocchi)
            {
                Colore = rnd.Next(1, 23);
                if (Colore >= 1 && Colore <= 9) pictureBox.Image = img_blu;
                else if (Colore > 9 && Colore <= 15) pictureBox.Image = img_giallo;
                else if (Colore > 15 && Colore <= 19) pictureBox.Image = img_rosso;
                else if (Colore > 19) pictureBox.Image = img_verde;
            }
        }

        public int PuntoBlocco(PictureBox pictureBox)
        {
            if (pictureBox.Image == img_blu) return 13;
            if (pictureBox.Image == img_giallo) return 17;
            if (pictureBox.Image == img_rosso) return 20;
            if (pictureBox.Image == img_verde) return 25;
            return 0;
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
            InizializzaBLocchi(false);
            single_p.Visible = true;
            punteggio.Visible = false;
            game_over = false;
            Comandi.Visible = true;
            Combat.Visible = true;
        }
        public void PowerUp()
        {

        }
        public record Movimento(int left, int right, int up, int down);
        CancellationTokenSource pallaCTS;
        public void Pallina_Movimento()
        {
            if (pallaCTS != null)
            {
                pallaCTS.Cancel();
                pallaCTS.Dispose();
            }
            pallaCTS = new CancellationTokenSource();
            var token = pallaCTS.Token;

            IProgress<Movimento> segue = new Progress<Movimento>(
                valore =>
                {
                    palla.Visible = true;
                    palla.Left = Barra.Left + (Barra.Width / 2) - (palla.Width / 2);
                    palla.Top = Barra.Top - palla.Height;
                });
            IProgress<Movimento> rimbalzo = new Progress<Movimento>(
                valore =>
                {
                    //prima versione ma non funzionava bene
                    //foreach (var blocco in Blocchi)
                    //{
                    //    if (palla.Top <= blocco.Bottom || palla.Bottom >= blocco.Top || palla.Top <= 0 || palla.Bottom >= this.ClientSize.Height)
                    //    {
                    //        palla_velY *= -1;
                    //    }
                    //    else
                    //    {
                    //        if (palla.Left <= blocco.Right || palla.Right >= blocco.Left || palla.Left <= 0 || palla.Right >= this.ClientSize.Width)
                    //        {
                    //            palla_velX *= -1;
                    //        }
                    //    }
                    //}
                    int p_d = palla.Right;
                    int p_s = palla.Left;
                    int p_so = palla.Bottom;
                    int p_su = palla.Top;
                    palla.Top -= palla_velY;
                    palla.Left += palla_velX;

                    if (palla.Left <= 0 || palla.Right >= this.ClientSize.Width)
                        palla_velX *= -1;

                    if (palla.Top <= 0)
                        palla_velY *= -1;
                    else if (palla.Bottom >= this.ClientSize.Height)
                        game_over = true;

                    foreach (var blocco in Blocchi)
                    {
                        if (!blocco.Visible) continue;
                        if (palla.Right >= blocco.Left && palla.Left <= blocco.Right &&
                            palla.Bottom >= blocco.Top && palla.Top <= blocco.Bottom)
                        {
                            punteggio_totale += PuntoBlocco(blocco);
                            punteggio.Text = "Punteggio: " + punteggio_totale;
                            if (p_d <= blocco.Left || p_s >= blocco.Right)
                                palla_velX *= -1;
                            else
                                palla_velY *= -1;
                            blocco.Visible = false;

                            break;
                        }
                    }
                    if (palla.Right >= Barra.Left && palla.Left <= Barra.Right &&
                        palla.Bottom >= Barra.Top && palla.Top <= Barra.Bottom)
                    {
                        if (p_so <= Barra.Top)
                            palla_velY *= -1;
                    }
                });
            var task = Task.Run(() =>
            {
                while (keys != Keys.Space && !token.IsCancellationRequested)
                {
                    segue.Report(new Movimento(speed, speed, speed, speed));
                    Task.Delay(10).Wait();
                }
            }).ContinueWith(t =>
            {
                while (!game_over && !token.IsCancellationRequested)
                {
                    rimbalzo.Report(new Movimento(speed, speed, speed, speed));
                    Task.Delay(10).Wait();
                }
            }).ContinueWith(t =>
            {
                if (game_over && !token.IsCancellationRequested)
                {
                    MessageBox.Show("Game Over!");
                    SchermataIniziale();
                }
                else
                {
                    MessageBox.Show("Hai vinto!");
                    SchermataIniziale();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        CancellationTokenSource barraCTS;
        private void Barra_Movimento()
        {
            if (barraCTS != null)
            {
                barraCTS.Cancel();
                barraCTS.Dispose();
            }
            barraCTS = new CancellationTokenSource();
            var token = barraCTS.Token;

            Barra.Left = (this.ClientSize.Width - Barra.Width) / 2;
            Barra.Visible = true;
            IProgress<Movimento> movimento = new Progress<Movimento>(
                valore =>
                {
                    if (keys == Keys.Left && Barra.Left > 0)
                        Barra.Left -= speed;
                    else if (keys == Keys.Right && Barra.Right < this.ClientSize.Width)
                        Barra.Left += speed;
                });

            var task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    movimento.Report(new Movimento(speed, speed, 0, 0));
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
            int direzione = rnd.Next(1, 2);
            InizializzaBLocchi(false);
            SchermataIniziale();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (logo != null)
            {
                this.Controls.Remove(logo);
                Combat.Visible= false;
                Comandi.Visible = false;
                logo = null;
                single_p.Visible = false;
                Barra_Movimento();
                RandomColor();
                InizializzaBLocchi(true);
                palla.Visible = true;
                punteggio.Visible = true;
                punteggio_totale = 0;
                Pallina_Movimento();
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

        private void Comandi_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Usa le frecce sinistra e destra per muovere la barra," +
                " premi spazio per far partire la pallina e colpisci i blocchi" +
                " per guadagnare punti! I blocchi blu valgono 13 punti, i gialli 17, i rossi 20 e i verdi 25." +
                " Per giocare in 1v1 un player utilizza A e D e l'altro utilizza le frecce sinistra e destra.");
        }
    }
}
