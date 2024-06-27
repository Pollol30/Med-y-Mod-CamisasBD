using MySql.Data.MySqlClient;
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

namespace Captura
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);
        }
        private void label1_Click(object sender, EventArgs e)
        {
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Deshabilitar el botón de actualización al cargar el formulario
            btnactualizar.Enabled = false;
            // Deshabilitar el botón de eliminar al cargar el formulario
            btneliminar.Enabled = false;
        }

        private void btnagregar_Click(object sender, EventArgs e)
        {


            //Valida que todos los campos del formulario los llene el usuario
            if (string.IsNullOrWhiteSpace(txtcodigo.Text) || string.IsNullOrWhiteSpace(txtprecio.Text) || string.IsNullOrWhiteSpace(txtdescrip.Text) || string.IsNullOrWhiteSpace(txtnombre.Text) || string.IsNullOrWhiteSpace(txtexistencias.Text) || string.IsNullOrWhiteSpace(txttalla.Text))
            {
                MessageBox.Show("Favor de completar los campos obligatorios");
                return;
            }
            else {

                // Valida que los campos contengan valores numéricos
                if (!int.TryParse(txtcodigo.Text, out int codigo) || !double.TryParse(txtprecio.Text, out double precio) || !int.TryParse(txtexistencias.Text, out int existencia))
                {
                    MessageBox.Show("Los campos 'código', 'precio' y 'existencia' deben contener solo valores numéricos.");
                    return;
                }

                ////capturar los valores de los textbox
                String nombre = txtnombre.Text;
                String descripcion = txtdescrip.Text;
                char talla = char.Parse(txttalla.Text);

                // Verificar si el código ya existe en la base de datos
                string existeSql = "SELECT COUNT(*) FROM productos WHERE codigo = @codigo";
                using (MySqlConnection con1 = Conexion.conexion())
                {
                    con1.Open();
                    using (MySqlCommand existeCmd = new MySqlCommand(existeSql, con1))
                    {
                        existeCmd.Parameters.AddWithValue("@codigo", codigo);
                        int count = Convert.ToInt32(existeCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("El código ya existe en la base de datos, ingrese uno nuevo");
                            return;
                        }
                    }
                }

                //sentencia sql para guardar los datos en la BD
                string sql = "INSERT INTO productos(codigo,nombre,descripcion,precio,existencias,talla)"+
                "VALUES ('"+codigo+"','"+nombre+"','"+descripcion+"','"+precio+"','"+existencia+"','"+talla+"')";

                ////instancia a la clase conexion
                MySqlConnection con = Conexion.conexion();
                con.Open();

            try {
                ////comprobar que se guarda en la BD
                MySqlCommand command = new MySqlCommand(sql, con);
                command.ExecuteNonQuery();
                MessageBox.Show("Registro almacenado");
            }catch(Exception ex){
                //si no se establece la conexión se muestra lo siguiente
                MessageBox.Show("Error al almacenar"+ex.Message);
            }
            finally {
                //se cierra la conexión con la BD
                con.Close();
            }
                //se manda a llamar el método limpiar para vaciar los campos una vez termina la operación de guardado
                limpiar();
            }
            
        }
        

        private void btnbuscar_Click(object sender, EventArgs e)
        {

            String codigo = txtcodigo.Text;
            MySqlDataReader reader = null;

            //sentencia sql para seleccionar la información correspondiente de la BD
            String sql = "SELECT id, codigo, nombre, descripcion, precio, existencias, talla FROM productos WHERE codigo LIKE '" + codigo + "' LIMIT 1";
            MySqlConnection conexionBD = Conexion.conexion();

            //se abre la conexión
            conexionBD.Open();

            //si se ejecuta la sentencia correctamente se ejecuta lo siguiente
            try
            {
                //se manda a llamar la sentencia sql y el lector del código
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                reader = comando.ExecuteReader();
                //si se encuentra el código ingresado
                if (reader.HasRows)
                {
                    //lee columna por columna la tabla de nuestra BD
                    while (reader.Read())
                    {
                        txtid.Text = reader.GetString(0);
                        txtnombre.Text = reader.GetString(2);
                        txtdescrip.Text = reader.GetString(3);
                        txtprecio.Text = reader.GetString(4);
                        txtexistencias.Text = reader.GetString(5);
                        txttalla.Text = reader.GetString(6);

                    }
                }
                //Si no se encuentra el código ingresado
                else
                {
                    MessageBox.Show(" No se encontraron registros con ese codigo");
                }
            }
            //si existe un error al buscar
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al buscar" + ex.Message);
            }
            //se finaliza la conexión con la BD
            finally
            {
                conexionBD.Close();
            }
            // Habilitar el botón de actualización después de la búsqueda
            btnactualizar.Enabled = true;
            // Habilitar el botón de eliminación después de la búsqueda
            btneliminar.Enabled = true;
        }

        private void btnactualizar_Click(object sender, EventArgs e)
        {
        
            // Validar que los campos numéricos contengan valores válidos
            if (!int.TryParse(txtcodigo.Text, out int codigo) || !double.TryParse(txtprecio.Text, out double precio) || !int.TryParse(txtexistencias.Text, out int existencia))
            {
                MessageBox.Show("Los campos 'código', 'precio' y 'existencia' deben contener solo valores numéricos.");
                return;
            }
            //capturar los valores del los textbox
            String id = txtid.Text;
            String nombre = txtnombre.Text;
            String descripcion = txtdescrip.Text;
            char talla = char.Parse(txttalla.Text);

            // Verificar si el código ya existe en la base de datos
            string existeSql = "SELECT COUNT(*) FROM productos WHERE codigo = @codigo";
            using (MySqlConnection con1 = Conexion.conexion())
            {
                con1.Open();
                using (MySqlCommand existeCmd = new MySqlCommand(existeSql, con1))
                {
                    existeCmd.Parameters.AddWithValue("@codigo", codigo);
                    int count = Convert.ToInt32(existeCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("El código ya existe en la base de datos, ingrese uno nuevo");
                        return;
                    }
                }
            }

            //sentencia sql para modificar los registros
            string sql = "UPDATE productos SET codigo='" + codigo + "', nombre='" + nombre + "' , descripcion='" + descripcion + "', precio='" + precio + "', existencias='" + existencia + "', talla='" + talla + "' WHERE id= '" + id + "'";

            //se muestra cuadro de confirmación para la operación
            DialogResult result = MessageBox.Show("¿Deseas actualizar este registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //si el usuario confirma la operación se ejecuta lo siguiente
            if (result == DialogResult.Yes) { 
                    //instancia a la clase de conexion
                    MySqlConnection con = Conexion.conexion();
                //se abre la conexión con la BD
                con.Open();

                try
                {
                    //comprobar que se guarda en la BD
                    MySqlCommand command = new MySqlCommand(sql, con);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Registro Modificado");

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error al modificar: " + ex.Message);

                }
                //se cierra la conexión con la BD
                finally
                {
                    con.Close();
                }
            }
            else
            {
                // El usuario canceló la actualización y se muestra mensaje
                MessageBox.Show("Actualización cancelada.");
            }
            limpiar();
            // Volver a deshabilitar el botón después de la actualización
            btnactualizar.Enabled = false;
            // Volver a deshabilitar el botón después de la actualización
            btneliminar.Enabled = false;
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            //capturar los valores del los textbox
            String id = txtid.Text;
            String codigo = txtcodigo.Text;
            String nombre = txtnombre.Text;
            String descripcion = txtdescrip.Text;
            double precio = double.Parse(txtprecio.Text);
            int existencia = int.Parse(txtexistencias.Text);
            char talla = char.Parse(txttalla.Text);

            //sentencia sql para eliminar el registro seleccionado
            string sql = "DELETE FROM productos  WHERE id= '" + id + "'";
    
            //se muestra cuadro de confirmción para realizar la operación
            DialogResult result = MessageBox.Show("¿Deseas eliminar este registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                 //instancia a la clase de conexion
                MySqlConnection con = Conexion.conexion();
                con.Open();

                try
                {
                    //comprobar que se elimina en la BD
                    MySqlCommand command = new MySqlCommand(sql, con);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Registro eliminado");

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error al eliminar " + ex.Message);

                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                // El usuario canceló la actualización
                MessageBox.Show("Eliminación cancelada.");
            }
            limpiar();
            // Volver a deshabilitar el botón después de la actualización
            btneliminar.Enabled = false;
            // Volver a deshabilitar el botón después de la actualización
            btnactualizar.Enabled = false;
        }

        //funcion para limpiar los campos del formulario
        private void limpiar()
        {
            txtcodigo.Clear();
            txtnombre.Clear();
            txtdescrip.Clear();
            txtprecio.Clear();
            txttalla.Clear();
            txtexistencias.Clear();
        }

        
        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            ////mandar a llamar el metodo limpiar
            limpiar();
        }

        

       
    }
}
