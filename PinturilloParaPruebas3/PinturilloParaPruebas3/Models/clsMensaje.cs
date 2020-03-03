﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinturilloParaPruebas3.Models
{
    public class clsMensaje : clsVMBase
    {
        #region"Atributos privados"
        private string _mensaje;
        private clsJugador _jugadorQueLoEnvia;


        #endregion

        #region"Constructor"

        public clsMensaje()
        {
        }

        public clsMensaje(string mensaje, clsJugador jugadorQueLoEnvia)
        {
            Mensaje = mensaje;
            JugadorQueLoEnvia = jugadorQueLoEnvia;
        }
        #endregion

        #region"Propiedades públicas"
        public string Mensaje { get => _mensaje;
            set => _mensaje = value; }
        public clsJugador JugadorQueLoEnvia { get => _jugadorQueLoEnvia; set => _jugadorQueLoEnvia = value; }

        #endregion

    }
}
