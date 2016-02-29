using UnityEngine;
using System.Collections;

public class Skeleton : MonoBehaviour
{
    public MediatorUnity MediatorUnity;

	// Use this for initialization
	void Start () {
	    //MediatorUnity.Mediator.AddEventHandler(new SkeletonMoved(this.gameObject));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class SkeletonMoved // : DomainEventHandler<SkeletonMoveEvent>
{
    private GameObject _skeleton;

    public SkeletonMoved(GameObject skeleton)
    {
        _skeleton = skeleton;
    }

    //public void HandleEvent(SkeletonMoveEvent e)
    //{
    //    Debug.Log("Hey! Skeleton moved! To: " + e.Coordinate);
        
    //    Vector3 old = _skeleton.transform.position;

    //    float x = (float)e.Coordinate.X;
    //    float z = (float)e.Coordinate.Z;

    //    Debug.Log("Moving skeleton-ui to " + x + " - " + z);

    //    _skeleton.transform.position = new Vector3(x, old.y, z);
    //}
}
