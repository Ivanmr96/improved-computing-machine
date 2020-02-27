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

        public void strokeDraw(List<clsPunto> puntos,string nombreGrupo) {
            Clients.OthersInGroup(nombreGrupo).mandarStroke(puntos);
        }

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

            clsJugador lider = new clsJugador(Context.ConnectionId, 0, nickLider, false, false, true, false);

            partida.ListadoJugadores.Add(lider);
            Groups.Add(Context.ConnectionId, partida.NombreSala);
            listadoSalas.ListadoPartidas.Add(partida);

            Clients.All.recibirSalas(listadoSalas.ListadoPartidas);
            Clients.Caller.salaCreada(partida);
        }


        //Metodo para indicar que se comienza la partida

        public void comenzarPartidaEnGrupo(clsPartida partida)  //TODO que aqui reciba el nombre de sala y ya
        {

            partida = obtenerPartidaPorNombreSala(partida.NombreSala);
            if(partida != null)
            {
                //Se pone el primer turno de la primera ronda
                //el turno es el del primer jugador
                if (partida.ListadoJugadores.Count > 0)
                partida.ConnectionIDJugadorActual = partida.ListadoJugadores[0].ConnectionID;
                partida.Turno = 1;
                partida.RondaActual = 1;
                partida.PalabraEnJuego = Utilidad.obtenerPalabraAleatoria();

                //TODO rellenar el listado de posiciones a descubrir
                partida.PosicionesADescubrir = Utilidad.rellenarPosicionesADescubrir(partida.PalabraEnJuego);

                partida.IsJugandose = true;


                //Por si acaso se le pone a todos que NO es su turno
                for (int i = 0; i < partida.ListadoJugadores.Count; i++)
                {
                    partida.ListadoJugadores[i].IsMiTurno = false;
                }


                //Se le pone que es su turno al jugador 
                clsJugador jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == partida.ConnectionIDJugadorActual);

                jugador.IsMiTurno = true;


                //Aqui haría falta guardar esta partida en la lista de partidas

                Clients.Group(partida.NombreSala).onPartidaComenzada(partida);

            }


        }



        public void yaHeNavegado(string nombreGrupo)
        {
            clsPartida partidaActual = obtenerPartidaPorNombreSala(nombreGrupo);
            partidaActual.JugadoresQueHanNavegado += 1;
            
            if(partidaActual.JugadoresQueHanNavegado == partidaActual.ListadoJugadores.Count)
            {
                //Todos han navegado
                comenzarPartidaEnGrupo(partidaActual);

            }


        }



        //Metodo que se llama cuando un contador de un cliente llega a 0
        public void miContadorHaLlegadoACero(string connectionIDJugador, string nombreGrupo)
        {
            bool todosHanTerminado = true;

            //Marcar ese jugador como que su timer ha terminado
            clsPartida partidaActual = obtenerPartidaPorNombreSala(nombreGrupo);

            clsJugador jugadorQueHaTerminado = partidaActual.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == connectionIDJugador);

            jugadorQueHaTerminado.HaTerminadoTimer = true;

            //Comprobar si todos los jugadores de la partida han terminado
            for(int i = 0; i < partidaActual.ListadoJugadores.Count; i++)
            {
                if (!partidaActual.ListadoJugadores[i].HaTerminadoTimer)
                {
                    //Si el timer de alguno NO ha terminado, entonces no todos han terminado
                    todosHanTerminado = false;
                }
            }


            if (todosHanTerminado)
            {
                //Vuelvo a poner sus "haTerminadoTimer" a false
                for (int i = 0; i < partidaActual.ListadoJugadores.Count; i++)
                {
                    partidaActual.ListadoJugadores[i].HaTerminadoTimer = false;
                }

                //Cuando todos han terminado, se llama a avanzar turno
                avanzarTurno(partidaActual);
            }

        }


        //Para cambiar el turno
        public void avanzarTurno(clsPartida partida)
        {
            clsPartida partidaObtenida = obtenerPartidaPorNombreSala(partida.NombreSala);
            if(partidaObtenida.RondaActual < partida.NumeroRondasGlobales)
            {
                //El cliente que termine su turno, llama a este método
                //este método avisa a los clientes de que el turno ha cambiado
                //si el cliente detecta que el id del nuevo jugador es el propio
                //hace cosas (habilita su canvas), los demas canvas de inhabilitan
                //se debe cambiar la palabra en juego

                //Obtengo el jugador actual
                clsJugador jugadorJugando = partida.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == partida.ConnectionIDJugadorActual);


                ////Se ponen todos los "isUltimaPalabraAcertada" a false
                for (int i = 0; i < partida.ListadoJugadores.Count; i++)
                {
                    partida.ListadoJugadores[i].IsUltimaPalabraAcertada = false;
                    //Por si acaso se le pone a todos que NO es su turno
                    partida.ListadoJugadores[i].IsMiTurno = false;
                }

                //Obtengo la posicion en la lista de jugadores del jugador actual
                int posicion = -1;

                for (int i = 0; i < partida.ListadoJugadores.Count || posicion == -1; i++)
                {
                    if (partida.ListadoJugadores[i].ConnectionID == jugadorJugando.ConnectionID)
                    {
                        posicion = i;
                    }

                }
                //Asigno una nueva palabra
                partida.PalabraEnJuego = Utilidad.obtenerPalabraAleatoria();
                //TODO rellenar el listado de posiciones a descubrir
                partida.PosicionesADescubrir = Utilidad.rellenarPosicionesADescubrir(partida.PalabraEnJuego);


                //Cambio el jugador jugando
                if (posicion < partida.ListadoJugadores.Count - 1)
                {
                    partida.ConnectionIDJugadorActual = partida.ListadoJugadores[(posicion + 1)].ConnectionID;

                }
                else
                {
                    //Si ya han jugado todos los jugadores de la lista, se vuelve a empezar
                    partida.ConnectionIDJugadorActual = partida.ListadoJugadores[0].ConnectionID;
                }


                //Se le pone que es su turno al jugador 
                clsJugador jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == partida.ConnectionIDJugadorActual);

                jugador.IsMiTurno = true;



                //Cambio el turno/ronda
                if (partida.Turno < partida.ListadoJugadores.Count)
                {
                    partida.Turno++;

                }
                else
                {
                    partida.Turno = 1;
                    if (partida.RondaActual < partida.NumeroRondasGlobales)
                    {
                        partida.RondaActual++;
                    }
                    else
                    {
                        //Terminaria la partida y se harian cosas
                    }

                }

                //Llamo al metodo haCambiadoElTurno de los clientes, y en ese metodo se debera comprobar si le toca al propio usuario
                Clients.Group(partida.NombreSala).haCambiadoElTurno(partida);
            }
           
        }


        //el cliente llama a este método cuando acierte la palabra
        public void addPuntosToUser(string connectionIDUSer, int puntosToAdd,string nombreGrupo)
        {
            clsPartida partidaActual = obtenerPartidaPorNombreSala(nombreGrupo);
            clsJugador jugador = partidaActual.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == connectionIDUSer);

            jugador.Puntuacion += puntosToAdd;
            jugador.IsUltimaPalabraAcertada = true;

            Clients.Group(nombreGrupo).puntosAdded(partidaActual);


            //Si todos los jugadores ya han acertado la palabra, se pasa el turno
            int acertantes = 0;

            //Comprobar si todos los jugadores de la partida han acertado ya
            for (int i = 0; i < partidaActual.ListadoJugadores.Count; i++)
            {
                if (partidaActual.ListadoJugadores[i].IsUltimaPalabraAcertada)
                {
                    acertantes++;
                }
            }

            if (acertantes == (partidaActual.ListadoJugadores.Count -1))    //deben haber acertado todos menos uno (el que pinta)
            {
                avanzarTurno(partidaActual);
            }

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

            Clients.OthersInGroup(nombreGrupo).empezarPartida();

        }

        public void borrarCanvas(String nombreGrupo) {
            Clients.OthersInGroup(nombreGrupo).borrarCanvas();
        }

        public void addJugadorToSala(string nombreGrupo, clsJugador jugador)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);
            clsJugador jugadorBuscado;
            if (partida != null)
            {
                
                jugadorBuscado = partida.ListadoJugadores.FirstOrDefault<clsJugador>(p => p.ConnectionID == jugador.ConnectionID);

                
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

            jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuario);


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
                    if (jugador.IsLider)
                    {
                        llamarConvertirEnLider(nombreSala);    
                        //luego (si era el lider) se pone de lider al primero de la lista
                    }

                }
            }

        }

        //Metodo que llama al metodo de un cliente en concreto y lo pone como lider
        //A este metodo se le llamara cuando salga de la partida o se desconecte un
        //jugador que sea el actual lider del grupo.
        public void llamarConvertirEnLider(string nombreGrupo)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);
            string conexionIDSiguienteJugador = "";

            if (partida.ListadoJugadores.Count > 0)
            {
                conexionIDSiguienteJugador = partida.ListadoJugadores[0].ConnectionID;    //el primer jugador que haya (el jugador lider ya debe haber salido del grupo)
                if (!String.IsNullOrEmpty(conexionIDSiguienteJugador))
                {
                    Clients.Client(conexionIDSiguienteJugador).nombrarComoLider();  //nombramos como lider a ese jugador
                }
            }


        }

        //A este método se le llama cuando hay un nuevo líder, para guardar el dato
        //en el listado de partidas del servidor
        public void habemusNuevoLider(string nickUsuario, string nombreGrupo)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);

            //Se ponen todos los "isLider" a false (en realidad creo que no haría falta ya que
            //el anterior líder habría salido ya de la lista de jugadores
            for (int i = 0; i < partida.ListadoJugadores.Count; i++)
            {
                partida.ListadoJugadores[i].IsLider = false;
            }

            //Se pone el nuevo lider
            clsJugador jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == nickUsuario);
            jugador.IsLider = true;
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