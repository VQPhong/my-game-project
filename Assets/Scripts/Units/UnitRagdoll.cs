using Assets.Scripts.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitRagdoll : MonoBehaviour
    {
        [SerializeField] private LayerMask ragdollLayerMask;

        private Animator animator;
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;


        private void Awake() {
            animator = GetComponent<Animator>();
            CollectRagdollParts();
            SetRagdollActive(false);
        }

        private void CollectRagdollParts()
        {
            List<Collider> colliders = new List<Collider>();
            List<Rigidbody> bodies = new List<Rigidbody>();

            foreach (Collider col in GetComponentsInChildren<Collider>(true))
            {
                bool isRagdollLayer = (ragdollLayerMask.value & (1 << col.gameObject.layer)) != 0;
                Rigidbody body = col.attachedRigidbody;
                if (!isRagdollLayer || body == null) continue;

                colliders.Add(col);
                if (!bodies.Contains(body)) bodies.Add(body);
            }

            ragdollColliders = colliders.ToArray();
            ragdollRigidbodies = bodies.ToArray();
        }


        public void Activate(DamageInfo damageInfo)
        {
            transform.SetParent(null);
            SetRagdollActive(true);
            switch (damageInfo.impactType)
            {
                case ImpactType.Point: ApplyPointImpact(damageInfo); break;
                case ImpactType.Radial: Debug.LogWarning("Radial impact not implemented yet"); break;
            }
            ApplyExplosionToRagdoll(transform, 0f, transform.position, 10f);
        }

        private void SetRagdollActive(bool active)
        {
            foreach (Rigidbody rigidBody in ragdollRigidbodies)
            {
                rigidBody.isKinematic = !active;
            }

            animator.enabled = !active;
        }

        [ContextMenu("Test Ragdoll")]
        private void TestRagdoll() { SetRagdollActive(true); }

        private void ApplyExplosionToRagdoll(
            Transform root,
            float explosionForce,
            Vector3 explosionPosition,
            float explosionRange)
        {
            foreach (Transform child in root)
            {
                if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                {
                    childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                }

                ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
            }
        }


        private void ApplyPointImpact(DamageInfo damageInfo)
        {
            Vector3 direction = damageInfo.GetDirection(-transform.forward);

            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = -transform.forward;
            }

            direction.Normalize();

            Rigidbody hitBody = FindClosestRigidbody(damageInfo.impactPoint);

            hitBody.AddForceAtPosition(direction * damageInfo.impactForce, damageInfo.impactPoint, ForceMode.Impulse);
        }

        private Rigidbody FindClosestRigidbody(Vector3 point)
        {
            Rigidbody closestBody = null;
            float closestSqrDistance = float.MaxValue;

            foreach (Collider col in ragdollColliders)
            {
                float sqrDistance = (col.ClosestPoint(point) - point).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestBody = col.attachedRigidbody;
                }
            }

            return closestBody;
        }
    }
}