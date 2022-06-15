using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private int priorityBoostAmount = 10;

    // [SerializeField] private Canvas thirdPersonCanvas; 
    // [SerializeField] private Canvas aimCanvas;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        // aimCanvas.enabled = false;
        // thirdPersonCanvas.enabled = true;
    }
    public void StartAiming()
    {
        _virtualCamera.Priority += priorityBoostAmount;
        // aimCanvas.enabled = true;
        // thirdPersonCanvas.enabled = false; 
    }
    public void CancelAiming()
    {
        _virtualCamera.Priority -= priorityBoostAmount;
        // aimCanvas.enabled = false;
        // thirdPersonCanvas.enabled = true;
    }
}