using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://refactoring.guru/es/design-patterns/mediator/csharp/example</ref>
    public class VentanaMediador
    {
        private MenuWindow menu;
        private SalaEsperaWindow sala;
        private PartidaWindow partida;

        public VentanaMediador(MenuWindow menu)
        {
            this.menu = menu;
        }

        public void AbrirSala(string idSala) //FIXME
        {
            //menu.Hide();
            //sala = new SalaEsperaWindow( idSala,this);
            //sala.Closed += (s, args) =>
            //{
            //    if (!Conexion.CerrarConexionesSalaConChat())
            //    {
            //        VentanasEmergentes.CrearVentanaEmergenteErrorServidor(menu);
            //        menu.Close();
            //    }
            //    if (sala.DebeAbrirPartida && String.IsNullOrWhiteSpace(Singleton.Instance.IdPartida) )
            //    {
            //        AbrirPartida(Singleton.Instance.IdPartida);
            //    }
            //    else
            //    {
            //        menu.Show();
            //    }
            //};
            //sala.Show();
        }

        public void AbrirPartida(string idPartida)
        {
            partida = new PartidaWindow(idPartida);
            partida.Closed += (s, args) => menu.Show();
            partida.Show();
        }
    }
}
