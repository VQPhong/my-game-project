using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon/Weapon Data")]
    public class WeaponSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name for UI")]
        [SerializeField] private string weaponName;

        [Tooltip("Prefab spawned into the unit's weapon socket. Grip at origin, barrel along +Z")]
        [SerializeField] private Weapon weaponPrefab;

        [Header("Combat")]
        [Tooltip("Damage rolled per hit, min and max inclusive")]
        [SerializeField] private IntRange damage = new IntRange { min = 1, max = 1 };

        [Tooltip("Max attack distance, in grid cells")]
        [Min(1)]
        [SerializeField] private int range = 7;

        [Tooltip("Hit chance, 0-1. Not used yet")]
        [Range(0f, 1f)]
        [SerializeField] private float baseAccuracy = 0.75f;

        [Header("Impact")]
        [SerializeField] private ImpactType impactType = ImpactType.Point;

        [Tooltip("Impulse applied to the target's ragdoll on death")]
        [Min(0f)]
        [SerializeField] private float impactForce = 5f;

        [Header("Visuals")]
        [Tooltip("Projectile visual. Just leave empty for non-projectile weapons")]
        [SerializeField] private Transform projectilePrefab;

        public string WeaponName => weaponName;
        public Weapon WeaponPrefab => weaponPrefab;
        public IntRange Damage => damage;
        public int Range => range;
        public float BaseAccuracy => baseAccuracy;
        public ImpactType ImpactType => impactType;
        public float ImpactForce => impactForce;
        public Transform ProjectilePrefab => projectilePrefab;

        private void OnValidate()
        {
            if (damage.min < 0) damage.min = 0;

            if (!damage.IsValid())
                Debug.LogWarning($"{name}: damage min ({damage.min}) > max ({damage.max})", this);

            if (weaponPrefab == null)
                Debug.LogWarning($"{name}: weaponPrefab is not assigned", this);
        }
    }

    public enum ImpactType 
    { 
        Point,          // For bullet, melee: force at one point, one direction
        Radial          // For AoE explosion: force radiates from a center
    }
}
