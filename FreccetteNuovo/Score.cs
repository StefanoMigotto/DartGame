using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreccetteNuovo
{
    internal class Score
    {   
        public int Arrow1 { get; set; }
        public int Arrow2 { get; set; }
        public int Arrow3 { get; set; }
        public int PartialScore { get; set; }
        public bool Ten {  get; set; }
        public bool X {  get; set; }

        public int CalculatePartial()
        {
            return Arrow1 + Arrow2 + Arrow3;
        }

    }
}
