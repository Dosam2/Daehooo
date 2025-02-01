using UnityEngine;

public class EnhancedShipController : MonoBehaviour
{
    [SerializeField] private ShipSpecification specs;
    [SerializeField] private float pathPreviewResolution = 50f;
    [SerializeField] private float minTurnRadius = 2f;
    private LineRenderer pathPreview;
    private Vector3 targetDestination;
    private float currentSpeed;
    private float turnProgress = 0f;
    private ShipType shipType; // Add this line

    private ShipController shipController;

    private void Awake()
    {
        shipController = GetComponent<ShipController>();
    }

    private void SetupPathPreview()
    {
        pathPreview = gameObject.AddComponent<LineRenderer>();
        pathPreview.startWidth = 0.2f;
        pathPreview.endWidth = 0.2f;
        pathPreview.material = new Material(Shader.Find("Sprites/Default"));
        pathPreview.startColor = Color.white;
        pathPreview.endColor = new Color(1, 1, 1, 0.2f);
    }

    public Vector3[] CalculatePathToDock(Vector3 dockPosition)
    {
        Vector3 startPos = transform.position;
        Vector3 startDir = transform.forward;
        Vector3 targetDir = (dockPosition - startPos).normalized;
        float angle = Vector3.SignedAngle(startDir, targetDir, Vector3.up);
        int pathPoints = 30;
        Vector3[] points = new Vector3[pathPoints];

        // 현재 속도와 선박 타입에 따른 회전 반경 조정
        float turnRadius = GetTurnRadius();
        float totalDistance = Vector3.Distance(startPos, dockPosition);

        // 3단계 경로 생성: 초기 선회 -> 중간 직진 -> 최종 접근
        if (Mathf.Abs(angle) > 5f)
        {
            int initialTurnPoints = pathPoints / 3;  // 초기 선회 구간
            int straightPoints = pathPoints / 3;     // 중간 직진 구간
            int finalTurnPoints = pathPoints - initialTurnPoints - straightPoints; // 최종 접근 구간

            // 1. 초기 선회 구간
            for (int i = 0; i < initialTurnPoints; i++)
            {
                float t = i / (float)(initialTurnPoints);
                float turnAngle = angle * EaseInOutQuad(t);
                Quaternion rotation = Quaternion.Euler(0, turnAngle, 0);
                Vector3 direction = rotation * startDir;
                points[i] = startPos + direction * (turnRadius * t);
                points[i].y = 0.1f;
            }

            // 2. 중간 직진 구간
            Vector3 turnEndPos = points[initialTurnPoints - 1];
            Vector3 straightDir = (dockPosition - turnEndPos).normalized;
            float straightDistance = totalDistance * 0.4f; // 전체 거리의 40%

            for (int i = 0; i < straightPoints; i++)
            {
                float t = i / (float)straightPoints;
                points[i + initialTurnPoints] = turnEndPos + straightDir * (straightDistance * t);
                points[i + initialTurnPoints].y = 0.1f;
            }

            // 3. 최종 접근 구간 (도크 방향으로 부드럽게)
            Vector3 straightEndPos = points[initialTurnPoints + straightPoints - 1];
            Vector3 finalDir = (dockPosition - straightEndPos).normalized;

            for (int i = 0; i < finalTurnPoints; i++)
            {
                float t = i / (float)finalTurnPoints;
                points[i + initialTurnPoints + straightPoints] = Vector3.Lerp(straightEndPos, dockPosition, EaseInOutQuad(t));
                points[i + initialTurnPoints + straightPoints].y = 0.1f;
            }
        }
        else
        {
            // 거의 직선 방향일 경우
            for (int i = 0; i < pathPoints; i++)
            {
                float t = i / (float)(pathPoints - 1);
                points[i] = Vector3.Lerp(startPos, dockPosition, EaseInOutQuad(t));
                points[i].y = 0.1f;
            }
        }

        return points;
    }

    private float GetTurnRadius()
    {
        float baseRadius = 2.0f;
        switch (shipController.Type)
        {
            case ShipType.Cargo:
                return baseRadius * 2.0f;
            case ShipType.Oil:
                return baseRadius * 2.5f;
            case ShipType.Passenger:
                return baseRadius * 1.5f;
            default:
                return baseRadius;
        }
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    private float CalculateSpeedAlongPath(Vector3[] pathPoints, float baseSpeed)
    {
        // 경로의 곡률에 따른 속도 조정
        float minSpeedMultiplier = 0.4f;
        float maxCurvature = 0f;

        for (int i = 1; i < pathPoints.Length - 1; i++)
        {
            Vector3 prevToCurrent = (pathPoints[i] - pathPoints[i - 1]).normalized;
            Vector3 currentToNext = (pathPoints[i + 1] - pathPoints[i]).normalized;
            float angle = Vector3.Angle(prevToCurrent, currentToNext);
            maxCurvature = Mathf.Max(maxCurvature, angle);
        }

        // 곡률이 클수록 속도 감소
        float speedMultiplier = Mathf.Lerp(1f, minSpeedMultiplier, maxCurvature / 180f);
        return baseSpeed * speedMultiplier;
    }

    private void FollowPath()
    {
        if (turnProgress < 1f)
        {
            turnProgress += Time.deltaTime * (currentSpeed / 10f);
            Vector3 nextPosition = pathPreview.GetPosition((int)(turnProgress * pathPreviewResolution));

            // 다음 위치를 향해 부드럽게 회전
            Vector3 direction = (nextPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

            // 현재 속도로 이동
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, currentSpeed * Time.deltaTime);
        }
    }
}