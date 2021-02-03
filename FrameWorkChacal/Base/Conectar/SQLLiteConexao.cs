using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace Base.Conectar
{

    public class SQLiteConectar
    {
        static SQLiteConectar conSQLite;
        static string nomeBanco = ConfigurationManager.ConnectionStrings["SQLiteConnection"].ToString();
        public static string pastaBanco;
        static string caminhoconexao = "";
        SQLiteConnection conn;

        private SQLiteConectar()
        {
            try
            {
                caminhoconexao = "Data Source= " + pastaBanco + @"\" + nomeBanco + ";Version=3";
                conn = new SQLiteConnection(caminhoconexao);
            }
            catch //(SQLiteException ex)
            {
                //throw new Exception("Erro no acesso ao SQLite :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

        }

        public static SQLiteConectar GetInstance()
        {
            if (SQLiteConectar.conSQLite == null)
            {
                SQLiteConectar.conSQLite = new SQLiteConectar();
            }
            return SQLiteConectar.conSQLite;
        }

        public static SQLiteConnection ConnectionNew()
        {
            try
            {               
                return new SQLiteConnection(caminhoconexao);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Erro no acesso ao SQLite :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }

        public SQLiteConnection Connection()
        {
            return conn;
        }        

        public string Caminho()
        {
            return caminhoconexao;
        }

        public SQLiteCommand Command(string sqltexto = "")
        {
            SQLiteCommand cmd = conn.CreateCommand();
            if (sqltexto != "")
                cmd.CommandText = sqltexto;
            return cmd;
        }

        public int Execute(string sqltexto = "")
        {
            SQLiteCommand cmd = Command(sqltexto);
            try
            {
                Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
            return 0;
        }
        public int Execute()
        {
            SQLiteCommand cmd = Command();
            try
            {
                Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
            return 0;
        }
        public SQLiteDataReader ExecuteReader(string sqltexto = "")
        {
            SQLiteCommand cmd = Command(sqltexto);
            SQLiteDataReader datareader;
            try
            {
                Open();
                datareader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
            return datareader;
        }
        public SQLiteDataAdapter ExecuteAdapter(string sqltexto = "")
        {
            SQLiteDataAdapter data = new SQLiteDataAdapter();
            data.SelectCommand = Command(sqltexto);
            try
            {
                Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
            return data;
        }

        public void RestoreDB(string filePath, string filePathDest, string srcFilename, string destFileName, bool IsCopy = false)
        {
            var srcfile = Path.Combine(filePath, srcFilename);
            var destfile = Path.Combine(filePath, destFileName);

            if (File.Exists(destfile)) File.Delete(destfile);

            if (IsCopy)
                BackupDB(filePath, filePathDest, srcFilename, destFileName);
            else
                File.Move(srcfile, destfile);
        }

        public void BackupDB(string filePath, string filePathDest, string srcFilename, string destFileName)
        {
            var srcfile = Path.Combine(filePath, srcFilename);
            var destfile = Path.Combine(filePathDest, destFileName);

            if (File.Exists(destfile)) File.Delete(destfile);

            File.Copy(srcfile, destfile);
        }       

        public void Open()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                //if (conn.State == ConnectionState.Open)
                //{
                //    conn.Dispose();
                //    conn.Open();
                //}
            }
            catch (SQLiteException ex)
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
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
