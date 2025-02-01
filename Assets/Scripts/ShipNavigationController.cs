using UnityEngine;

public class ShipNavigationController : MonoBehaviour
{
    private ShipController shipController;
    private Camera mainCamera;
    private Vector3 dragStartPosition;
    private bool isDragging;
    private LineRenderer pathLine;
    private Vector3 targetPoint;
    private bool isPathActive = false;
    private EnhancedShipController enhancedController;
    private bool isTargetDock = false; // Dock 여부 추적
    private Vector3 lastDockPosition; // 마지막 Dock 위치 저장

    [SerializeField] private float pathWidth = 0.2f;
    [SerializeField] private Color pathColor = Color.white;
    [SerializeField] private float pathHeight = 0.1f;
    [SerializeField] private float dragThreshold = 0.1f;
    [SerializeField] private int pathPoints = 30;
    [SerializeField] private float turnRadius = 5f;

    private void Awake()
    {
        shipController = GetComponent<ShipController>();
        mainCamera = Camera.main;
        enhancedController = GetComponent<EnhancedShipController>();
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        // if (GetComponent<LineRenderer>() != null)
        //     Destroy(GetComponent<LineRenderer>());

        pathLine = GetComponent<LineRenderer>();

        if (pathLine == null)
        {
            pathLine = gameObject.AddComponent<LineRenderer>();
        }
        pathLine.startWidth = pathWidth;
        pathLine.endWidth = pathWidth;

        // 점선 효과를 위한 머티리얼 설정
        Material dottedLineMaterial = new Material(Shader.Find("Sprites/Default"));
        dottedLineMaterial.color = pathColor;

        // 텍스처 생성 (점선 패턴)
        Texture2D texture = new Texture2D(8, 2);
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                texture.SetPixel(x, y, x < 4 ? Color.white : Color.clear);
            }
        }
        texture.Apply();

        dottedLineMaterial.mainTexture = texture;
        pathLine.material = dottedLineMaterial;

        pathLine.textureMode = LineTextureMode.Tile;
        pathLine.positionCount = pathPoints;
        pathLine.useWorldSpace = true;
        pathLine.enabled = false;
    }

    private void Update()
    {
        HandlePathDrawing();

        // 경로가 활성화되어 있을 때 지속적으로 업데이트
        if (isPathActive && !isDragging)
        {
            UpdateActivePath();
        }
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
                dragStartPosition = transform.position;
                pathLine.enabled = true;
            }
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = new Vector3(hit.point.x, pathHeight, hit.point.z);
                isTargetDock = hit.collider.CompareTag("Dock");

                if (isTargetDock)
                {
                    lastDockPosition = targetPoint;
                    Vector3[] pathPoints = enhancedController.CalculatePathToDock(targetPoint);
                    pathLine.positionCount = pathPoints.Length;
                    pathLine.SetPositions(pathPoints);
                }
                else
                {
                    isTargetDock = false;
                    DrawPredictedPath(transform.position, targetPoint);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 endPoint = new Vector3(hit.point.x, 0, hit.point.z);
                if (Vector3.Distance(dragStartPosition, endPoint) > dragThreshold)
                {
                    targetPoint = endPoint;
                    isPathActive = true;
                    shipController.Navigate(endPoint);
                }
            }
        }
    }

    private void UpdateActivePath()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
        if (distanceToTarget > 0.1f)
        {
            if (isTargetDock)
            {
                // Dock으로 가는 경로 업데이트
                Vector3[] pathPoints = enhancedController.CalculatePathToDock(lastDockPosition);
                pathLine.positionCount = pathPoints.Length;
                pathLine.SetPositions(pathPoints);
            }
            else
            {
                DrawPredictedPath(transform.position, targetPoint);
            }
        }
        else
        {
            isPathActive = false;
            pathLine.enabled = false;
            isTargetDock = false;
        }

    }

    private void DrawPredictedPath(Vector3 startPos, Vector3 endPos)
    {
        Vector3 startDir = transform.forward;
        Vector3 targetDir = (endPos - startPos).normalized;
        float angle = Vector3.SignedAngle(startDir, targetDir, Vector3.up);
        bool needRotation = Mathf.Abs(angle) > 0.1f;

        // 선박 타입별 속도와 회전 속도
        float maxSpeed = 0f;
        float rotationSpeed = 0f;
        float turnRadius = 0f;

        switch (shipController.Type)
        {
            case ShipType.Cargo:
                maxSpeed = 2.0f;
                rotationSpeed = 45f;
                turnRadius = 2.0f;
                break;
            case ShipType.Oil:
                maxSpeed = 2.2f;
                rotationSpeed = 40f;
                turnRadius = 2.2f;
                break;
            case ShipType.Passenger:
                maxSpeed = 2.5f;
                rotationSpeed = 50f;
                turnRadius = 1.8f;
                break;
        }

        // 30도 이상 회전시 회전 반경 증가
        if (Mathf.Abs(angle) > 30f)
        {
            turnRadius *= 1.5f;
        }

        Vector3[] points = new Vector3[pathPoints];

        if (needRotation)
        {
            int rotationPoints = pathPoints / 2; // 회전 구간에 더 많은 포인트 할당

            // 회전 경로 계산
            for (int i = 0; i < rotationPoints; i++)
            {
                float t = i / (float)(rotationPoints - 1);
                float currentAngle = angle * t;

                // 회전 반경을 고려한 위치 계산
                Vector3 center = startPos + (Quaternion.Euler(0, angle / 2, 0) * startDir * turnRadius);
                Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
                Vector3 offset = rotation * (-startDir * turnRadius);
                // points[i] = center + offset;
                points[i] = Vector3.Lerp(startPos, endPos, currentAngle);
                points[i].y = pathHeight;
            }

            // 회전 후 직진 경로
            Vector3 rotationEndPos = points[rotationPoints - 1];
            Vector3 finalDir = Quaternion.Euler(0, angle, 0) * startDir;

            for (int i = rotationPoints; i < pathPoints; i++)
            {
                float t = (i - rotationPoints) / (float)(pathPoints - rotationPoints);
                points[i] = rotationEndPos + finalDir * Vector3.Distance(rotationEndPos, endPos) * t;
                points[i].y = pathHeight;
            }
        }
        else
        {
            // 직진 경로
            for (int i = 0; i < pathPoints; i++)
            {
                float t = i / (float)(pathPoints - 1);
                points[i] = Vector3.Lerp(startPos, endPos, t);
                points[i].y = pathHeight;
            }
        }

        pathLine.SetPositions(points);
    }


    private void OnDestroy()
    {
        if (pathLine != null)
            Destroy(pathLine);
    }
}