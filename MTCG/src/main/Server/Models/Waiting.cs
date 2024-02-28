using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Models;

namespace MTCG.Server.Models
{
    public class Waiting
    {
        private User waitingUser = null;
        private bool waiting = false;

        private Battle newBattle;

        public Waiting() { }

        public User getCurrentWaitingUser()
        {
            return this.waitingUser;
        }

        public void assignWaitingUser(User userToWait)
        {
            this.waitingUser = userToWait;
        }

        public bool isUserWaiting()
        {
            return this.waiting;
        }

        public void updateWaitingStatus(bool isWaiting)
        {
            this.waiting = isWaiting;
        }

        public Battle getCurrentBattle()
        {
            return this.newBattle;
        }

        public void initializeBattle(Battle newbattle)
        {
            this.newBattle = newbattle;
        }
    }
}
