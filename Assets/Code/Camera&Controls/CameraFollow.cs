using UnityEngine;
using System.Collections;
using UnityEngine.U2D;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float overshootAmount = 0.1f;

    //[SerializeField] private float sizeSmoothSpeed = 0.125f;
    //[SerializeField] private float sizeMultiplier = 1.0f;
    //private float basePPU;

    private Vector3 velocity = Vector3.zero;
    private float shakeMagnitude;
    private float shakeDuration;
    private float shakeTimer;
    //private PixelPerfectCamera pixelPerfectCamera;

    void Start()
    {
        //pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        //basePPU = pixelPerfectCamera.assetsPPU;
    }

    void FixedUpdate()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("cameraTarget").transform;
        Vector3 finalPosition = CalculatePosition();
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref velocity, smoothSpeed);

        //AdjustCameraSize();
    }

    private Vector3 CalculatePosition()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            return GetShakenPosition();
        }
        return GetFollowingPosition();
    }

    private Vector3 GetShakenPosition()
    {
        Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
        shakeOffset.z = 0;  // Preserve the camera's fixed z position
        return new Vector3(target.position.x, target.position.y, target.position.z) + offset + shakeOffset;
    }

    private Vector3 GetFollowingPosition()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z) + offset;
        Vector3 overshootPosition = targetPosition + (velocity * overshootAmount);
        return overshootPosition;
    }

    /*private void AdjustCameraSize()
    {
        if (target != null)
        {
            //TODO: smooth the size change
            float targetSize = target.localScale.x * sizeMultiplier;
            int newAssetsPPU = Mathf.RoundToInt(basePPU * targetSize);
            pixelPerfectCamera.assetsPPU = newAssetsPPU;
        }
    }*/

    public void ShakeCamera(float magnitude, float duration)
    {
        shakeMagnitude = magnitude;
        shakeDuration = duration;
        shakeTimer = duration;
    }
}