using Assets.Scripts.Weapons;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO data;
    [SerializeField] private Transform attackPoint;     // Where the projectile lauch from
    [SerializeField] private Transform grip;            // Where the weapon is held
    [SerializeField] private Rigidbody body;
    [SerializeField] private Collider dropCollider;
    [SerializeField] private float dropTorque = 2f;


    public WeaponSO GetData() => data;

    public Transform GetAttackPoint() => attackPoint;

    public void Drop(Vector3 impulse)
    {
        transform.SetParent(null, true);
        dropCollider.enabled = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.isKinematic = false;
        body.AddForce(impulse, ForceMode.Impulse);
        body.AddTorque(Random.insideUnitSphere * dropTorque, ForceMode.Impulse);
    }

    public void AttachTo(Transform socket)
    {
        transform.SetParent(socket, false);

        Vector3 s = socket.lossyScale;
        transform.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);

        Quaternion gripRelative = Quaternion.Inverse(transform.rotation) * grip.rotation;
        transform.rotation = socket.rotation * Quaternion.Inverse(gripRelative);

        transform.position += socket.position - grip.position;
    }
}
