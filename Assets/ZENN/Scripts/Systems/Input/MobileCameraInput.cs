using UnityEngine;
using MyGame.Core;

namespace MyGame.Systems
{
    public class MobileCameraInput : BaseTouchInputHandler, ICameraInputProvider
    {
        protected override bool IsInMyZone(float normalizedY)
        {
            return normalizedY >= splitThresholdY;
        }
    }
}