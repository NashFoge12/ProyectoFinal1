using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Camara 3D orbital: arrastrar con botón derecho, rueda para zoom, Q/E para girar.</summary>
[DisallowMultipleComponent]
public class PujllayCameraController : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 targetOffset;
    [Range(8f,35f)] public float distance=18f;
    [Range(20f,75f)] public float pitch=43f;
    public float yaw=0f;
    public float zoomSpeed=2.2f;
    public float rotateSpeed=150f;
    public float minDistance=9f;
    public float maxDistance=32f;
    Vector3 fixedTarget;
    bool hasFixedTarget;

    public void SetTarget(Vector3 worldPosition)
    {
        fixedTarget=worldPosition;
        hasFixedTarget=true;
        ApplyCamera();
    }

    void LateUpdate()
    {
        // Escritorio: botón derecho + arrastrar y rueda del ratón.
        if(Input.GetMouseButton(1) && !PointerOverUI())
        {
            yaw+=Input.GetAxis("Mouse X")*rotateSpeed*Time.deltaTime;
            pitch-=Input.GetAxis("Mouse Y")*rotateSpeed*Time.deltaTime;
            pitch=Mathf.Clamp(pitch,22f,72f);
        }
        float scroll=Input.mouseScrollDelta.y;
        if(Mathf.Abs(scroll)>.001f)
            distance=Mathf.Clamp(distance-scroll*zoomSpeed,minDistance,maxDistance);

        // Móvil/tablet: un dedo para orbitar y dos dedos para zoom.
        if(Input.touchCount==1)
        {
            Touch touch=Input.GetTouch(0);
            bool touchOnWorld=touch.position.y>Screen.height*.36f;
            if(touchOnWorld && touch.phase==TouchPhase.Moved)
            {
                yaw+=touch.deltaPosition.x*.22f;
                pitch-=touch.deltaPosition.y*.18f;
                pitch=Mathf.Clamp(pitch,22f,72f);
            }
        }
        else if(Input.touchCount>=2)
        {
            Touch a=Input.GetTouch(0), b=Input.GetTouch(1);
            if(a.position.y<=Screen.height*.36f || b.position.y<=Screen.height*.36f)
            {
                ApplyCamera();
                return;
            }
            Vector2 aPrev=a.position-a.deltaPosition;
            Vector2 bPrev=b.position-b.deltaPosition;
            float previous=(aPrev-bPrev).magnitude;
            float current=(a.position-b.position).magnitude;
            float pinch=current-previous;
            distance=Mathf.Clamp(distance-pinch*.012f,minDistance,maxDistance);
        }
        if(Input.GetKey(KeyCode.Q))yaw-=rotateSpeed*Time.deltaTime*.55f;
        if(Input.GetKey(KeyCode.E))yaw+=rotateSpeed*Time.deltaTime*.55f;
        ApplyCamera();
    }

    bool PointerOverUI()
    {
        return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
    }

    void ApplyCamera()
    {
        Vector3 focus=targetTransform?targetTransform.position+targetOffset:
            (hasFixedTarget?fixedTarget:Vector3.zero);
        Quaternion rotation=Quaternion.Euler(pitch,yaw,0f);
        transform.position=focus+rotation*new Vector3(0,0,-distance);
        transform.rotation=rotation;
        transform.LookAt(focus,Vector3.up);
    }
}
