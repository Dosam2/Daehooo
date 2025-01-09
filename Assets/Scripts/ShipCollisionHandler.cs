using UnityEngine;

public class ShipCollisionHandler : MonoBehaviour
{
    private ShipController shipController;
    private float lastCollisionTime;
    private float collisionCooldown = 0.5f;
    
    private void Awake()
    {
        shipController = GetComponent<ShipController>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastCollisionTime < collisionCooldown) return;
        
        lastCollisionTime = Time.time;
        
        float collisionForce = collision.impulse.magnitude;
        HandleCollision(collision.gameObject, collisionForce);
    }
    
    private void HandleCollision(GameObject other, float force)
    {
        // 충돌 대상에 따른 처리
        if (other.CompareTag("Ship"))
        {
            // 다른 배와의 충돌
            ApplyDamage(force * 1.5f);
        }
        else if (other.CompareTag("Dock"))
        {
            // 선착장과의 충돌
            ApplyDamage(force * 2f);
        }
        else if (other.CompareTag("Obstacle"))
        {
            // 장애물과의 충돌
            ApplyDamage(force);
        }
    }
    
    private void ApplyDamage(float force)
    {
        // 데미지 처리 및 시각/청각 효과
        Debug.Log($"Collision Damage: {force}");
    }
}