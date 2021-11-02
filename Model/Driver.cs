using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Driver : IParticipant
    {


        public string Name { get; set; }
        public int Points {  get; set; } 
        public IEquipment Equipment { get; set; } 
        public TeamColors TeamColor {  get; set; }

        public Driver(String Name, int Points, IEquipment Equipment, TeamColors Teamcolor)
        {
            this.Name = Name;
            this.Points = Points;
            this.Equipment = Equipment;
            this.TeamColor = Teamcolor;
        }
    }
}
