using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpellCard : Card
{
    public SpellCard(string name, int damage, ElementType element)
        : base(name, damage, element)
    {
    }

    // Additional properties or methods specific to spell cards
}
