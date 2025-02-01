using UnityEngine;
public class CargoSystem : MonoBehaviour
{
    public int maxCapacity = 1000;
    public int currentCargo = 0;
    
    public void LoadCargo(int amount)
    {
        currentCargo = Mathf.Clamp(currentCargo + amount, 0, maxCapacity);
    }
    
    public void UnloadCargo(int amount)
    {
        currentCargo = Mathf.Clamp(currentCargo - amount, 0, maxCapacity);
    }
}
