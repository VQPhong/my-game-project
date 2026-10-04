using Assets.Scripts;
using Assets.Scripts.Actions;
using Assets.Scripts.Grids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Units
{
    public class UnitActionSystem : MonoBehaviour
    {

        public static UnitActionSystem Instance { get; private set; }


        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;


        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;

        private BaseAction selectedAction;
        private bool isBusy;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (IsUnitSelectable(selectedUnit))
            {
                SetSelectedUnit(selectedUnit);
            }
            else
            {
                selectedUnit = null;
                Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
            }
        }

        private void Update()
        {
            if (isBusy)
            {
                return;
            }

            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (TryHandleUnitSelection())
            {
                return;
            }

            if (selectedAction == null)
            {
                return;
            }

            HandleSelectedAction();
        }


        private void HandleSelectedAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }

                if (!selectedUnit.TryTakeAction(selectedAction))
                {
                    return;
                }

                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetBusy()
        {
            isBusy = true;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        private void ClearBusy()
        {
            isBusy = false;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        private bool TryHandleUnitSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                    {
                        if (unit == selectedUnit)
                        {
                            // Unit is already selected
                            return false;
                        }
                        if (unit.IsEnemy())
                        {
                            return false;
                        }

                        SetSelectedUnit(unit);
                        return true;
                    }
                }
            }

            return false;
        }

        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;

            BaseAction[] actions = unit.GetBaseActionArray();
            SetSelectedAction(actions.Length > 0 ? actions[0] : null);

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedAction(BaseAction baseAction)
        {
            selectedAction = baseAction;

            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }

        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }

        public BaseAction GetSelectedAction()
        {
            return selectedAction;
        }

        public bool IsUnitSelectable(Unit unit) => unit != null && unit.gameObject.activeInHierarchy && !unit.IsEnemy();


        private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
        {
            if (IsUnitSelectable(selectedUnit))
            {
                return;
            }

            Unit spawnedUnit = sender as Unit;

            if (!IsUnitSelectable(spawnedUnit))
            {
                return;
            }

            SetSelectedUnit(spawnedUnit);
            Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        }


    }
}
