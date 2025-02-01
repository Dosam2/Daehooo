using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public float dragSpeed = 25f;
    public float zoomSpeed = 500f;
    public float minZoom = 40f;
    public float maxZoom = 100f;

    private Vector3 dragOrigin;

    void Update()
    {
        // 마우스 드래그 이동
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
            transform.Translate(-move, Space.World);
        }

        // 마우스 휠 줌
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.fieldOfView = Mathf.Clamp(
            Camera.main.fieldOfView - scroll * zoomSpeed * Time.deltaTime,
            minZoom,
            maxZoom
        );
    }
}