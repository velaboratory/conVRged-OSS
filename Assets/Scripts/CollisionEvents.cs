using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    public string needsTag;
    public UnityEvent CollisionEnter;
    public UnityEvent CollisionExit;
    public UnityEvent TriggerEnter;
    public UnityEvent TriggerExit;
    public List<Collider> touchedObjectsCollider = new List<Collider>();
    public List<Collider> touchedObjectsTrigger = new List<Collider>();

    public void OnCollisionEnter(Collision collision)
    {
        if (string.IsNullOrEmpty(needsTag) || collision.collider.CompareTag(needsTag))
        {
            CollisionEnter?.Invoke();
            touchedObjectsCollider.RemoveAll(c => c == null);
            touchedObjectsCollider.Add(collision.collider);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (string.IsNullOrEmpty(needsTag) || collision.collider.CompareTag(needsTag))
        {
            CollisionExit?.Invoke();
            touchedObjectsCollider.RemoveAll(c => c == null);
            touchedObjectsCollider.Remove(collision.collider);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(needsTag) || other.CompareTag(needsTag))
        {
            TriggerEnter?.Invoke();
            touchedObjectsTrigger.RemoveAll(c => c == null);
            touchedObjectsTrigger.Add(other);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (string.IsNullOrEmpty(needsTag) || other.CompareTag(needsTag))
        {
            TriggerExit?.Invoke();
            touchedObjectsTrigger.RemoveAll(c => c == null);
            touchedObjectsTrigger.Remove(other);
        }
    }
}