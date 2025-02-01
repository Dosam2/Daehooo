using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public ShipControllerTest testShip;
    public MapExpander mapExpander;

    IEnumerator Start()
    {
        // 초기 선박 테스트
        yield return new WaitForSeconds(2f);
        testShip.SetTargetPosition(new Vector3(50, 0, 50));

        // 맵 확장 테스트
        yield return new WaitForSeconds(30f);
        mapExpander.ExpandMap();
    }
}
