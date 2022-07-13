using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField] private CinemachineVirtualCamera aimCamera; 
    [SerializeField] private CinemachineVirtualCamera freeLookCamera; 
    // [SerializeField] private Canvas thirdPersonCanvas; 
    // [SerializeField] private Canvas aimCanvas;


    public void StartAiming()
    {
        aimCamera.enabled = true;
        freeLookCamera.enabled = false;
        // aimCanvas.enabled = true;
        // thirdPersonCanvas.enabled = false; 
    }
    public void CancelAiming()
    {
        aimCamera.enabled = false;
        freeLookCamera.enabled = true;
        // aimCanvas.enabled = false;
        // thirdPersonCanvas.enabled = true;
    }
}