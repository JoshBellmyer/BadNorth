using System.Collections.Generic;
using UnityEngine;

public class Group<T> where T : Unit {

    private Vector3 _targetPosition;
    private List<T> _units;
    private bool _canAttack;
    public bool CanAttack
    {
        get
        {
            return _canAttack;
        }
        set
        {
            // TODO: Implement
            _canAttack = value;
        }
    }
    
    public void TeleportTo(Vector3 position, float rotation)
    {
        // TODO: Implement
    }
    public void TeleportTo(Vector3 position)
    {
        // TODO: Implement
        TeleportTo(position, 0f);
    }
    public void MoveTo(Vector3 position)
    {
        // TODO: Implement
    }
    public int GetLiving()
    {
        // TODO: Implement
        return 0;
    }
    public void DestroyGroup()
    {
        // TODO: Implement
    }
    public Vector3 GetDestination()
    {
        // TODO: Implement
        return Vector3.zero;
    }
    public string GetDescription()
    {
        return null;
    }
    
}
