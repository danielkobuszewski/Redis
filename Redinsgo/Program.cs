using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redinsgo
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            for (var i = 1; i <= 99; i++)
            {
                cache.SetAdd("Numeros", i);
            }

            for (var i = 1; i <= 50; i++)
            {
                var usuarioKey = $"Usuario:{i.ToString("00")}";
                var cartelaKey = $"Cartela:{i.ToString("00")}";

                cache.KeyDelete(usuarioKey);
                cache.KeyDelete(cartelaKey);

                cache.HashSet(usuarioKey, new HashEntry[] { new HashEntry("Nome", usuarioKey), new HashEntry("Acertos", 0) });

                var cartela = cache.SetRandomMembers("Numeros", 15);

                cache.SetAdd(cartelaKey, cartela);
            }

            var lstUsuariosGanhadores = new List<Int32>();
            var lstNumerosSorteados = new List<Int32>();

            while (!lstUsuariosGanhadores.Any())
            {
                var numeroSorteado = cache.SetPop("Numeros");

                lstNumerosSorteados.Add((Int32)numeroSorteado);

                for (var i = 1; i <= 50; i++)
                {
                    var usuarioKey = $"Usuario:{i.ToString("00")}";
                    var cartelaKey = $"Cartela:{i.ToString("00")}";

                    if (cache.SetContains(cartelaKey, numeroSorteado))
                    {
                        var acertos = cache.HashIncrement(usuarioKey, "Acertos", 1);

                        if (acertos == 15)
                            lstUsuariosGanhadores.Add(i);
                    }
                }
            }

            Console.WriteLine($"Quantidade de números sorteados: {lstNumerosSorteados.Count()}");
            Console.WriteLine($"Números sorteados: {String.Join(",", lstNumerosSorteados)}");

            {
                var i = lstUsuariosGanhadores.First();
                var usuarioKey = $"Usuario:{i.ToString("00")}";
                var cartelaKey = $"Cartela:{i.ToString("00")}";

                Console.WriteLine($"Usuário vencedor: {usuarioKey} => Números da cartela: {String.Join(",", cache.SetMembers(cartelaKey))}");
            }

            Console.ReadKey();
        }
    }
}
