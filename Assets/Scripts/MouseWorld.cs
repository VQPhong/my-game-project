using UnityEngine;

public class MouseWorld : MonoBehaviour
{

    private static MouseWorld instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;


    //// Debug: Move the object (3d-sphere) to the valid mouse each frame
    //// Uncomment when mouse pointer visual debugging is needed
    //private void Update()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    Debug.Log(Physics.Raycast(ray, out RaycastHit raycastHit));
    //    transform.position = raycastHit.point;

    //}

    private void Awake()
    {
        instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }

}