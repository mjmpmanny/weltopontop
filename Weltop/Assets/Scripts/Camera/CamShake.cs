using System;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CamShake : MonoBehaviour
{
    public CinemachineCamera playerCamera;

    private CinemachineCameraOffset camOffset;

    private void Awake()
    {
        camOffset = playerCamera.GetComponent<CinemachineCameraOffset>();
    }

    public void ShakeCamera(float duration, float strength)
    {
        DOTween.Shake(() => camOffset.Offset, x => camOffset.Offset = x, duration, strength)
            .OnComplete(() => camOffset.Offset = Vector3.zero);
    }
}
