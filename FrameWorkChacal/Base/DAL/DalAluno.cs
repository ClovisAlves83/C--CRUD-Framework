using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Base.Classes;
using Base.Conectar;

namespace Base.DAL
{
    public class DalAluno
    {
        static DalAluno _obj;

        public static DalAluno Instance
        {
            get
            {
                if (_obj == null)
                {
                    _obj = new DalAluno();
                }
                return _obj;
            }
        }

        public List<Aluno> Pesquisar(int id, string banco, bool todosRegistros, string campoExtra, bool like, string campoPesquisa, string orderby)
        {
            string sql = "";
            List<Aluno> ret = new List<Aluno>();
            try
            {
                Aluno obj = new Aluno();
                obj.ID_ALUNO = id;

                sql = LeituraObjetos_SQL.GerarSQLSelectDelete(obj, true, todosRegistros, campoExtra, like, campoPesquisa, orderby);

                DbDataReader rdr = null;
                switch(banco)
                {
                    case "M":
                        rdr = ClasseConectar.mySql.ExecuteReader(sql);
                        break;
                    case "L":
                        rdr = ClasseConectar.sqlLite.ExecuteReader(sql);
                        break;
                    case "F":
                        rdr = ClasseConectar.fb.ExecuteReader(sql);
                        break;
                }
                
                while (rdr.Read())
                {
                    obj = (Aluno)LeituraObjetos_SQL.montagem(rdr, obj);
                    ret.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return ret;
        }

        public List<Aluno> PesquisarSQLIVRE(string banco, string sql)
        {
            List<Aluno> ret = new List<Aluno>();
            try
            {
                Aluno obj = new Aluno();

                DbDataReader rdr = null;
                switch (banco)
                {
                    case "M":
                        rdr = ClasseConectar.mySql.ExecuteReader(sql);
                        break;
                    case "L":
                        rdr = ClasseConectar.sqlLite.ExecuteReader(sql);
                        break;
                    case "F":
                        rdr = ClasseConectar.fb.ExecuteReader(sql);
                        break;
                }

                while (rdr.Read())
                {
                    obj = (Aluno)LeituraObjetos_SQL.montagem(rdr, obj);
                    ret.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
            return ret;
        }

        public int Salvar(Aluno obj, bool update, string banco, bool sincronizadoLocal, List<string> camposAdicionais, bool deixarInformarPrimeiroCampoID = false)
        {
            try
            {
                return LeituraObjetos_SQL.Salvar(obj, update, banco, sincronizadoLocal, camposAdicionais, deixarInformarPrimeiroCampoID);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }
    }
}
