using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;


    private List<ActionButtonUI> actionButtonUIList;


    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged +=
            UnitActionSystem_OnSelectedUnitChanged;

        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;

        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        BaseAction[] actions = selectedUnit.GetBaseActionArray();

        foreach (BaseAction baseAction in actions)
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

}
