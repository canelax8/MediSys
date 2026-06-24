using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_MediSys.Data
{
    class Conexion
    {
        private string cadenaConexion =
            @"Server=LuisCanela;
              Database=MediSysV2;
              Trusted_Connection=True;
              TrustServerCertificate=True;";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}
