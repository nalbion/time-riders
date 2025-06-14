using UnityEngine;

/// <summary>
/// ScriptableObject for configuring bike parameters (speed, acceleration, etc.).
/// </summary>
[CreateAssetMenu(fileName = "BikeConfig", menuName = "TimeRiders/BikeConfig", order = 1)]
public class BikeConfig : ScriptableObject
{
    [Header("Movement Settings")]
    [Tooltip("Maximum speed in km/h")]
    public float maxSpeed = 30f;

    [Tooltip("Acceleration rate in m/s²")]
    public float acceleration = 5f;

    [Header("Physics Settings")]
    [Tooltip("Bike mass in kg")]
    public float mass = 100f;
    [Tooltip("Wheel radius in meters")]
    public float wheelRadius = 0.3f;
    [Tooltip("Suspension spring strength")]
    public float suspensionSpring = 18000f;
    [Tooltip("Suspension damper strength")]
    public float suspensionDamper = 7000f;
}
