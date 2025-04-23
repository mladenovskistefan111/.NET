using System;
using System.Collections.Generic;
using System.Linq;

namespace Async
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tea = await MakeTeaAsync();
            System.Console.WriteLine(tea);
        }

        public async Task<string> MakeTeaAsync()
        {
            var boilingWater = BoilWaterAsync();

            var water = await boilingWater;

            System.Console.WriteLine("Take cups out");

            var tea = $"Pour {water} in cups";

            return tea;
        }

        public async Task<string> BoilWaterAsync()
        {
            System.Console.WriteLine("Start the kettle");
            await Task.Delay(1000);

            return "water";
        }
    }
}