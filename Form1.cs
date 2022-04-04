using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace bmp_istasyon_deneme_2
{
    public partial class Form1 : Form
    {
        private string data;    
        double maxTemp = 0, maxYuk=0, maxPres = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;                    //textBox1'i yalnızca okunabilir şekilde ayarla
            string[] ports = SerialPort.GetPortNames();  //Seri portları diziye ekleme
            foreach (string port in ports)
                comboBox1.Items.Add(port);               //Seri portları comboBox1'e ekleme

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //DataReceived eventini oluşturma
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            data = serialPort1.ReadLine();                     //Gelen veriyi okuma
            this.Invoke(new EventHandler(displayData_event));
        }
        private void displayData_event(object sender, EventArgs e)
        {
            DateTime myDateValue = DateTime.Now;    //Güncel zaman bilgisini al
            label7.Text = myDateValue.ToString();  //Güncel zaman bilgisini label8'e yaz
           // textBox5.Text += DateTime.Now.ToString() + "        " + data + "\n"; //Gelen veriyi textBox içine güncel zaman ile ekle
            string[] value = data.Split('/');    //'/' gördüğün yerlerden stringi ayır ve diziye ata
            textBox1.Text = value[2];
            textBox2.Text = value[0];
            textBox3.Text = value[1];
            textBox5.Text += DateTime.Now.ToString() + "      " + "sıcaklık: " + value[2] + "  " + "basınç: " + value[0] + "   " + "yükseklik: " + value[1] + Environment.NewLine + "\n";

            double yukseklik = Convert.ToDouble(value[0]);    //String değişkenlerini double'a dönüştür
            double temp = Convert.ToDouble(value[1]);
            double pressure = Convert.ToDouble(value[2]);
            maksimumBul(temp, yukseklik, pressure);   //Maksimum değerleri bulmak için
        }
        private void maksimumBul(double t, double h, double p)
        {
            if (t > maxTemp)    //Güncel sıcaklık değeri maksimumdan büyükse
                maxTemp = t;

            if (h > maxYuk)     //Güncel nem değeri maksimumdan büyükse
                maxYuk = h;

            if (p > maxPres)    //Güncel basınç değeri maksimumdan büyükse
                maxPres = p;

            textBox7.Text = maxTemp.ToString();  //Maksimum sıcaklığı yazdır
            textBox6.Text = maxYuk.ToString();   //Maksimum nemi yazdır
            textBox4.Text = maxPres.ToString();  //Maksimum basıncı yazdır
        }

        private void baglan_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;       //Port ismini comboBox1'in text'i olarak belirle
                serialPort1.BaudRate = 9600;                 //BaudRate'i 9600 olarak ayarla
                serialPort1.Parity = Parity.None;            //Eşlik biti yok
                serialPort1.DataBits = 8;                    //Byte başına bit uzunluğu
                serialPort1.StopBits = StopBits.One;
                serialPort1.Open();                          //Seri portu aç

                kes.Enabled = true;                          //"Kes" butonunu tıklanabilir yap
                baglan.Enabled = false;                      //"Bağlan" butonunu tıklanamaz yap 
                label6.Text = "Bağlantı sağlandı";
                label6.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata");  //Hata mesajı
            }
        }

        private void kes_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();              //Seri portu kapa
                kes.Enabled = false;              //"Kes" butonunu tıklanamaz yap
                baglan.Enabled = true;            //"Bağlan" butonunu tıklanabilir yap
                label6.Text = "Bağlantı kesildi";
                label6.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message); //Hata mesajı
            }
        }

        private void kayit_Click(object sender, EventArgs e)
        {
            try
            {
                string filelocation = @"C:\Users\burak\dosya\";                                   //Dosyanın kaydedileceği konumu belirliyoruz
                string filename = "data.txt";                                                               //Kaydedilecek dosyanın ismi
                System.IO.File.WriteAllText(filelocation + filename, "Zaman\t\t\tDeğer\n" + textBox5.Text); //Dosya konumuna textBox1 üstündeki verilerden oluşan text dosyamızı kaydediyoruz uğraş buraya
                MessageBox.Show("Dosya başarıyla kaydedildi", "Mesaj");                                     //Dosya kaydedildiğinde kullanıcıya mesaj gönder
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Hata");       //Hata mesajı
            }
        }

        private void sifirla_Click(object sender, EventArgs e)
        {
            textBox1.Clear();           //textBox1'i sıfırla
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();    //Seri port açıksa kapat
        }
    }
}
