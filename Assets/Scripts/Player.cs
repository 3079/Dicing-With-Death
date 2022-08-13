using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [SerializeField] [NotNull] private GameObject _cube1;
    [SerializeField] [NotNull] private GameObject _cube2;
    [SerializeField] private float _rollSpeed = 3f;
    [SerializeField] private GridLayout _gridLayout;
    private bool _multipleCubes;
    private Grid _grid;
    [SerializeField] private LayerMask _collisionMask;  
    private GameObject _currentCube;
    private Transform _playerObject;
    // [SerializeField] private Transform _laserPoint;
    private Transform _mesh;
    private bool _isMoving;
    private bool _canMove = true;
    private float _camAngle = 0;

    private List<Laser> _lasers;

    public PlayerInput playerInput;
    public InputAction input_move;
    public InputAction input_switch;
    public InputAction input_rotateCam;

    private void OnEnable()
    {
        input_move = playerInput.Player.Move;
        input_switch = playerInput.Player.Switch;
        input_rotateCam = playerInput.Player.RotateCam;
        input_move.Enable();
        input_switch.Enable();
        input_rotateCam.Enable();
        input_move.performed += Move;
        input_switch.performed += Switch;
        input_rotateCam.performed += OnCameraRotated;
    }

    private void Awake()
    {
        _grid = _gridLayout.GetComponent<Grid>();
        playerInput = new PlayerInput();
        if (_cube2 != null)
            _multipleCubes = true;
        _currentCube = _cube1;
        UpdateCurrentCube();
        
        var pos = _playerObject.position;
        pos = SnapCoordinateToGrid(pos);
        _playerObject.position = pos;
        _lasers = new List<Laser>(GetComponentsInChildren<Laser>());
    }

    private void Move(InputAction.CallbackContext context)
    {
        // if (_isMoving || LevelManager.instance.IsBusy()) return;
        if (_isMoving) return;
        
        var pos = SnapCoordinateToGrid(_playerObject.position);
        var move = input_move.ReadValue<Vector2>();
        if(move.x != 0 && move.y != 0)
            return;
        
        // var newMove = Quaternion.AngleAxis(_camAngle, Vector3.forward) * move;
        // Debug.Log(newMove);
        // var dir = new Vector3(newMove.x, 0, newMove.y);
        var dir = new Vector3(move.x, 0, move.y);
        // Debug.Log("Rotating " + dir + " by " + _camAngle + " degrees");
        dir = Quaternion.AngleAxis(_camAngle, Vector3.up) * dir;
        // Debug.Log("Result: " + dir);
        
        // _playerObject.gameObject.GetComponent<Rigidbody>().Tr(pos + dir);
        // var newPos = SnapCoordinateToGrid(_playerObject.position);
        // if (newPos == pos)
        //     return;
        
        RaycastHit hit;
        if (Physics.Raycast(_playerObject.position, dir, out hit, 1f, _collisionMask))
        {
            return;
        }

        var offset = (Vector3.down + dir) * 0.5f;
        var anchor = pos + offset;
        var axis = Vector3.Cross(Vector3.up, dir);
        StartCoroutine(Roll(anchor, axis, 1, _rollSpeed));

        pos = SnapCoordinateToGrid(pos + dir);
        _playerObject.position = pos;
        
        LevelManager.instance.NewAction(GetCurrentCube(),-dir, anchor, axis);
    }

    private Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        var cellPos = _gridLayout.WorldToCell(position);
        position = _grid.GetCellCenterWorld(cellPos);
        return position;
    }

    private IEnumerator Roll(Vector3 anchor, Vector3 axis, int sign, float speed)
    {
        SetMoving(true);

        float remainingAngle = 90f;

        while (remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * speed, remainingAngle);
            _mesh.transform.RotateAround(anchor, axis, sign * rotationAngle);
            remainingAngle -= rotationAngle;
            yield return null;
        }
        SoundManager.instance.PlayStep(0);
        //Debug.Log("played step");
        
        
        SoundManager.instance.PlayStep(0);
        //Debug.Log("played step"); 
        
        // LevelManager.instance.SetBusyMove(false);
        
        SetMoving(false);
    }

    private void SetMoving(bool value)
    {
        _isMoving = value;
        LevelManager.instance.SetBusyMove(value);
        foreach (var laser in _lasers)
        {
            laser.SetMoving(value);
        }
    }

    public void RollBack(Vector3 dir, Vector3 anchor, Vector3 axis)
    {
        // if (_isMoving || LevelManager.instance.IsBusy()) return;
        if (_isMoving) return;

        var pos = SnapCoordinateToGrid(_playerObject.position);

        // dir = Quaternion.AngleAxis(_camAngle, Vector3.up) * dir;

        StartCoroutine(Roll(anchor, axis, -1, _rollSpeed * 2));

        pos = SnapCoordinateToGrid(pos + dir);
        _playerObject.position = pos;
    }

    public void Switch(InputAction.CallbackContext context)
    {
        if(!_multipleCubes || _isMoving) return;
        _currentCube = _currentCube == _cube1 ? _cube2 : _cube1;
        UpdateCurrentCube();
    }

    public int GetCurrentCube()
    {
        return _currentCube == _cube1? 1 : 2;
    }

    private void OnCameraRotated(InputAction.CallbackContext context)
    {
        var input = input_rotateCam.ReadValue<float>();
        _camAngle = (int)(_camAngle + (int)input * 90f) % 360;
    }

    private void UpdateCurrentCube()
    {
        _mesh = _currentCube.GetComponentInChildren<MeshRenderer>().transform;
        _playerObject = _currentCube.GetComponentInChildren<Collider>().transform;
        // _laserPoint = _mesh.transform.GetChild(0);
    }

    public float GetRollSpeed()
    {
        return _rollSpeed;
    }

    private void OnDisable()
    {
        input_move.Disable();
        input_switch.Disable();
        input_rotateCam.Disable();
    }
}
