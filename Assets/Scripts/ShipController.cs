using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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

    private bool hasReachedTargetRotation = false;

    private Vector3 targetDestination;
    public ShipType Type => shipType;

    private EnhancedShipController enhancedController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = specs.mass;
        startPosition = transform.position;
        SetupShipTypeSpecifications();
        enhancedController = GetComponent<EnhancedShipController>();
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
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 targetDir = (targetDestination - transform.position).normalized;
        float angle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
        bool isRotating = !hasReachedTargetRotation && Mathf.Abs(angle) > 0.1f;

        if (isRotating)
        {
            // 회전각에 따른 속도 조절
            float speedMultiplier = 1f;
            float rotationMultiplier = 1f;

            if (Mathf.Abs(angle) > 90f)
            {
                speedMultiplier = 0.3f;
                rotationMultiplier = 0.7f;
            }
            else if (Mathf.Abs(angle) > 60f)
            {
                speedMultiplier = 0.4f;
                rotationMultiplier = 0.8f;
            }
            else if (Mathf.Abs(angle) > 30f)
            {
                speedMultiplier = 0.5f;
                rotationMultiplier = 0.9f;
            }

            float rotationAmount = Mathf.Sign(angle) * specs.rotationSpeed * rotationMultiplier * Time.deltaTime;
            if (Mathf.Abs(rotationAmount) > Mathf.Abs(angle))
            {
                rotationAmount = angle;
                hasReachedTargetRotation = true;
            }

            transform.Rotate(Vector3.up, rotationAmount);
            targetSpeed = specs.maxSpeed * speedMultiplier;
        }
        else
        {
            targetSpeed = specs.maxSpeed;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, specs.acceleration * Time.deltaTime);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }

    public void Navigate(Vector3 destination)
    {
        targetDestination = destination;
        targetDirection = (destination - transform.position).normalized;
        targetDirection.y = 0;
        hasReachedTargetRotation = false; // 새로운 목적지가 설정될 때 리셋

        isMoving = true;
        isDocking = false;

        if (!isEngineRunning)
        {
            StartEngine();
            isFirstMove = true;
        }
    }
    //EnhanchedShipController에서 속도 전송시 사용
        public float GetCurrentSpeed()
    {
        return currentSpeed;
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