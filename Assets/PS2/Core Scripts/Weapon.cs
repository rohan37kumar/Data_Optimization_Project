using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData Data { get; private set; }

    protected int currentAmmo;
    protected int totalAmmo;
    protected float lastFireTime;

    public event Action<Weapon> OnAmmoChanged;

    public virtual void Initialize(WeaponData data)
    {
        Data = data;
        currentAmmo = data.magazineSize;
        totalAmmo = data.maxAmmo - currentAmmo;
    }

    public virtual bool CanFire()
    {
        if (currentAmmo <= 0) return false;
        return Time.time >= lastFireTime + (1f / Data.fireRate);
    }

    public virtual void Fire()
    {
        if (!CanFire()) return;

        currentAmmo--;
        lastFireTime = Time.time;

        OnAmmoChanged?.Invoke(this);
    }

    public virtual void Reload()
    {
        int needed = Data.magazineSize - currentAmmo;
        int reloadAmount = Mathf.Min(needed, totalAmmo);

        currentAmmo += reloadAmount;
        totalAmmo -= reloadAmount;

        OnAmmoChanged?.Invoke(this);
    }

    public int CurrentAmmo => currentAmmo;
    public int TotalAmmo => totalAmmo;
}
