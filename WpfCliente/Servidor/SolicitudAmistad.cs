﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfCliente.Utilidad;

namespace WpfCliente.ServidorDescribelo
{
    public partial class SolicitudAmistad
    {
        private BitmapImage bitmapImagen;
        public BitmapImage BitmapImagen
        {
            get
            {
                try 
                {
                    bitmapImagen= this.Remitente.BitmapImagen;
                    return bitmapImagen;
                }
                catch (FileFormatException excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                catch (NotSupportedException excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                return Imagen.CargarImagenDesdeRecursos();
            }
            set
            {
                bitmapImagen = value;
            }
        }
    }
}