using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Classes
{
    public partial class Aluno
    {
        public int ID_ALUNO { get; set; }
        public Nullable<System.DateTime> DATA_ANIVERSARIO { get; set; }
        public string NOME { get; set; }
        public string CPF { get; set; }
    }
}
