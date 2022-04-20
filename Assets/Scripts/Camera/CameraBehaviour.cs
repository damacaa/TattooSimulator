using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//https://www.youtube.com/watch?v=zVX9-c_aZVg
public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour instance;

    private float _rotationY = 0;
    private float _rotationX = 0;

    [SerializeField]
    private Transform _target;
    Vector3 defaultTargetPosition;

    [SerializeField]
    private float mouseSensitivity = 7;
    [SerializeField]
    private float scrollSensitivity = 0.1f;

    [SerializeField]
    internal float _distanceFromTarget = 5f;
    [SerializeField]
    private float maxDistance = 5f;
    [SerializeField]
    private float minDistance = 0.1f;
    private float _desiredDistanceFromTarget;

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;
    private Vector3 _movementVelocity = Vector3.zero;
    Vector3 targetPosition;

    [SerializeField]
    private float _smoothTime = 0.2f;

    [SerializeField]
    private Vector2 _rotationXMinMax = new Vector2(-90, 90);

    [SerializeField]
    private bool freeMovement = true;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        defaultTargetPosition = _target.position;
        targetPosition = defaultTargetPosition;
        _desiredDistanceFromTarget = maxDistance;
    }

    void Update()
    {
        // Apply clamping for x rotation 
        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

        // Apply damping between rotation changes
        _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
        transform.localEulerAngles = _currentRotation;

        // Substract forward vector of the GameObject to point its forward vector to the target
        targetPosition = Vector3.SmoothDamp(targetPosition, _target.position, ref _movementVelocity, _smoothTime);
        transform.position = targetPosition - transform.forward * _distanceFromTarget;
        _distanceFromTarget = Mathf.SmoothStep(_distanceFromTarget, _desiredDistanceFromTarget, 20f*Time.deltaTime);
    }

    public void Zoom(float amount)
    {
        _desiredDistanceFromTarget = Mathf.Clamp(_desiredDistanceFromTarget - (amount * scrollSensitivity) * _desiredDistanceFromTarget, minDistance, maxDistance);
    }

    public void Rotate(float mouseX, float mouseY)
    {
        if (freeMovement)
        {
            _rotationY += mouseX * mouseSensitivity;
            _rotationX -= mouseY * mouseSensitivity;
        }
    }

    public void RotateRight()
    {
        _rotationY -= 90;
    }

    public void RotateLeft()
    {
        _rotationY += 90;
    }

    public void RotateUp()
    {
        _rotationX += 180;
    }

    public void RotateDown()
    {
        _rotationX -= 180;
    }

    public void Reset()
    {
        _rotationY = 0f;
        _rotationX = 0f;
        _target.position = defaultTargetPosition;
        _desiredDistanceFromTarget = maxDistance;
    }

    public void MoveCenter(float x, float y)
    {
        _target.Translate(0.5f * _distanceFromTarget * mouseSensitivity * (transform.right * x + transform.up * y));
    }

    public void MoveTargetTo(Vector3 v)
    {
        _target.position = v;
    }

}
