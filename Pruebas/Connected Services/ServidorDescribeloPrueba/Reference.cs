﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pruebas.ServidorDescribeloPrueba {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioAmistad", CallbackContract=typeof(Pruebas.ServidorDescribeloPrueba.IServicioAmistadCallback))]
    public interface IServicioAmistad {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioAmistad/AbrirCanalParaPeticiones")]
        void AbrirCanalParaPeticiones(WpfCliente.ServidorDescribelo.Usuario usuario);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioAmistad/AbrirCanalParaPeticiones")]
        System.Threading.Tasks.Task AbrirCanalParaPeticionesAsync(WpfCliente.ServidorDescribelo.Usuario usuario);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioAmistadCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAmistad/ObtenerListaAmigoCallback", ReplyAction="http://tempuri.org/IServicioAmistad/ObtenerListaAmigoCallbackResponse")]
        void ObtenerListaAmigoCallback(WpfCliente.ServidorDescribelo.Amigo[] amigos);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAmistad/ObtenerPeticionAmistadCallback", ReplyAction="http://tempuri.org/IServicioAmistad/ObtenerPeticionAmistadCallbackResponse")]
        void ObtenerPeticionAmistadCallback(WpfCliente.ServidorDescribelo.SolicitudAmistad nuevaSolicitudAmistad);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioAmistadChannel : Pruebas.ServidorDescribeloPrueba.IServicioAmistad, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioAmistadClient : System.ServiceModel.DuplexClientBase<Pruebas.ServidorDescribeloPrueba.IServicioAmistad>, Pruebas.ServidorDescribeloPrueba.IServicioAmistad {
        
        public ServicioAmistadClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServicioAmistadClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServicioAmistadClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioAmistadClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioAmistadClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void AbrirCanalParaPeticiones(WpfCliente.ServidorDescribelo.Usuario usuario) {
            base.Channel.AbrirCanalParaPeticiones(usuario);
        }
        
        public System.Threading.Tasks.Task AbrirCanalParaPeticionesAsync(WpfCliente.ServidorDescribelo.Usuario usuario) {
            return base.Channel.AbrirCanalParaPeticionesAsync(usuario);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioChat")]
    public interface IServicioChat {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChat/CrearChat", ReplyAction="http://tempuri.org/IServicioChat/CrearChatResponse")]
        bool CrearChat(string idChat);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChat/CrearChat", ReplyAction="http://tempuri.org/IServicioChat/CrearChatResponse")]
        System.Threading.Tasks.Task<bool> CrearChatAsync(string idChat);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioChatChannel : Pruebas.ServidorDescribeloPrueba.IServicioChat, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioChatClient : System.ServiceModel.ClientBase<Pruebas.ServidorDescribeloPrueba.IServicioChat>, Pruebas.ServidorDescribeloPrueba.IServicioChat {
        
        public ServicioChatClient() {
        }
        
        public ServicioChatClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioChatClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioChatClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioChatClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool CrearChat(string idChat) {
            return base.Channel.CrearChat(idChat);
        }
        
        public System.Threading.Tasks.Task<bool> CrearChatAsync(string idChat) {
            return base.Channel.CrearChatAsync(idChat);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioChatMotor", CallbackContract=typeof(Pruebas.ServidorDescribeloPrueba.IServicioChatMotorCallback))]
    public interface IServicioChatMotor {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChatMotor/AgregarUsuarioChat", ReplyAction="http://tempuri.org/IServicioChatMotor/AgregarUsuarioChatResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WpfCliente.ServidorDescribelo.UsuarioDuplicadoFalla), Action="http://tempuri.org/IServicioChatMotor/AgregarUsuarioChatUsuarioDuplicadoFallaFaul" +
            "t", Name="UsuarioDuplicadoFalla", Namespace="http://schemas.datacontract.org/2004/07/WcfServicioLibreria.Modelo")]
        bool AgregarUsuarioChat(string idChat, string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChatMotor/AgregarUsuarioChat", ReplyAction="http://tempuri.org/IServicioChatMotor/AgregarUsuarioChatResponse")]
        System.Threading.Tasks.Task<bool> AgregarUsuarioChatAsync(string idChat, string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChatMotor/DesconectarUsuarioChat", ReplyAction="http://tempuri.org/IServicioChatMotor/DesconectarUsuarioChatResponse")]
        bool DesconectarUsuarioChat(string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChatMotor/DesconectarUsuarioChat", ReplyAction="http://tempuri.org/IServicioChatMotor/DesconectarUsuarioChatResponse")]
        System.Threading.Tasks.Task<bool> DesconectarUsuarioChatAsync(string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChatMotor/EnviarMensaje", ReplyAction="http://tempuri.org/IServicioChatMotor/EnviarMensajeResponse")]
        void EnviarMensaje(string idChat, WpfCliente.ServidorDescribelo.ChatMensaje mensaje);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioChatMotor/EnviarMensaje", ReplyAction="http://tempuri.org/IServicioChatMotor/EnviarMensajeResponse")]
        System.Threading.Tasks.Task EnviarMensajeAsync(string idChat, WpfCliente.ServidorDescribelo.ChatMensaje mensaje);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioChatMotorCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioChatMotor/RecibirMensajeCliente")]
        void RecibirMensajeCliente(WpfCliente.ServidorDescribelo.ChatMensaje mensaje);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioChatMotorChannel : Pruebas.ServidorDescribeloPrueba.IServicioChatMotor, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioChatMotorClient : System.ServiceModel.DuplexClientBase<Pruebas.ServidorDescribeloPrueba.IServicioChatMotor>, Pruebas.ServidorDescribeloPrueba.IServicioChatMotor {
        
        public ServicioChatMotorClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServicioChatMotorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServicioChatMotorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioChatMotorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioChatMotorClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool AgregarUsuarioChat(string idChat, string nombreUsuario) {
            return base.Channel.AgregarUsuarioChat(idChat, nombreUsuario);
        }
        
        public System.Threading.Tasks.Task<bool> AgregarUsuarioChatAsync(string idChat, string nombreUsuario) {
            return base.Channel.AgregarUsuarioChatAsync(idChat, nombreUsuario);
        }
        
        public bool DesconectarUsuarioChat(string nombreUsuario) {
            return base.Channel.DesconectarUsuarioChat(nombreUsuario);
        }
        
        public System.Threading.Tasks.Task<bool> DesconectarUsuarioChatAsync(string nombreUsuario) {
            return base.Channel.DesconectarUsuarioChatAsync(nombreUsuario);
        }
        
        public void EnviarMensaje(string idChat, WpfCliente.ServidorDescribelo.ChatMensaje mensaje) {
            base.Channel.EnviarMensaje(idChat, mensaje);
        }
        
        public System.Threading.Tasks.Task EnviarMensajeAsync(string idChat, WpfCliente.ServidorDescribelo.ChatMensaje mensaje) {
            return base.Channel.EnviarMensajeAsync(idChat, mensaje);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioRegistro")]
    public interface IServicioRegistro {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioRegistro/RegistrarUsuario", ReplyAction="http://tempuri.org/IServicioRegistro/RegistrarUsuarioResponse")]
        bool RegistrarUsuario(WpfCliente.ServidorDescribelo.Usuario usuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioRegistro/RegistrarUsuario", ReplyAction="http://tempuri.org/IServicioRegistro/RegistrarUsuarioResponse")]
        System.Threading.Tasks.Task<bool> RegistrarUsuarioAsync(WpfCliente.ServidorDescribelo.Usuario usuario);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioRegistroChannel : Pruebas.ServidorDescribeloPrueba.IServicioRegistro, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioRegistroClient : System.ServiceModel.ClientBase<Pruebas.ServidorDescribeloPrueba.IServicioRegistro>, Pruebas.ServidorDescribeloPrueba.IServicioRegistro {
        
        public ServicioRegistroClient() {
        }
        
        public ServicioRegistroClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioRegistroClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioRegistroClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioRegistroClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool RegistrarUsuario(WpfCliente.ServidorDescribelo.Usuario usuario) {
            return base.Channel.RegistrarUsuario(usuario);
        }
        
        public System.Threading.Tasks.Task<bool> RegistrarUsuarioAsync(WpfCliente.ServidorDescribelo.Usuario usuario) {
            return base.Channel.RegistrarUsuarioAsync(usuario);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioSala")]
    public interface IServicioSala {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSala/CrearSala", ReplyAction="http://tempuri.org/IServicioSala/CrearSalaResponse")]
        string CrearSala(string nombreUsuarioAnfitrion);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSala/CrearSala", ReplyAction="http://tempuri.org/IServicioSala/CrearSalaResponse")]
        System.Threading.Tasks.Task<string> CrearSalaAsync(string nombreUsuarioAnfitrion);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSala/ValidarSala", ReplyAction="http://tempuri.org/IServicioSala/ValidarSalaResponse")]
        bool ValidarSala(string idSala);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSala/ValidarSala", ReplyAction="http://tempuri.org/IServicioSala/ValidarSalaResponse")]
        System.Threading.Tasks.Task<bool> ValidarSalaAsync(string idSala);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioSalaChannel : Pruebas.ServidorDescribeloPrueba.IServicioSala, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioSalaClient : System.ServiceModel.ClientBase<Pruebas.ServidorDescribeloPrueba.IServicioSala>, Pruebas.ServidorDescribeloPrueba.IServicioSala {
        
        public ServicioSalaClient() {
        }
        
        public ServicioSalaClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioSalaClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioSalaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioSalaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string CrearSala(string nombreUsuarioAnfitrion) {
            return base.Channel.CrearSala(nombreUsuarioAnfitrion);
        }
        
        public System.Threading.Tasks.Task<string> CrearSalaAsync(string nombreUsuarioAnfitrion) {
            return base.Channel.CrearSalaAsync(nombreUsuarioAnfitrion);
        }
        
        public bool ValidarSala(string idSala) {
            return base.Channel.ValidarSala(idSala);
        }
        
        public System.Threading.Tasks.Task<bool> ValidarSalaAsync(string idSala) {
            return base.Channel.ValidarSalaAsync(idSala);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioSalaJugador", CallbackContract=typeof(Pruebas.ServidorDescribeloPrueba.IServicioSalaJugadorCallback))]
    public interface IServicioSalaJugador {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/AgregarJugadorSala", ReplyAction="http://tempuri.org/IServicioSalaJugador/AgregarJugadorSalaResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WpfCliente.ServidorDescribelo.UsuarioDuplicadoFalla), Action="http://tempuri.org/IServicioSalaJugador/AgregarJugadorSalaUsuarioDuplicadoFallaFa" +
            "ult", Name="UsuarioDuplicadoFalla", Namespace="http://schemas.datacontract.org/2004/07/WcfServicioLibreria.Modelo")]
        bool AgregarJugadorSala(string gamertag, string idSala);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/AgregarJugadorSala", ReplyAction="http://tempuri.org/IServicioSalaJugador/AgregarJugadorSalaResponse")]
        System.Threading.Tasks.Task<bool> AgregarJugadorSalaAsync(string gamertag, string idSala);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/RemoverJugadorSala", ReplyAction="http://tempuri.org/IServicioSalaJugador/RemoverJugadorSalaResponse")]
        void RemoverJugadorSala(string gamertag, string ididSalaRoom);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/RemoverJugadorSala", ReplyAction="http://tempuri.org/IServicioSalaJugador/RemoverJugadorSalaResponse")]
        System.Threading.Tasks.Task RemoverJugadorSalaAsync(string gamertag, string ididSalaRoom);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioSalaJugador/ObtenerJugadoresSala")]
        void ObtenerJugadoresSala(string gamertag, string idSala);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioSalaJugador/ObtenerJugadoresSala")]
        System.Threading.Tasks.Task ObtenerJugadoresSalaAsync(string gamertag, string idSala);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioSalaJugador/EmpezarPartida")]
        void EmpezarPartida(string idSala);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioSalaJugador/EmpezarPartida")]
        System.Threading.Tasks.Task EmpezarPartidaAsync(string idSala);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioSalaJugador/AsignarColor")]
        void AsignarColor(string idSala);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioSalaJugador/AsignarColor")]
        System.Threading.Tasks.Task AsignarColorAsync(string idSala);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioSalaJugadorCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/ObtenerJugadoresSalaCallback", ReplyAction="http://tempuri.org/IServicioSalaJugador/ObtenerJugadoresSalaCallbackResponse")]
        void ObtenerJugadoresSalaCallback(string[] jugardoresEnSala);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/EmpezarPartidaCallBack", ReplyAction="http://tempuri.org/IServicioSalaJugador/EmpezarPartidaCallBackResponse")]
        void EmpezarPartidaCallBack();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioSalaJugador/AsignarColorCallback", ReplyAction="http://tempuri.org/IServicioSalaJugador/AsignarColorCallbackResponse")]
        void AsignarColorCallback(System.Collections.Generic.Dictionary<string, char> jugadoresColores);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioSalaJugadorChannel : Pruebas.ServidorDescribeloPrueba.IServicioSalaJugador, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioSalaJugadorClient : System.ServiceModel.DuplexClientBase<Pruebas.ServidorDescribeloPrueba.IServicioSalaJugador>, Pruebas.ServidorDescribeloPrueba.IServicioSalaJugador {
        
        public ServicioSalaJugadorClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServicioSalaJugadorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServicioSalaJugadorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioSalaJugadorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioSalaJugadorClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool AgregarJugadorSala(string gamertag, string idSala) {
            return base.Channel.AgregarJugadorSala(gamertag, idSala);
        }
        
        public System.Threading.Tasks.Task<bool> AgregarJugadorSalaAsync(string gamertag, string idSala) {
            return base.Channel.AgregarJugadorSalaAsync(gamertag, idSala);
        }
        
        public void RemoverJugadorSala(string gamertag, string ididSalaRoom) {
            base.Channel.RemoverJugadorSala(gamertag, ididSalaRoom);
        }
        
        public System.Threading.Tasks.Task RemoverJugadorSalaAsync(string gamertag, string ididSalaRoom) {
            return base.Channel.RemoverJugadorSalaAsync(gamertag, ididSalaRoom);
        }
        
        public void ObtenerJugadoresSala(string gamertag, string idSala) {
            base.Channel.ObtenerJugadoresSala(gamertag, idSala);
        }
        
        public System.Threading.Tasks.Task ObtenerJugadoresSalaAsync(string gamertag, string idSala) {
            return base.Channel.ObtenerJugadoresSalaAsync(gamertag, idSala);
        }
        
        public void EmpezarPartida(string idSala) {
            base.Channel.EmpezarPartida(idSala);
        }
        
        public System.Threading.Tasks.Task EmpezarPartidaAsync(string idSala) {
            return base.Channel.EmpezarPartidaAsync(idSala);
        }
        
        public void AsignarColor(string idSala) {
            base.Channel.AsignarColor(idSala);
        }
        
        public System.Threading.Tasks.Task AsignarColorAsync(string idSala) {
            return base.Channel.AsignarColorAsync(idSala);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioUsuario")]
    public interface IServicioUsuario {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioUsuario/EditarUsuario", ReplyAction="http://tempuri.org/IServicioUsuario/EditarUsuarioResponse")]
        bool EditarUsuario(WpfCliente.ServidorDescribelo.Usuario usuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioUsuario/EditarUsuario", ReplyAction="http://tempuri.org/IServicioUsuario/EditarUsuarioResponse")]
        System.Threading.Tasks.Task<bool> EditarUsuarioAsync(WpfCliente.ServidorDescribelo.Usuario usuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioUsuario/Ping", ReplyAction="http://tempuri.org/IServicioUsuario/PingResponse")]
        bool Ping();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioUsuario/Ping", ReplyAction="http://tempuri.org/IServicioUsuario/PingResponse")]
        System.Threading.Tasks.Task<bool> PingAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioUsuarioChannel : Pruebas.ServidorDescribeloPrueba.IServicioUsuario, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioUsuarioClient : System.ServiceModel.ClientBase<Pruebas.ServidorDescribeloPrueba.IServicioUsuario>, Pruebas.ServidorDescribeloPrueba.IServicioUsuario {
        
        public ServicioUsuarioClient() {
        }
        
        public ServicioUsuarioClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioUsuarioClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioUsuarioClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioUsuarioClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool EditarUsuario(WpfCliente.ServidorDescribelo.Usuario usuario) {
            return base.Channel.EditarUsuario(usuario);
        }
        
        public System.Threading.Tasks.Task<bool> EditarUsuarioAsync(WpfCliente.ServidorDescribelo.Usuario usuario) {
            return base.Channel.EditarUsuarioAsync(usuario);
        }
        
        public bool Ping() {
            return base.Channel.Ping();
        }
        
        public System.Threading.Tasks.Task<bool> PingAsync() {
            return base.Channel.PingAsync();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDescribeloPrueba.IServicioUsuarioSesion", CallbackContract=typeof(Pruebas.ServidorDescribeloPrueba.IServicioUsuarioSesionCallback))]
    public interface IServicioUsuarioSesion {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioUsuarioSesion/ObtenerSessionJugador")]
        void ObtenerSessionJugador(WpfCliente.ServidorDescribelo.Usuario usuario);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicioUsuarioSesion/ObtenerSessionJugador")]
        System.Threading.Tasks.Task ObtenerSessionJugadorAsync(WpfCliente.ServidorDescribelo.Usuario usuario);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioUsuarioSesionCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioUsuarioSesion/ObtenerSessionJugadorCallback", ReplyAction="http://tempuri.org/IServicioUsuarioSesion/ObtenerSessionJugadorCallbackResponse")]
        void ObtenerSessionJugadorCallback(bool esSesionAbierta);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioUsuarioSesionChannel : Pruebas.ServidorDescribeloPrueba.IServicioUsuarioSesion, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioUsuarioSesionClient : System.ServiceModel.DuplexClientBase<Pruebas.ServidorDescribeloPrueba.IServicioUsuarioSesion>, Pruebas.ServidorDescribeloPrueba.IServicioUsuarioSesion {
        
        public ServicioUsuarioSesionClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServicioUsuarioSesionClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServicioUsuarioSesionClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioUsuarioSesionClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioUsuarioSesionClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void ObtenerSessionJugador(WpfCliente.ServidorDescribelo.Usuario usuario) {
            base.Channel.ObtenerSessionJugador(usuario);
        }
        
        public System.Threading.Tasks.Task ObtenerSessionJugadorAsync(WpfCliente.ServidorDescribelo.Usuario usuario) {
            return base.Channel.ObtenerSessionJugadorAsync(usuario);
        }
    }
}
