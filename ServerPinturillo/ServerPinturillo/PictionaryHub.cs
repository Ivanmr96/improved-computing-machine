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

        /*Debemos guardar una lista de tuplas que contendrá
            item1 -> el nick del usuario
            item2 -> el connectionID del usuario
            Esto es porque si no no se puede saber si un usuario que no está en ninguna partida existe
            */
        private SingletonNicksConConnectionID listadoNickConConnectionID;


        public PictionaryHub(SingletonSalas salas, SingletonNicksConConnectionID singletonNicks)
        {
            this.listadoSalas = salas;
            this.listadoNickConConnectionID = singletonNicks;

        }

        /// <summary>
        /// Método que será llamado cuando un cliente esté pintando y se lo enviará
        /// a los demás clientes del grupo
        /// </summary>
        /// <param name="puntos"> el listado de puntos que ha pintado el cliente</param>
        /// <param name="nombreGrupo">el nombre del grupo donde está el cliente que pinta</param>
        public void strokeDraw(List<clsPunto> puntos,string nombreGrupo) {
            Clients.OthersInGroup(nombreGrupo).mandarStroke(puntos);
        }

        /// <summary>
        /// Le envía el listado de salas del servidor al cliente que lo llama.
        /// </summary>
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

        /// <summary>
        /// A este método lo llamará el cliente que cree una partida, y
        /// así se añadirá al listado de partidas del servidor y se 
        /// añadirá el usuario creador como líder de la partida.
        /// Se añadirá el nuevo grupo y los clientes recibirán las partidas
        /// para que vean la nueva partida reflejada en su listado de salas.
        /// El que llama también recibirá la partida creada.
        /// </summary>
        /// <param name="partida">partida creada</param>
        /// <param name="nickLider">nick del líder (quien ha creado la partida)</param>
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


        /// <summary>
        /// Metodo para indicar que se comienza la partida
        /// Se establecen propiedades como el turno, la ronda, la palabra en juego, IsJugandose.
        /// Llama al OnPartidaComenzada de los clientes del grupo.
        /// </summary>
        /// <param name="partida">partida que comienza</param>
        public void comenzarPartidaEnGrupo(clsPartida partida)  
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


        /// <summary>
        /// Método que será llamado por un cliente cuando ya haya navegado de una pantalla concreta
        /// a otra, para evitar que el servidor dé los datos de las partidas antes de que todos
        /// los integrantes del grupo hayan viajado.
        /// </summary>
        /// <param name="nombreGrupo">nombre del grupo</param>
        public void yaHeNavegado(string nombreGrupo)
        {
            clsPartida partidaActual = obtenerPartidaPorNombreSala(nombreGrupo);

            if(partidaActual != null)
            {
                partidaActual.JugadoresQueHanNavegado += 1;

                if (partidaActual.JugadoresQueHanNavegado == partidaActual.ListadoJugadores.Count)
                {
                    //Todos han navegado
                    partidaActual.ListadoMensajes = new System.Collections.ObjectModel.ObservableCollection<clsMensaje>();
                    comenzarPartidaEnGrupo(partidaActual);

                }
            }
        }



        /// <summary>
        /// Método que será llamado por un cliente cuando un contador de un cliente llega a 0, 
        /// para evitar que el servidor dé los datos del siguiente turno antes de que todos
        /// los integrantes del grupo hayan terminado.
        /// </summary>
        /// <param name="connectionIDJugador"></param>
        /// <param name="nombreGrupo"></param>
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


        /// <summary>
        /// Método que avanza un turno en una partida, cambiando todas las propiedades
        /// pertinentes como el turno actual, la ronda y el jugador actual.
        /// Se llamará al método OnHaCambiadoElTurno de todos los clientes del grupo.
        /// </summary>
        /// <param name="partida">partida en juego en la que se pasará el turno</param>
        public void avanzarTurno(clsPartida partida)
        {
            clsPartida partidaObtenida = obtenerPartidaPorNombreSala(partida.NombreSala);
            if(partidaObtenida.RondaActual <= partida.NumeroRondasGlobales)
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
                    if (jugadorJugando.IsDesconectado)
                    {
                        partida.Turno--;
                        jugadorJugando.IsDesconectado = false;
                    }

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
                        Clients.Group(partida.NombreSala).HaTerminadoLaPartida();
                    }
                }

                //Llamo al metodo haCambiadoElTurno de los clientes, y en ese metodo se debera comprobar si le toca al propio usuario
                Clients.Group(partida.NombreSala).haCambiadoElTurno(partida);
            }
        }


        /// <summary>
        /// El cliente llama a este método cuando acierte la palabra y haya que sumar
        /// puntos a un usuario.
        /// El método pasa el turno si todos los usuarios (menos el que pintaba)
        /// de una partida han acertado.
        /// </summary>
        /// <param name="connectionIDUSer">connection ID del usuario al que han de sumarse los puntos</param>
        /// <param name="puntosToAdd">cantidad de puntos a sumar</param>
        /// <param name="nombreGrupo">nombre del grupo al que pertenece el usuario</param>
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
                //avanzarTurno(partidaActual);
                //Todos han acertado ya
                Clients.Group(nombreGrupo).todosHanAcertado();
            }

        }

        /// <summary>
        /// Añade un mensaje al listado de mensajes de una partida.
        /// Llama al método addMensajeToChat de todos los clientes del grupo.
        /// </summary>
        /// <param name="mensaje">mensaje a añadir</param>
        /// <param name="nombreGrupo">nombre del grupo donde hay que añadir el mensaje</param>
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

        /// <summary>
        /// Avisa a los otros miembros del grupo de que empieza la partida.
        /// </summary>
        /// <param name="nombreGrupo">nombre del grupo</param>
        public void empezarPartida(String nombreGrupo) {
            Clients.OthersInGroup(nombreGrupo).empezarPartida();

        }

        /// <summary>
        /// Método que es llamado cuando un cliente que pinta
        /// borra su canvas. Llama al método borrarCanvas de los demás
        /// clientes del grupo.
        /// </summary>
        /// <param name="nombreGrupo">nombre del grupo al que pertenece el cliente que llama</param>
        public void borrarCanvas(String nombreGrupo) {
            Clients.OthersInGroup(nombreGrupo).borrarCanvas();
        }

        /// <summary>
        /// Método que es llamado cuando un jugador se une a una sala creada.
        /// Se llama al método jugadorAdded de los clientes.
        /// </summary>
        /// <param name="nombreGrupo">nombre de la sala a la que se une</param>
        /// <param name="jugador">jugador que se une</param>
        public void addJugadorToSala(string nombreGrupo, clsJugador jugador)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);
            clsJugador jugadorBuscado;
            if (partida != null)
            {

               jugadorBuscado = partida.ListadoJugadores
               .FirstOrDefault<clsJugador>(p => p.ConnectionID == jugador.ConnectionID);

                if (jugadorBuscado == null)
                {
                    if (partida.ListadoJugadores.Count < partida.NumeroMaximoJugadores)
                    {
                       
                        jugador.ConnectionID = Context.ConnectionId;
                        Groups.Add(Context.ConnectionId, partida.NombreSala);
                        partida.ListadoJugadores.Add(jugador);

                        
                        Clients.Client(jugador.ConnectionID).jugadorAdded(jugador, partida);
                            Clients.AllExcept(jugador.ConnectionID).jugadorAdded(jugador, partida);
                        
                    }
                }
            }
            
        }

        /// <summary>
        /// Método que devuelve una partida dado el nombre de la misma.
        /// </summary>
        /// <param name="nombreGrupo">nombre de la partida a obtener</param>
        /// <returns>partida cuyo nombre se corresponde con el nombre dado</returns>
        private clsPartida obtenerPartidaPorNombreSala(String nombreGrupo)
        {
            return listadoSalas.ListadoPartidas.Find(x => x.NombreSala == nombreGrupo);
        }

        /// <summary>
        /// Método que se llamará cuando un cliente haya salido de una partida.
        /// El método se encargará de evaluar si es necesario pasar el turno si el jugador
        /// estaba jugando y era su turno. También se borra al usuario del grupo y de la partida.
        /// </summary>
        /// <param name="usuario">nick de usuario del cliente que ha salido</param>
        /// <param name="nombreSala">nombre del grupo del cual ha salido</param>
        public void jugadorHaSalido(string usuario, string nombreSala)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreSala);
            clsJugador jugador;

            jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuario);


            if(jugador != null)
            {
                if (jugador.IsMiTurno == true)
                {
                    // isJugadorDesconectado = true;
                    jugador.IsDesconectado = true;
                    avanzarTurno(partida);
                }
                //Elimina al jugador del array de jugadores de la partida
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


        /// <summary>
        //  Metodo que llama al metodo de un cliente en concreto y lo pone como lider
        //  A este metodo se le llamara cuando salga de la partida o se desconecte un
        //  jugador que sea el actual lider del grupo.
        /// </summary>
        /// <param name="nombreGrupo">nombre del grupo del cliente que llama</param>
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


        /// <summary>
        //  A este método se le llama cuando hay un nuevo líder, para guardar el dato
        //  en el listado de partidas del servidor
        /// </summary>
        /// <param name="nickUsuario">nick del nuevo lider</param>
        /// <param name="nombreGrupo">nombre del grupo al que pertenece el cliente que llama</param>
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



        /// <summary>
        /// Comprueba si un nick dado ya existe entre los usuarios conectados.
        /// Si existe lo añade al listado de nicks de usuarios conectados junto con su connectionID.
        /// Llama al método nickComprobado del cliente que llama.
        /// </summary>
        /// <param name="nick">nick a comprobar</param>
        public void comprobarNickUnico(string nick)
        {
            
            bool isNickUnico = true;
            for (int i = 0; i < this.listadoNickConConnectionID.ListadoNickConConnectionID.Count && isNickUnico; i++)
            {
                if (this.listadoNickConConnectionID.ListadoNickConConnectionID[i].Item1 == nick)
                {
                  
                    //Ya existe ese nick
                    isNickUnico = false;

                }
                
            }

            if (isNickUnico)
            {
                //Se guarda en el listado de nicks con el connection ID
                this.listadoNickConConnectionID.ListadoNickConConnectionID.Add(new Tuple<string, string>(nick, Context.ConnectionId));
            }

            //Llamo al método en el cliente caller
            Clients.Caller.nickComprobado(isNickUnico);  //Le mando el boolean porque en el cliente si es false mostrará mensaje y si es true irá hacia delante

        }

        /// <summary>
        /// Comprueba si el nombre de la sala dado ya existe.
        /// Llama al método nombreSalaComprobado del cliente que llama.
        /// </summary>
        /// <param name="nombreSala">nombre de la sala a comprobar</param>
        public void comprobarNombreSalaUnico(string nombreSala)
        {

            bool isNombreSalaUnico = true;
            for (int i = 0; i < this.listadoSalas.ListadoPartidas.Count 
                && isNombreSalaUnico; i++)
            {
                if (this.listadoSalas.ListadoPartidas[i].NombreSala == nombreSala)
                {
                    //Ya existe ese nombre de sala
                    isNombreSalaUnico = false;
                }

            }
            //Llamo al método en el cliente caller
            Clients.Caller.nombreSalaComprobado(isNombreSalaUnico);  //Le mando el boolean porque en el cliente si es false mostrará mensaje y si es true irá hacia delante

        }

        /// <summary>
        /// Callback sobreescrito que se llama cuando un cliente se desconecta.
        /// Este método llama a jugador ha salido si el cliente desconectado pertenecía a un grupo.
        /// </summary>
        /// <param name="stopCalled">indica si la desconexión fue forzosa o no</param>
        /// <returns></returns>
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

            encontrado = false;
            for(int i = 0; i< this.listadoNickConConnectionID.ListadoNickConConnectionID.Count && !encontrado; i ++)
            {
                if(Context.ConnectionId == this.listadoNickConConnectionID.ListadoNickConConnectionID[i].Item2)
                {
                    encontrado = true;
                    this.listadoNickConConnectionID.ListadoNickConConnectionID.RemoveAt(i);    //Se elimina del listado de nicks
                }
            }
            
            return base.OnDisconnected(stopCalled);
        }
    }
}