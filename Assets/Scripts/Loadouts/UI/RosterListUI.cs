using Assets.Scripts.Soldiers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Loadouts.UI
{
    /// <summary>
    /// Builds one slot per roster soldier and redraws them when the screen reports a change.
    /// </summary>
    public class RosterListUI : MonoBehaviour
    {
        [SerializeField] private LoadoutScreen screen;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private RosterSlotUI slotPrefab;

        private readonly List<RosterSlotUI> slots = new List<RosterSlotUI>();


        private void Awake()
        {
            if (!HasValidWiring())
            {
                enabled = false;
                return;
            }

            foreach (Transform child in slotContainer)
                Destroy(child.gameObject);

            foreach (SoldierSO soldier in screen.Roster.Soldiers)
            {
                if (soldier == null)
                    continue;

                RosterSlotUI slot = Instantiate(slotPrefab, slotContainer);
                slot.Setup(soldier, screen.SelectSoldier, screen.ToggleSoldier);
                slots.Add(slot);
            }

            screen.OnSquadChanged += LoadoutScreen_OnSquadChanged;
            screen.OnSelectionChanged += LoadoutScreen_OnSelectionChanged;
        }

        private void OnDestroy()
        {
            if (screen == null)
                return;

            screen.OnSquadChanged -= LoadoutScreen_OnSquadChanged;
            screen.OnSelectionChanged -= LoadoutScreen_OnSelectionChanged;
        }

        private bool HasValidWiring()
        {
            bool isValid = true;

            if (screen == null)
            {
                Debug.LogError($"{name}: assign screen in the Inspector", this);
                isValid = false;
            }

            if (slotContainer == null)
            {
                Debug.LogError($"{name}: no slotContainer", this);
                isValid = false;
            }

            if (slotPrefab == null)
            {
                Debug.LogError($"{name}: no slotPrefab", this);
                isValid = false;
            }

            return isValid;
        }

        private void LoadoutScreen_OnSquadChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void LoadoutScreen_OnSelectionChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// The only place that works out each slot's state.
        /// </summary>
        private void Refresh()
        {
            bool canAdd = !screen.Squad.IsFull;

            foreach (RosterSlotUI slot in slots)
            {
                bool inSquad = screen.Squad.Contains(slot.Soldier);
                bool isSelected = slot.Soldier == screen.SelectedSoldier;

                slot.Refresh(inSquad, canAdd, isSelected);
            }
        }
    }
}
