using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using FirebirdSql.Data.FirebirdClient;
using Base.Conectar;
using System.Transactions;
using System.Data;

namespace Base
{
    public class LeituraObjetos_SQL
    {
        //static MySqlConectar mySql = ClasseConectar.mySql;
        //static SQLiteConectar sqlLite = ClasseConectar.sqlLite;
        //static FbConectar fb = ClasseConectar.fb;
        public static string GerarSQLSelectDelete(object obj, bool select, bool todosRegistros, string campoExtra, bool like, string campoPesquisa, string orderby)
        {
            string sql = "";
            try
            {
                var prop = obj.GetType().GetProperties();
                string tabela = obj.GetType().GetTypeInfo().Name;
                string campoZero = "";
                int id = 0;
                if (!todosRegistros & !like)
                    foreach (var p in prop)
                    {
                        campoZero = p.Name;
                        id = Convert.ToInt32(p.GetValue(obj));
                        break;
                    }

                if (select)
                {
                    if (todosRegistros)
                    {
                        sql = "select * from " + tabela;
                    }
                    else
                    {
                        if (id > 0)
                        {
                            sql = "select * from " + tabela + " where " + campoZero + " = " + id;
                        }
                        else if (like)
                        {
                            sql = "select * from " + tabela + " where " + campoExtra + " LIKE " + "('%" + campoPesquisa + "%')";
                        }
                        else if (campoExtra.Length > 0)
                        {
                            sql = "select * from " + tabela + " where " + campoExtra;
                        }
                    }

                    if (orderby.Length > 0)
                        sql = sql + " order by " + orderby;
                }
                else
                {
                    sql = "delete from " + tabela + " where " + campoExtra;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro GerarSQLSelectDelete : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

            return sql;
        }

        public static string GerarSQLInsertUpdate(object obj, bool update, string banco, List<string> camposUpdateAdicionais, bool deixarInformarPrimeiroCampoID = false)
        {
            string sql = "";
            try
            {
                List<string> sqlCriar = new List<string>();
                var prop = obj.GetType().GetProperties();

                string tabela = obj.GetType().GetTypeInfo().Name;
                string campoZero = "";
                if (update)
                {                    
                    string linha = "update " + tabela + " set ";
                    sqlCriar.Add(linha);
                    foreach (var p in prop)
                    {
                        if (campoZero.Length == 0)
                            campoZero = p.Name;
                        if (p.PropertyType.FullName.Contains("System") & !p.PropertyType.FullName.Contains("ICollection"))
                        {
                            linha = p.Name + "= @" + p.Name + ", ";
                            sqlCriar.Add(linha);
                        }
                    }
                    sqlCriar[sqlCriar.Count - 1] = sqlCriar[sqlCriar.Count - 1].Replace(",", "");
                    linha = " where " + campoZero + " = @" + campoZero;
                    sqlCriar.Add(linha);
                    if (camposUpdateAdicionais != null)
                    {
                        foreach (var i in camposUpdateAdicionais)
                        {
                            linha = " and " + i + " = @" + i;
                            sqlCriar.Add(linha);
                        }
                    }
                }
                else
                {
                    string linha = "insert into " + tabela + "( ";
                    sqlCriar.Add(linha);
                    for (int i = 0; i < prop.Count(); i++)
                    {
                        if (prop[i].PropertyType.FullName.Contains("System") & !prop[i].PropertyType.FullName.Contains("ICollection"))
                        {
                            if (i == 0 & !deixarInformarPrimeiroCampoID) { }
                            else
                            {
                                if (campoZero.Length == 0 & banco == "F")
                                    campoZero = prop[i].Name;
                                linha = prop[i].Name + ", ";
                                sqlCriar.Add(linha);
                            }
                        }
                    }
                    sqlCriar[sqlCriar.Count - 1] = sqlCriar[sqlCriar.Count - 1].Replace(",", "") + " ) VALUES ( ";
                    for (int i = 0; i < prop.Count(); i++)
                    {
                        if (prop[i].PropertyType.FullName.Contains("System") & !prop[i].PropertyType.FullName.Contains("ICollection"))
                        {
                            if (i == 0 & !deixarInformarPrimeiroCampoID) { }
                            else
                            {
                                linha = "@" + prop[i].Name + ", ";
                                sqlCriar.Add(linha);
                            }
                        }
                    }
                    sqlCriar[sqlCriar.Count - 1] = sqlCriar[sqlCriar.Count - 1].Replace(",", "") + " );";
                }

                foreach (var p in sqlCriar)
                {
                    sql += p;
                }

                if (campoZero.Length > 0 & banco == "F")
                {
                    sql = sql.Replace(";", "");
                    sql += " returning " + campoZero + ";";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro GerarSQLInsertUpdate : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

            return sql;
        }

        public static string GerarSQLInsertDelete(object obj, bool insert, string banco, string campoExtra, bool deixarInformarPrimeiroCampoID = false)
        {
            string sql = "";
            try
            {
                List<string> sqlCriar = new List<string>();
                var prop = obj.GetType().GetProperties();

                string tabela = obj.GetType().GetTypeInfo().Name;
                string campoZero = "";

                if (insert)
                {
                    string linha = "insert into " + tabela + "( ";
                    sqlCriar.Add(linha);
                    for (int i = 0; i < prop.Count(); i++)
                    {
                        if (prop[i].PropertyType.FullName.Contains("System") & !prop[i].PropertyType.FullName.Contains("ICollection"))
                        {
                            if (i == 0 & !deixarInformarPrimeiroCampoID) { }
                            else
                            {
                                linha = prop[i].Name + ", ";
                                sqlCriar.Add(linha);
                            }
                        }
                    }
                    sqlCriar[sqlCriar.Count - 1] = sqlCriar[sqlCriar.Count - 1].Replace(",", "") + " ) VALUES ( ";
                    for (int i = 0; i < prop.Count(); i++)
                    {
                        if (prop[i].PropertyType.FullName.Contains("System") & !prop[i].PropertyType.FullName.Contains("ICollection"))
                        {
                            if (i == 0 & !deixarInformarPrimeiroCampoID) { }
                            else
                            {
                                if (campoZero.Length == 0 & banco == "F")
                                    campoZero = prop[i].Name;
                                linha = "@" + prop[i].Name + ", ";
                                sqlCriar.Add(linha);
                            }
                        }
                    }
                    sqlCriar[sqlCriar.Count - 1] = sqlCriar[sqlCriar.Count - 1].Replace(",", "") + " );";

                    foreach (var p in sqlCriar)
                    {
                        sql += p;
                    }

                    if (campoZero.Length > 0 & banco == "F")
                    {
                        sql = sql.Replace(";", "");
                        sql += " returning " + campoZero + ";";
                    }
                }
                else
                {
                    sql = "delete from " + tabela + " where " + campoExtra;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro GerarSQLInsertDelete : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

            return sql;
        }

        public static DbParameter parametro(string parametro, object valor, string banco)
        {
            DbParameter p = null;
            try
            {
                switch (banco)
                {
                    case "M":
                        p = new MySqlParameter(parametro, valor);
                        break;
                    case "L":
                        p = new SQLiteParameter(parametro, valor);
                        break;
                    case "F":
                        p = new FbParameter(parametro, valor);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro parametro : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return p;
        }

        public static object montagem(DbDataReader rdr, object obj)
        {
            object item = null;
            try
            {
                Type classe = obj.GetType();
                item = Activator.CreateInstance(classe);
                foreach (var p in item.GetType().GetProperties())
                {
                    if (!p.PropertyType.FullName.Contains("ICollection") && p.PropertyType.FullName.Contains("System") && rdr.GetOrdinal(p.Name) >= 0)
                    {
                        if (p.PropertyType == typeof(int) | p.PropertyType == typeof(int?))
                            p.SetValue(item, (string.IsNullOrEmpty((rdr[p.Name]).ToString()) ? 0 : Convert.ToInt32(rdr[p.Name])));
                        else if (p.PropertyType == typeof(string))
                            p.SetValue(item, (string.IsNullOrEmpty((rdr[p.Name]).ToString()) ? "" : Convert.ToString(rdr[p.Name])));
                        else if (p.PropertyType == typeof(decimal) | p.PropertyType == typeof(decimal?))
                            p.SetValue(item, (string.IsNullOrEmpty((rdr[p.Name]).ToString()) ? 0 : Convert.ToDecimal(rdr[p.Name])));
                        else if (p.PropertyType == typeof(byte))
                            p.SetValue(item, ((rdr[p.Name]).Equals(System.DBNull.Value) ? new byte() : Convert.ToByte(rdr[p.Name])));
                        else if (p.PropertyType == typeof(byte[]))
                            p.SetValue(item, ((rdr[p.Name]).Equals(System.DBNull.Value) ? new byte[] { 0 } : ((byte[])rdr[p.Name])));
                        else if (p.PropertyType == typeof(DateTime) | p.PropertyType == typeof(DateTime?))
                            p.SetValue(item, (string.IsNullOrEmpty((rdr[p.Name]).ToString()) ? DateTime.MinValue : Convert.ToDateTime(rdr[p.Name])));
                    }
                    else
                    {
                        if (p.PropertyType == typeof(int) | p.PropertyType == typeof(int?))
                            p.SetValue(item, 0);
                        else if (p.PropertyType == typeof(string))
                            p.SetValue(item, "");
                        else if (p.PropertyType == typeof(decimal) | p.PropertyType == typeof(decimal?))
                            p.SetValue(item, 0);
                        else if (p.PropertyType == typeof(byte))
                            p.SetValue(item, new byte());
                        else if (p.PropertyType == typeof(byte[]))
                            p.SetValue(item, new byte[] { 0 });
                        else if (p.PropertyType == typeof(DateTime) | p.PropertyType == typeof(DateTime?))
                            p.SetValue(item, DateTime.MinValue);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro montagem : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return item;
        }

        public static int Salvar(object obj, bool update, string banco, bool sincronizadoLocal, List<string> camposUpdateAdicionais, bool deixarInformarPrimeiroCampoID = false)
        {
            int idRet = 0;
            try
            {
                string sql = GerarSQLInsertUpdate(obj, update, banco, camposUpdateAdicionais, deixarInformarPrimeiroCampoID);
                DbDataAdapter adp = null;
                DbParameter pFirebird = null;
                switch (banco)
                {
                    case "M":
                        adp = ClasseConectar.mySql.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.mySql.Command(sql);
                        break;
                    case "L":
                        adp = ClasseConectar.sqlLite.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.sqlLite.Command(sql);
                        break;
                    case "F":                        
                        adp = ClasseConectar.fb.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.fb.Command(sql);
                        if (!update)
                        {
                            pFirebird = new FbParameter();
                            pFirebird.ParameterName = "ID";
                            pFirebird.Direction = ParameterDirection.Output;
                            pFirebird.DbType = DbType.Int32;
                            pFirebird.Size = 10;
                            adp.SelectCommand.Parameters.Add(pFirebird);
                        }
                        break;
                }
                
                foreach (var p in obj.GetType().GetProperties())
                {
                    if (p.Name == "DATA_HORA_ALTERACAO")
                        p.SetValue(obj, DateTime.Now);
                    if (p.Name == "SINCRONIZADO")
                    {
                        if (sincronizadoLocal) p.SetValue(obj, "S");
                        else
                        { if (update) p.SetValue(obj, "U"); else p.SetValue(obj, "I"); }
                    }
                    if (p.Name != ("QTD_PDV") && p.Name.Contains("PDV_"))
                    {
                        if (sincronizadoLocal) p.SetValue(obj, "S");
                        else
                        { if (update) p.SetValue(obj, "U"); else p.SetValue(obj, "I"); }
                    }
                    adp.SelectCommand.Parameters.Add(parametro("@" + p.Name, p.GetValue(obj), banco));
                }

                if (update)
                    idRet = adp.SelectCommand.ExecuteNonQuery();
                else
                {
                    switch (banco)
                    {
                        case "M":
                            var cmd = (MySqlCommand)adp.SelectCommand;
                            cmd.ExecuteNonQuery();
                            idRet = Convert.ToInt32(cmd.LastInsertedId);
                            break;
                        case "L":
                            adp.SelectCommand.ExecuteNonQuery();
                            idRet = Convert.ToInt32(ClasseConectar.sqlLite.Connection().LastInsertRowId);
                            adp.SelectCommand.Dispose();
                            break;
                        case "F":
                            var cmdFb = (FbCommand)adp.SelectCommand;
                            cmdFb.ExecuteNonQuery();
                            idRet = Convert.ToInt32(pFirebird.Value);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Salvar : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return idRet;
        }

        public static int Salvar(object obj, bool insert, string banco, string campoExtra,  bool deixarInformarPrimeiroCampoID = false)
        {
            int idRet = 0;
            try
            {
                string sql = GerarSQLInsertDelete(obj, insert, banco, campoExtra, deixarInformarPrimeiroCampoID);
                DbDataAdapter adp = null;
                DbParameter pFirebird = null;
                switch (banco)
                {
                    case "M":
                        adp = ClasseConectar.mySql.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.mySql.Command(sql);
                        break;
                    case "L":
                        adp = ClasseConectar.sqlLite.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.sqlLite.Command(sql);
                        break;
                    case "F":
                        adp = ClasseConectar.fb.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.fb.Command(sql);
                        if (insert)
                        {
                            pFirebird = new FbParameter();
                            pFirebird.ParameterName = "ID";
                            pFirebird.Direction = ParameterDirection.Output;
                            pFirebird.DbType = DbType.Int32;
                            pFirebird.Size = 10;
                            adp.SelectCommand.Parameters.Add(pFirebird);
                        }
                        break;
                }                

                foreach (var p in obj.GetType().GetProperties())
                {
                    if (p.Name == "DATA_HORA_ALTERACAO")
                        p.SetValue(obj, DateTime.Now);
                    if (p.Name == "SINCRONIZADO")
                    {
                        if (!insert) p.SetValue(obj, "U"); else p.SetValue(obj, "I");
                    }
                    if (p.Name != ("QTD_PDV") && p.Name.Contains("PDV_"))
                    {
                        if (!insert) p.SetValue(obj, "U"); else p.SetValue(obj, "I");
                    }
                    adp.SelectCommand.Parameters.Add(parametro("@" + p.Name, p.GetValue(obj), banco));
                }

                if (!insert)
                    idRet = adp.SelectCommand.ExecuteNonQuery();
                else
                {
                    switch (banco)
                    {
                        case "M":
                            var cmd = (MySqlCommand)adp.SelectCommand;
                            cmd.ExecuteNonQuery();
                            idRet = Convert.ToInt32(cmd.LastInsertedId);
                            break;
                        case "L":
                            adp.SelectCommand.ExecuteNonQuery();
                            idRet = Convert.ToInt32(ClasseConectar.sqlLite.Connection().LastInsertRowId);
                            adp.SelectCommand.Dispose();
                            break;
                        case "F":
                            var cmdFb = (FbCommand)adp.SelectCommand;
                            cmdFb.ExecuteNonQuery();
                            idRet = Convert.ToInt32(pFirebird.Value);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Salvar : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return idRet;
        }

        public static int Salvar(object obj, bool insert, string sql, bool mySQL, string banco)
        {
            int idRet = 0;
            try
            {
                DbDataAdapter adp = null;
                switch (banco)
                {
                    case "M":
                        adp = ClasseConectar.mySql.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.mySql.Command(sql);
                        break;
                    case "L":
                        adp = ClasseConectar.sqlLite.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.sqlLite.Command(sql);
                        break;
                    case "F":
                        adp = ClasseConectar.fb.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.fb.Command(sql);
                        break;
                }

                if (!insert)
                    idRet = adp.SelectCommand.ExecuteNonQuery();
                else
                {
                    switch (banco)
                    {
                        case "M":
                            var cmd = (MySqlCommand)adp.SelectCommand;
                            cmd.ExecuteNonQuery();
                            idRet = Convert.ToInt32(cmd.LastInsertedId);
                            break;
                        case "L":
                            adp.SelectCommand.ExecuteNonQuery();
                            idRet = Convert.ToInt32(ClasseConectar.sqlLite.Connection().LastInsertRowId);
                            adp.SelectCommand.Dispose();
                            break;
                        case "F":
                            var cmdFb = (FbCommand)adp.SelectCommand;
                            cmdFb.ExecuteNonQuery();
                           // idRet = Convert.ToInt32(cmdFb.LastInsertedId);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Salvar : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return idRet;
        }

        public static int Deletar(object obj, string banco, string campoExtra, bool deixarInformarPrimeiroCampoID = false)
        {
            int idRet = 0;
            try
            {
                string sql = GerarSQLInsertDelete(obj, false, banco, campoExtra, deixarInformarPrimeiroCampoID);
                DbDataAdapter adp = null;
                switch (banco)
                {
                    case "M":
                        adp = ClasseConectar.mySql.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.mySql.Command(sql);
                        break;
                    case "L":
                        adp = ClasseConectar.sqlLite.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.sqlLite.Command(sql);
                        break;
                    case "F":
                        adp = ClasseConectar.fb.ExecuteAdapter(sql);
                        adp.SelectCommand = ClasseConectar.fb.Command(sql);
                        break;
                }

                foreach (var p in obj.GetType().GetProperties())
                {
                    adp.SelectCommand.Parameters.Add(parametro("@" + p.Name, p.GetValue(obj), banco));
                }

                idRet = adp.SelectCommand.ExecuteNonQuery();
                if (banco == "L")
                {
                    adp.SelectCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Salvar : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return idRet;
        }

        public static DbDataAdapter RetornarAdapter(string sql, string banco)
        {
            DbDataAdapter adp = null;
            try
            {
                switch (banco)
                {
                    case "M":
                        adp = ClasseConectar.mySql.ExecuteAdapter(sql);
                        break;
                    case "L":
                        adp = ClasseConectar.sqlLite.ExecuteAdapter(sql);
                        break;
                    case "F":
                        adp = ClasseConectar.fb.ExecuteAdapter(sql);
                        break;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro RetornarAdapter : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return adp;
        }

        public static int SalvarTransactionScope(Dictionary<object, bool> obj, string banco, List<string> camposAdicionais)
        {
            int idRet = 0;
            DbDataAdapter adp = null;
            List<DbCommand> command = new List<DbCommand>();
            DbConnection connect = null;
            switch (banco)
            {
                case "M":
                    connect = ClasseConectar.mySql.Connection();
                    break;
                case "L":
                    connect = ClasseConectar.sqlLite.Connection();
                    break;
                case "F":
                    connect = ClasseConectar.fb.Connection();
                    break;
            }

            try
            {
                using (var txscope = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (connect.State == ConnectionState.Closed)
                        connect.Open();
                    try
                    {

                        foreach (var item in obj)
                        {
                            string sql = GerarSQLInsertUpdate(item.Key, item.Value, banco, camposAdicionais);
                            camposAdicionais = null;
                            switch (banco)
                            {
                                case "M":
                                    var cmd = connect.CreateCommand();
                                    cmd.CommandText = sql;
                                    adp = new MySqlDataAdapter();
                                    adp.SelectCommand = cmd;
                                    break;
                                case "L":
                                    cmd = connect.CreateCommand();
                                    cmd.CommandText = sql;
                                    adp = new SQLiteDataAdapter();
                                    adp.SelectCommand = cmd;
                                    break;
                                case "F":
                                    cmd = connect.CreateCommand();
                                    cmd.CommandText = sql;
                                    adp = new FbDataAdapter();
                                    adp.SelectCommand = cmd;
                                    break;
                            }

                            foreach (var p in item.Key.GetType().GetProperties())
                            {
                                if (p.Name == "DATA_HORA_ALTERACAO")
                                    p.SetValue(item.Key, DateTime.Now);
                                if (p.Name == "SINCRONIZADO")
                                {
                                    if (item.Value)
                                        p.SetValue(item.Key, "U");
                                    else
                                        p.SetValue(item.Key, "I");
                                }
                                if (p.Name.Contains("PDV_"))
                                {
                                    if (item.Value)
                                        p.SetValue(item.Key, "U");
                                    else
                                        p.SetValue(item.Key, "I");
                                }
                                adp.SelectCommand.Parameters.Add(parametro("@" + p.Name, p.GetValue(item.Key), banco));
                            }
                            command.Add(adp.SelectCommand);
                        }
                        bool retornarIDTabelaPrincipal = false;
                        foreach (var item in command)
                        {
                            switch (banco)
                            {
                                case "M":
                                    var cmd = (MySqlCommand)item;
                                    cmd.ExecuteNonQuery();
                                    if (!retornarIDTabelaPrincipal)
                                    {
                                        retornarIDTabelaPrincipal = true;
                                        idRet = Convert.ToInt32(cmd.LastInsertedId);
                                    }
                                    break;
                                case "L":
                                    var connLocal = (SQLiteConnection)connect;
                                    var cmdL = (SQLiteCommand)item;
                                    item.ExecuteNonQuery();
                                    if (!retornarIDTabelaPrincipal)
                                    {
                                        retornarIDTabelaPrincipal = true;
                                        idRet = Convert.ToInt32(connLocal.LastInsertRowId);
                                    }
                                    item.Dispose();
                                    cmdL.Dispose();
                                    break;
                                case "F":
                                    var cmdF = (FbCommand)item;
                                    cmdF.ExecuteNonQuery();
                                    if (!retornarIDTabelaPrincipal)
                                    {
                                        retornarIDTabelaPrincipal = true;
                                        //idRet = Convert.ToInt32(cmdF.LastInsertedId);
                                    }
                                    break;
                            }
                        }
                        txscope.Complete();
                        txscope.Dispose();
                        if (banco == "L")
                            connect.Close();
                    }
                    catch (Exception ex)
                    {
                        // Log error    
                        txscope.Dispose();
                        throw new Exception(ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Salvar : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

            return idRet;
        }

        public static int SalvarTransaction(Dictionary<object, bool> obj, string banco, List<string> camposAdicionais)
        {
            int idRet = 0;
            DbDataAdapter adp = null;
            List<DbCommand> command = new List<DbCommand>();
            DbConnection connect = null;
            switch (banco)
            {
                case "M":
                    connect = ClasseConectar.mySql.Connection();
                    break;
                case "L":
                    connect = ClasseConectar.sqlLite.Connection();
                    break;
                case "F":
                    connect = ClasseConectar.fb.Connection();
                    break;
            }

            try
            {
                connect.Open();
                using (var tran = connect.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in obj)
                        {
                            string sql = GerarSQLInsertUpdate(item.Key, item.Value, banco, camposAdicionais);
                            camposAdicionais = null;
                            switch (banco)
                            {
                                case "M":
                                    var cmd = connect.CreateCommand();
                                    cmd.CommandText = sql;
                                    adp = new MySqlDataAdapter();
                                    adp.SelectCommand = cmd;
                                    break;
                                case "L":
                                    cmd = connect.CreateCommand();
                                    cmd.CommandText = sql;
                                    adp = new SQLiteDataAdapter();
                                    adp.SelectCommand = cmd;
                                    break;
                                case "F":
                                    cmd = connect.CreateCommand();
                                    cmd.CommandText = sql;
                                    adp = new FbDataAdapter();
                                    adp.SelectCommand = cmd;
                                    break;
                            }

                            foreach (var p in item.Key.GetType().GetProperties())
                            {
                                if (p.Name == "DATA_HORA_ALTERACAO")
                                    p.SetValue(item.Key, DateTime.Now);
                                if (p.Name == "SINCRONIZADO")
                                {
                                    if (item.Value)
                                        p.SetValue(item.Key, "U");
                                    else
                                        p.SetValue(item.Key, "I");
                                }
                                if (p.Name.Contains("PDV_"))
                                {
                                    if (item.Value)
                                        p.SetValue(item.Key, "U");
                                    else
                                        p.SetValue(item.Key, "I");
                                }
                                adp.SelectCommand.Parameters.Add(parametro("@" + p.Name, p.GetValue(item.Key), banco));
                            }
                            command.Add(adp.SelectCommand);
                        }
                        foreach (var item in command)
                        {
                            idRet = item.ExecuteNonQuery();
                            if (banco == "L")
                            {
                                item.Dispose();
                            }
                        }
                        tran.Commit();
                        connect.Close();
                    }
                    catch (Exception ex)
                    {
                        // Log error    
                        tran.Rollback();
                        throw new Exception(ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Salvar : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }

            return idRet;
        }        
    }
}
