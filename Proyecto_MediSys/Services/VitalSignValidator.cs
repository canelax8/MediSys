using Proyecto_MediSys.Models;
using System.Windows;
using System.Windows.Media;

namespace Proyecto_MediSys.Services
{
    public class VitalSignResult
    {
        public string Estado { get; set; } = "";

        public Brush Color { get; set; } = Brushes.Green;

        public bool EsCritico { get; set; }
    }

    public static class VitalSignValidator
    {
        public static VitalSignResult ValidarTemperatura(decimal temperatura)
        {
            VitalSignResult resultado = new();

            if (temperatura < 35)
            {
                resultado.Estado = "Hipotermia";
                resultado.Color = Brushes.SteelBlue;
                resultado.EsCritico = true;
            }
            else if (temperatura <= 37.5m)
            {
                resultado.Estado = "Normal";
                resultado.Color = Brushes.Green;
            }
            else if (temperatura <= 38.5m)
            {
                resultado.Estado = "Fiebre";
                resultado.Color = Brushes.DarkOrange;
            }
            else
            {
                resultado.Estado = "Hipertermia";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar temperatura

        public static VitalSignResult ValidarPresion(string valor)
        {
            VitalSignResult resultado = new();

            string[] partes = valor.Split('/');

            if (partes.Length != 2)
            {
                resultado.Estado = "";
                resultado.Color = Brushes.Gray;
                return resultado;
            }

            if (!int.TryParse(partes[0], out int sistolica))
                return resultado;

            if (!int.TryParse(partes[1], out int diastolica))
                return resultado;

            if (sistolica < 90 || diastolica < 60)
            {
                resultado.Estado = "Hipotensión";
                resultado.Color = Brushes.SteelBlue;
                resultado.EsCritico = true;
            }
            else if (sistolica <= 120 && diastolica <= 80)
            {
                resultado.Estado = "Normal";
                resultado.Color = Brushes.Green;
            }
            else if (sistolica <= 139 || diastolica <= 89)
            {
                resultado.Estado = "Prehipertensión";
                resultado.Color = Brushes.DarkOrange;
            }
            else
            {
                resultado.Estado = "Hipertensión";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar presion

        public static VitalSignResult ValidarFrecuenciaCardiaca(string valor)
        {
            VitalSignResult resultado = new();

            if (!int.TryParse(valor, out int frecuencia))
                return resultado;

            if (frecuencia < 60)
            {
                resultado.Estado = "Bradicardia";
                resultado.Color = Brushes.SteelBlue;
                resultado.EsCritico = true;
            }
            else if (frecuencia <= 100)
            {
                resultado.Estado = "Normal";
                resultado.Color = Brushes.Green;
            }
            else if (frecuencia <= 140)
            {
                resultado.Estado = "Taquicardia";
                resultado.Color = Brushes.DarkOrange;
            }
            else
            {
                resultado.Estado = "Taquicardia Severa";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar frecuencia cardiaca

        public static VitalSignResult ValidarFrecuenciaRespiratoria(string valor)
        {
            VitalSignResult resultado = new();

            if (!int.TryParse(valor, out int frecuencia))
                return resultado;

            if (frecuencia < 12)
            {
                resultado.Estado = "Bradipnea";
                resultado.Color = Brushes.SteelBlue;
                resultado.EsCritico = true;
            }
            else if (frecuencia <= 20)
            {
                resultado.Estado = "Normal";
                resultado.Color = Brushes.Green;
            }
            else if (frecuencia <= 30)
            {
                resultado.Estado = "Taquipnea";
                resultado.Color = Brushes.DarkOrange;
            }
            else
            {
                resultado.Estado = "Taquipnea Severa";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar frecuencia respiratoria

        public static VitalSignResult ValidarSaturacion(string valor)
        {
            VitalSignResult resultado = new();

            if (!int.TryParse(valor, out int saturacion))
                return resultado;

            if (saturacion < 90)
            {
                resultado.Estado = "Hipoxemia Severa";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }
            else if (saturacion < 95)
            {
                resultado.Estado = "Hipoxemia";
                resultado.Color = Brushes.DarkOrange;
            }
            else
            {
                resultado.Estado = "Normal";
                resultado.Color = Brushes.Green;
            }

            return resultado;
        }//metodo de validar saturacion

        public static VitalSignResult ValidarGlucemia(string valor)
        {
            VitalSignResult resultado = new();

            if (!int.TryParse(valor, out int glucosa))
                return resultado;

            if (glucosa < 70)
            {
                resultado.Estado = "Hipoglucemia";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }
            else if (glucosa <= 140)
            {
                resultado.Estado = "Normal";
                resultado.Color = Brushes.Green;
            }
            else if (glucosa <= 200)
            {
                resultado.Estado = "Hiperglucemia";
                resultado.Color = Brushes.DarkOrange;
            }
            else
            {
                resultado.Estado = "Hiperglucemia Severa";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar glucemia

        public static VitalSignResult ValidarPeso(string valor)
        {
            VitalSignResult resultado = new();

            if (!decimal.TryParse(valor, out decimal peso))
                return resultado;

            if (peso < 2)
            {
                resultado.Estado = "Valor inválido";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }
            else if (peso <= 300)
            {
                resultado.Estado = "Correcto";
                resultado.Color = Brushes.Green;
            }
            else
            {
                resultado.Estado = "Valor inválido";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar peso

        public static VitalSignResult ValidarTalla(string valor)
        {
            VitalSignResult resultado = new();

            if (!decimal.TryParse(valor, out decimal talla))
                return resultado;

            if (talla < 30)
            {
                resultado.Estado = "Valor inválido";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }
            else if (talla <= 250)
            {
                resultado.Estado = "Correcto";
                resultado.Color = Brushes.Green;
            }
            else
            {
                resultado.Estado = "Valor inválido";
                resultado.Color = Brushes.Red;
                resultado.EsCritico = true;
            }

            return resultado;
        }//metodo de validar talla

        public static VitalSignResult Validar(
            TipoSignoVital tipo,
            string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return new VitalSignResult
                {
                    Estado = "",
                    Color = Brushes.Gray,
                    EsCritico = false
                };
            }

            switch (tipo)
            {
                case TipoSignoVital.Temperatura:

                    if (decimal.TryParse(valor, out decimal temperatura))
                        return ValidarTemperatura(temperatura);

                    break;
                case TipoSignoVital.Presion:

                    return ValidarPresion(valor);
                case TipoSignoVital.FrecuenciaCardiaca:

                    return ValidarFrecuenciaCardiaca(valor);
                case TipoSignoVital.FrecuenciaRespiratoria:

                    return ValidarFrecuenciaRespiratoria(valor);
                case TipoSignoVital.Saturacion:

                    return ValidarSaturacion(valor);

                case TipoSignoVital.Glucemia:

                    return ValidarGlucemia(valor);

                case TipoSignoVital.Peso:

                    return ValidarPeso(valor);

                case TipoSignoVital.Talla:

                    return ValidarTalla(valor);
            }

            return new VitalSignResult
            {
                Estado = "",
                Color = Brushes.Gray,
                EsCritico = false
            };
        }
    }
}