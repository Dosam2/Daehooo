using UnityEngine;
using System.Collections.Generic;

public class PathPredictor : MonoBehaviour
{
    public LineRenderer predictionLine;
    public ShipControllerTest shipController;
    public float predictionTime = 3f;
    public float stepInterval = 0.1f;

    void Update()
    {
        if(Input.GetMouseButton(0)) // 좌클릭 드래그 중
        {
            PredictPath();
        }
    }

    void PredictPath()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            List<Vector3> pathPoints = new List<Vector3>();
            Vector3 currentPos = transform.position;
            Vector3 direction = (hit.point - currentPos).normalized;

            for(float t=0; t<predictionTime; t+=stepInterval)
            {
                currentPos += direction * shipController.moveSpeed * stepInterval;
                pathPoints.Add(currentPos);

                if(CheckCollision(currentPos))
                {
                    predictionLine.startColor = Color.red;
                    predictionLine.endColor = Color.red;
                    break;
                }
            }

            predictionLine.positionCount = pathPoints.Count;
            predictionLine.SetPositions(pathPoints.ToArray());
        }
    }

    bool CheckCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 2f);
        return colliders.Length > 0;
    }
}
