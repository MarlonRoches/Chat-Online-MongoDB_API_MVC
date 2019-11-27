using System;
using System.IO;

namespace Back.Data
{
    public class Singleton
    {
        private static Singleton _instance = null;
        public static Singleton Instance
        {
            get
            {
                if (_instance == null) _instance = new Singleton();
                return _instance;
            }
        }

        public string CifradoSDES(int LlaveDelUsuario, string Contraseña)
        {
            #region VariablesGlobales
            string index_p10 = "2416390875";
            //string Index_PermutacionSeleccionada = "39860127";
            string index_p8 = "52637498";
            string index_p4 = "0321";
            string index_Expand = "13023201";

            string index_inicial = "15203746";

            string index_IPinverse = "30246175";
            string index_leftshift1 = "12340";
            string[,] S0 = new string[4, 4];
            {
                S0[0, 0] = "01";
                S0[0, 1] = "00";
                S0[0, 2] = "11";
                S0[0, 3] = "10";
                S0[1, 0] = "11";
                S0[1, 1] = "10";
                S0[1, 2] = "01";
                S0[1, 3] = "00";
                S0[2, 0] = "00";
                S0[2, 1] = "10";
                S0[2, 2] = "01";
                S0[2, 3] = "11";
                S0[3, 0] = "11";
                S0[3, 1] = "01";
                S0[3, 2] = "11";
                S0[3, 3] = "10";
            }
            string[,] S1 = new string[4, 4];
            {
                S1[0, 0] = "00";
                S1[0, 1] = "01";
                S1[0, 2] = "10";
                S1[0, 3] = "11";
                S1[1, 0] = "10";
                S1[1, 1] = "00";
                S1[1, 2] = "01";
                S1[1, 3] = "11";
                S1[2, 0] = "11";
                S1[2, 1] = "00";
                S1[2, 2] = "01";
                S1[2, 3] = "00";
                S1[3, 0] = "10";
                S1[3, 1] = "01";
                S1[3, 2] = "00";
                S1[3, 3] = "11";
            }

            #endregion
            //generacion de llaves

            var originalkey = LlaveDelUsuario;

            var KEYAR = Generarkeys(originalkey);
            var KEY1 = KEYAR[0];
            var KEY2 = KEYAR[1];
            //CIFRANDO
            var decoded = Contraseña;

            var encoded = "";

            foreach (var item in Contraseña)
            {

                var caracter = (char)item;
                var bin = Convert.ToString(item, 2).PadLeft(8, '0');
                var monitor = Convert.ToByte(BinarioADecimal(CifradoSDES(KEY1, KEY2, bin)));
                caracter = (char)monitor;
                encoded += caracter;
            }
            decoded = "";
            foreach (var item in encoded)
            {

                var caracter = (char)item;
                var bin = Convert.ToString(item, 2).PadLeft(8, '0');
                var monitor = Convert.ToByte(BinarioADecimal(CifradoSDES(KEY2, KEY1, bin)));
                caracter = (char)monitor;
                decoded += caracter;
            }
            return encoded;

            string CifradoSDES(string key1, string key2, string actual)
            {
                //1 permutar 8
                var entrada = inicial(actual);

                //2 tomar izquierda y derecha 
                var Mitadizquierda = entrada.Substring(0, 4);
                var MitadDerecha = entrada.Remove(0, 4);

                //3 expandir derecho
                var expandido = Expandir(MitadDerecha);

                //4 xor key1 y lado derecho
                var xorResultado = XOR(key1, expandido);

                //5 separar en bloques de 4
                var xor1izquierda = xorResultado.Substring(0, 4);
                var xor1derecha = xorResultado.Remove(0, 4);

                //6 s0box para xorizq y s1box para xorder
                var Yaux = BinarioADecimal(($"{xor1izquierda[1]}{xor1izquierda[2]}"));
                var XAux = BinarioADecimal(($"{xor1izquierda[0]}{xor1izquierda[3]}"));

                var BoxResultL = S0[XAux, Yaux];//izquierda
                Yaux = BinarioADecimal(($"{xor1derecha[1]}{xor1derecha[2]}"));
                XAux = BinarioADecimal(($"{xor1derecha[0]}{xor1derecha[3]}"));
                var BoxResultD = S1[XAux, Yaux];//derecha

                //7 P4 a BoxResultL 
                var paso7 = P4($"{BoxResultL}{BoxResultD}");

                //8 XOR con mitarizquierda
                var paso8 = XOR(Mitadizquierda, paso7);

                //ppaso 9 y 10
                var juntosSwaped = MitadDerecha + paso8;

                //paso 11 EP bloque 2 del paso10 
                var segundoexpandido = Expandir(juntosSwaped.Remove(0, 4));
                var monico = segundoexpandido.Length;

                ////paso12 xor de segundo expandido con key 2
                var xorPaso12 = XOR(key2, segundoexpandido);
                var xorpaso12izq = xorPaso12.Substring(0, 4);
                var xorpaso12der = xorPaso12.Remove(0, 4);

                //13 s0box para xorpaso12izq y 1 para el derecho
                Yaux = BinarioADecimal(($"{xorpaso12izq[1]}{xorpaso12izq[2]}"));
                XAux = BinarioADecimal(($"{xorpaso12izq[0]}{xorpaso12izq[3]}"));
                var s0result = S0[XAux, Yaux];
                Yaux = BinarioADecimal(($"{xorpaso12der[1]}{xorPaso12.Remove(0, 4)[2]}"));
                XAux = BinarioADecimal(($"{xorpaso12der[0]}{xorpaso12der[3]}"));
                var s1result = S1[XAux, Yaux];

                //14 P4 para s0 + s1
                var Pas14 = P4(s0result + s1result);

                //15 XOR resultado paso14 con bloque1 del swap(paso10) 
                var paso15 = XOR(juntosSwaped.Substring(0, 4), Pas14);

                //16 union
                var paso16 = paso15 + juntosSwaped.Remove(0, 4);

                //17 Ip inverso
                var SalidaCifrada = IPReverse(paso16);
                return SalidaCifrada;
            }
            string[] Generarkeys(int llave)
            {
                var Devolver = new string[2];

                var binarikey = Convert.ToString(llave, 2); //1010000010
                binarikey = binarikey.PadLeft(10, '0');
                var binarikeyp10 = P10(binarikey);
                var subkey1 = binarikeyp10.Substring(0, 5);
                var subkey2 = binarikeyp10.Remove(0, 5);
                var shifedsubkey1 = LeftShift1(subkey1);
                var shifedsubkey2 = LeftShift1(subkey2);
                //primera key
                Devolver[0] = P8($"{shifedsubkey1}{shifedsubkey2}");
                //segunda key
                Devolver[1] = P8($"{LeftShift1(LeftShift1(shifedsubkey1))}{LeftShift1(LeftShift1(shifedsubkey2))}");

                return Devolver;
            }
            int BinarioADecimal(string Binario) //String binario a byte
            {

                int num, ValorBinario, ValorDecimal = 0, baseVal = 1, rem;
                num = int.Parse(Binario);
                ValorBinario = num;

                while (num > 0)
                {
                    rem = num % 10;
                    ValorDecimal = ValorDecimal + rem * baseVal;
                    num = num / 10;

                    baseVal = baseVal * 2;
                }
                return Convert.ToInt32(ValorDecimal);
            }
            string XOR(string Comparador, string AComparar)
            {
                var xorResult = string.Empty;
                for (int i = 0; i < Comparador.Length; i++)
                {
                    if (AComparar[i] == Comparador[i])
                    {
                        xorResult = $"{xorResult}{0}";
                    }
                    else
                    {
                        xorResult = $"{xorResult}{1}";
                    }
                }
                return xorResult;
            }
            string Expandir(string aExpandir)
            {
                var Expandido = string.Empty;
                foreach (var index in index_Expand)
                {
                    Expandido = $"{Expandido}{aExpandir[int.Parse(index.ToString())]}";
                }
                return Expandido;
            }
            string IPReverse(string actual)
            {
                var IP8RevReturn = string.Empty;
                foreach (var index in index_IPinverse)
                {
                    IP8RevReturn = $"{IP8RevReturn}{actual[int.Parse(index.ToString())]}";
                }
                return IP8RevReturn;
            }
            string inicial(string actual)
            {
                var iniciaretl = string.Empty;
                foreach (var index in index_inicial)
                {
                    iniciaretl = $"{iniciaretl}{actual[int.Parse(index.ToString())]}";
                }
                return iniciaretl;
            }
            string P8(string actual)
            {
                var P8return = string.Empty;
                foreach (var index in index_p8)
                {
                    P8return = $"{P8return}{actual[int.Parse(index.ToString())]}";
                }
                return P8return;
            }
            string LeftShift1(string aShiftear)
            {
                var Shifted = string.Empty;
                foreach (var index in index_leftshift1)
                {
                    Shifted = $"{Shifted}{aShiftear[int.Parse(index.ToString())]}";
                }
                return Shifted;
            }
            string P10(string Entrada10bits)
            {
                var P10return = string.Empty;
                foreach (var index in index_p10)
                {
                    P10return = $"{P10return}{Entrada10bits[Convert.ToInt32(Convert.ToString(index))]}";
                }
                return P10return;
            }
            string P4(string aPermutar)
            {
                var permmuted = string.Empty;
                foreach (var index in index_p4)
                {
                    permmuted = $"{permmuted}{aPermutar[Convert.ToInt32(Convert.ToString(index))]}";
                }
                return permmuted;
            }
        }



        public string DesifradoSDES(string Contraseña)
        {
            #region VariablesGlobales
            string index_p10 = "2416390875";
            //string Index_PermutacionSeleccionada = "39860127";
            string index_p8 = "52637498";
            string index_p4 = "0321";
            string index_Expand = "13023201";

            string index_inicial = "15203746";

            string index_IPinverse = "30246175";
            string index_leftshift1 = "12340";
            string[,] S0 = new string[4, 4];
            {
                S0[0, 0] = "01";
                S0[0, 1] = "00";
                S0[0, 2] = "11";
                S0[0, 3] = "10";
                S0[1, 0] = "11";
                S0[1, 1] = "10";
                S0[1, 2] = "01";
                S0[1, 3] = "00";
                S0[2, 0] = "00";
                S0[2, 1] = "10";
                S0[2, 2] = "01";
                S0[2, 3] = "11";
                S0[3, 0] = "11";
                S0[3, 1] = "01";
                S0[3, 2] = "11";
                S0[3, 3] = "10";
            }
            string[,] S1 = new string[4, 4];
            {
                S1[0, 0] = "00";
                S1[0, 1] = "01";
                S1[0, 2] = "10";
                S1[0, 3] = "11";
                S1[1, 0] = "10";
                S1[1, 1] = "00";
                S1[1, 2] = "01";
                S1[1, 3] = "11";
                S1[2, 0] = "11";
                S1[2, 1] = "00";
                S1[2, 2] = "01";
                S1[2, 3] = "00";
                S1[3, 0] = "10";
                S1[3, 1] = "01";
                S1[3, 2] = "00";
                S1[3, 3] = "11";
            }

            #endregion
            //generacion de llaves
            var originalkey = 837;
            var KEYAR = Generarkeys(originalkey);
            var KEY1 = KEYAR[0];
            var KEY2 = KEYAR[1];
            //CIFRANDO
            var decoded = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\SpiderSenses.txt", FileMode.Open);
            var lector = new BinaryReader(decoded);

            var buffer = new byte[100000];
            var nombrearchivo = $"{Path.GetFileName(decoded.Name).Split('.')[0]}_SDES.{"txt"}";
            var encoded = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\" + nombrearchivo, FileMode.OpenOrCreate);
            var writer = new BinaryWriter(encoded);
            while (lector.BaseStream.Position != lector.BaseStream.Length)
            {
                buffer = lector.ReadBytes(100000);
                foreach (var item in buffer)
                {
                    var caracter = (char)item;
                    var bin = Convert.ToString(item, 2).PadLeft(8, '0');
                    var monitor = Convert.ToByte(BinarioADecimal(CifradoSDES(KEY1, KEY2, bin)));
                    caracter = (char)monitor;
                    writer.Write(monitor);
                }
            }
            decoded.Close();
            encoded.Close();
            //decifrado
            buffer = null;
            encoded = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\" + nombrearchivo, FileMode.Open);
            lector = new BinaryReader(encoded);
            nombrearchivo = $"{Path.GetFileName(encoded.Name).Split('.')[0]}_SDES.{"txt"}";

            decoded = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\SpiderSensesSDESUncypher.txt", FileMode.OpenOrCreate);
            writer = new BinaryWriter(decoded);
            while (lector.BaseStream.Position != lector.BaseStream.Length)
            {
                buffer = lector.ReadBytes(100000);
                foreach (var item in buffer)
                {
                    var bin = Convert.ToString(item, 2).PadLeft(8, '0');
                    var monitor = Convert.ToByte(BinarioADecimal(CifradoSDES(KEY2, KEY1, bin)));
                    writer.Write(monitor);
                }
            }
            decoded.Close();
            encoded.Close();
            return "";

            string CifradoSDES(string key1, string key2, string actual)
            {
                //1 permutar 8
                var entrada = inicial(actual);

                //2 tomar izquierda y derecha 
                var Mitadizquierda = entrada.Substring(0, 4);
                var MitadDerecha = entrada.Remove(0, 4);

                //3 expandir derecho
                var expandido = Expandir(MitadDerecha);

                //4 xor key1 y lado derecho
                var xorResultado = XOR(key1, expandido);

                //5 separar en bloques de 4
                var xor1izquierda = xorResultado.Substring(0, 4);
                var xor1derecha = xorResultado.Remove(0, 4);

                //6 s0box para xorizq y s1box para xorder
                var Yaux = BinarioADecimal(($"{xor1izquierda[1]}{xor1izquierda[2]}"));
                var XAux = BinarioADecimal(($"{xor1izquierda[0]}{xor1izquierda[3]}"));

                var BoxResultL = S0[XAux, Yaux];//izquierda
                Yaux = BinarioADecimal(($"{xor1derecha[1]}{xor1derecha[2]}"));
                XAux = BinarioADecimal(($"{xor1derecha[0]}{xor1derecha[3]}"));
                var BoxResultD = S1[XAux, Yaux];//derecha

                //7 P4 a BoxResultL 
                var paso7 = P4($"{BoxResultL}{BoxResultD}");

                //8 XOR con mitarizquierda
                var paso8 = XOR(Mitadizquierda, paso7);

                //ppaso 9 y 10
                var juntosSwaped = MitadDerecha + paso8;

                //paso 11 EP bloque 2 del paso10 
                var segundoexpandido = Expandir(juntosSwaped.Remove(0, 4));
                var monico = segundoexpandido.Length;

                ////paso12 xor de segundo expandido con key 2
                var xorPaso12 = XOR(key2, segundoexpandido);
                var xorpaso12izq = xorPaso12.Substring(0, 4);
                var xorpaso12der = xorPaso12.Remove(0, 4);

                //13 s0box para xorpaso12izq y 1 para el derecho
                Yaux = BinarioADecimal(($"{xorpaso12izq[1]}{xorpaso12izq[2]}"));
                XAux = BinarioADecimal(($"{xorpaso12izq[0]}{xorpaso12izq[3]}"));
                var s0result = S0[XAux, Yaux];
                Yaux = BinarioADecimal(($"{xorpaso12der[1]}{xorPaso12.Remove(0, 4)[2]}"));
                XAux = BinarioADecimal(($"{xorpaso12der[0]}{xorpaso12der[3]}"));
                var s1result = S1[XAux, Yaux];

                //14 P4 para s0 + s1
                var Pas14 = P4(s0result + s1result);

                //15 XOR resultado paso14 con bloque1 del swap(paso10) 
                var paso15 = XOR(juntosSwaped.Substring(0, 4), Pas14);

                //16 union
                var paso16 = paso15 + juntosSwaped.Remove(0, 4);

                //17 Ip inverso
                var SalidaCifrada = IPReverse(paso16);
                return SalidaCifrada;
            }
            string[] Generarkeys(int llave)
            {
                var Devolver = new string[2];

                var binarikey = Convert.ToString(llave, 2); //1010000010
                binarikey = binarikey.PadLeft(10, '0');
                var binarikeyp10 = P10(binarikey);
                var subkey1 = binarikeyp10.Substring(0, 5);
                var subkey2 = binarikeyp10.Remove(0, 5);
                var shifedsubkey1 = LeftShift1(subkey1);
                var shifedsubkey2 = LeftShift1(subkey2);
                //primera key
                Devolver[0] = P8($"{shifedsubkey1}{shifedsubkey2}");
                //segunda key
                Devolver[1] = P8($"{LeftShift1(LeftShift1(shifedsubkey1))}{LeftShift1(LeftShift1(shifedsubkey2))}");

                return Devolver;
            }
            int BinarioADecimal(string Binario) //String binario a byte
            {

                int num, ValorBinario, ValorDecimal = 0, baseVal = 1, rem;
                num = int.Parse(Binario);
                ValorBinario = num;

                while (num > 0)
                {
                    rem = num % 10;
                    ValorDecimal = ValorDecimal + rem * baseVal;
                    num = num / 10;

                    baseVal = baseVal * 2;
                }
                return Convert.ToInt32(ValorDecimal);
            }
            string XOR(string Comparador, string AComparar)
            {
                var xorResult = string.Empty;
                for (int i = 0; i < Comparador.Length; i++)
                {
                    if (AComparar[i] == Comparador[i])
                    {
                        xorResult = $"{xorResult}{0}";
                    }
                    else
                    {
                        xorResult = $"{xorResult}{1}";
                    }
                }
                return xorResult;
            }
            string Expandir(string aExpandir)
            {
                var Expandido = string.Empty;
                foreach (var index in index_Expand)
                {
                    Expandido = $"{Expandido}{aExpandir[int.Parse(index.ToString())]}";
                }
                return Expandido;
            }
            string IPReverse(string actual)
            {
                var IP8RevReturn = string.Empty;
                foreach (var index in index_IPinverse)
                {
                    IP8RevReturn = $"{IP8RevReturn}{actual[int.Parse(index.ToString())]}";
                }
                return IP8RevReturn;
            }
            string inicial(string actual)
            {
                var iniciaretl = string.Empty;
                foreach (var index in index_inicial)
                {
                    iniciaretl = $"{iniciaretl}{actual[int.Parse(index.ToString())]}";
                }
                return iniciaretl;
            }
            string P8(string actual)
            {
                var P8return = string.Empty;
                foreach (var index in index_p8)
                {
                    P8return = $"{P8return}{actual[int.Parse(index.ToString())]}";
                }
                return P8return;
            }
            string LeftShift1(string aShiftear)
            {
                var Shifted = string.Empty;
                foreach (var index in index_leftshift1)
                {
                    Shifted = $"{Shifted}{aShiftear[int.Parse(index.ToString())]}";
                }
                return Shifted;
            }
            string P10(string Entrada10bits)
            {
                var P10return = string.Empty;
                foreach (var index in index_p10)
                {
                    P10return = $"{P10return}{Entrada10bits[Convert.ToInt32(Convert.ToString(index))]}";
                }
                return P10return;
            }
            string P4(string aPermutar)
            {
                var permmuted = string.Empty;
                foreach (var index in index_p4)
                {
                    permmuted = $"{permmuted}{aPermutar[Convert.ToInt32(Convert.ToString(index))]}";
                }
                return permmuted;
            }
        }
    }
}
