using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Deletar
{
    public class Deletar
    {
        //MySqlConexao mySql = MySqlConexao.GetInstance();
        //SQLLiteConexao sqlLite = SQLLiteConexao.GetInstance();
        //MySqlConexao mySql = ClasseConectar.mySql;
        //SQLLiteConexao sqlLite = ClasseConectar.sqlLite;

        private static Deletar _obj;

        public static Deletar Instance
        {

            get
            {
                try
                {
                    if (_obj == null)
                        _obj = new Deletar();
                }
                catch (Exception ex)
                {
                    throw new Exception(Environment.NewLine + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
                }
                return _obj;
            }

        }

        public int DeletarRegistro(object obj, string banco, string campoExtra, bool deixarInformarPrimeiroCampoID = false)
        {
            try
            {
                return LeituraObjetos_SQL.Deletar(obj, banco, campoExtra, deixarInformarPrimeiroCampoID);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro : " + ex.Message + (ex.InnerException != null ? ex.InnerException.ToString() : String.Empty));
            }
        }

    }
}
