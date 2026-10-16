using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Units;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIs
{

    public class UnitWorldUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Unit unit;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private HealthSystem healthSystem;

        private void Start()
        {
            Unit.OnAnyResourceChanged += Unit_OnAnyActionPointsChanged;
            healthSystem.OnDamaged += HealthSystem_OnDamaged;

            UpdateActionPointsText();
            UpdateHealthBar();
        }

        private void UpdateActionPointsText()
        {
            string text = "";

            foreach (ResourceTypeSO type in unit.GetResourceTypes())
            {
                text += /*type.resourceName + ": " +*/ unit.GetResourceAmount(type) + " | ";
            }
            actionPointsText.text = text;
        }

        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPointsText();
        }

        private void UpdateHealthBar()
        {
            healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            UpdateHealthBar();
        }

    }
}