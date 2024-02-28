using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.DB;
using System.Numerics;

namespace Models
{
    public class Battle
    {
        private User playerA, playerB;
        private int roundsCompleted = 0;
        private string matchLog = "";
        private User winner = null;
        dbCommunication dbCommunication;
        private Deck deckA, deckB;

        public Battle(User playerA, User playerB, dbCommunication dbComm)
        {
            this.playerA = playerA;
            this.playerB = playerB;
            this.dbCommunication = dbComm;
        }

        public void start()
        {
            matchLog = $"\nThe battle between {playerA.username} and {playerB.username} starts.";

            int deckIdA = dbCommunication.getDeckId(playerA.username);
            int deckIdB = dbCommunication.getDeckId(playerB.username);

            this.deckA = dbCommunication.printDeck(deckIdA);
            this.deckB = dbCommunication.printDeck(deckIdB);

            while (winner == null && roundsCompleted < 100)
            {
                executeRound(playerA, playerB);
                winner = determineWinner(deckA, deckB);
            }
        }

        public void executeRound(User playerA, User playerB)
        {
            var random = new Random();

            if (deckA.isEmpty() || deckB.isEmpty())
            {
                roundsCompleted = 100;
                return;
            }

            var cardIndexA = random.Next(deckA.size());
            var cardIndexB = random.Next(deckB.size());

            Card cardA = deckA.getCard(cardIndexA);
            Card cardB = deckB.getCard(cardIndexB);

            matchLog += $"\nRound {roundsCompleted + 1}\n {playerA.username} plays: {cardA.name}";
            matchLog += $"\n {playerB.username} plays: {cardB.name}";

            matchLog += $"\n{cardA.name} deals {cardA.damage * cardA.calculateEffectiveness(cardB.cardType, cardB.element)} damage.";
            matchLog += $"\n{cardB.name} deals {cardB.damage * cardB.calculateEffectiveness(cardA.cardType, cardB.element)} damage.";

            var result = cardA.battle(cardB);
            switch (result)
            {
                case 1:
                    matchLog += $"\n {playerA.username} won the round!\n";
                    deckA.addCard(cardB);
                    deckB.removeCard(cardB);
                    roundsCompleted++;
                    break;
                case -1:
                    matchLog += $"\n {playerB.username} won the round!\n";
                    deckA.removeCard(cardA);
                    deckB.addCard(cardA);
                    roundsCompleted++;
                    break;
                default:
                    matchLog += "\nThe Round ended in a draw...\n";
                    roundsCompleted++;
                    break;
            }
        }

        public User determineWinner(Deck deckA, Deck deckB)
        {
            if (!(roundsCompleted > 0) || (deckA.isEmpty() && deckB.isEmpty()))
            {
                return null;
            }

            if (deckA.isEmpty())
            {
                dbCommunication.changeElo(playerB.id, playerA.id);
                return playerB;
            }
            else if (deckB.isEmpty())
            {
                dbCommunication.changeElo(playerA.id, playerB.id);
                return playerA;
            }
            return null;
        }

        public string Outcome()
        {
            if (winner == null)
            {
                return matchLog + "\nThe battle ended in a draw!";
            }
            else
            {
                return matchLog + $"\nThe winner is: {winner.username}";
            }
        }
    }
}
