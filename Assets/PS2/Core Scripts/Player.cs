using UnityEngine;

public class Player : MonoBehaviour
{
    private WeaponController weaponController;

    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
            weaponController.Fire();

        if (Input.GetKeyDown(KeyCode.R))
            weaponController.Reload();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            weaponController.EquipWeapon(WeaponSlotType.Primary1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            weaponController.EquipWeapon(WeaponSlotType.Primary2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            weaponController.EquipWeapon(WeaponSlotType.Secondary);
    }
}
