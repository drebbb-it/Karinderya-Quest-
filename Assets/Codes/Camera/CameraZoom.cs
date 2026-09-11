using UnityEngine;
[RequireComponent(typeof(Camera))]

public class CameraZoom : MonoBehaviour
{
   [SerializeField] private float _defaultSize = 5f;
   [SerializeField] private float _zoomSpeed = 3f;
   
   private Camera _camera;
   private float _targetSize;

   private void Awake()
   {
       _camera = GetComponent<Camera>();
       
       if (!_camera.orthographic)
       {
           Debug.LogWarning($"{nameof(CameraZoom)}: Camera is not orthographic. Zooming will not work as expected.");
       }
       _targetSize = _defaultSize;
       _camera.orthographicSize = _defaultSize;
    
   }
   private void Update()
   {
       if (!Mathf.Approximately(_camera.orthographicSize, _targetSize))
        {
            _camera.orthographicSize = Mathf.Lerp(
                _camera.orthographicSize,
                _targetSize,
                Time.deltaTime * _zoomSpeed
            );
        }
   }
   private void SetZoom(float newSize)
   {
       _targetSize = _defaultSize;
   }
}
