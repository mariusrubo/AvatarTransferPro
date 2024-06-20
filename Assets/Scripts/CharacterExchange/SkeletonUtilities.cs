using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The Mesh of a SkinnedMeshRenderer is tied to an underlying skeleton structure where the relative positions and rotations of all specified joints are stored
 * (e.g., the position and rotation of the left elbow relative to the left shoulder). Therefore the positions and rotations of these joints, or often referred to as bones,
 * must also be transmitted when a character is recreated. 
 * by Marius Rubo, 2023
 * */
namespace CharacterDataProcessing
{
    public class SkeletonUtilities : MonoBehaviour
    {
        /// <summary>
        /// A simple class holding the relative positions and rotations of all the bones in the skeleton.
        /// </summary>
        public class BonesData
        {
            public Vector3[] localPositions;
            public Quaternion[] localRotations;

            public BonesData()
            {
                localPositions = new Vector3[0];
                localRotations = new Quaternion[0];
            }

            /// <summary>
            /// A constructor which automatically obtains all required data from the skeleton.
            /// </summary>
            /// <param name="transforms">Array of the whole skeleton. Often the topmost parent is called "Root".</param>
            public BonesData(Transform[] transforms)
            {
                localPositions = new Vector3[transforms.Length];
                localRotations = new Quaternion[transforms.Length];
                for (int i = 0; i < transforms.Length; i++)
                {
                    localPositions[i] = transforms[i].localPosition;
                    localRotations[i] = transforms[i].localRotation;
                }
            }
        }


    }
}
