using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class RaceTest
    {
        private Race race;
        private List<IParticipant> participants;
        private IEquipment equipment;
        private Track track;

        [SetUp]
        public void SetUp()
        {
            Track tr = new Track("Rondje2",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.RightCorner, SectionTypes.RightCorner,
                    SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner
                });

            participants = new List<IParticipant>();
            equipment = new Car(0, 0, 0, false);
            participants.Add(new Driver("Jacco", 0, new Car(10, 8, 10, false), TeamColors.Red));
            participants.Add(new Driver("Mama", 0, new Car(10, 5, 9, false), TeamColors.Yellow));

            track = tr;
            race = new Race(track, participants);
        }

        [Test]
        public void Test_ParticipantSpeed_RandomizeEquipment_IsNotNull()
        {
            race.PlaceParticipantsOnStartGrid();
            race.CalculateParticipantSpeed(race.GetSectionData(track.Sections.First?.Value).Left);
            Assert.IsNotEmpty(race.GetSectionData(track.Sections.First?.Value).Left.Equipment.Speed.ToString());
        }

        [Test]
        public void Test_GoToNextTrack()
        {
            Data.Initialize();
            Data.NextRace();
            Data.CurrentRace.GoToNextTrack();

            Assert.IsTrue(Data.CurrentRace.timer.Enabled);
        }

        [Test]
        public void Test_SetLapParticipants_IsNotNull()
        {
            race.PlaceParticipantsOnStartGrid();
            race.SetLapParticipants(race.GetSectionData(track.Sections.First?.Value).Left);
            Assert.IsNotNull(race._rounds.Keys.Count);
        }

        [Test]
        public void Test_DriverIsBroken_isTrue()
        {
            race.PlaceParticipantsOnStartGrid();
            race.IsBroken(race.GetSectionData(track.Sections.First?.Value).Left);
            race.IsBroken(race.GetSectionData(track.Sections.First?.Value).Right);
            Assert.IsFalse(race.GetSectionData(track.Sections.First?.Value).Left.Equipment.IsBroken);
            Assert.IsFalse(race.GetSectionData(track.Sections.First?.Value).Right.Equipment.IsBroken);
        }

        [Test]
        public void Test_MoveDriver_isTrue()
        {
            race.PlaceParticipantsOnStartGrid();
            List<Section> sections = track.Sections.ToList();
            Data.SectionLength = 75;

            participants[0].Equipment.Speed = 10;
            participants[0].Equipment.Performance = 9;
            participants[1].Equipment.Performance = 10;
            participants[1].Equipment.Performance = 6;

            SectionData currentSectionData = race.GetSectionData(sections[0]);
            SectionData nextSectionData = race.GetSectionData(track.Sections.Find(sections[1]).Next != null ? track.Sections.Find(sections[1]).Next.Value : track.Sections.First.Value); // eerst checkt ie of currentSection null is, vervolgens past hij de value aan.

            Assert.IsNotNull(currentSectionData.Left);
            Assert.IsNotNull(currentSectionData.Right);

            Assert.IsNull(nextSectionData.Left);
            Assert.IsNull(nextSectionData.Right);

            race.MoveDriver(currentSectionData, nextSectionData, sections[1], null);
            Assert.IsNull(currentSectionData.Left);
            Assert.IsNotNull(currentSectionData.Right);
        }

        [Test]
        public void Test_Race_StartTime_isNotNull()
        {
            race.StartTime = DateTime.Now;
            Assert.IsNotNull(race.StartTime);
        }

        [Test]
        public void Test_DriverFinished()
        {
            List<Section> sections = track.Sections.ToList();
            

            SectionData currentSectionData = race.GetSectionData(sections[0]);

            Assert.IsNotNull(currentSectionData.Left);
            Assert.IsNotNull(currentSectionData.Right);

            race._rounds[currentSectionData.Left] += Data.Rounds;

            race.DriverFinishedChecker(currentSectionData,sections[1],null);

            Assert.IsNull(currentSectionData.Left);
            Assert.IsNotNull(currentSectionData.Right);

        }
    }
}
