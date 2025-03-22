using System.Collections;
using UnityEngine;

namespace NekoLegends
{
    public class SwayAnimator : MonoBehaviour
    {
        [Header("Sway Settings")]
        public bool _isEnabled = false;
        public float speed = 0.2f;      // Sway speed
        public float range = 20f;       // Sway range in degrees
        public float _angle = 0f;   // Fixed angle on the X-axis

        private float localTime = 0f;
        private bool firstEnable = true; // Flag to indicate the first time the script is enabled

        void Update()
        {
            if (_isEnabled)
            {
                // Handle the first enable
                if (firstEnable)
                {
                    // Optionally, initialize localTime based on current rotation
                    // For simplicity, we'll start from time = 0
                    localTime = 0f;
                    firstEnable = false;
                }

                // Increment local time
                localTime += Time.deltaTime;

                // Calculate new rotation using Mathf.Sin for smooth oscillation
                float swayAngle = Mathf.Sin(localTime * speed * Mathf.PI * 2) * range;

                // Apply the new rotation
                transform.localRotation = Quaternion.Euler(_angle, swayAngle, 0);
            }
            else
            {
                // Reset the flag when disabled
                if (!firstEnable)
                {
                    // Optionally, reset rotation to default when sway is disabled
                    transform.localRotation = Quaternion.Euler(_angle, 0, 0);
                }
                firstEnable = true; // Reset the first enable flag
            }
        }
    }
}
