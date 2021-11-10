using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using Controller;
using System.Runtime.ExceptionServices;
using System.Runtime.Intrinsics.X86;
using System.Security;
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
        public DateTime Laptime { get; set; }
        private Random _random;
        public Dictionary<IParticipant, int> _rounds = new Dictionary<IParticipant, int>();
        private Dictionary<Section, SectionData> _positions;
        public Dictionary<IParticipant, DateTime> fastestLapTime;

        public Timer timer;
        public int countParticipants;

        public event EventHandler<DriversChangedEventArgs> DriversChanged;
        public event EventHandler NextRaceRace;

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
            fastestLapTime = new Dictionary<IParticipant, DateTime>();
            RandomizeEquipment();
            PlaceParticipantsOnStartGrid();
            SetTimer();
            Start();
        }


        /*TIMER*/
        public void SetTimer()
        {
            timer = new Timer(200);
            timer.Elapsed += OnTimedEvent;
        }



        private void OnTimedEvent(Object source, ElapsedEventArgs args)
        {
            // add sections to track
            List<Section> sections = Track.Sections.ToList();
            sections.Reverse();

            foreach (Section currentSection in sections)
            {
                SectionData currentSectionData = GetSectionData(currentSection);
                SectionData nextSectionData = GetSectionData(Track.Sections.Find(currentSection).Next != null ? Track.Sections.Find(currentSection).Next.Value : Track.Sections.First.Value); // eerst checkt ie of currentSection null is, vervolgens past hij de value aan.

                //move the driver
                MoveDriver(currentSectionData, nextSectionData, currentSection, args);

            }
            //'refresh' the view
                DriversChanged?.Invoke(this, new DriversChangedEventArgs()
                {
                    Track = this.Track
                });
            
        }

        public void Start()
        {
            foreach (IParticipant p in Participants)
            {
                //give the drivers rounds
                _rounds.Add(p, 0);
            }
 
            timer.Enabled = true;
        }

        /*EQUIPMENT*/
        public void RandomizeEquipment()
        {
            foreach (IParticipant p in Participants)
            {
                //randomize the equipment of the drivers
                p.Equipment.Speed = _random.Next(7, 10);
                p.Equipment.Performance = _random.Next(8, 9);

            }
            _random.Next();
        }

        public bool IsBroken(IParticipant participant)
        {
            bool isBroken;
            if (participant.Equipment.IsBroken == false)
            {
                int broken = _random.Next(1, 80);

                if (broken == 10)
                {
                    participant.Equipment.IsBroken = true;
                    isBroken =  true;
                    if (participant.Equipment.Performance != 3)
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
                            // if driver passes finish, _round += 1
                            DriverFinishedChecker(currentSection, section,
                                args); 
                            nextSection.Left = currentSection.Left;
                            currentSection.Left = null;
                        }
                        else if (nextSection.Right == null)
                        {
                            currentSection.DistanceLeft = 0;
                            // if driver passes finish, _round += 1
                            DriverFinishedChecker(currentSection, section,
                                args); 
                            nextSection.Right = currentSection.Left;
                            currentSection.Left = null;
                        }
                        else if (nextSection.Left != null && nextSection.Right != null)
                        {
                            //if both nextSections are taken, currentsection.driver should wait
                            currentSection.DistanceLeft -= SpeedDriver;
                        }

                    }
                }
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
                            DriverFinishedChecker(currentSection, section, args); // if driver passes finish, _round += 1
                            nextSection.Right = currentSection.Right;
                            currentSection.Right = null;
                        }
                        else if (nextSection.Left == null)
                        {
                            currentSection.DistanceRight = 0;
                            DriverFinishedChecker(currentSection, section,
                                args); // if driver passes finish, _round += 1
                            nextSection.Left = currentSection.Right;
                            currentSection.Right = null;
                        }
                        else if (nextSection.Left != null && nextSection.Right != null)
                        {
                            //if both nextSections are taken, currentsection.driver should wait
                            currentSection.DistanceRight -= SpeedDriver;
                        }

                    }
                }
            }
        }
        public void SetLapParticipants(IParticipant part)
        {
            if (_rounds.ContainsKey(part))
            {
                //add a round to the participant
                _rounds[part]++;
                Trace.WriteLine($"Driver {part.Name}\t ---> rondenummer: {_rounds[part]}/{Data.Rounds}");
            }
            else
            {
                _rounds.Add(part, 0);
            }
        }

        public void SetLapTimeDriver(IParticipant part)
        {

            if (!fastestLapTime.ContainsKey(part))
            {
                StartTime = DateTime.Now;
                fastestLapTime.Add(part, StartTime);
            }
            else
            {
                Laptime = DateTime.Now;
                Laptime -= StartTime.TimeOfDay;
                if((Laptime - fastestLapTime[part].TimeOfDay)<fastestLapTime[part])
                {
                    fastestLapTime.Remove(part);
                    fastestLapTime.Add(part, Laptime);
                }
            }
        }

        public void DriverFinishedChecker(SectionData currentSection, Section section, ElapsedEventArgs args)
        {
            if (currentSection.Left != null)
            {
                if (section.SectionType == SectionTypes.Finish)
                {
                    //sets the time to a driver
                    SetLapTimeDriver(currentSection.Left);
                    //if laptime is faster than the previous laptime, laptime is added to the driver
                    currentSection.Left.Equipment.LapTimeDriver = fastestLapTime[currentSection.Left];
                    Trace.WriteLine($"Driver {currentSection.Left.Name}\t ---> laptime: {fastestLapTime[currentSection.Left]}");
                    
                    if (_rounds[currentSection.Left] >= Data.Rounds)
                    {
                        //if driver finished all rounds of the track, points are given to drivers.
                        CalculateDriverPoints(currentSection.Left, countParticipants);
                        currentSection.Left = null;
                        countParticipants -= 1;
                        Trace.WriteLine($"Participants left: {countParticipants}");
                        if (countParticipants == 0)
                        {
                            //if all drivers are finished, next track should be started.
                            GoToNextTrack();
                        }
                    }
                    else
                    {
                        SetLapParticipants(currentSection.Left);
                    }
                }
            }
        
        if (currentSection.Right != null)
            {
                if (section.SectionType == SectionTypes.Finish)
                {
                    SetLapTimeDriver(currentSection.Right);
                    currentSection.Right.Equipment.LapTimeDriver = fastestLapTime[currentSection.Right];
                    Trace.WriteLine($"Driver {currentSection.Right.Name}\t ---> laptime: {fastestLapTime[currentSection.Right]}");
                    if (_rounds[currentSection.Right] >= Data.Rounds)
                    {
                        CalculateDriverPoints(currentSection.Right, countParticipants);
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
                    }
                }
            }
        }

        public void CalculateDriverPoints(IParticipant part, int countPart)
        {
            if (countPart % Participants.Count == 0)
                part.Points += Data.FirstPlace;
            if (countPart % Participants.Count-1 == 0)
                part.Points += Data.SecondPlace;
            if (countPart % Participants.Count-2 == 0)
                part.Points += Data.ThirdPlace;
            //if the participants fastest lap is less fast than the current laptime, LapTimeDriver is being updated.
            if(part.Equipment.LapTimeDriver == null || part.Equipment.LapTimeDriver > fastestLapTime[part])
            {
                part.Equipment.LapTimeDriver = fastestLapTime[part];
            }
        }


        public void GoToNextTrack()
        {
            //clear the past track and start the new race.
            timer.Enabled = false;
            DriversChanged = null;
            countParticipants = Participants.Count;

            if (Data.CurrentRace.NextRaceRace != null)
            { Data.CurrentRace.NextRaceRace.Invoke(this, new EventArgs()); }
            else Console.WriteLine("Race finished");

            Data.NextRace();
        }
    }
}

