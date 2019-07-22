using StackExchange.Redis;
using System;
using Newtonsoft.Json;

namespace Azure.Redis.Basic
{
    class Program
    {
        #region Properties & Attributes

        static ConnectionMultiplexer Connection { get => LazyConnection.Value; }

        static Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = "HERE COMES YOUR REDIS CONNECTION STRING";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        #endregion
        
        static void Main(string[] args)
        {
            // Retrieves the IDatabase... ¿? Well, it is the Redis client
            IDatabase cache = LazyConnection.Value.GetDatabase();

            // Talking about Redis, "PING" is the IsAlive command.
            // What do you expect to receive if I tell you "PING"? That's right!
            string cacheCommand = "PING";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

            // Let's create a dummy object and serialize it
            DummyModel dummy = new DummyModel()
            {
                Id = 1,
                Name = "Dummy",
                TimeStamp = DateTime.Now
            };
            string dummyString = JsonConvert.SerializeObject(dummy);

            // Now, let's play with Redis

            // 1. As you can see, the cache is empty
            cacheCommand = "GET DummyData";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("DummyData").ToString());

            // 2. Now, we store the new key
            cacheCommand = string.Format("SET Message \"{0}\"", dummyString);
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringSet()");
            Console.WriteLine("Cache response : " + cache.StringSet("DummyData", dummyString).ToString());

            // 3. Our object is stored, niiiice
            cacheCommand = "GET DummyData";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("DummyData").ToString());

            // 4. Now, let's clean our house
            // The following code is commented so you decide if you want to clean up EVERYTHING on your Redis instance
            /*
            cacheCommand = "FLUSHALL";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());
            */

            LazyConnection.Value.Dispose();

            Console.ReadLine();
        }
    }
}
