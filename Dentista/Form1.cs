using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        List<string> imagenes = new List<string>();
        List<string> imagenes2 = new List<string>();

        int cont = 0;

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "xL72WVNg3zNeNfkWo2aQ7WRX4Nxhjqe1mbtCNru7",
            BasePath = "https://dentista-99f4c.firebaseio.com/"
        };

        public object reader { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);

            if (client != null)
            {
                MessageBox.Show("Conexion establecida");
            }

            //Lista de imagenes
            listView1.View = View.Details;

            listView1.Columns.Add("Imagen",150);
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void AgregarImagenes()
        {

        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new Data
                {
                    Id = textBoxId.Text,
                    Name = textBoxNombre.Text,
                    Address = textBoxDireccion.Text,
                    Age = textBoxEdad.Text,
                    Img = imagenes
                };

                SetResponse response = await client.SetTaskAsync("Cliente/" + textBoxId.Text, data);

                if (data.Img != null)
                {

                }

                Data result = response.ResultAs<Data>();

                MessageBox.Show("Insertado correctamente " + result.Name);               
            }
            catch(Exception w)
            {
                MessageBox.Show("No insertado");
            }


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
            FirebaseResponse response = await client.DeleteTaskAsync("Cliente/" + textBoxId.Text);

            MessageBox.Show("Registro eliminado correctamente");
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            /*MemoryStream ms = new MemoryStream();
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

            imageBox.Image = null;*/

            /*MemoryStream ms = new MemoryStream();

            imageBox.Image.Save(ms, ImageFormat.Jpeg);

            byte[] a = ms.GetBuffer();

            string output = Convert.ToBase64String(a);

            imagenes.Add(output);
            */

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
            ofd.Title = "Selecciona la(s) imagen(es)";
            ofd.Filter = "Image files(*.jpg) | *.jpg";
            ofd.FilterIndex = 10;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in ofd.FileNames)
                {
                    Image img = new Bitmap(file);
                    MemoryStream ms = new MemoryStream();

                    img.Save(ms, ImageFormat.Jpeg);

                    byte[] a = ms.GetBuffer();

                    string output = Convert.ToBase64String(a);
                    Console.WriteLine(output);

                    //output = file;

                    imagenes.Add(output);
                    //cont++;
                    //imagenes.Add(file);
                    //imageBox.Image = img.GetThumbnailImage(424, 152, null, new IntPtr());

                }

            }


        }
        private async void TraerImagenes()
        {            
            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(100, 100);

            
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
                    imagenes = obj.Img;                    

                    foreach (string i in imagenes)
                        {
                                                                    
                            byte[] b = Convert.FromBase64String(i);

                            MemoryStream ms = new MemoryStream();
                            ms.Write(b, 0, Convert.ToInt32(b.Length));

                            Bitmap bm = new Bitmap(ms, false);
                            ms.Dispose();

                        //pictureBox1.Image = bm;
                        imgs.Images.Add(bm);
                        //imagenes2.Add(bm);
                        cont++;
                       
                    }
                    for(int i=1;i<=cont;i++)
                    {
                        listView1.SmallImageList = imgs;
                        listView1.Items.Add("Imagen " + i, cont-i);
                        
                    }
                    MessageBox.Show("Registro encontrado");
                }                
            }
            catch (Exception w)
            {
                Console.WriteLine(w);
                MessageBox.Show("Registro no encontrado verifique su busqueda");

            }

        }

        private void Button8_Click(object sender, EventArgs e)
        {
            TraerImagenes();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
