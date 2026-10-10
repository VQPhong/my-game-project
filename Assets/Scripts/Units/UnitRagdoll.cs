using Assets.Scripts.Weapons;
using System.Drawing;
using Unity.AppUI.UI;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitRagdoll : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;


        private void Awake() {
            animator = GetComponent<Animator>();
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            SetRagdollActive(false);
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
            Vector3 direction = damageInfo.impactPoint - damageInfo.sourcePosition;

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

            for (int i = 0; i < ragdollRigidbodies.Length; i++)
            {
                Vector3 surfacePoint = ragdollColliders[i].ClosestPoint(point);
                float sqrDistance = (surfacePoint - point).sqrMagnitude;

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestBody = ragdollRigidbodies[i];
                }
            }

            return closestBody;
        }

    }

}