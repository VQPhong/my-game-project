using Assets.Scripts.Soldiers;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Loadouts.UI
{
    /// <summary>
    /// Loadout scene controller: owns the squad being edited.
    /// Every change to the squad goes through here so views can redraw on OnSquadChanged.
    /// </summary>
    public class LoadoutScreen : MonoBehaviour
    {
        public event EventHandler OnSquadChanged;

        [SerializeField] private RosterSO roster;
        [SerializeField] private LoadoutOptionsSO options;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI squadCountText;

        public RosterSO Roster => roster;
        public SquadLoadout Squad { get; private set; }


        // Squad is ready in Awake, so views can read it from their own Awake / OnEnable
        private void Awake()
        {
            if (!HasValidWiring())
            {
                // Disabled before Start -> Start never runs, no event with a null squad
                enabled = false;
                return;
            }

            Squad = ResolveSquad();
        }

        // First redraw in Start: every view has subscribed by now (Awake / OnEnable run before any Start)
        private void Start()
        {
            NotifySquadChanged();
        }

        private bool HasValidWiring()
        {
            bool isValid = true;

            if (roster == null || options == null)
            {
                Debug.LogError($"{name}: assign roster and options in the Inspector", this);
                isValid = false;
            }

            if (squadCountText == null)
            {
                Debug.LogError($"{name}: assign squadCountText in the Inspector", this);
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Same source order as BattleSetup: coming back from a battle shows the squad just used.
        /// Nothing is written to the session here, only Deploy commits the squad.
        /// </summary>
        private SquadLoadout ResolveSquad()
        {
            bool fromSession = LoadoutSession.HasSquad;

            SquadLoadout squad = fromSession
                                ? LoadoutSession.Current
                                : SquadLoadout.CreateDefault(roster, options);

            Debug.Log($"[LoadoutScreen] {(fromSession ? "squad from session" : "default squad")} ({squad.Members.Count})", this);
            return squad;
        }

        /// <summary>
        /// Call after every successful change to the squad (4.2 / 4.3 add the change methods).
        /// </summary>
        private void NotifySquadChanged()
        {
            UpdateSquadCountText();
            OnSquadChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateSquadCountText()
        {
            squadCountText.text = $"Squad {Squad.Members.Count}/{Squad.MaxSize}";
        }
    }
}
