using System;
using System.Data;
using System.Data.SqlClient;
using LibParametros;

namespace LibConexionBD
{
    public class ConexionBD
    {
        #region "Atributos"
        private string archivoParametros;
        private string cadenaConexion;
        private object valorUnico;
        private string sql;
        private string error;
        private bool hayConexion;
        private SqlConnection cnn;
        private SqlCommand cmd;
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private DataSet ds;
        #endregion

        #region "Propiedades"
        public string SQL { set { sql = value; } }
        public object ValorUnico { get { return valorUnico; } }
        public SqlDataReader Dr { get { return dr; } }
        public SqlConnection Cnn { get { return cnn; } }
        public DataSet Ds { get { return ds; } }
        public SqlDataAdapter Da { get { return da; } }
        public string Error { get { return error; } }
        #endregion

        #region "Constructor"
        public ConexionBD(string archivoParametros)
        {
            this.archivoParametros = archivoParametros;
            hayConexion = false;
            cnn = new SqlConnection();
            cmd = new SqlCommand();
            da = new SqlDataAdapter();
            ds = new DataSet();
        }
        #endregion


        #region "Métodos Privados"
        private bool GenerarCadenaConexion()
        {
            try
            {
                Parametros parametros = new Parametros();
                if (!parametros.GenerarCadenaConexion(archivoParametros))
                {
                    error = parametros.Error;
                    parametros = null;
                    return false;
                }

                cadenaConexion = parametros.CadenaConexion;
                parametros = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region "Metodos Públicos"
        public bool AbrirConexion()
        {
            if (!GenerarCadenaConexion())
            {
                return false;
            }

            cnn.ConnectionString = cadenaConexion;

            try
            {
                cnn.Open();
                hayConexion = true;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                hayConexion = false;
                return false;
            }
        }

        public void CerrarConexion()
        {
            try
            {
                cnn.Close();
                hayConexion = false;
            }
            catch (Exception ex)
            {
                error = "No se cerró o liberó la conexión, " + ex.Message;
            }
        }

        public bool Consultar(bool blnParametros)
        {
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    error = "No definió la instruccion SQL";
                    return false;
                }

                if (!hayConexion)
                {
                    if (!AbrirConexion())
                    {
                        return false;
                    }
                }

                //Preparar el Comando para ejecutar la transacción SQL en la BD
                cmd.Connection = cnn;
                cmd.CommandText = sql;

                if (blnParametros)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                }

                //Realizar la transacción en la BD
                dr = cmd.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }


        public bool ConsultarValorUnico(bool blnParametros)
        {
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    error = "No definió la instruccion SQL";
                    return false;
                }

                if (!hayConexion)
                {
                    if (!AbrirConexion())
                    {
                        return false;
                    }
                }

                //Preparar el Comando para ejecutar la transacción SQL en la BD
                cmd.Connection = cnn;
                cmd.CommandText = sql;

                if (blnParametros)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                }

                //Realizar la transacción en la BD
                valorUnico = cmd.ExecuteScalar();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool EjecutarSentencia(bool blnParametros)
        {
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    error = "No definió la instruccion SQL";
                    return false;
                }

                if (!hayConexion)
                {
                    if (!AbrirConexion())
                    {
                        return false;
                    }
                }

                //Preparar el Comando para ejecutar la transacción SQL en la BD
                cmd.Connection = cnn;
                cmd.CommandText = sql;

                if (blnParametros)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                }

                //Realizar la transacción en la BD
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }


        public bool LLenarDataSet(bool blnParametros)
        {
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    sql = "No definió la instrucción SQL";
                    return false;
                }

                if (!hayConexion)
                {
                    if (!AbrirConexion())
                    {
                        return false;
                    }
                }

                //Preparar el Comando para ejecutar la transacción SQL en la BD
                cmd.Connection = cnn;
                cmd.CommandText = sql;

                if (blnParametros)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                }

                //Preparar el DataAdapter para el uso del comando en la BD.
                da.SelectCommand = cmd;

                //Realizar la transacción en la BD y el llenadodel DataSet/DataTable
                ds.Clear();
                da.Fill(ds);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
