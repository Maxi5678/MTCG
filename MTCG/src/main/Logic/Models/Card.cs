using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Card
    {
        public string cid;
        public string name;
        public double damage;
        public string cardType;
        public string element;
        public int special;

        public Card() 
        {
            
        }

        public Card(string id, String name, double dmg, String cardType, String element, int special)
        {
            this.cid = id;
            this.name = name;
            this.damage = dmg;
            this.cardType = cardType;
            this.element = element;
            this.special = special;
        }

        public Card(string id, String name, double dmg, String cardType, String element)
        {
            this.cid = id;
            this.name = name;
            this.damage = dmg;
            this.cardType = cardType;
            this.element = element;
        }


        public Card(string id, string Name, double dmg)
        {
            this.cid = id;
            this.name = Name;
            this.damage = dmg;
            this.cardType = getTypeFromName();
            this.element = getElementFromName();
        }

        public string getTypeFromName()
        {
            string typ = "monster";
            string copyName = name.ToLower();
            if (copyName.Contains("spell"))
            {
                typ = "spell";
            }
            return typ;
        }

        public string getElementFromName()
        {
            string element = "regular";
            string copyName = name.ToLower();
            if (copyName.Contains("fire"))
            {
                element = "fire";
            }
            else if (copyName.Contains("water"))
            {
                element = "water";
            }
            return element;
        }

        public int getSpecialFromName()
        {
            int special = 0;
            string copyName = name.ToLower();
            if (copyName.Contains("spell"))
            {
                if (copyName.Contains("water"))
                {
                    special = 2;
                }
                special = 1;
            }
            else if (copyName.Contains("knight"))
            {
                special = 3;
            }
            else if (copyName.Contains("dragon"))
            {
                special = 4;
            }
            else if (copyName.Contains("fireelf"))
            {
                special = 5;
            }
            else if (copyName.Contains("kraken"))
            {
                special = 6;
            }
            else if (copyName.Contains("wizzard"))
            {
                special = 7;
            }
            else if (copyName.Contains("ork"))
            {
                special = 8;
            }
            return special;
        }
    }

}

