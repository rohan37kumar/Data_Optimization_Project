//mock weapon

using UnityEngine;

public class GunWeapon : Weapon
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private float range = 100f;

    public override void Fire()
    {
        base.Fire();

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            // here do damage application logic
            Debug.Log($"Hit {hit.collider.name}");
        }
    }
}
