using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace NekoLegends
{
    public class DemoCameraScene : DemoScenes
    {
        [SerializeField] private DemoCameraController _cameraController;
        [Space]
        [SerializeField] private Button ChangeCameraBtn, NextVisibleBtn, PrevVisibleBtn;

        [SerializeField] private bool DisableCameraControl;
        [SerializeField] private bool FocusOnNewTargets;

        [Header("Target Configuration")]
        [SerializeField] private bool UseSceneTargetPositions; // New Checkbox
        [SerializeField] private List<Transform> Targets; // Local Targets list

        [Space]
        [SerializeField] private Button ToggleSpotLightBtn;
        [SerializeField] private Light Spotlight;

        private int currentTargetIndex, currentVisibleTargetIndex;

        [SerializeField] private float transitionDuration = 1.0f;
        public bool IsMovingTarget { get; private set; }

        // Reference to the active coroutine
        private Coroutine moveTargetCoroutine;

        // The dummy target transform that the camera will actually follow
        private Transform dummyTarget;  // Offset to be added to the dummy target position
        [SerializeField] private Vector3 dummyTargetOffset;


        // New GameObject to hold the list of items
        [SerializeField] private GameObject AutoHideContainer;
        private List<GameObject> items;
        
       protected override void Start()
        {
            base.Start();

            if (UseSceneTargetPositions)
            {
                if (TargetPositions != null && TargetPositions.Count > 0)
                {
                    Targets = new List<Transform>(TargetPositions);
                }
                else
                {
                    Debug.LogWarning("UseSceneTargetPositions is enabled, but TargetPositions in DemoScenes are not set.");
                }
            }

            GameObject dummyGO = new GameObject("CameraDummyTarget");
            dummyTarget = dummyGO.transform;

            if (Targets.Count > 0)
            {
                dummyTarget.position = Targets[currentTargetIndex].position + dummyTargetOffset;
            }

            if (!DisableCameraControl && _cameraController != null)
            {
                _cameraController.target = dummyTarget;
            }

            if (AutoHideContainer != null)
            {
                items = new List<GameObject>();
                foreach (Transform child in AutoHideContainer.transform)
                {
                    items.Add(child.gameObject);
                }
            }

            InitTargets();
        }

        protected override void OnEnable()
        {
            if (ChangeCameraBtn)
                ChangeCameraBtn.onClick.AddListener(ChangeTarget);
            if (NextVisibleBtn)
                NextVisibleBtn.onClick.AddListener(ChangeVisibleTarget);
            if (PrevVisibleBtn)
                PrevVisibleBtn.onClick.AddListener(ChangeVisibleTargetBackward);
            if (ToggleSpotLightBtn && Spotlight)
                ToggleSpotLightBtn.onClick.AddListener(ToggleSpotLightBtnHandler);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (ChangeCameraBtn)
                ChangeCameraBtn.onClick.RemoveListener(ChangeTarget);
            if (NextVisibleBtn)
                NextVisibleBtn.onClick.RemoveListener(ChangeVisibleTarget);
            if (PrevVisibleBtn)
                PrevVisibleBtn.onClick.RemoveListener(ChangeVisibleTargetBackward);

            if (ToggleSpotLightBtn && Spotlight)
                ToggleSpotLightBtn.onClick.RemoveListener(ToggleSpotLightBtnHandler);
            base.OnDisable();
        }

        private void InitTargets()
        {
            currentVisibleTargetIndex = 0;

            for (int i = 0; i < Targets.Count; i++)
            {
                Transform targetTransform = Targets[i];
                targetTransform.gameObject.SetActive(i == 0);
            }

            if (Targets.Count > 0)
            {
                SetDescriptionText(Targets[0].name);
            }

            // Initialize items visibility
            UpdateItemsVisibility();
        }

        private void ChangeTarget()
        {
            if (Targets.Count == 0) return;

            currentTargetIndex = (currentTargetIndex + 1) % Targets.Count;
            if (!DisableCameraControl)
            {
                MoveTargetSmoothly(Targets[currentTargetIndex].position, transitionDuration);
                SetDescriptionText(Targets[currentTargetIndex].name);
            }
        }

        private void ChangeVisibleTarget()
        {
            if (Targets.Count == 0) return;

            currentVisibleTargetIndex = (currentVisibleTargetIndex + 1) % Targets.Count;

            // Update items visibility
            UpdateItemsVisibility();

            for (int i = 0; i < Targets.Count; i++)
            {
                int targetIndex = (i + currentVisibleTargetIndex) % Targets.Count;
                Transform targetTransform = Targets[targetIndex];
                targetTransform.gameObject.SetActive(i == 0);
            }

            SetDescriptionText(Targets[currentVisibleTargetIndex].name);

            if (FocusOnNewTargets && !DisableCameraControl)
            {
                _cameraController.AutoDOFTarget = Targets[currentVisibleTargetIndex];
                MoveTargetSmoothly(Targets[currentVisibleTargetIndex].position, transitionDuration);
            }
        }

        private void ChangeVisibleTargetBackward()
        {
            if (Targets.Count == 0) return;

            if (currentVisibleTargetIndex == 0)
                currentVisibleTargetIndex = Targets.Count - 1;
            else
                currentVisibleTargetIndex--;

            // Update items visibility
            UpdateItemsVisibility();

            for (int i = 0; i < Targets.Count; i++)
            {
                int targetIndex = (i + currentVisibleTargetIndex) % Targets.Count;
                Transform targetTransform = Targets[targetIndex];
                targetTransform.gameObject.SetActive(i == 0);
            }

            SetDescriptionText(Targets[currentVisibleTargetIndex].name);

            if (FocusOnNewTargets && !DisableCameraControl)
            {
                _cameraController.AutoDOFTarget = Targets[currentVisibleTargetIndex];
                MoveTargetSmoothly(Targets[currentVisibleTargetIndex].position, transitionDuration);
            }
        }

        private void UpdateItemsVisibility()
        {
            if (items == null || items.Count == 0) return;

            int prevIndex = (currentVisibleTargetIndex - 1 + Targets.Count) % Targets.Count;
            int nextIndex = (currentVisibleTargetIndex + 1) % Targets.Count;

            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(i == currentVisibleTargetIndex || i == prevIndex || i == nextIndex);
            }

            // Hide all other items after the transition duration
            StartCoroutine(HideOtherItemsAfterTransition());
        }

        private IEnumerator HideOtherItemsAfterTransition()
        {
            yield return new WaitForSeconds(transitionDuration);

            for (int i = 0; i < items.Count; i++)
            {
                if (i != currentVisibleTargetIndex)
                {
                    items[i].SetActive(false);
                }
            }
        }

        private void ToggleSpotLightBtnHandler()
        {
            Spotlight.gameObject.SetActive(!Spotlight.gameObject.activeSelf);
        }

        private void MoveTargetSmoothly(Vector3 newPosition, float duration)
        {
            if (moveTargetCoroutine != null)
            {
                StopCoroutine(moveTargetCoroutine);
            }

            moveTargetCoroutine = StartCoroutine(SmoothMoveTarget(newPosition + dummyTargetOffset, duration));
        }

        private IEnumerator SmoothMoveTarget(Vector3 newPosition, float duration)
        {
            IsMovingTarget = true;
            Vector3 startPosition = dummyTarget.position;
            Vector3 endPosition = newPosition;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = Mathf.SmoothStep(0f, 1f, t);
                dummyTarget.position = Vector3.Lerp(startPosition, endPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            dummyTarget.position = endPosition;
            IsMovingTarget = false;

            if (currentTargetIndex >= 0 && currentTargetIndex < Targets.Count)
            {
                _cameraController.AutoDOFTarget = Targets[currentTargetIndex];
            }

            moveTargetCoroutine = null;
        }
    }
}
