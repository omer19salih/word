using NAudio.Wave;
using sonödev1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace sonödev1
{
    public partial class giris : Form
    {
        private WaveOutEvent waveOut;
        private Mp3FileReader mp3Reader;

        int progressValue = 0;
        public giris()
        {
            InitializeComponent();
        }

        

           

        private void giris_Load(object sender, EventArgs e)
        {
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            timer1.Interval = 100; // 50ms hızında çalışacak
            timer1.Start();
            try
            {
                // MP3 dosyasının yolunu belirtin
                string mp3FilePath = @"C:\Users\salih ömer\source\repos\sonödev1\Resources\sess.mp3";

                // MP3 dosyasını oku
                mp3Reader = new Mp3FileReader(mp3FilePath);

                // Ses çıkışı ayarla
                waveOut = new WaveOutEvent();
                waveOut.Init(mp3Reader);

                // Çalmaya başla
                waveOut.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            progressValue += 2; // ProgressBar'ı artır
            progressBar1.Value = progressValue;

            if (progressValue >= 100)
            {
                timer1.Stop();
                progressBar1.Visible = false; // ProgressBar'ı gizle
                waveOut?.Dispose();
                mp3Reader?.Dispose();
                Form3 form3 = new Form3();
                form3.Show();
                this.Hide();

            }
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

     
    }
}
