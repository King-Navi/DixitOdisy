using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente
{
    public sealed class Sigleton
    {
        private static Sigleton _instance;
        public static Sigleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Sigleton();
                }
                return _instance;
            }
        }
    }
}
