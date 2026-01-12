using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSlot[] weaponSlots;

    private Weapon currentWeapon;

    public event Action<Weapon> OnWeaponChanged;

    public void EquipWeapon(WeaponSlotType slotType)
    {
        WeaponSlot slot = GetSlot(slotType);
        if (slot == null || slot.equippedWeapon == null) return;

        if (currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);

        currentWeapon = slot.equippedWeapon;
        currentWeapon.gameObject.SetActive(true);

        OnWeaponChanged?.Invoke(currentWeapon);
    }

    public void Fire()
    {
        currentWeapon?.Fire();
    }

    public void Reload()
    {
        currentWeapon?.Reload();
    }

    private WeaponSlot GetSlot(WeaponSlotType type)
    {
        foreach (var slot in weaponSlots)
            if (slot.slotType == type)
                return slot;

        return null;
    }

    public Weapon CurrentWeapon => currentWeapon;
}
