using Assets.Scripts.Actions;
using Assets.Scripts.RetailObjects;
using System;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitAnimator : MonoBehaviour
    {

        [SerializeField] private Animator animator;


        private void Awake()
        {
            if (TryGetComponent(out MoveAction moveAction))
            {
                moveAction.OnStartMoving += MoveAction_OnStartMoving;
                moveAction.OnStopMoving += MoveAction_OnStopMoving;
            }

            if (TryGetComponent(out ShootAction shootAction))
            {
                shootAction.OnShoot += ShootAction_OnShoot;
            }
        }

        private void MoveAction_OnStartMoving(object sender, EventArgs e)
        {
            animator.SetBool("IsWalking", true);
        }

        private void MoveAction_OnStopMoving(object sender, EventArgs e)
        {
            animator.SetBool("IsWalking", false);
        }

        private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            Weapon weapon = e.shootingUnit.GetWeapon();
            Transform projectilePrefab = weapon.GetData().ProjectilePrefab;
            Vector3 attackPointPosition = weapon.GetAttackPoint().position;

            animator.SetTrigger("Shoot");

            if (projectilePrefab == null)
            {
                return;
            }

            Transform bulletProjectileTransform =
                Instantiate(projectilePrefab, attackPointPosition, Quaternion.identity);

            BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

            bulletProjectile.Setup(e.impactPoint);

        }

    }

}
