using System;
using System.Collections.Generic;
using System.Text;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest.Data_Test
{
    [TestFixture]
    public class DataTest
    {
        [SetUp]
        public void SetUp()
        {

        }


        [Test]
        public void Test_Competition_IsNotNull()
        {
            Data.Initialize();
            var output = Data.competition.Participants;

            Assert.IsNotEmpty(output);
        }

        [Test]
        public void Test_NextRace_ShouldStart()
        {
            Data.Initialize();
            Data.NextRace();

            Assert.IsNotEmpty(Data.CurrentRace.Participants);
            Assert.IsNotEmpty(Data.CurrentRace.Track.Name);
            Assert.IsNotNull(new NextRaceEventArgs() {Race = Data.CurrentRace});
        }

        [Test]
        public void Test_CurrentRace_IsNotNull()
        {
            Data.Initialize();
            Data.NextRace();

            Assert.IsNotNull(Data.CurrentRace);
        }
    }
}
