using Unity.Cinemachine;
using UnityEngine;

public class CameraFacade : MonoBehaviour
{
    [field: SerializeField]
    public CinemachineBrain Brain { get; private set; }
}