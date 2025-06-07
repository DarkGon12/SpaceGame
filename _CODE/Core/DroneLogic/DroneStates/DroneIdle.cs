using UnityEngine;

[CreateAssetMenu(fileName = "Custom_State", menuName = "Drone/States/Idle")]
public class DroneIdle : DroneState
{
    private Transform _dronePosition;


    public override void Init()
    {

    }

    public override void StartAI()
    {
        if (IsFinished) return;

    }
}