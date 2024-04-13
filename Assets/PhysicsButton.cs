using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    [SerializeField] private float threshold = .1f;
    [SerializeField] private float deadZone = 0.025f;

    private bool _isPressed;
    private Vector3 _startPos;
    private ConfigurableJoint _joint;
    public CameraCapture cameraCapture;
    public Camera mainCamera;
    public Vector3 defaultCameraPosition;
    public Quaternion defaultCameraRotation;

    public UnityEvent onPressed, onReleased;

    void Start()
    {
        _startPos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();

        if (mainCamera != null)
        {
            mainCamera.transform.position = defaultCameraPosition;
            mainCamera.transform.rotation = defaultCameraRotation;
        }
    }

    void Update()
    {
        if (!_isPressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }
        if (_isPressed && GetValue() - threshold <= 0)
        {
            Released();
        }
    }

    private float GetValue()
    {
        var value = Vector3.Distance(_startPos, transform.localPosition) / _joint.linearLimit.limit;

        if (Mathf.Abs(value) < deadZone)
        {
            value = 0;
        }
        return Mathf.Clamp(value, -1f, 1f);
    }

    private void Pressed() {
        _isPressed = true;
        onPressed.Invoke();
        StartCoroutine(cameraCapture.CaptureAndSavePNG());
        ResetCamera();
        Debug.Log("Pressed");
    }

    private void Released() {
        _isPressed = false;
        onReleased.Invoke();
        Debug.Log("Released");
    }

    private void ResetCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = defaultCameraPosition;
            mainCamera.transform.rotation = defaultCameraRotation;
            Debug.Log("Camera reset to default position and rotation");
        }
    }
}
