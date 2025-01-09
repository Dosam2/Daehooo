[System.Serializable]
public class ShipSpecification
{
    public float maxSpeed;         // 직진 시 최대 속도
    public float rotatingSpeed;    // 회전 시 속도
    public float acceleration;     // 가속도
    public float rotationSpeed;    // 회전 각속도
    public float mass = 1000f;
    public float brakingPower = 2f;
    public float minSpeedThreshold = 0.1f;
    public float dockingDistance = 20f;
    public float collisionDamageMultiplier = 1f;
}