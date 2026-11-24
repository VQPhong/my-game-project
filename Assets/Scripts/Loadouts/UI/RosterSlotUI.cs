using Assets.Scripts.Soldiers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Loadouts.UI
{
    /// <summary>
    /// One roster row. Shows a soldier and reports clicks; knows nothing about the squad rules.
    /// </summary>
    public class RosterSlotUI : MonoBehaviour
    {
        [SerializeField] private Button selectButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button toggleButton;
        [SerializeField] private TextMeshProUGUI toggleText;
        [SerializeField] private GameObject selectedVisual;

        public SoldierSO Soldier { get; private set; }


        /// <summary>
        /// Call once right after Instantiate.
        /// </summary>
        public void Setup(SoldierSO soldier, Action<SoldierSO> onSelect, Action<SoldierSO> onToggle)
        {
            Soldier = soldier;
            nameText.text = soldier.DisplayName;

            selectButton.onClick.AddListener(() => onSelect(Soldier));
            toggleButton.onClick.AddListener(() => onToggle(Soldier));
        }

        /// <summary>
        /// Display only: the list decides the values, the slot just shows them.
        /// </summary>
        public void Refresh(bool inSquad, bool canAdd, bool isSelected)
        {
            toggleText.text = inSquad ? "REMOVE" : "ADD";
            toggleButton.interactable = inSquad || canAdd;
            selectButton.interactable = inSquad;

            selectedVisual.SetActive(isSelected);
        }
    }
}
