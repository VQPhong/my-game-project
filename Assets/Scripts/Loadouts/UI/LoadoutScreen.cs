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

        public event EventHandler OnSelectionChanged;
        public SoldierSO SelectedSoldier { get; private set; }


        private void Awake()
        {
            if (!HasValidWiring())
            {
                enabled = false;
                return;
            }

            Squad = ResolveSquad();
        }

        private void Start()
        {
            NotifySquadChanged();
            SelectSoldier(Squad.Members.Count > 0 ? Squad.Members[0].Soldier : null);
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

        private SquadLoadout ResolveSquad()
        {
            bool fromSession = LoadoutSession.HasSquad;

            SquadLoadout squad = fromSession
                                ? LoadoutSession.Current
                                : SquadLoadout.CreateDefault(roster, options);

            Debug.Log($"[LoadoutScreen] {(fromSession ? "squad from session" : "default squad")} ({squad.Members.Count})", this);
            return squad;
        }

        private void NotifySquadChanged()
        {
            UpdateSquadCountText();
            OnSquadChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateSquadCountText()
        {
            squadCountText.text = $"Squad {Squad.Members.Count}/{Squad.MaxSize}";
        }

        /// <summary>
        /// Adds the soldier if not in the squad (and selects them), removes them otherwise.
        /// Rejected adds (full / duplicate / null) change nothing and raise no event.
        /// </summary>
        public void ToggleSoldier(SoldierSO soldier)
        {
            if (Squad.Contains(soldier))
            {
                Squad.Remove(soldier);

                if (SelectedSoldier == soldier)
                    SelectSoldier(null);
            }
            else
            {
                if (!Squad.TryAdd(soldier))
                    return;

                SelectSoldier(soldier);
            }

            NotifySquadChanged();
        }

        /// <summary>
        /// Selects a squad member, or clears the selection with null.
        /// Soldiers outside the squad are ignored, so SelectedSoldier is always null or a member.
        /// </summary>
        public void SelectSoldier(SoldierSO soldier)
        {
            if (soldier != null && !Squad.Contains(soldier))
                return;

            if (soldier == SelectedSoldier)
                return;

            SelectedSoldier = soldier;
            OnSelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
