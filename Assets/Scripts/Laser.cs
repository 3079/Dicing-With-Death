using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private LayerMask _laserCollisionMask;
    private bool _isMoving;
    private bool _laserON = true;
    private LineRenderer _lineRenderer;

    RaycastHit _hit;

    private void Awake()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
    }
    private void UpdateLaser()
    {
        if (Physics.Raycast(transform.position, transform.forward, out _hit, 1000f, _laserCollisionMask))
        {
            if (_hit.distance < 0.02f)
            {
                // LASER IS BLOCKED
                DisableLaser();
                return;
            }
            
            // LASER IS HITTING SOMETHING

            LaserEffect();
            // Debug.Log("Hit Someone!");
        }
        
        // LASER HAS NO OBSTACLES
        ActivateLaser();
        Vector3[] positions = new Vector3[2];
        positions[0] = transform.position;
        float distance = (_hit.distance == 0 ? 1000f : _hit.distance);
        positions[1] = transform.position + transform.forward * distance;
        _lineRenderer.SetPositions(positions);
    }

    private void LaserEffect()
    {
        if (_isMoving  || LevelManager.instance.IsBusy()) return;
        
        IDamageable npc = _hit.collider.GetComponentInParent<IDamageable>();
        if (npc != null)
        {
            // Debug.Log("Damage Applied!");
            npc.OnHit();
        }
    }

    private void DisableLaser()
    {
        if (!_laserON) return;
        _laserON = false;
        _lineRenderer.enabled = false;
    }
    
    private void ActivateLaser()
    {
        if (_laserON) return;
        _laserON = true;
        _lineRenderer.enabled = true;
    }

    public void SetMoving(bool value)
    {
        _isMoving = value;
    }
    
    private void Update()
    {
        UpdateLaser();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * (_hit.distance == 0? 1000f : _hit.distance));
    }
}
