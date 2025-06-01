using System;
using Unity.Cinemachine;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class FocusPointRayCasterSettings
    {
        public float MaxDistance = 100f;
        public float DefaultDistance = 5f;
        public LayerMask Layers = 1;

        [Space]
        public bool CenteredWeight = false;
        public float WeightRadius = 0.1f;

        [Space]
        public float SpeedNearToFar = 2f; // Speed when increasing distance
        public float SpeedFarToNear = 5f; // Speed when decreasing distance

        public bool ShowDebugSphere = true;
    }

    [RequireComponent(typeof(Camera))]
    public class CameraFocusPointRaycaster : MonoBehaviour
    {
        public FocusPointRayCasterSettings Settings;

        private Camera _camera;
        private CinemachineCamera _cinemachineCamera;

        public float CurrentFocusDistance { get; private set; }
        public Vector3 CurrentTargetPoint { get; private set; }

        public void Awake()
        {
            _camera = GetComponent<Camera>();
            _cinemachineCamera = GetComponent<CinemachineCamera>();

            var prefab = ResourceLoader.InstantiatePrefab("UnityEssentials_Prefab_CameraDoFVolume", "DoF Volume", this.gameObject.transform);
        }

        public void Start()
        {
            CurrentFocusDistance = Settings.MaxDistance;
            CurrentTargetPoint = transform.position;
        }

        public void Update()
        {
            if (_camera == null)
                return;

            var origin = transform.position;
            var direction = transform.forward;
            var targetFocusDistance = Settings.DefaultDistance;
            var targetPoint = origin + direction * Settings.MaxDistance;

            if (Settings.CenteredWeight)
            {
                // Centered Weight mode (Sphere Casting)
                if (Physics.SphereCast(origin, Settings.WeightRadius, direction, out RaycastHit hit, Settings.MaxDistance, Settings.Layers))
                {
                    targetPoint = hit.point;
                    targetFocusDistance = hit.distance + Settings.WeightRadius;
                }
            }
            else
            {
                // Spot mode (Raycasting)
                if (Physics.Raycast(origin, direction, out RaycastHit hit, Settings.MaxDistance, Settings.Layers))
                {
                    targetPoint = hit.point;
                    targetFocusDistance = hit.distance;
                }
            }

            // Determine the appropriate speed based on direction of focus change
            float focusSpeed = (targetFocusDistance > CurrentFocusDistance) ? Settings.SpeedNearToFar : Settings.SpeedFarToNear;

            // Smooth transition to the new target point
            CurrentTargetPoint = Vector3.Lerp(CurrentTargetPoint, targetPoint, Time.deltaTime * focusSpeed);
            // Smoothly interpolate focus distance
            CurrentFocusDistance = Mathf.Lerp(CurrentFocusDistance, targetFocusDistance, Time.deltaTime * focusSpeed);

            if (_cinemachineCamera != null)
                _cinemachineCamera.Lens.PhysicalProperties.FocusDistance = CurrentFocusDistance;
            else _camera.focusDistance = CurrentFocusDistance;
        }

#if UNITY_EDITOR
        // Draw debug sphere at CurrentTargetPoint
        private void OnDrawGizmos()
        {
            if (!Settings.ShowDebugSphere)
                return;

            // Use different color for clarity
            Gizmos.color = Color.cyan;

            float radius = Settings?.CenteredWeight ?? false ? Settings.WeightRadius : 0.1f;
            Gizmos.DrawWireSphere(CurrentTargetPoint, radius);
        }
#endif
    }
}
