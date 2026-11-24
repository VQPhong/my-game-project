using System;
using UnityEngine;

namespace Assets.Scripts.Loadouts
{
    /// <summary>
    /// Carries the player's squad from the Loadout scene to the battle scene.
    /// </summary>
    public static class LoadoutSession
    {
        public static SquadLoadout Current { get; private set; }

        public static bool HasSquad => Current != null && !Current.IsEmpty;

        public static void Set(SquadLoadout squad)
        {
            if (squad == null)
                throw new ArgumentNullException(nameof(squad));

            Current = squad;
        }

        public static void Clear()
        {
            Current = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay()
        {
            Clear();
        }
    }
}
