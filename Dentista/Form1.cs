using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


namespace Dentista
{
    public partial class Form1 : Form
    {
        IFirebaseClient client;

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "xL72WVNg3zNeNfkWo2aQ7WRX4Nxhjqe1mbtCNru7",
            BasePath = "https://dentista-99f4c.firebaseio.com/"
        }; 




        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);

            if(client != null)
            {
                //MessageBox.Show("Conexion establecida");
            }
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            var data = new Data {
                Id = textBoxId.Text,
                Name = textBoxEdad.Text,
                Address = textBoxDireccion.Text,
                Age = textBoxNombre.Text
                };

            SetResponse response = await client.SetTaskAsync("Cliente/" + textBoxId.Text,data);
            Data result = response.ResultAs<Data>();

            MessageBox.Show("Insertado correctamente " + result.Name);

        }

        private async void Button2_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetTaskAsync("Cliente/" + textBoxId.Text);

            try
            {
                Data obj = response.ResultAs<Data>();

                if (obj != null)
                {
                    textBoxId.Text = obj.Id;
                    textBoxEdad.Text = obj.Name;
                    textBoxDireccion.Text = obj.Address;
                    textBoxNombre.Text = obj.Age;

                    MessageBox.Show("Registro encontrado");
                }


            }
            catch (Exception w)
            {
                Console.WriteLine(w);
                MessageBox.Show("Registro no encontrado verifique su busqueda");

            }

            

        }

        private async void Button3_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                Id = textBoxId.Text,
                Name = textBoxEdad.Text,
                Address = textBoxDireccion.Text,
                Age = textBoxNombre.Text
            };

            FirebaseResponse response = await client.UpdateTaskAsync("Cliente/" + textBoxId.Text, data);
            Data result = response.ResultAs<Data>();
            MessageBox.Show("Registro modificado exitosamente");

        }

        private async void Button4_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteTaskAsync("Cliente/"+textBoxId.Text);

            MessageBox.Show("Registro eliminado correctamente");    
        }

        private async void Button6_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            imageBox.Image.Save(ms, ImageFormat.Jpeg);

            byte [] a = ms.GetBuffer();

            string output = Convert.ToBase64String(a);

            var data = new ImageModal
            {
                Img = output
            };

            SetResponse response = await client.SetTaskAsync("Imagen/" + textBoxId.Text,data);
            ImageModal result = response.ResultAs<ImageModal>();

            MessageBox.Show("Se inserto la imagen correctamente");

            imageBox.Image = null;

        }

        private async void Button7_Click(object sender, EventArgs e)
        {
            try {

                FirebaseResponse response = await client.GetTaskAsync("Imagen/" + textBoxId.Text);
                ImageModal image = response.ResultAs<ImageModal>();
                                
                byte[] b = Convert.FromBase64String(image.Img);

                MemoryStream ms = new MemoryStream();
                ms.Write(b, 0, Convert.ToInt32(b.Length));

                Bitmap bm = new Bitmap(ms, false);
                ms.Dispose();

                imageBox.Image = bm;
            }
            catch (Exception w)
            {
                Console.WriteLine(w);
                MessageBox.Show("No se encontro ninguna imagen");
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Selecciona la imagen";
            ofd.Filter = "Image files(*.jpg) | *.jpg";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                Image img = new Bitmap(ofd.FileName);
                imageBox.Image = img.GetThumbnailImage(424, 152, null, new IntPtr());
            }
        }
    }
}
