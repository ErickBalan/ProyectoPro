using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Net.NetworkInformation;

namespace ProyectoPro
{
    public partial class Form1 : Form
    {
        private Gasolinera gasolinera;
        private SerialPort serialPort;
        private Timer timer;
        private int porcentajeLlenado;
        public Form1()
        {
            InitializeComponent();
            gasolinera = new Gasolinera(10.0);

          
            serialPort = new SerialPort("COM3", 9600);
            serialPort.Open();
            timer = new Timer();
            timer.Interval = 50; 
            timer.Tick += Timer_Tick;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox8.Text = "Guardado";
            string nombreCliente = textBox1.Text;
            string apellidoCliente = textBox2.Text;
            string idCliente = textBox3.Text;

            if (string.IsNullOrEmpty(nombreCliente) || string.IsNullOrEmpty(apellidoCliente) || string.IsNullOrEmpty(idCliente))
            {
                MessageBox.Show("Complete los campos.");
                return;
            }

            string nuevoDato = $"Id: {idCliente}; Nombre: {nombreCliente}; Apellido: {apellidoCliente};";
            gasolinera.GuardarAbastecimiento(nuevoDato);

            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tipoAbastecimiento = radioButton1.Checked ? "Tanque lleno" : "Prepago";

            double precioPorLitro;
            if (!double.TryParse(textBox4.Text, out precioPorLitro) || precioPorLitro <= 0)
            {
                MessageBox.Show("Por favor, ingrese un precio válido.");
                return;
            }

            double cantidadLitros = tipoAbastecimiento == "Tanque lleno" ? 2 : 1;
            double precioTotal = cantidadLitros * precioPorLitro;
            DateTime horaActual = DateTime.Now;

            porcentajeLlenado = 0;
            listBox1.Items.Clear();
            timer.Start();

            listBox3.Items.Add($"Tipo de abastecimiento: {tipoAbastecimiento}");
            listBox3.Items.Add($"Precio total: Q{precioTotal}");

            string nuevoDato = $"Tipo de abastecimiento: {tipoAbastecimiento}, Precio total: Q{precioTotal}, Hora: {horaActual}";
            gasolinera.GuardarAbastecimiento(nuevoDato);

            LimpiarCasillas();

            button1.Enabled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (porcentajeLlenado <= 100)
            {
                listBox1.Items.Add($"Porcentaje de llenado: {porcentajeLlenado}%");
                porcentajeLlenado++;
            }
            else
            {
                timer.Stop();
            }
        }

        private void LimpiarCasillas()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox6.Clear();
            textBox8.Clear();
            radioButton1.Checked = false;
            radioButton2.Checked = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            textBox5.Clear();
            textBox7.Clear();

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        listBox2.Items.Add(line);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al leer el archivo: {ex.Message}");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            string selectedItem = listBox2.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedItem) && selectedItem.Contains("Precio"))
            {
                string[] partes = selectedItem.Split(new[] { ": " }, StringSplitOptions.None);
                if (partes.Length > 1)
                {
                    string precioConEtiqueta = partes[1].Trim();
                    string precioSinEtiqueta = precioConEtiqueta.Split(new[] { " " }, StringSplitOptions.None)[0];
                    string precioSinPuntoYComa = precioSinEtiqueta.Replace(";", "");
                    textBox4.Text = precioSinPuntoYComa;
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = listBox2.SelectedItem?.ToString();
            if (selectedItem == null) return;

            string[] partes = selectedItem.Split(new[] { ": " }, StringSplitOptions.None);

            if (partes.Length > 1)
            {
                string numeros = new string(partes[1].Where(char.IsDigit).ToArray());
                textBox5.Text = numeros;

                if (partes.Length > 2)
                {
                    string numeros2 = new string(partes[2].Where(char.IsDigit).ToArray());
                    textBox7.Text = numeros2;
                }
                else
                {
                    textBox7.Clear();
                }
            }
        }

       

        private void ActualizarDatosDelArchivo()
        {
            var dataTable = (DataTable)dataGridView1.DataSource;
            if (dataTable != null)
            {
                var lines = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string nuevoDato = $"Id: {row["IdCliente"]}; Nombre: {row["NombreCliente"]}; Apellido: {row["ApellidoCliente"]};";
                    lines.Add(nuevoDato);
                }

                File.WriteAllLines("Cuentas.txt", lines);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string filePath = "Cuentas.txt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i += 2)
                {
                    if (i + 1 < lines.Length)
                    {
                       
                        string[] clienteDatos = lines[i].Split(';');
                        string id = clienteDatos[0].Split(':')[1].Trim();
                        string nombre = clienteDatos[1].Split(':')[1].Trim();
                        string apellido = clienteDatos[2].Split(':')[1].Trim();

                     
                        string[] abastecimientoDatos = lines[i + 1].Split(',');
                        string tipo = abastecimientoDatos[0].Split(':')[1].Trim();
                        string precio = abastecimientoDatos[1].Split(':')[1].Trim();
                        string hora = abastecimientoDatos[2].Split(':')[1].Trim();

                       
                        dataGridView1.Rows.Add(id, nombre, apellido, tipo, precio, hora);
                    }
                }
            }

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
    
}
