using System;
using System.Collections.Generic;
using System.Text;
using Controller;

namespace Model
{
    public class NextRaceEventArgs: EventArgs
    {
        public Race Race { get; set; }

    }
}
