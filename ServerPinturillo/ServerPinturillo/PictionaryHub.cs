using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using ServerPinturillo.Clases;

namespace ServerPinturillo
{
    public class PictionaryHub : Hub
    {

        //Listado de salas que hay en el juego
        private SingletonSalas listadoSalas;

        public PictionaryHub(SingletonSalas salas)
        {
            this.listadoSalas = salas;
        }

        /*
         -crear partida
         -enviar listado de salas
         -enviar lista de mensajes 
         -enviar el mensaje añadido
         -notificar que se ha unido una persona a la partida 
         -notificar que la partida ha empezado para que comiencen a contar el timer
         -enviar partida (solo se enviaría al usuario que crea la partido)
         -
         */

        public void sendSalas()
        {
            //List<clsPartida> listadoSalas = new List<clsPartida>();
            //listadoSalas.Add(new clsPartida(false, null, null, "Sala1", null, null, "Palabra1", 5, 0, 4, 0, null));
            //for (int i = 0; i < 50; i++)
            //{
            //    listadoSalas.Add(new clsPartida(false, null, null, "Sala " + i, null, null, "Palabra1", 5, 0, 4, 0, null));
            //}

            Clients.Caller.recibirSalas(listadoSalas.ListadoPartidas);
        }

        public void añadirPartida(clsPartida partida, String nickLider)
        {
            //clsJugador jugador = partida.ListadoJugadores.First<clsJugador>(j => j.ConnectionID == Context.ConnectionId);

            clsJugador lider = new clsJugador(Context.ConnectionId, 0, nickLider, false, false, true);

            partida.ListadoJugadores.Add(lider);
            Groups.Add(Context.ConnectionId, partida.NombreSala);
            listadoSalas.ListadoPartidas.Add(partida);

            Clients.All.recibirSalas(listadoSalas.ListadoPartidas);
            Clients.Caller.salaCreada(partida);
        }



        public void sendMensaje (clsMensaje mensaje, string nombreGrupo)
        {

            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);

            if(partida != null)
            {
                partida.ListadoMensajes.Add(mensaje);
                Clients.Group(nombreGrupo).addMensajeToChat(mensaje);
                //Clients.All.addMensajeToChat(mensaje);
            }
            
        }

        public void empezarPartida(String nombreGrupo) {
            Clients.Group(nombreGrupo).empezarPartida();
        }

        public void addJugadorToSala(string nombreGrupo, clsJugador jugador)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);
            clsJugador jugadorBuscado;
            if (partida != null)
            {
                try {
                    jugadorBuscado = partida.ListadoJugadores.First<clsJugador>(p => p.ConnectionID == jugador.ConnectionID);
                } catch (Exception e) {
                    jugadorBuscado = null;
                }
                
                if (jugadorBuscado == null)
                {
                    if (partida.ListadoJugadores.Count < partida.NumeroMaximoJugadores)
                    {

                        jugador.ConnectionID = Context.ConnectionId;
                        Groups.Add(Context.ConnectionId, partida.NombreSala);
                        partida.ListadoJugadores.Add(jugador);

                        Clients.All.jugadorAdded(jugador, partida);
                    }
                }
            }
            
        }

        private clsPartida obtenerPartidaPorNombreSala(String nombreGrupo)
        {
            return listadoSalas.ListadoPartidas.Find(x => x.NombreSala == nombreGrupo);
        }

        public void jugadorHaSalido(string usuario, string nombreSala)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreSala);
            clsJugador jugador;
            try
            {
                jugador = partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuario);
            }catch (Exception e)
            {
                jugador = null;
            }

            if(jugador != null)
            {
                partida.ListadoJugadores.Remove(jugador);
                Groups.Remove(Context.ConnectionId, partida.NombreSala);
                if (partida.ListadoJugadores.Count == 0)
                {
                    listadoSalas.ListadoPartidas.Remove(partida);
                    Clients.All.eliminarPartidaVacia(partida.NombreSala);
                }
                else
                {
                    Clients.All.jugadorDeletedSala(jugador.Nickname, nombreSala);
                }              
            }

        }



        //Si algo falla esto es lo que hay que quitar
        //TODO falta asignarle el connection id a cada usuario  //añadido
        public override Task OnDisconnected(bool stopCalled)
        {
            //Obtener el ID de conexion del usuario que ha salido
            clsJugador jugadorQueSeDesconecta = null;
            string nombreGrupo = "";
            bool encontrado = false;

                for(int i = 0; i < listadoSalas.ListadoPartidas.Count && !encontrado; i++)
                {
                    for (int j = 0; j < listadoSalas.ListadoPartidas[i].ListadoJugadores.Count && !encontrado; j++)
                    {
                    if(listadoSalas.ListadoPartidas[i].ListadoJugadores[j].ConnectionID == Context.ConnectionId)
                    {
                        jugadorQueSeDesconecta = listadoSalas.ListadoPartidas[i].ListadoJugadores[j];
                        nombreGrupo = listadoSalas.ListadoPartidas[i].NombreSala;
                        encontrado = true;

                    }

                    }

                }
            //Comprobar si estaba en algun grupo 

            if(encontrado) //Si estaba en un grupo, sacarlo 
            {
                jugadorHaSalido(jugadorQueSeDesconecta.Nickname, nombreGrupo);
            }
            else //no esta en ningun grupo
            {
                //Resto de lógica de cuando un usuario de grupo sale
                //TODO
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}