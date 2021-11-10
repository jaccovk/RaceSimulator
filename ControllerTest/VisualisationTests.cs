using System;
using System.Collections.Generic;
using System.Text;
using ConsoleView;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class VisualisationTests
    {
        [SetUp]
        public void SetUp()
        {
            Data.Initialize();
            Data.CurrentRace.DriversChanged+= Visualisation.OnDriversChanged;
            Data.NextRace();
            Visualisation.Initialize(Data.CurrentRace);
        }

        [Test]
        public void Test_ChangeDirection()
        {
            Visualisation.ChangeDirectionToLeft();
            Assert.AreEqual(Visualisation.dir, Direction.North);
            Visualisation.ChangeDirectionToLeft();
            Assert.AreEqual(Visualisation.dir, Direction.West);

            Visualisation.dir = Direction.East;
            Visualisation.ChangeDirectionToRight();
            Assert.AreEqual(Visualisation.dir, Direction.South);
            Visualisation.ChangeDirectionToRight();
            Assert.AreEqual(Visualisation.dir, Direction.West);
            Visualisation.ChangeDirectionToRight();
            Assert.AreEqual(Visualisation.dir, Direction.North);

        }

        [Test]
        public void Test_OnNextRaceEvent_isNotNull()
        {
            Assert.IsNotNull(Data.CurrentRace);
        }

        [Test]
        public void Test_DriversChangedEventArgs_isNotNull()
        {
            var DriverChanged = new DriversChangedEventArgs()
            {
                Track = Data.CurrentRace.Track
            };
            Assert.IsNotNull(DriverChanged);
            Assert.IsNotNull(DriverChanged.Track);
        }
    }
}
