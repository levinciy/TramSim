using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MistakeSignal : MonoBehaviour{
    [Header("UI")]
    public Image topFlash;
    public Image bottomFlash;
    public Image leftFlash;
    public Image rightFlash;

    [Header("Camera")]
    public Camera mainCamera;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.3f;

    [Header("Effect")]
    public float fadeSpeed = 5f;
    public float maxAlpha = 0.6f;

    private Vector3 originalCameraPos;
    private bool isShaking = false;


    void Start(){
        mainCamera=Camera.main;
        originalCameraPos = mainCamera.transform.localPosition;
        topFlash.enabled=false;
        bottomFlash.enabled=false;
        leftFlash.enabled=false;
        rightFlash.enabled=false;
    }

    public void triggerViolation(){
        StartCoroutine(FadeInFlash(leftFlash));
        StartCoroutine(FadeInFlash(rightFlash));
        StartCoroutine(FadeInFlash(topFlash));
        StartCoroutine(FadeInFlash(bottomFlash));

        if(!isShaking){
            StartCoroutine(ShakeCamera());
        }

        Debug.Log("триггерд");
    }

    IEnumerator FadeInFlash(Image flash){
        flash.color = new Color(1,0,0,0);
        flash.enabled = true;

        float t = 0;
        while (t<1f){
            t+=Time.unscaledDeltaTime*fadeSpeed;
            float alpha = Mathf.Lerp(0, maxAlpha, t);
            flash.color = new Color(1, 0, 0, alpha);
            yield return null;
        }


        t = 0;
        while (t<1f){
            t+=Time.unscaledDeltaTime*fadeSpeed;
            float alpha = Mathf.Lerp(maxAlpha, 0, t);
            flash.color = new Color(1, 0, 0, alpha);
            yield return null;
        }

        flash.enabled = false;
    }

    IEnumerator ShakeCamera(){
        isShaking = true;
        float elapsed = 0f;

        while (elapsed<shakeDuration){
            float x = Random.Range(-1f, 1f)*shakeIntensity;
            float y = Random.Range(-1f, 1f)*shakeIntensity;

            mainCamera.transform.localPosition = originalCameraPos + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;

        }

        mainCamera.transform.localPosition = originalCameraPos;
        isShaking = false;
    }
}
