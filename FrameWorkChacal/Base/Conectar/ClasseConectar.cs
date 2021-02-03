using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace Base.Conectar
{
    public static class ClasseConectar
    {        
        public static MySqlConectar mySql = null;
        public static SQLiteConectar sqlLite = null;
        public static FbConectar fb = null;

        public static void Conectar(string bancoDados)
        {
            switch(bancoDados)
            {
                case "M":
                    mySql = MySqlConectar.GetInstance();
                    break;
                case "L":
                    sqlLite = SQLiteConectar.GetInstance();
                    break;
                case "F":
                    fb = FbConectar.GetInstance();
                    break;
            }
        }        
    }
}
