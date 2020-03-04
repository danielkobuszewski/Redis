using System;

namespace Redinsgo
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            cache.StringSet("Key", "OK");

            Console.WriteLine(cache.StringGet("Key"));

            Console.ReadLine();
        }
    }
}
