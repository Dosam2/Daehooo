using UnityEngine;

public class ShipControllerTest : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public LineRenderer pathLine;
    
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Update()
    {
        if(Input.GetMouseButtonDown(1)) // 우클릭 시 경로 설정
        {
            SetTargetPosition(Input.mousePosition);
        }

        if(isMoving)
        {
            MoveShip();
            UpdatePathVisualization();
        }
    }

    public void SetTargetPosition(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPosition = hit.point;
            isMoving = true;
            pathLine.positionCount = 0;
        }
    }

    void MoveShip()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            rotationSpeed * Time.deltaTime
        );

        if(Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            isMoving = false;
        }
    }

    void UpdatePathVisualization()
    {
        pathLine.positionCount++;
        pathLine.SetPosition(pathLine.positionCount-1, transform.position);
    }
}
