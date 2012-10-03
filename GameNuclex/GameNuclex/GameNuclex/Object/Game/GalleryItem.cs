using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameNuclex.Object.Game
{
    class GalleryItem
    {
        public string Name;
        public string Origin;
        public string Description;

        public GalleryItem(string name, string origin, string description)
        {
            this.Name = name;
            this.Origin = origin;
            this.Description = description;
        }
    }
}
