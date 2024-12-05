using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    public class JugadorTablaPuntaje : INotifyPropertyChanged
    {
        private string nombre;
        private int puntos;
        private bool mostrarBoton;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                OnPropertyChanged();
            }
        }

        public int Puntos
        {
            get => puntos;
            set
            {
                puntos = value;
                OnPropertyChanged();
            }
        }

        public bool MostrarBoton
        {
            get => mostrarBoton;
            set
            {
                mostrarBoton = value;
                OnPropertyChanged();
            }
        }

    }
}
