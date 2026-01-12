public enum WeaponSlotType
{
    Primary1,
    Primary2,
    Secondary
}

[System.Serializable]
public class WeaponSlot
{
    public WeaponSlotType slotType;
    public Weapon equippedWeapon;
}
