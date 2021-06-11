using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    bool isShake = false;

    public void Shake(float duration, float magnitude)
    {
        if (isShake)
        {
            return;
        }
        StartCoroutine(ShakeOnec(duration, magnitude));
    }

    public void Shake(float magnitude)
    {
        if (isShake)
        {
            return;
        }
        StartCoroutine(ShakeStart(magnitude));
    }

    public void ShakeStop()
    {
        isShake = false;
        StopCoroutine(ShakeOnec(0, 0));
        StopCoroutine(ShakeStart(0));
    }

    IEnumerator ShakeOnec(float duration, float magnitude)
    {
        isShake = true;
        float elapsed = 0.0f;
        while (elapsed < duration) 
        {
            Vector3 originPos = transform.localPosition;

            float x = Random.Range(-10, 10) * 0.1f * magnitude;
            float y = Random.Range(-10, 10) * 0.1f * magnitude;
            transform.localPosition = new Vector3(originPos.x + x, originPos.y + y, originPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isShake = false;
    }

    IEnumerator ShakeStart(float magnitude)
    {
        isShake = true;
        while (isShake)
        {
            Vector3 originPos = transform.localPosition;
            float x = Random.Range(-10, 10) * 0.1f * magnitude;
            float y = Random.Range(-10, 10) * 0.1f * magnitude;
            transform.localPosition = new Vector3(originPos.x + x, originPos.y + y, originPos.z);
            yield return null;
        }
    }
}
