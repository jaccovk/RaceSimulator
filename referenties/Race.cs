using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Model;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Controller
{

    public class Race
    {
        public Track Track { get; set; }
        public List<IParticipant> Participants { get; set; }
        public DateTime StartTime { get; set; }
        private Random _random;
        private Dictionary<IParticipant, int> _rounds = new Dictionary<IParticipant, int>();
        public Dictionary<string, string> previousName = new Dictionary<string, string>();
        private Dictionary<Section, SectionData> _positions;
        private Timer timer;
        public int countParticipants;

        public event EventHandler<DriversChangedEventArgs> DriversChanged;
        public event EventHandler<NextRaceEventArgs> NextRace;

        public Race(Track track, List<IParticipant> participants)
        {

            this.Track = track;
            this.Participants = participants;
            _positions = new Dictionary<Section, SectionData>();
            _random = new Random(DateTime.Now.Millisecond);
            this.countParticipants = Participants.Count;
            Initialize();
        }

        public void Initialize()
        {
            RandomizeEquipment();
            PlaceParticipantsOnStartGrid();
            SetTimer();
            Start();
        }


        /*TIMER*/
        public void SetTimer()
        {
            timer = new Timer(100);
            timer.Elapsed += OnTimedEvent;
            //OnTimedEvent(null, null);
        }



        private void OnTimedEvent(Object source, ElapsedEventArgs args)
        {
            List<Section> sections = Track.Sections.ToList();
            sections.Reverse();

            foreach (Section currentSection in sections)
            {
                SectionData currentSectionData = GetSectionData(currentSection);
                SectionData nextSectionData = GetSectionData(Track.Sections.Find(currentSection).Next != null ? Track.Sections.Find(currentSection).Next.Value : Track.Sections.First.Value); // eerst checkt ie of currentSection null is, vervolgens past hij de value aan.

                MoveDriver(currentSectionData, nextSectionData, currentSection, args);
                //DriverFinishedChecker(currentSectionData, currentSection, args);// if driver passes finish, _round += 1
                
            }
                DriversChanged?.Invoke(this, new DriversChangedEventArgs()
                {
                    Track = this.Track
                });
            
        }

        public void Start()
        {
            foreach (IParticipant p in Participants)
            {
                
                    _rounds.Add(p, 0);
                
            }
            //SetLapParticipants();
            //StartTime.AddMilliseconds(timer.Interval);
            timer.Enabled = true;
        }

        /*EQUIPMENT*/
        public void RandomizeEquipment()
        {
            foreach (IParticipant p in Participants)
            {
                p.Equipment.Speed = _random.Next(4, 10);
                p.Equipment.Performance = _random.Next(3, 9);

            }
            _random.Next();
        }

        public bool IsBroken(IParticipant participant)
        {
            bool isBroken;
            if (participant.Equipment.IsBroken == false)
            {
                int broken = _random.Next(1, 50);

                if (broken == 10)
                {
                    participant.Equipment.IsBroken = true;
                    isBroken =  true;
                    if (participant.Equipment.Performance != 1)
                    {
                        participant.Equipment.Performance--;
                    }
                }
                else
                {
                    isBroken =  false;
                }
            }
            else
            {
                int unBroken = _random.Next(1, 5);
                if (unBroken == 1)
                {
                    isBroken = false;
                    participant.Equipment.IsBroken = false;
                }

                else
                {
                    isBroken = true;
                }
            }
            return isBroken;
        }

        


        /*PLACE PARTICIPANTS ON TRACK*/
        public SectionData GetSectionData(Section section)
        {
            if (_positions.ContainsKey(section))
            {
                return _positions[section];
            }
            else
            {
                _positions.Add(section, new SectionData());
                return _positions[section];
            }
        }


        public void PlaceParticipantsOnStartGrid()
        {
            List<Section> grids = Track.Sections.Where(Track => Track.SectionType == SectionTypes.StartGrid).ToList();

            int gridsCount = grids.Count;
            bool side = true;
            int participantCount = Participants.Count;
            int participantsSet = 0;
            int pointSectionNumber = 0;
            gridsCount *= 2;

            if (participantCount <= gridsCount)
            {
                participantsSet = participantCount;
            }
            else if (participantCount > gridsCount)
            {
                participantsSet = gridsCount;
            }
            for (int i = 0; i < participantsSet; i++)
            {
                if (side)
                {
                    GetSectionData(grids[pointSectionNumber]).Left = Participants[i];

                }
                else
                {
                    GetSectionData(grids[pointSectionNumber]).Right = Participants[i];
                }
                side = !side;
                if (i % 2 == 1)
                {
                    pointSectionNumber++;
                }
            }
        }



        /*MOVE PARTICIPANTS*/
        public int CalculateParticipantSpeed(IParticipant participant)
        {
            return (participant.Equipment.Speed * participant.Equipment.Performance);
        }


        public void MoveDriver(SectionData currentSection, SectionData nextSection, Section section, ElapsedEventArgs args)
        {
            int totalSectionLength = Data.SectionLength;
            //bij finish i++ voor lapround

            if (currentSection.Left != null)
            {
                if(IsBroken(currentSection.Left) == false)
                {
                    int SpeedDriver = CalculateParticipantSpeed(currentSection.Left);
                    currentSection.DistanceLeft += SpeedDriver;
                    if (currentSection.DistanceLeft >= totalSectionLength)
                    {
                        if (nextSection.Left == null)
                        {
                            currentSection.DistanceLeft = 0;
                            //currentSection.DistanceLeft -= totalSectionLength;
                            DriverFinishedChecker(currentSection, section,
                                args); // if driver passes finish, _round += 1
                            nextSection.Left = currentSection.Left;
                            currentSection.Left = null;
                        }
                        else if (nextSection.Right == null)
                        {
                            currentSection.DistanceLeft = 0;
                            //currentSection.DistanceLeft -= totalSectionLength;
                            DriverFinishedChecker(currentSection, section,
                                args); // if driver passes finish, _round += 1
                            nextSection.Right = currentSection.Left;
                            currentSection.Left = null;
                        }
                        else if (nextSection.Left != null && nextSection.Right != null)
                        {
                            currentSection.DistanceLeft -= SpeedDriver;
                        }

                    }
                }
                //DriversChanged += Visualisation.OnDriversChanged;
            }
            if (currentSection.Right != null)
            {
                if (IsBroken(currentSection.Right) == false)
                {
                    int SpeedDriver = CalculateParticipantSpeed(currentSection.Right);
                    currentSection.DistanceRight += SpeedDriver;
                    if (currentSection.DistanceRight >= totalSectionLength)
                    {
                        if (nextSection.Right == null)
                        {
                            currentSection.DistanceRight = 0;
                            //currentSection.DistanceRight -= totalSectionLength;
                            DriverFinishedChecker(currentSection, section, args); // if driver passes finish, _round += 1
                            nextSection.Right = currentSection.Right;
                            currentSection.Right = null;
                        }
                        else if (nextSection.Left == null)
                        {
                            currentSection.DistanceRight = 0;
                            //currentSection.DistanceRight -= totalSectionLength;
                            DriverFinishedChecker(currentSection, section,
                                args); // if driver passes finish, _round += 1
                            nextSection.Left = currentSection.Right;
                            currentSection.Right = null;
                        }
                        else if (nextSection.Left != null && nextSection.Right != null)
                        {
                            currentSection.DistanceRight -= SpeedDriver;
                        }

                    }
                }
            }
        }

        /*public void SetLapParticipants()
        {
            foreach (IParticipant part in Participants)
            {
                _rounds.Add(part, 0);
            }
        }*/

        public void SetLapParticipants(IParticipant part)
        {
            if (_rounds.ContainsKey(part))
            {
                _rounds[part]++;
                Trace.WriteLine($"Driver {part.Name}\t ---> rondenummer: {_rounds[part]}/{Data.Rounds}");
            }
            else
            {
                _rounds.Add(part, 0);
            }
        }

        public void DriverFinishedChecker(SectionData currentSection, Section section , ElapsedEventArgs args)
        {
            if (currentSection.Left != null)
            {
                if (section.SectionType == SectionTypes.Finish)
                {
                    if (_rounds[currentSection.Left] == Data.Rounds)
                    {
                        currentSection.Left = null;
                        countParticipants -= 1;
                        Trace.WriteLine($"Participants left: {countParticipants}");
                        if (countParticipants == 0)
                        {
                            Console.Clear();
                            GoToNextTrack();
                        }
                    }
                    else
                    {
                        SetLapParticipants(currentSection.Left);
                        //AddRound(currentSection.Left);
                    }
                }
            }

            if (currentSection.Right != null)
            {
                if (section.SectionType == SectionTypes.Finish)
                {
                    if (_rounds[currentSection.Right] == Data.Rounds)
                    {
                        currentSection.Right = null;
                        countParticipants -= 1;
                        Trace.WriteLine($"Participants left: {countParticipants}");
                        if(countParticipants == 0)
                        {
                            //Console.Clear();
                            GoToNextTrack();
                        }
                    }
                    else
                    {
                        SetLapParticipants(currentSection.Right);
                        //AddRound(currentSection.Right);
                    }
                }
            }
        }

        public void GoToNextTrack()
        {
            timer.Enabled = false;
            DriversChanged = null;
            //NextRace = null;
            countParticipants = Participants.Count;
            Data.NextRace();
            NextRace += Visualisation.OnNextRaceEvent;
            NextRace?.Invoke(this, new NextRaceEventArgs(){Race = Data.CurrentRace});
            //Data.CurrentRace.DriversChanged += Visualisation.OnDriversChanged;

        }
    }
}

