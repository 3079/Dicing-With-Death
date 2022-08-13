using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class LevelManager : MonoBehaviour
{
    private class Action
    {
        private Tuple<int, Vector3, Vector3, Vector3> _coordinates;
        private bool _enemydestroyed;
        private List<Vector3> _destroyedEnemiesCoordinates;
        
        public Action(int cube, Vector3 dir, Vector3 anchor, Vector3 axis)
        {
            _coordinates = new Tuple<int, Vector3, Vector3, Vector3>(cube, dir, anchor, axis);
            _destroyedEnemiesCoordinates = new List<Vector3>();
        }

        public Tuple<Vector3, Vector3, Vector3> GetCoordinates()
        {
            Tuple<Vector3, Vector3, Vector3> coord =
                new Tuple<Vector3, Vector3, Vector3>(_coordinates.Item2, _coordinates.Item3, _coordinates.Item4);
            return coord;
        }

        public bool EnemyDestroyed()
        {
            return _enemydestroyed;
        }

        public List<Vector3> GetDestroyedEnemiesCoordinates()
        {
            return _destroyedEnemiesCoordinates;
        }

        public void OnEnemyDestroyed(Enemy enemy)
        {
            _enemydestroyed = true;
            _destroyedEnemiesCoordinates.Add(enemy.transform.position);
        }

        public int GetCube()
        {
            return _coordinates.Item1;
        }
    }

    public static LevelManager instance;

    [SerializeField] private GameObject _enemyGrid;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _enemyRespawnDelay = 0.3f;
    private List<Enemy> _enemies;
    private Stack<Action> _actions;
    private Player _player;
    private bool _busyMove;
    private bool _busy;
    
    public PlayerInput playerInput;
    public InputAction input_undo;

    private float _rotationSpeed = 100f;
    private bool _isMoving;
    
    private void OnEnable()
    {
        input_undo = playerInput.Player.Undo;
        input_undo.Enable();
        input_undo.performed += UndoAction;
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        _enemies = new List<Enemy>();
        _actions = new Stack<Action>();
        
        playerInput = new PlayerInput();

        foreach (var enemy in _enemyGrid.GetComponentsInChildren<Enemy>())
        {
            _enemies.Add(enemy);
        }

        _player = FindObjectOfType<Player>();
        _enemyRespawnDelay = 90f / _player.GetRollSpeed();
    }

    public void NewAction(int cube, Vector3 dir, Vector3 anchor, Vector3 axis)
    {
        var action = new Action(cube, dir, anchor, axis);
        _actions.Push(action);
        // Debug.Log("Added new action " + dir + ", " + anchor + ", " + axis);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (!_enemies.Contains(enemy)) return;
        
        if(_actions.Count == 0) return;

        // Debug.Log("Enemy removed");
        _actions.Peek().OnEnemyDestroyed(enemy);
        _enemies.Remove(enemy);
        
        if(_enemies.Count == 0)
            SceneController.instance.LevelSelection();
    }

    public IEnumerator AddEnemies(List<Vector3> coordinates)
    {
        yield return new WaitForSeconds(_enemyRespawnDelay);
        for (int i = 0; i < coordinates.Count; i++)
        {
            Enemy enemy = Instantiate(_enemyPrefab, coordinates[i], Quaternion.identity).GetComponent<Enemy>();
            _enemies.Add(enemy);
        }

        _busy = false;
    }

    public void UndoAction(InputAction.CallbackContext context)
    {
        if (_busy || _busyMove) return;
        if (_actions.Count == 0) return;

        _busyMove = true;
        
        var action = _actions.Pop();

        var coords = action.GetCoordinates();

        var cube = action.GetCube();
        if(cube != _player.GetCurrentCube())
            _player.Switch(new InputAction.CallbackContext());
        _player.RollBack(coords.Item1, coords.Item2, coords.Item3);
        
        // Debug.Log("Undone move " + coords.Item1 + ", " + coords.Item2 + ", " + coords.Item3);

        if (action.EnemyDestroyed())
        {
            _busy = true;
            StartCoroutine(AddEnemies(action.GetDestroyedEnemiesCoordinates()));
            // Debug.Log("Replaced enemy");
        }
    }

    public void SetBusyMove(bool value)
    {
        _busyMove = value;
    }
    
    public bool IsBusy()
    {
        return _busy || _busyMove;
    }
    
    private void OnDisable()
    {
        input_undo.Disable();
    }
}
