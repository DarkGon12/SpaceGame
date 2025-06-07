using UnityEngine;

public abstract class DroneState : ScriptableObject
{
    protected DroneLogic MainLogic { get; private set; }
    public bool IsFinished { get; protected set; }

    public void SetLogic(DroneLogic logic) => MainLogic = logic;

    public virtual void Init() { }

    public abstract void StartAI();
}