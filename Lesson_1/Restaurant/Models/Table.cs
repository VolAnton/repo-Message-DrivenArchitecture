using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Table
    {
        public State State { get; private set; }

        public int SeatsCount { get; }

        public int Id { get; }

        public Table(int id)
        {
            Id = id;
            State = State.Free;            
            SeatsCount = new Random().Next(2, 5);
        }

        public bool SetState(State state)
        {
            if (state == State)
            {
                return false;
            }

            State = state;
            return true;
        }

    }
}
