using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class DefaultPlayer : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 20f;
    private (float x, float z) _currentInput;
    [SerializeField] private bool useNewInputSystem = false;

    [SerializeField] private InputActionReference movementAction;

    void Update()
    {
        _currentInput = GetInput();

        UpdatePosition(_currentInput.x, _currentInput.z);
    }


    (float x, float z) GetInput()
    {
        float xPlane = 0f, zPlane = 0f;
        if(useNewInputSystem)
        {
            Vector2 inputVector = movementAction.action.ReadValue<Vector2>();
            xPlane = inputVector.x;
            zPlane = inputVector.y;
        }
        else
        {
            xPlane = Input.GetAxis("Horizontal");
            zPlane = Input.GetAxis("Vertical");

        }
        return (xPlane, zPlane);
    }

    private void UpdatePosition(float x, float z)
    {
        Vector3 movement = new Vector3(x, 0, z) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }

}
