using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int magazineSize;
    public int maxAmmo;
    public float fireRate;
    public bool isAutomatic;
}
