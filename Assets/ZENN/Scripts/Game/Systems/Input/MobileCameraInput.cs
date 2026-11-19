using UnityEngine;
using ZENN.Core;

namespace ZENN.Systems
{
    public class MobileCameraInput : BaseTouchInputHandler, ICameraInputProvider
    {
        protected override bool IsInMyZone(float normalizedY)
        {
            return normalizedY >= splitThresholdY;
        }
    }
}