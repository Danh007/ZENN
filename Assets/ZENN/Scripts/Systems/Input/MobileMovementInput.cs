using UnityEngine;
using MyGame.Core;

namespace MyGame.Systems
{
    public class MobileMovementInput : BaseTouchInputHandler, IMovementInputProvider
    {
        protected override bool IsInMyZone(float normalizedY)
        {
            return normalizedY < splitThresholdY;
        }
    }
}