using UnityEngine;
using Exceptions;
using System.Collections.Generic;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour {
        
        // The sorting layer edge for finding the object
        private EdgeCollider2D sortingLayerEdge
        {
            get { return SetupBackEdgeCollider(); }
        }
        private EdgeCollider2D SetupBackEdgeCollider()
        {
            return GetComponentInChildren<EdgeCollider2D>() ?? CreateBackEdgeCollider();
        }
        
        // The Default BackEdgeCollider
        // TODO : Unit Test
        protected EdgeCollider2D CreateBackEdgeCollider()
        {
            List<Vector2> backEdgePointsOfObject = new List<Vector2>();

            // Detect Edge Points here
            int leftIndex = 0;
            int rightIndex = 0;

            // Determine the indexes of the left and the right most point
            float leftMostPoint = 0;
            float rightMostPoint = 0;

            GetMaximaPoints(SpriteColliderVectices, out leftIndex, out rightIndex, out leftMostPoint, out rightMostPoint);
            
            // Determine the clockwise direction TODO: Get the correct direction
            bool clockWise = SpriteColliderVectices[leftIndex].y < SpriteColliderVectices[(leftIndex + 1) % SpriteColliderVectices.Length].y;

            // Add backEdgePoints based on direction
            int index = leftIndex;
            while (index != rightIndex)
            {
                backEdgePointsOfObject.Add(SpriteColliderVectices[index]);
                index = (clockWise ? index + 1 : index - 1) % SpriteColliderVectices.Length;
            }

            // Add the left line and the right line
            AddLeftAndRightEdges(ref backEdgePointsOfObject);

            // Use the Edge Points to create a back edge
            EdgeCollider2D backOfObject = gameObject.AddComponent<EdgeCollider2D>();
            backOfObject.points = backEdgePointsOfObject.ToArray();
            backOfObject.isTrigger = true;
            return backOfObject;
        }

        private void GetMaximaPoints(Vector2[] points, out int leftIndex, out int rightIndex, out float leftMostPoint, out float rightMostPoint)
        {
            leftIndex = 0;
            rightIndex = 0;
            leftMostPoint = points[0].x;
            rightMostPoint = points[0].x;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x < leftMostPoint)
                {
                    leftIndex = i;
                    leftMostPoint = points[i].x;
                }
                if (points[i].x > rightMostPoint)
                {
                    rightIndex = i;
                    rightMostPoint = points[i].x;
                }
            }
        }

        protected abstract Collider2D proximityBox { get; }
        protected abstract Vector2[] SpriteProximityVertices { get; }

        private void AddLeftAndRightEdges(ref List<Vector2> backEdgePointsOfObject)
        {
            int leftIndex, rightIndex;
            float leftMostPoint, rightMostPoint;
            
            GetMaximaPoints(SpriteProximityVertices, out leftIndex, out rightIndex, out leftMostPoint, out rightMostPoint);
            backEdgePointsOfObject.Insert(0, SpriteProximityVertices[leftIndex]);
            backEdgePointsOfObject.Add(SpriteProximityVertices[rightIndex]);
        }
    }
}
