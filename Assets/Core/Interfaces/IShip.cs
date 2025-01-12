using UnityEngine;

// Define the ShipType enum
public enum ShipType
{
    Cargo,
    Oil,
    Passenger
}


public interface IShip
{
    ShipType Type { get; }
    void StartEngine();
    void StopEngine();
    void Navigate(Vector3 destination);
}