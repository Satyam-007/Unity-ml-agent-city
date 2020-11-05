using System;
using System.Collections.Generic;
using UnityEngine;

namespace MLAgents
{

	
	public class RayPercept : RayPerception
	{
		Vector3 endPosition;
		RaycastHit hit;
		private float[] subList;

	
		public override List<float> Perceive(float rayDistance,
			float[] rayAngles, string[] detectableObjects,
			float startOffset, float endOffset)
		{
			if (subList == null || subList.Length != detectableObjects.Length + 2)
				subList = new float[detectableObjects.Length + 2];

			perceptionBuffer.Clear();
			perceptionBuffer.Capacity = subList.Length * rayAngles.Length;
            
			foreach (float angle in rayAngles)
			{
				endPosition = transform.TransformDirection(
					PolarToCartesian(rayDistance, angle));
				endPosition.y = endOffset;
				if (Application.isEditor)
				{
					Debug.DrawRay(transform.position + new Vector3(0f, startOffset, 0f),
						endPosition, Color.black, 0.01f, true);
				}

				Array.Clear(subList, 0, subList.Length);

				if (Physics.SphereCast(transform.position +
					new Vector3(0f, startOffset, 0f), 0.5f,
					endPosition, out hit, rayDistance))
				{
					for (int i = 0; i < detectableObjects.Length; i++)
					{
						if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
						{
							subList[i] = 1;
							subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
							break;
						}
					}
				}
				else
				{
					subList[detectableObjects.Length] = 1f;
				}

				Utilities.AddRangeNoAlloc(perceptionBuffer, subList);
			}

			return perceptionBuffer;
		}

		public static Vector3 PolarToCartesian(float radius, float angle)
		{
			float x = radius * Mathf.Cos(DegreeToRadian(angle));
			float z = radius * Mathf.Sin(DegreeToRadian(angle));
			return new Vector3(x, 0f, z);
		}

	}
}
