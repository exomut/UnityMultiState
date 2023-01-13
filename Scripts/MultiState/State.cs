using System;

namespace EXOMUT.MultiState
{
    public class State
    {
        public readonly string Name;

        public State(string name) => Name = name;

        internal Action OnStateEnter { get; set; }

        internal Action OnStateExit { get; set; }

        internal Action OnStateUpdate { get; set; }

        public override string ToString() => Name;
    }
}