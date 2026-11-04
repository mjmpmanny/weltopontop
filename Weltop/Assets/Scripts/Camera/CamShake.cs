using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class CamShake : MonoBehaviour
{
    public Volume PostproccesingVolume;

    private CinemachineCameraOffset camOffset;

    private void Awake()
    {
        camOffset = GetComponent<CinemachineCameraOffset>();
    }

    public void ShakeCamera(float duration, float strength)
    {
        DOTween.Shake(() => camOffset.Offset, x => camOffset.Offset = x, duration, strength)
            .OnComplete(() => camOffset.Offset = Vector3.zero);

        if (PostproccesingVolume == null) return;
        StartCoroutine(ShovePostProcessing());
    }


    private IEnumerator ShovePostProcessing()
    {
        StartCoroutine(ApplyShovePostProcessing());
        yield return new WaitForSeconds(GlobalParameters.RecoverAfterShoveDelaySeconds);
        StartCoroutine(RemoveShovePostProcessing());
    }

    public IEnumerator ApplyShovePostProcessing()
    {
        float time = 0;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            PostproccesingVolume.weight += time * 2;
            yield return null;
        }

        PostproccesingVolume.weight = 1;
    }
    public IEnumerator RemoveShovePostProcessing()
    {
        float time = 0;
        Volume targetVolume = PostproccesingVolume;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            targetVolume.weight -= time * 2;
            yield return null;
        }

        targetVolume.weight = 0;
    }
}
