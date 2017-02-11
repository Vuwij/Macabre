using UnityEngine;
using Exceptions;
using System.Collections.Generic;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour {
        
        protected virtual void SetupBackEdgeCollider()
        {
            if (GetComponentInChildren<EdgeCollider2D>() != null) return;
            CreateBackEdgeCollider();
        }
        
        // The Default BackEdgeCollider
        protected EdgeCollider2D CreateBackEdgeCollider()
        {
            List<Vector2> backEdgePointsOfObject = new List<Vector2>();

            // Detect Edge Points here
            int leftIndex = 0;
            int rightIndex = 0;

            // Determine the indexes of the left and the right most point
            float leftMostPoint = 0;
            float rightMostPoint = 0;

            GetMaximaPoints(collisionVertices, out leftIndex, out rightIndex, out leftMostPoint, out rightMostPoint);
            
            // Determine the clockwise direction TODO: Get the correct direction
            bool clockWise = collisionVertices[leftIndex].y < collisionVertices[(leftIndex + 1) % collisionVertices.Length].y;

            // Add backEdgePoints based on direction
            int index = leftIndex;
            while (index != rightIndex)
            {
                backEdgePointsOfObject.Add(collisionVertices[index]);
                index = (clockWise ? index + 1 : index - 1) % collisionVertices.Length;
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
        
        private void AddLeftAndRightEdges(ref List<Vector2> backEdgePointsOfObject)
        {
            int leftIndex, rightIndex;
            float leftMostPoint, rightMostPoint;
            
            GetMaximaPoints(proximityVertices, out leftIndex, out rightIndex, out leftMostPoint, out rightMostPoint);
            backEdgePointsOfObject.Insert(0, proximityVertices[leftIndex]);
            backEdgePointsOfObject.Add(proximityVertices[rightIndex]);
        }
    }
}
