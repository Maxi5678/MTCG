using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Card
    {
        private int cid;
        private string name;
        private double damage;
        private string cardType;
        private string element;
        private int special;

        public Card() 
        {
            
        }

        public Card(int id, String name, double dmg, String cardType, String element, int special)
        {
            this.cid = id;
            this.name = name;
            this.damage = dmg;
            this.cardType = cardType;
            this.element = element;
            this.special = special;
        }

        public Card(int id, String Name, double dmg, String cardType, String element)
        {
            this.cid = id;
            this.name = name;
            this.damage = dmg;
            this.cardType = cardType;
            this.element = element;
            this.special = getSpecialFromName();
        }


        public Card(int id, string Name, double dmg)
        {
            this.cid = id;
            this.name = Name;
            this.damage = dmg;
            this.cardType = getTypeFromName();
            this.element = getElementFromName();
            this.special = getSpecialFromName();
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

