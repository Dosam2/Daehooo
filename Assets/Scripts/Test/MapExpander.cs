using UnityEngine;

public class MapExpander : MonoBehaviour
{
    public float expansionInterval = 30f; // 30초마다 확장
    public float expansionAmount = 20f; // 한번에 20m 확장
    public GameObject dockPrefab;
    public GameObject obstaclePrefab;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        
        if(timer >= expansionInterval)
        {
            ExpandMap();
            timer = 0f;
        }
    }

    public void ExpandMap()
    {
        // 맵 경계 확장
        transform.localScale += new Vector3(expansionAmount, 0, expansionAmount);

        // 새로운 도크 생성
        Vector3 newDockPos = new Vector3(
            Random.Range(-transform.localScale.x/2, transform.localScale.x/2),
            0,
            Random.Range(-transform.localScale.z/2, transform.localScale.z/2)
        );
        Instantiate(dockPrefab, newDockPos, Quaternion.identity);

        // 장애물 생성 (3~5개)
        for(int i=0; i<Random.Range(3,6); i++)
        {
            Vector3 obstaclePos = new Vector3(
                Random.Range(-transform.localScale.x/2, transform.localScale.x/2),
                0,
                Random.Range(-transform.localScale.z/2, transform.localScale.z/2)
            );
            Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity);
        }
    }
}
