using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;
    private Weapon boundWeapon;

    public void Bind(Weapon weapon)
    {
        if (boundWeapon != null)
            boundWeapon.OnAmmoChanged -= UpdateDisplay;

        boundWeapon = weapon;
        boundWeapon.OnAmmoChanged += UpdateDisplay;

        UpdateDisplay(boundWeapon);
    }

    private void UpdateDisplay(Weapon weapon)
    {
        ammoText.text = $"{weapon.CurrentAmmo} / {weapon.TotalAmmo}";
    }
}
