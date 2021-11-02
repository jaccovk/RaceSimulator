using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Model;
using Controller;
using NUnit.Framework.Constraints;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrackShould
    {


        
        //3.1 TEST
        [Test]
        public void NextTrack_EmptyQueue_ReturnNull()
        {
            Competition competition = new Competition();
            Track result = competition.NextTrack();
            Assert.IsNull(result);
        }
        [Test]
        public void NextTrack_OneInQueue_ReturnTrack()
        {
            Track track = new Track("Spa_Section1",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.RightCorner,
                    SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Finish
                });
            Competition competition = new Competition();
            competition.Tracks.Enqueue(track);
            Track result = competition.NextTrack();
            Assert.AreEqual(track, result);

        }

        [Test]
        public void NextTrack_OneInQueue_RemoveTrackFromQueue()
        {
            Track track = new Track("Spa_Section1",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.RightCorner,
                    SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Finish
                });
            Competition competition = new Competition();
            Track result = competition.NextTrack();
            result = competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_TwoInQueue_ReturnNextTrack()
        {
            Competition competition = new Competition();
            Track track = new Track("Spa_Section1",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.RightCorner,
                    SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Finish
                });
            Track track2 = new Track("Spa_Section2",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.RightCorner,
                    SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Finish
                });

            competition.Tracks.Enqueue(track);
            competition.Tracks.Enqueue(track2);
            
            var result = competition.NextTrack();
            Assert.AreEqual(result, track);

        }


        [SetUp]
        public void SetUp() 
        {
        
            Competition competition;
                competition = new Competition();
        }
    }
}
