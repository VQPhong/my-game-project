using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Actions;
using Assets.Scripts.Grids;
using Assets.Scripts.Squads;
using Assets.Scripts.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class Unit : MonoBehaviour
    {
        public static event EventHandler OnAnyResourceChanged;
        public static event EventHandler OnAnyUnitSpawned;


        [SerializeField] private List<ResourceAllotment> unitResourceAllotments;
        [SerializeField] private bool isEnemy;
        [SerializeField] private UnitRagdoll unitRagdoll;
        [SerializeField] private WeaponSO weaponData;
        [SerializeField] private Transform weaponSocket;


        private List<ResourceTypeSO> resourceTypes;
        private ResourcePool resourcePool = new ResourcePool();
        private GridPosition gridPosition;
        private HealthSystem healthSystem;
        private MoveAction moveAction;
        private SpinAction spinAction;
        private BaseAction[] baseActionArray;
        private DamageInfo lastDamageInfo;
        private Weapon weapon;


        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();

            EquipWeapon(weaponData);

            RefreshActionReferences();

            InitializeResources(unitResourceAllotments);
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
            if (moveAction != null)
            {
                moveAction.ResetTargetPosition();
            }
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

            healthSystem.OnDead += HealthSystem_OnDead;
        }

        private void Update()
        {
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                // Unit changed Grid Position
                LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }


        public void Configure(UnitConfigSO config)
        {
            InitializeResources(config.resourceAllotments);

            foreach (ActionConfig actionConfig in config.actions)
            {
                BaseAction action = ActionFactory.AddAction(gameObject, actionConfig.actionType);
                action.SetActionCosts(actionConfig.actionCosts);
                action.SetOrder(actionConfig.order);
            }

            RefreshActionReferences();
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public BaseAction[] GetBaseActionArray()
        {
            return baseActionArray;
        }

        private ResourcePool GetPoolForCost(ActionCost cost)
        {
            return cost.resourceType.isShared ? TeamResourceSystem.Instance.ResourcePool : resourcePool;
        }

        public bool CanAfford(BaseAction action)
        {
            foreach (ActionCost cost in action.GetActionCosts())
            {
                if (GetPoolForCost(cost).GetAmount(cost.resourceType) < cost.amount) return false;
            }
            return true;
        }

        public bool TryTakeAction(BaseAction action)
        {
            if (!CanAfford(action)) return false;

            foreach (ActionCost cost in action.GetActionCosts())
            {
                GetPoolForCost(cost).Spend(cost.resourceType, cost.amount);
            }

            OnAnyResourceChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public int GetResourceAmount(ResourceTypeSO type) => resourcePool.GetAmount(type);

        public List<ResourceTypeSO> GetResourceTypes() => resourceTypes;

        public Weapon GetWeapon() => weapon;

        private void InitializeResources(List<ResourceAllotment> allotments)
        {
            resourceTypes = allotments.ConvertAll(a => a.resourceType);
            resourcePool.Initialize(allotments);
        }

        private void RefreshActionReferences()
        {
            baseActionArray = GetComponents<BaseAction>();
            Array.Sort(baseActionArray, (a, b) => a.GetOrder().CompareTo(b.GetOrder()));

            moveAction = GetComponent<MoveAction>();
            spinAction = GetComponent<SpinAction>();
        }

        public bool IsEnemy()
        {
            return isEnemy;
        }


        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
            {
                resourcePool.ApplyRegen();

                OnAnyResourceChanged?.Invoke(this, EventArgs.Empty);
            }                
        }

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }

        public void Damage(DamageInfo damageInfo)
        {
            lastDamageInfo = damageInfo;
            healthSystem.Damage(damageInfo.amount);
        }

        private void HealthSystem_OnDead(object sender, EventArgs e)
        {
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

            unitRagdoll.Activate(lastDamageInfo.sourcePosition, lastDamageInfo.impactForce);

            Destroy(gameObject);
        }

        private void EquipWeapon(WeaponSO data)
        {
            if (weapon != null) Destroy(weapon.gameObject);

            weaponData = data;
            if (data == null)
            {
                return;
            }

            if (weaponSocket == null)
            {
                Debug.LogWarning($"{name}: weaponSocket is not assigned", this);
                return;
            }

            if (data.WeaponPrefab == null)
            {
                Debug.LogWarning($"{name}: WeaponPrefab is not assigned", this);
                return;
            }

            weapon = Instantiate(data.WeaponPrefab, weaponSocket);
        }



    }
}
