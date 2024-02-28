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
        private static Random random = new Random();

        public Card() 
        {
            
        }

        public Card(string id)
        {
            this.cid = id;
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

        public int battle(Card opposingCard)
        {
            double cardDamage, opposingCardDamage;

            cardDamage = this.damage * calculateEffectiveness(opposingCard.cardType, opposingCard.element);
            opposingCardDamage = opposingCard.damage * calculateEffectiveness(this.cardType, this.element);

            if(cardDamage > opposingCardDamage) 
            {
                return 1;
            }
            else if(cardDamage < opposingCardDamage)
            {
                return -1;
            }
            else 
            { 
                return 0; 
            }
        }

        public double calculateEffectiveness(string opposingCardType, string opposingCardElement)
        {

            bool isUnlucky = random.Next(200) == 0;
            bool isLucky = random.Next(200) == 0;

            if (isUnlucky)
            {
                return 0; 
            }
            else if (isLucky)
            {
                return 10;
            }
            else if (this.cardType == "monster" && opposingCardType == "monster")
            {
                return 1;
            }
            else
            {
                if (this.element == "water" && opposingCardElement == "fire")
                {
                    return 2;
                }
                else if (this.element == "water" && opposingCardElement == "normal")
                {
                    return 0.5;
                }
                else if (this.element == "fire" && opposingCardElement == "normal")
                {
                    return 2;
                }
                else if (this.element == "fire" && opposingCardElement == "water")
                {
                    return 0.5;
                }
                else if (this.element == "normal" && opposingCardElement == "water")
                {
                    return 2;
                }
                else if (this.element == "normal" && opposingCardElement == "fire")
                {
                    return 0.5;
                }
                else
                {
                    return 1;
                }
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            Card other = (Card)obj;
            return this.cid == other.cid;
        }

        public override int GetHashCode()
        {
            return cid.GetHashCode();
        }
    }

}

