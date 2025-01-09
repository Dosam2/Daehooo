using System.Collections;
using UnityEngine;

public class ShipController : MonoBehaviour, IShip
{
    [SerializeField] private ShipType shipType;
    [SerializeField] private ShipSpecification specs;

    private Rigidbody rb;
    private float currentSpeed;
    private bool isEngineRunning;
    private Vector3 targetDirection;  // 목표 방향
    private bool isDocking;
    public bool IsDocking => isDocking;
    private bool isMoving;
    private bool isRespawning;
    private Vector3 startPosition;
    private float targetSpeed; // 목표 속도 추가
    private bool isFirstMove = true;

    public ShipType Type => shipType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = specs.mass;
        startPosition = transform.position;
        SetupShipTypeSpecifications();
    }

    private void Update()
    {
        if (isEngineRunning && isMoving && !isDocking && !isRespawning)
        {
            UpdateMovement();
        }
    }

    private void UpdateMovement()
    {
        // Y 위치 고정
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        // 현재 방향과 목표 방향 사이의 각도 계산
        float angle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
        bool isRotating = Mathf.Abs(angle) > 0.1f;

        if (isFirstMove && isRotating)
        {
            // 첫 출발 시에만 즉시 회전
            transform.rotation = Quaternion.LookRotation(targetDirection);
            isFirstMove = false;
            targetSpeed = specs.maxSpeed;
        }
        else if (isRotating)
        {
            // 이동 중 회전 - 부드럽게 회전
            float rotationAmount = Mathf.Sign(angle) * specs.rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);

            // 회전 중일 때의 목표 속도로 천천히 조절
            if (currentSpeed > specs.rotationSpeed)
            {
                targetSpeed = specs.rotationSpeed;
            }
        }
        else
        {
            // 직진 시의 목표 속도
            targetSpeed = specs.maxSpeed;
        }

        // 현재 속도를 목표 속도로 부드럽게 변경
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, specs.acceleration * Time.deltaTime);

        // 전진
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);

        // 디버그 정보 출력
        Debug.Log($"Current Speed: {currentSpeed:F4}, Target Speed: {targetSpeed:F4}, Rotating: {isRotating}");
    }

    public void Navigate(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0;
        targetDirection = direction;

        isMoving = true;
        isDocking = false;

        if (!isEngineRunning)
        {
            StartEngine();
            isFirstMove = true; // 엔진 시작할 때만 첫 출발로 설정
        }
    }

    public void OnDockReached()
    {
        isDocking = true;
        isMoving = false;
        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        float fadeTime = 2f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = 1 - (elapsed / fadeTime);

            foreach (MeshRenderer renderer in renderers)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }

            yield return null;
        }

        if (shipType == ShipType.Cargo)
        {
            StartCoroutine(RespawnSequence());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RespawnSequence()
    {
        isRespawning = true;
        yield return new WaitForSeconds(5f);

        // 위치 초기화
        transform.position = startPosition;

        // 렌더러 초기화
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = 1f;
            renderer.material.color = color;
        }

        isRespawning = false;
        isDocking = false;
        isMoving = false;
        isEngineRunning = false;
    }

    private void StopMoving()
    {
        isMoving = false;
        currentSpeed = 0f;
    }
    private void SetupShipTypeSpecifications()
    {
        switch (shipType)
        {
            case ShipType.Cargo:
                specs.maxSpeed = 2.0f;         // 최대 속도
                specs.acceleration *= 0.6f;
                specs.rotationSpeed = 45f;     // 회전 속도 증가 (기존보다 높게)
                specs.mass *= 1.5f;
                break;

            case ShipType.Oil:
                specs.maxSpeed = 2.2f;         // 최대 속도
                specs.acceleration *= 0.5f;
                specs.rotationSpeed = 40f;     // 회전 속도 증가
                specs.mass *= 1.8f;
                break;

            case ShipType.Passenger:
                specs.maxSpeed = 2.5f;         // 최대 속도
                specs.acceleration *= 0.7f;
                specs.rotationSpeed = 50f;     // 회전 속도 증가
                specs.mass *= 1.0f;
                break;
        }
    }

    public void StartEngine()
    {
        isEngineRunning = true;
        StartCoroutine(EngineStartSequence());
    }

    private IEnumerator EngineStartSequence()
    {
        float startupTime = 0f;
        while (startupTime < 1f)
        {
            startupTime += Time.deltaTime;
            yield return null;
        }
    }

    public void StopEngine()
    {
        isEngineRunning = false;
        currentSpeed = 0f;
        isMoving = false;
    }

    public void StartDocking()
    {
        isDocking = true;
        isMoving = false;
    }

    public void EndDocking()
    {
        isDocking = false;
    }

}