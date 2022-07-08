using System.Collections.Generic;
using UnityEngine;

public class MissileExplosion : MonoBehaviour
{
    private List<Rigidbody> _affectedBarriersRB;
    private List<int> addedObjects;
    private bool _isAffected = false;
    private bool _isExploded = false;


    public SphereCollider Trigger;
    public float TimeToExplode = 3f;
    public float TimeToDestroy = 5f;
    public Material AffectedMaterial;

    void Start()
    {
        _affectedBarriersRB = new List<Rigidbody>();
        addedObjects = new List<int>();
        Trigger.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Barrier")
        {
            Trigger.enabled = true;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Barrier")
        {
            var parent = collider.transform.parent;
            if (!addedObjects.Contains(parent.GetInstanceID()))
            {
                addedObjects.Add(parent.GetInstanceID());
                var childCount = parent.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = parent.GetChild(i);
                    _affectedBarriersRB.Add(child.GetComponent<Rigidbody>());
                    child.GetComponent<MeshRenderer>().material = AffectedMaterial;
                }
                _isAffected = true;
            }
        }
    }
    void Update()
    {
        if (_isAffected)
        {
            TimeToExplode -= Time.deltaTime;

            if (TimeToExplode < 0)
            {
                SoundManager.Instance.PlayExplosionSound();
                try
                {
                    foreach (var barrierRB in _affectedBarriersRB)
                    {
                        barrierRB.isKinematic = false;
                        barrierRB.gameObject.tag = "Untagged";
                    }
                }
                catch
                {
                    //Уже может быть уничтожено другим снарядом
                } 

                _isExploded = true;
                _isAffected = false;
            }
        }
        if (_isExploded)
        {
            TimeToDestroy -= Time.deltaTime;

            if (TimeToDestroy < 0)
            {
                try
                {
                    foreach (var barrierRB in _affectedBarriersRB)
                    {
                        Destroy(barrierRB.gameObject);
                    }
                }
                catch { _affectedBarriersRB.Clear(); } //Уже может быть уничтожено другим снарядом
                Destroy(this.gameObject);
            }
        }
    }
}
