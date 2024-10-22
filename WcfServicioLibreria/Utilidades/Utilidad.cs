using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Utilidades
{
    public static class Utilidad
    {
        public static byte[] StreamABytes(Stream stream)
        {
            using (MemoryStream memoriaStream = new MemoryStream())
            {
                if (stream == null)
                {
                    return null;
                }
                stream.CopyTo(memoriaStream);
                return memoriaStream.ToArray();
            }
        }
    }
}
