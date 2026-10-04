using Assets.Scripts.Weapons;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponSO data;
    [SerializeField] Transform attackPoint;


    public WeaponSO GetData() => data;

    public Transform GetAttackPoint() => attackPoint;
}
