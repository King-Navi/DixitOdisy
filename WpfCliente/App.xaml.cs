using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCliente
{
    public partial class App : Application
    {
        App()
        {
            IdiomaGuardo.CargarIdiomaGuardado();

        }
    }
}
