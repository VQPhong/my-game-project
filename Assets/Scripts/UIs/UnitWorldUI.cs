using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIs
{

    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private HealthSystem healthSystem;
        [SerializeField] private Transform pipContainer;

        private readonly Dictionary<ResourceTypeSO, List<ResourcePipUI>> pipsByType =
            new Dictionary<ResourceTypeSO, List<ResourcePipUI>>();


        private void Start()
        {
            Unit.OnAnyResourceChanged += Unit_OnAnyResourceChanged;
            healthSystem.OnDamaged += HealthSystem_OnDamaged;

            CreatePips();
            UpdatePips();
            UpdateHealthBar();
        }

        private void Unit_OnAnyResourceChanged(object sender, EventArgs e)
        {
            UpdatePips();
        }

        private void CreatePips()
        {
            foreach (Transform child in pipContainer)
            {
                Destroy(child.gameObject);   // xóa pip dùng để xem trước bố cục
            }
            pipsByType.Clear();

            foreach (ResourceTypeSO type in unit.GetResourceTypes())
            {
                if (type.display != ResourceDisplay.WorldPips) continue;

                if (type.pipPrefab == null)
                {
                    Debug.LogWarning($"{type.name}: pipPrefab is not assigned", type);
                    continue;
                }

                List<ResourcePipUI> pips = new List<ResourcePipUI>();
                int max = unit.GetResourceMaxAmount(type);
                for (int i = 0; i < max; i++)
                {
                    pips.Add(Instantiate(type.pipPrefab, pipContainer));
                }
                pipsByType[type] = pips;
            }
        }

        private void UpdatePips()
        {
            foreach (KeyValuePair<ResourceTypeSO, List<ResourcePipUI>> pair in pipsByType)
            {
                int amount = unit.GetResourceAmount(pair.Key);
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    pair.Value[i].SetAvailable(i < amount);
                }
            }
        }

        private void UpdateHealthBar()
        {
            healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            UpdateHealthBar();
        }

        private void OnDestroy()
        {
            Unit.OnAnyResourceChanged -= Unit_OnAnyResourceChanged;
        }
    }
}