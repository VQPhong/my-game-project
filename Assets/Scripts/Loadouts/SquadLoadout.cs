using Assets.Scripts.Soldiers;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Loadouts
{
    public class SquadLoadout
    {
        public LoadoutOptionsSO Options { get; }
        public IReadOnlyList<SoldierLoadout> Members => members;
        public int MaxSize => Options.MaxSquadSize;
        public bool IsFull => members.Count >= MaxSize;
        public bool IsEmpty => members.Count == 0;

        private readonly List<SoldierLoadout> members = new List<SoldierLoadout>();

        public SquadLoadout(LoadoutOptionsSO options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Options = options;
        }

        public bool Contains(SoldierSO soldier) => TryGet(soldier, out _);

        public bool TryGet(SoldierSO soldier, out SoldierLoadout loadout)
        {
            foreach (SoldierLoadout member in members)
            {
                if (member.Soldier == soldier)
                {
                    loadout = member;
                    return true;
                }
            }

            loadout = null;
            return false;
        }

        public bool TryAdd(SoldierSO soldier)
        {
            if (soldier == null)
                return false;

            if (IsFull)
                return false;

            if (Contains(soldier))
                return false;

            members.Add(new SoldierLoadout(soldier, Options));
            return true;
        }

        public bool Remove(SoldierSO soldier)
        {
            if (!TryGet(soldier, out SoldierLoadout loadout))
                return false;

            return members.Remove(loadout);
        }

        public static SquadLoadout CreateDefault(RosterSO roster, LoadoutOptionsSO options)
        {
            if (roster == null) throw new ArgumentNullException(nameof(roster));

            SquadLoadout squad = new SquadLoadout(options);

            foreach (SoldierSO soldier in roster.Soldiers)
            {
                if (squad.IsFull) break;

                squad.TryAdd(soldier);
            }

            return squad;
        }

    }

}
