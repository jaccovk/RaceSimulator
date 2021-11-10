using System;
using System.Threading;
using Controller;

namespace ConsoleView
{
    class Program
    {
        static void Main(string[] args)
        {
            Data.Initialize();
            Data.NextRace();
            Data.CurrentRace.DriversChanged += Visualisation.OnDriversChanged;
            Data.CurrentRace.NextRaceRace += Visualisation.OnNextRace;
            //Visualisation.Initialize();
            for (;;)
            {
                Thread.Sleep(100);
            }
        }
    }
}
