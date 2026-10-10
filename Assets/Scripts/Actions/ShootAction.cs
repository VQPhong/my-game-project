using Assets.Scripts.Grids;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class ShootAction : BaseAction
    {
        public event EventHandler<OnShootEventArgs> OnShoot;

        public class OnShootEventArgs : EventArgs
        {
            public Unit targetUnit;
            public Unit shootingUnit;
            public Vector3 impactPoint;
        }


        private enum State
        {
            Aiming,
            Shooting,
            Cooloff,
        }


        private State state;
        private float stateTimer;
        private Unit targetUnit;
        private bool canShootBullet;
        private const float impactPointSpread = 0.25f;


        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.Aiming:
                    Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                    float rotateSpeed = 10f;
                    transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                    break;
                case State.Shooting:
                    if (canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case State.Cooloff:
                    break;
            }

            if (stateTimer <= 0f)
            {
                NextState();
            }
        }


        private void NextState()
        {
            switch (state)
            {
                case State.Aiming:
                    state = State.Shooting;
                    float shootingStateTime = 0.1f;
                    stateTimer = shootingStateTime;
                    break;
                case State.Shooting:
                    state = State.Cooloff;
                    float coolOffStateTime = 0.5f;
                    stateTimer = coolOffStateTime;
                    break;
                case State.Cooloff:
                    ActionComplete();
                    break;
            }
        }

        private void Shoot()
        {
            Weapon weapon = unit.GetWeapon();
            WeaponSO weaponData = weapon.GetData();

            Vector3 attackPointPosition = weapon.GetAttackPoint().position;
            Vector3 impactPoint = targetUnit.GetWorldPosition();
            impactPoint.y = attackPointPosition.y;               // cùng quy tắc với đạn visual hiện tại
            impactPoint += UnityEngine.Random.insideUnitSphere * impactPointSpread;

            DamageInfo damageInfo = new DamageInfo
            {
                amount = weaponData.Damage.Roll(),
                impactForce = weaponData.ImpactForce,
                sourcePosition = weapon.GetAttackPoint().position,
                impactType = weaponData.ImpactType,
                impactPoint = impactPoint
            };

            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit,
                impactPoint = impactPoint
            });
            targetUnit.Damage(damageInfo);
        }

        public override string GetActionName()
        {
            return "Shoot";
        }

        private WeaponSO GetWeaponData()
        {
            Weapon weapon = unit.GetWeapon();
            return weapon != null ? weapon.GetData() : null;
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            WeaponSO weaponData = GetWeaponData();

            if (weaponData == null)
            {
                return validGridPositionList;
            }


            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -weaponData.Range; x <= weaponData.Range; x++)
            {
                for (int z = -weaponData.Range; z <= weaponData.Range; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > weaponData.Range)
                    {
                        continue;
                    }

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        // Grid Position is empty, no Unit
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    if (targetUnit.IsEnemy() == unit.IsEnemy())
                    {
                        // Both Units on same 'team'
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            ActionStart(onActionComplete);

            targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

            state = State.Aiming;
            float aimingStateTime = 1f;
            stateTimer = aimingStateTime;

            canShootBullet = true;
        }

    }
}
