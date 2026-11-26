using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ZENN.Core
{
    [Serializable]
    public class LevelSaveData
    {
        public string levelId;
        public List<string> collectedCoinIds = new List<string>();

        public bool hasLastPosition;
        public Vector3 lastPosition;

        public bool hasGroundedPosition;
        public Vector3 lastGroundedPosition;

        public LevelSaveData()
        {
        }

        public LevelSaveData(string levelId)
        {
            this.levelId = levelId;
        }

        public bool IsCoinCollected(string coinId)
        {
            return !string.IsNullOrWhiteSpace(coinId) && collectedCoinIds.Contains(coinId);
        }

        public void MarkCoinCollected(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId)) return;
            if (!collectedCoinIds.Contains(coinId))
            {
                collectedCoinIds.Add(coinId);
            }
        }

        public void RecordPosition(Vector3 position, bool isGrounded)
        {
            hasLastPosition = true;
            lastPosition = position;

            if (isGrounded)
            {
                hasGroundedPosition = true;
                lastGroundedPosition = position;
            }
        }

        public bool TryGetRestorePosition(out Vector3 restorePosition)
        {
            if (hasGroundedPosition)
            {
                restorePosition = lastGroundedPosition;
                return true;
            }

            if (hasLastPosition)
            {
                restorePosition = lastPosition;
                return true;
            }

            restorePosition = default;
            return false;
        }

        public void ResetForFreshRun()
        {
            collectedCoinIds.Clear();
            hasLastPosition = false;
            lastPosition = default;
            hasGroundedPosition = false;
            lastGroundedPosition = default;
        }
        public string ToDebugString()
        {
            var sb = new StringBuilder();
            sb.Append($"[LevelSaveData] levelId='{levelId}', ");
            sb.Append($"coins({collectedCoinIds.Count})=");

            if (collectedCoinIds.Count == 0)
            {
                sb.Append("[]");
            }
            else
            {
                sb.Append("[");
                sb.Append(string.Join(", ", collectedCoinIds));
                sb.Append("]");
            }

            sb.Append($", hasLastPos={hasLastPosition} ({lastPosition})");
            sb.Append($", hasGroundedPos={hasGroundedPosition} ({lastGroundedPosition})");
            return sb.ToString();
        }
    }
}