using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Base.Conectar
{

    public class MySqlConectar
    {
        static MySqlConectar mySql;
        static string caminhoconexao = ConfigurationManager.ConnectionStrings["MySqlConnection"].ToString();
        
        MySqlConnection conn;

        private MySqlConectar()
        {
            caminhoconexao = caminhoconexao + ";charset = utf8;convertzerodatetime=true;";
            try
            {
                conn = new MySqlConnection(caminhoconexao);
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro no acesso ao MySql :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

        }

        public static MySqlConectar GetInstance()
        {
            if (MySqlConectar.mySql == null)
            {
                MySqlConectar.mySql = new MySqlConectar();
            }
            return MySqlConectar.mySql;
        }
               

        public static MySqlConnection ConnectionNew()
        {
            caminhoconexao = caminhoconexao + ";charset = utf8;convertzerodatetime=true;";
            try
            {
                return new MySqlConnection(caminhoconexao);
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro no acesso ao MySql :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }

        public MySqlConnection Connection()
        {
            return conn;
        }

        public MySqlCommand Command(string sqltexto = "")
        {
            MySqlCommand cmd = conn.CreateCommand();
            try
            {
                if (sqltexto != "")
                    cmd.CommandText = sqltexto;
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return cmd;
        }

        public MySqlCommand CommandTransaction(string sqltexto = "")
        {
            MySqlCommand cmd = conn.CreateCommand();
            MySqlTransaction transaction = conn.BeginTransaction();
            cmd.Transaction = transaction; 
            try
            {
                if (sqltexto != "")
                    cmd.CommandText = sqltexto;
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return cmd;
        }

        public int Execute(string sqltexto = "")
        {            
            try
            {
                MySqlCommand cmd = Command(sqltexto);
                Open();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return 0;
        }
        public int Execute()
        {           
            try
            {
                MySqlCommand cmd = Command();
                Open();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return 0;
        }
        public MySqlDataReader ExecuteReader(string sqltexto = "")
        {
            MySqlDataReader datareader;
            try
            {
                MySqlCommand cmd = Command(sqltexto);               
                Open();
                datareader = cmd.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return datareader;
        }
        public MySqlDataAdapter ExecuteAdapter(string sqltexto = "")
        {
            MySqlDataAdapter data = new MySqlDataAdapter();
           
            try
            {
                data.SelectCommand = Command(sqltexto);
                Open();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return data;
        }

        public void Backup(string caminhoArquivo, bool resetAutoincremento)
        {
            MySqlCommand cmd = Command();
           
            try
            {
                MySqlBackup mb = new MySqlBackup(cmd);
                Open();
                mb.ExportInfo.AddCreateDatabase = true;
                mb.ExportInfo.ExportTableStructure = true;
                mb.ExportInfo.ExportEvents = true;
                mb.ExportInfo.ExportFunctions = true;
                mb.ExportInfo.ExportProcedures = true;
                mb.ExportInfo.ExportTriggers = true;
                if (resetAutoincremento)
                    mb.ExportInfo.ResetAutoIncrement = true;
                mb.ExportToFile(caminhoArquivo);
                conn.Close();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }

        public void Restore(string caminhoArquivo)
        {
            MySqlCommand cmd = Command();
           
            try
            {
                MySqlBackup mb = new MySqlBackup(cmd);
                Open();
                mb.ImportInfo.IgnoreSqlError = true;
                mb.ImportFromFile(caminhoArquivo);
                conn.Close();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }
        
        public void Open()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Dispose();
                    conn.Open();
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }
        public void Close()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
    }
}
