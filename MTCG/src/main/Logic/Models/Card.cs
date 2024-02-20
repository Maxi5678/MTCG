using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public abstract class Card
    {
        public string Name { get; private set; }
        public int Damage { get; private set; }
        public ElementType Element { get; private set; }

        protected Card(string name, int damage, ElementType element)
        {
            Name = name;
            Damage = damage;
            Element = element;
        }
    }

    public enum ElementType
    {
        Fire,
        Water,
        Normal
    }
}

