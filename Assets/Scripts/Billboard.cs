using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Billboard : MonoBehaviour
{
    private UnityEngine.Camera _camera;
    public PlayerInput playerInput;
    public InputAction input_rotateCam;

    private float _rotationSpeed = 100f;
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
        _camera = UnityEngine.Camera.main;
        SetRotation();
    }
    
    private void OnCameraRotated(InputAction.CallbackContext context)
    {
        // if (_isMoving)
        // return;
        var input = input_rotateCam.ReadValue<float>();
        StartCoroutine(RotateSprite((int)input));
    }

    private IEnumerator RotateSprite(int sign)
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

    private void SetRotation()
    {
        var rot = _camera.transform.rotation;
        transform.rotation = new Quaternion(0, rot.y, 0, rot.w);
    }

    private void OnDisable()
    {
        input_rotateCam.Disable();
    }
}
