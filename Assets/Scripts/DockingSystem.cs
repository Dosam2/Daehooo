using System.Collections;
using UnityEngine;

public class DockingSystem : MonoBehaviour
{
    private ShipController shipController;
    private Transform targetDock;
    
    [SerializeField] private float fadeTime = 2f;
    private Vector3 startPosition;
    
    private void Awake()
    {
        shipController = GetComponent<ShipController>();
        startPosition = transform.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dock"))
        {
            shipController.OnDockReached();
        }
    }
    
    private IEnumerator DockingSequence()
    {
        // isDockingMode = true;
        shipController.StartDocking();
        shipController.StopEngine();

        // 페이드 아웃
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
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

        // 화물선인 경우 5초 후 리스폰
        if (shipController.Type == ShipType.Cargo)
        {
            yield return new WaitForSeconds(5f);
            RespawnShip();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void RespawnShip()
    {
        transform.position = startPosition;
        
        // 렌더러 초기화
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = 1f;
            renderer.material.color = color;
        }

        // isDockingMode = false;
        shipController.EndDocking();
    }
}