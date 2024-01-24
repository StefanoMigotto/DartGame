using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreccetteNuovo
{
    internal class Archer
    {
        public string NameOfArcher { get; set; }
        public Society Society { get; set; }
        public override string ToString()
        {
            return NameOfArcher;
        }

        public List<Score> ArcherScore {  get; set; }
    }
}
