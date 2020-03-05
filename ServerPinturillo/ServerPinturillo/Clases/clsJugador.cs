﻿namespace ServerPinturillo.Clases
{
    public class clsJugador
    {
        #region"Atributos privados"
        private string _connectionID;
        private int _puntuacion;
        private string _nickname;
        private bool _haTerminadoTimer;
        private bool _isUltimaPalabraAcertada;
        private bool _isLider;

        private bool _isMiTurno;
        private bool _isDesconectado;
        #endregion


        #region"Constructor"
        public clsJugador(string connectionID, int puntuacion, string nickname, bool haTerminadoTimer, bool isUltimaPalabraAcertada, bool isLider, bool isMyTurno)
        {
            ConnectionID = connectionID;
            Puntuacion = puntuacion;
            Nickname = nickname;
            HaTerminadoTimer = haTerminadoTimer;
            IsUltimaPalabraAcertada = isUltimaPalabraAcertada;
            IsLider = isLider;
            IsMiTurno = isMyTurno;
            IsDesconectado = false;

        }

        public clsJugador()
        {
        }


        #endregion


        #region"Propiedades públicas"
        public bool IsMiTurno { get => _isMiTurno; set => _isMiTurno = value; }
        public string ConnectionID { get => _connectionID; set => _connectionID = value; }
        public int Puntuacion { get => _puntuacion; set => _puntuacion = value; }
        public string Nickname { get => _nickname; set => _nickname = value; }
        public bool HaTerminadoTimer { get => _haTerminadoTimer; set => _haTerminadoTimer = value; }
        public bool IsUltimaPalabraAcertada { get => _isUltimaPalabraAcertada; set => _isUltimaPalabraAcertada = value; }
        public bool IsLider { get => _isLider; set => _isLider = value; }
        public bool IsDesconectado { get => _isDesconectado; set => _isDesconectado = value; }
        #endregion
    }
}