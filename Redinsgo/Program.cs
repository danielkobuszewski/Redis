using StackExchange.Redis;
using System;
using System.Linq;
/*
O Redinsgo é basicamente um bingo com estruturas em um banco chave/valor.Você deve instalar o Redis em sua máquina ou usar um as a service.
Em seguida, irá implementar na sua linguagem preferida alguns controles usando o máximo de estruturas do Redis.
O bingo será jogado por 50 pessoas.
Você deve utilizar um HASH para armazenar as informações de cada participante. 
user:01 ->  name: “user01”, bcartela: “cartela:01”, bscore: “score:01” 
user:02 -> name: “user02”, bcartela: “cartela:02”, bscore: “score:02” 
O Hash de certa forma indica chaves para outras estruturas como as cartelas, que devem estar no Redis também.
Nesse caso, utilize sets com 15 números aleatórios cada: cartela:01 -> [10, 23, ..., 58] cartela:02 -> [3, 16, ..., 65]

Para gerar as cartelas, utilize um set com números de 1 a 99 e a função SRANDMEMBER.
Depois de gerar as cartelas para cada usuário, utilize uma estrutura de set score para controlar a pontuação de cada participante.
Com tudo preparado, crie um “jogo” com um set para ter as “pedras” e retire uma a uma.
Em seguida, verifique cada cartela e pontue no score.O primeiro jogador que somar 15 pontos, deve ser colocado como vencedor.
*/

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

                cache.HashSet(usuarioKey, new HashEntry[] { new HashEntry("Nome", usuarioKey), new HashEntry("Acertos", 0) });

                var cartelaKey = $"Cartela:{i.ToString("00")}";

                var cartela = cache.SetRandomMembers("Numeros", 15);

                cache.SetAdd(cartelaKey, cartela);

                Console.WriteLine($"{usuarioKey} => {String.Join(",", cartela)}");
            }

            Console.ReadKey();
        }
    }
}
