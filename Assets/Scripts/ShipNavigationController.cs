using UnityEngine;

public class ShipNavigationController : MonoBehaviour
{
    private ShipController shipController;
    private Camera mainCamera;
    private Vector3 dragStartPosition;
    private bool isDragging;
    private LineRenderer pathLine; // 경로를 보여주기 위한 라인

    [SerializeField]
    private float dragThreshold = 0.1f; // 드래그 감지 최소 거리
    [SerializeField]
    private int pathResolution = 20; // 곡선 경로
    private void Awake()
    {
        shipController = GetComponent<ShipController>();
        mainCamera = Camera.main;
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        pathLine = gameObject.AddComponent<LineRenderer>();
        pathLine.startWidth = 0.2f;
        pathLine.endWidth = 0.2f;
        pathLine.material = new Material(Shader.Find("Sprites/Default"));
        pathLine.startColor = Color.blue;
        pathLine.endColor = Color.red;
        pathLine.positionCount = pathResolution;
    }

    private void Update()
    {
        HandlePathDrawing();
    }

    private void HandlePathDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isDragging = true;
                dragStartPosition = new Vector3(hit.point.x, 0, hit.point.z); // Y값을 0으로 고정
                DrawPath(dragStartPosition, dragStartPosition);
                pathLine.enabled = true;
            }
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 endPoint = new Vector3(hit.point.x, 0, hit.point.z); // Y값을 0으로 고정
                DrawPath(dragStartPosition, endPoint);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 endPoint = new Vector3(hit.point.x, 0, hit.point.z); // Y값을 0으로 고정
                if (Vector3.Distance(dragStartPosition, endPoint) > dragThreshold)
                {
                    shipController.StartEngine();
                    shipController.Navigate(endPoint);
                }
            }

            pathLine.enabled = false;
        }
    }

    private void DrawPath(Vector3 start, Vector3 end)
    {
        // 곡선 경로 생성
        Vector3[] pathPoints = new Vector3[pathResolution];
        for (int i = 0; i < pathResolution; i++)
        {
            float t = i / (float)(pathResolution - 1);
            // 베지어 곡선 계산
            Vector3 currentRotation = transform.rotation * Vector3.forward;
            Vector3 control = start + currentRotation * Vector3.Distance(start, end) * 0.5f;
            pathPoints[i] = CalculateBezierPoint(t, start, control, end);
            pathPoints[i].y = 0; // Y값을 0으로 고정
        }

        pathLine.SetPositions(pathPoints);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 start, Vector3 control, Vector3 end)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * start;
        point += 2 * u * t * control;
        point += tt * end;

        return point;
    }
}