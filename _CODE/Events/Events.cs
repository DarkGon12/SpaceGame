
public class StartDroneWorkEvent 
{
    public string DroneName;
}

public class ReturnDroneWorkEvent
{
    public string DroneName;
}

public class StopDroneWorkEvent
{
    public string DroneName;
}

public class UpdateUserInventoryEvent
{
    public InventoryData ResourceData;
}

public class BuyDroneEvent
{
    public int DroneCount;
}