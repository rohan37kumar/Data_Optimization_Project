using UnityEngine;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private AmmoDisplay ammoDisplay;

    public void Bind(WeaponController controller)
    {
        controller.OnWeaponChanged += HandleWeaponChanged;
    }

    private void HandleWeaponChanged(Weapon weapon)
    {
        ammoDisplay.Bind(weapon);
    }
}
