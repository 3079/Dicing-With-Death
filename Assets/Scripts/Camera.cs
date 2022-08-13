using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    public PlayerInput playerInput;
    public InputAction input_rotateCam;
    private bool _isMoving;

    private void OnEnable()
    {
        input_rotateCam = playerInput.Player.RotateCam;
        input_rotateCam.Enable();
        input_rotateCam.performed += OnCameraRotated;
    }
    
    private void Awake()
    {
        playerInput = new PlayerInput();
    }
    
    private void OnCameraRotated(InputAction.CallbackContext context)
    {
        // if (_isMoving)
            // return;
        var input = input_rotateCam.ReadValue<float>();
        StartCoroutine(RotateCamera((int)input));
    }

    private IEnumerator RotateCamera(int sign)
    {
        _isMoving = true;

        float remainingAngle = 90f;
        
        while (remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * _rotationSpeed, remainingAngle);
            transform.Rotate(Vector3.up, sign * rotationAngle);
            remainingAngle -= rotationAngle;
            yield return null;
        }

        _isMoving = false;
    }

    private void OnDisable()
    {
        input_rotateCam.Disable();
    }
}
