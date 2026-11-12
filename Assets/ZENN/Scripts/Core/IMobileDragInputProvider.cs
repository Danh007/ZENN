using UnityEngine;

namespace MyGame.Core
{
    public interface IMobileDragInputProvider
    {
        bool IsDragging { get; }
        Vector2 InputVector { get; }
    }
}