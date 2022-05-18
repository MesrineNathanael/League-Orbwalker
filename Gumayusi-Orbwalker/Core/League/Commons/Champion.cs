using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public class Champion
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri PictureUri { get; set; }

        public int Windup { get; set; }

    }
}
