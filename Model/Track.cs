using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Model
{
    public class Track
    {

        public string  Name { get; set; }
        public LinkedList<Section> Sections { get; }

        public Track(string name, SectionTypes[] sections )
        {
            this.Name = name;
            Sections = ListToArray(sections);
        }


        public LinkedList<Section> ListToArray(SectionTypes[] section)
        {
            LinkedList<Section> sectie1 = new LinkedList<Section>();
            foreach (SectionTypes s in section)
            {
                Section k = new Section(s);
                sectie1.AddLast(k);
            }

            return sectie1;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
