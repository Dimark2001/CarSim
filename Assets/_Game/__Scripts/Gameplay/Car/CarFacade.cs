using UnityEngine;

public class CarFacade : MonoBehaviour
{
    [field: SerializeField]
    public CarCamera Camera { get; private set; }

    [field: SerializeField]
    public CarInputController Input { get; private set; }
    
    [field: SerializeField]
    public CarMovement Movement { get; private set; }
    
    [field: SerializeField]
    public CarSeat Seat { get; private set; }
    
    [field: SerializeField]
    public CarSkeleton Skeleton { get; private set; }

    private void Reset()
    {
        Camera = GetComponentInChildren<CarCamera>(true);
        Input = GetComponentInChildren<CarInputController>(true);
        Movement = GetComponentInChildren<CarMovement>(true);
        Seat = GetComponentInChildren<CarSeat>(true);
        Skeleton = GetComponentInChildren<CarSkeleton>(true);
    }
}