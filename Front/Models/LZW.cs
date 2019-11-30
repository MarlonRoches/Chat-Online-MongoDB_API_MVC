using System;
using System.Collections.Generic;
using System.Linq;

namespace LZW
{
    public class LZW
    {
        public Tuple<Dictionary<string, int>, byte[]> Comprimir(string texto)
        {
            Dictionary<string, int> diccionarioInicial = Diccionario(texto);
            Dictionary<string, int> diccionarioAdaptativo = new Dictionary<string, int>(diccionarioInicial);
            int i = 0;
            string caracteresLeidos = string.Empty;
            List<byte> bytes = new List<byte>();
            while (i < texto.Length)
            {
                caracteresLeidos += texto[i];
                i++;

                while (diccionarioAdaptativo.ContainsKey(caracteresLeidos) && i < texto.Length)
                {
                    caracteresLeidos += texto[i];
                    i++;
                }

                if (diccionarioAdaptativo.ContainsKey(caracteresLeidos))
                {
                    bytes.Add(Convert.ToByte(diccionarioAdaptativo[caracteresLeidos]));
                }
                else
                {
                    string llave_secreta = caracteresLeidos.Substring(0, caracteresLeidos.Length - 1);
                    bytes.Add(Convert.ToByte(diccionarioAdaptativo[llave_secreta]));
                    diccionarioAdaptativo.Add(caracteresLeidos, diccionarioAdaptativo.Count + 1);
                    i--;
                    caracteresLeidos = string.Empty;
                }
            }
            return new Tuple<Dictionary<string, int>, byte[]>(diccionarioInicial, bytes.ToArray());
        }

        public string Descomprimir(Dictionary<string, int> diccionarioInicial, byte[] bytesComprimidos)
        {
            Dictionary<string, int> diccionarioAdaptativo = new Dictionary<string, int>(diccionarioInicial);
            int antiguo = Convert.ToInt32(bytesComprimidos[0]);
            string textoDescomprimido = diccionarioAdaptativo.FirstOrDefault(x => x.Value == antiguo).Key;
            int i = 1;
            while (i < bytesComprimidos.Length)
            {
                string texto = string.Empty;
                string caracteresLeidos = diccionarioAdaptativo.FirstOrDefault(x => x.Value == antiguo).Key;
                int nuevo = Convert.ToInt32(bytesComprimidos[i]);
                texto = diccionarioAdaptativo.FirstOrDefault(x => x.Value == nuevo).Key;
                diccionarioAdaptativo.Add(caracteresLeidos + texto[0], diccionarioAdaptativo.Count + 1);
                textoDescomprimido += texto;
                antiguo = nuevo;
                i++;
            }
            return textoDescomprimido;
        }

        public Dictionary<string, int> Diccionario(string texto)
        {
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            foreach (char caracter in texto)
            {
                if (!diccionario.ContainsKey(caracter.ToString()))
                    diccionario.Add(caracter.ToString(), diccionario.Count + 1);
            }
            return diccionario;
        }
    }
}