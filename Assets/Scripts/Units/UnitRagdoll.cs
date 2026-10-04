using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitRagdoll : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody[] ragdollRigidbodies;

        private void Awake() {
            animator = GetComponent<Animator>();
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

            SetRagdollActive(false);
        }

        public void Activate()
        {
            transform.SetParent(null);
            SetRagdollActive(true);
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

    }

}