using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MonsterCard : Card
{
    public MonsterCard(string name, int damage, ElementType element)
        : base(name, damage, element)
    {
    }

    // Additional properties or methods specific to monster cards
}
