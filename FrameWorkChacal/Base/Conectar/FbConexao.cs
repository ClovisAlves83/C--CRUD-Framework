using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.IO;

namespace Base.Conectar
{   
        public class FbConectar
        {
            static FbConectar Fb;
        static string caminhoconexao = ConfigurationManager.ConnectionStrings["FbConnection"].ToString();
        public static string pastaBanco;
        FbConnection conn;

        private FbConectar()
        {
            try
            {
                string[] ModelsArray = caminhoconexao.Split(';');
                ModelsArray[3] = "Database=" + pastaBanco + @"\" + ModelsArray[3].Replace("Database=", "");
                foreach (var i in ModelsArray)
                {
                    caminhoconexao += i.TrimStart().Trim() + ";";
                }
                conn = new FbConnection(caminhoconexao);
            }
            catch (FbException ex)
            {
                throw new Exception("Erro no acesso ao Fb :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }

            public static FbConectar GetInstance()
            {
                if (FbConectar.Fb == null)
                {
                    FbConectar.Fb = new FbConectar();
                }
                return FbConectar.Fb;
            }


            public static FbConnection ConnectionNew()
            {
                caminhoconexao = caminhoconexao + ";charset = utf8;convertzerodatetime=true;";
                try
                {
                    return new FbConnection(caminhoconexao);
                }
                catch (FbException ex)
                {
                    throw new Exception("Erro no acesso ao Fb :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
            }

            public FbConnection Connection()
            {
                return conn;
            }

            public FbCommand Command(string sqltexto = "")
            {
                FbCommand cmd = conn.CreateCommand();
                try
                {
                    if (sqltexto != "")
                        cmd.CommandText = sqltexto;
                }
                catch (FbException ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                return cmd;
            }

            public FbCommand CommandTransaction(string sqltexto = "")
            {
                FbCommand cmd = conn.CreateCommand();
                FbTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;
                try
                {
                    if (sqltexto != "")
                        cmd.CommandText = sqltexto;
                }
                catch (FbException ex)
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
                    FbCommand cmd = Command(sqltexto);
                    Open();
                    cmd.ExecuteNonQuery();
                }
                catch (FbException ex)
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
                    FbCommand cmd = Command();
                    Open();
                    cmd.ExecuteNonQuery();
                }
                catch (FbException ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                return 0;
            }
            public FbDataReader ExecuteReader(string sqltexto = "")
            {
                FbDataReader datareader;
                try
                {
                    FbCommand cmd = Command(sqltexto);
                    Open();
                    datareader = cmd.ExecuteReader();
                }
                catch (FbException ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                return datareader;
            }
            public FbDataAdapter ExecuteAdapter(string sqltexto = "")
            {
                FbDataAdapter data = new FbDataAdapter();

                try
                {
                    data.SelectCommand = Command(sqltexto);
                    Open();
                }
                catch (FbException ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                return data;
            }

            //public void Backup(string caminhoArquivo, bool resetAutoincremento)
            //{
            //    FbCommand cmd = Command();

            //    try
            //    {
            //        FbBackup mb = new FbBackup(cmd);
            //        Open();
            //        mb.ExportInfo.AddCreateDatabase = true;
            //        mb.ExportInfo.ExportTableStructure = true;
            //        mb.ExportInfo.ExportEvents = true;
            //        mb.ExportInfo.ExportFunctions = true;
            //        mb.ExportInfo.ExportProcedures = true;
            //        mb.ExportInfo.ExportTriggers = true;
            //        if (resetAutoincremento)
            //            mb.ExportInfo.ResetAutoIncrement = true;
            //        mb.ExportToFile(caminhoArquivo);
            //        conn.Close();
            //    }
            //    catch (FbException ex)
            //    {
            //        throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            //    }
            //}

            //public void Restore(string caminhoArquivo)
            //{
            //    FbCommand cmd = Command();

            //    try
            //    {
            //        FbBackup mb = new FbBackup(cmd);
            //        Open();
            //        mb.ImportInfo.IgnoreSqlError = true;
            //        mb.ImportFromFile(caminhoArquivo);
            //        conn.Close();
            //    }
            //    catch (FbException ex)
            //    {
            //        throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("Erro :" + Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            //    }
            //}

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
                catch (FbException ex)
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
