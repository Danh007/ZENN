using UnityEngine;
using ZENN.Core;

namespace ZENN.Systems
{
    public class MobileMovementInput : BaseTouchInputHandler, IMovementInputProvider
    {
        protected override bool IsInMyZone(float normalizedY)
        {
            return normalizedY < splitThresholdY;
        }
    }
}