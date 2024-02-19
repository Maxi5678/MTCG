using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Package
    {
        public List<Card> Cards { get; private set; }

        public Package(List<Card> cards)
        {
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
        }

        // You might add methods to generate random cards, etc.
    }
}
