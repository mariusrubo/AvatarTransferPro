using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/* Functionality to handle data of a Mesh by value. A mesh consists of a range of data structures. At its core, one may argue, is the Vector3[] array of the vertices
 * which define the actual shape of the object. In addition, the mesh is linked to bones in the skeleton, is partitioned into submeshes, stores boneweights and bindposes
 * which define how it is streteched when the underlying skeleton moves, can have blendshapes etc. These data structures are all stored here by value so that they can
 * be serialized/deserialized and applied to another SkinnedMeshRenderer.
 * by Marius Rubo, 2023
 * */
namespace CharacterDataProcessing
{
    public class MeshUtilities : MonoBehaviour
    {
        /// <summary>
        /// A class which holds all relevant data of the Mesh of a SkinnedMeshRenderer.
        /// </summary>
        public class SkinnedMeshData
        {
            public int rootBoneIndex; // the index of the root bone in the skeleton
            public int[] bonesIndices; // indices of all the bones to which the skin links in the skeleton

            // the Mesh
            public Vector3[] vertices;
            public int[] triangles;
            public Color[] colors;
            public Vector2[] uvs;

            // bounds
            public Vector3 boundsCenter;
            public Vector3 boundsSize;

            // submeshes
            public int[][] submeshes;

            // boneweights
            public int[,] BoneWeightIndices;
            public float[,] BoneWeightWeights;
            public Matrix4x4[] bindposes;

            // blendshapes
            public int blendshapeCount;
            public int verticesCount;
            public string[] blendshapeNames;
            public Vector3[][] deltaVertices;
            public Vector3[][] deltaNormals;
            public Vector3[][] deltaTangents;

            public SkinnedMeshData()
            {

            }

            /// <summary>
            /// A constructor which automatically goes through an existing character. If the mesh links to bones, we must pass an array of those bones
            /// and an array of the whole skeleton's bones in order to be able to note the indices of the referenced bones. 
            /// </summary>
            /// <param name="mesh">The SkinnedMeshRenderer's mesh.</param>
            /// <param name="skeleton_bones">all the bones in the skeleton (including those which may not be linked in the SkinnedMeshRenderer).</param>
            /// <param name="skin_rootBone">the skinnedMeshRenderer's root bone.</param>
            /// <param name="skin_bones">all the bones linked in the skinnedMeshRenderer.</param>
            public SkinnedMeshData(Mesh mesh, Transform[] skeleton_bones = null, Transform skin_rootBone = null, Transform[] skin_bones = null)
            {
                InitializeBoneData(skeleton_bones, skin_rootBone, skin_bones);
                InitializeMeshData(mesh);
            }

            /// <summary>
            /// Initiate the retreival of the indices of the rootBone and the whole array of bones within the skeleton.
            /// </summary>
            /// <param name="skeleton_bones">see above.</param>
            /// <param name="skin_rootBone">see above.</param>
            /// <param name="skin_bones">see above.</param>
            private void InitializeBoneData(Transform[] skeleton_bones, Transform skin_rootBone, Transform[] skin_bones)
            {
                rootBoneIndex = GetBoneIndex(skeleton_bones, skin_rootBone);
                bonesIndices = GetBonesIndices(skeleton_bones, skin_bones);
            }

            /// <summary>
            /// Find the index of a single bone Transform within an entire array.
            /// </summary>
            /// <param name="bones">Array of all of the skeleton's bones which should contain the specified bone.</param>
            /// <param name="bone">The bone of which we would like to know the index in the skeleton.</param>
            /// <returns>The index of the bone within the whole skeleton (bones).</returns>
            private int GetBoneIndex(Transform[] bones, Transform bone)
            {
                if (bones != null && bone != null)
                {
                    for (int i = 0; i < bones.Length; i++)
                    {
                        if (bone.gameObject == bones[i].gameObject)
                            return i;
                    }
                }
                return -1;
            }

            /// <summary>
            /// As above, for for an array of bones at once: Find the indices of bones within an entire array.
            /// </summary>
            /// <param name="bones">Array of all of the skeleton's bones which should contain the specified bone.</param>
            /// <param name="skin_bones">The bones of which we would like to know the indices in the skeleton.</param>
            /// <returns>The indices of the bones within the whole skeleton (bones).</returns>

            private int[] GetBonesIndices(Transform[] skeleton_bones, Transform[] bones)
            {
                if (skeleton_bones == null || bones == null)
                    return new int[0];

                int[] indices = new int[bones.Length];
                for (int i = 0; i < bones.Length; i++)
                {
                    indices[i] = GetBoneIndex(skeleton_bones, bones[i]);
                }
                return indices;
            }

            /// <summary>
            /// Obtain data from a Mesh and store it by value in this SkinnedMeshData object.
            /// </summary>
            /// <param name="mesh">The SkinnedMeshRenderer's Mesh to be stored by value.</param>
            private void InitializeMeshData(Mesh mesh)
            {
                vertices = mesh.vertices;
                triangles = mesh.triangles;
                colors = mesh.colors;
                uvs = mesh.uv;

                boundsCenter = mesh.bounds.center;
                boundsSize = mesh.bounds.size;

                // submeshes
                submeshes = new int[mesh.subMeshCount][];
                for (int i = 0; i < submeshes.GetLength(0); i++) submeshes[i] = mesh.GetTriangles(i);

                // boneweights
                BoneWeightIndices = new int[mesh.boneWeights.Length, 4]; // multidimensional; they all have 4 entries
                BoneWeightWeights = new float[mesh.boneWeights.Length, 4];
                for (int i = 0; i < mesh.boneWeights.Length; i++)
                {
                    BoneWeight bw = mesh.boneWeights[i]; // load once for all 8 array entries: reduced time for this step from 32 to 8 sec
                    BoneWeightIndices[i, 0] = bw.boneIndex0;
                    BoneWeightIndices[i, 1] = bw.boneIndex1;
                    BoneWeightIndices[i, 2] = bw.boneIndex2;
                    BoneWeightIndices[i, 3] = bw.boneIndex3;

                    BoneWeightWeights[i, 0] = bw.weight0;
                    BoneWeightWeights[i, 1] = bw.weight1;
                    BoneWeightWeights[i, 2] = bw.weight2;
                    BoneWeightWeights[i, 3] = bw.weight3;
                }

                bindposes = mesh.bindposes;

                // blendshapes
                blendshapeCount = mesh.blendShapeCount;
                verticesCount = mesh.vertices.Length;
                blendshapeNames = new string[blendshapeCount];
                deltaVertices = new Vector3[blendshapeCount][];
                deltaNormals = new Vector3[blendshapeCount][];
                deltaTangents = new Vector3[blendshapeCount][];
                for (int i = 0; i < blendshapeCount; i++)
                {
                    deltaVertices[i] = new Vector3[verticesCount];
                    deltaNormals[i] = new Vector3[verticesCount];
                    deltaTangents[i] = new Vector3[verticesCount];
                    mesh.GetBlendShapeFrameVertices(i, 0, deltaVertices[i], deltaNormals[i], deltaTangents[i]);
                    blendshapeNames[i] = mesh.GetBlendShapeName(i);
                }
            }
        }

        /// <summary>
        /// Apply the data in a SkinnedMeshData object onto an existing SkinnedMeshRenderer's mesh.
        /// </summary>
        /// <param name="data">The SkinnedMeshData to be applied to that mesh.</param>
        /// <param name="skin">The SkinnedMeshRenderer's onto which the new data should be applied.</param>
        /// <param name="bones">An array of the whole skeleton so that the rootBone and bones can be linked.</param>
        public static async Task ApplyMeshData(SkinnedMeshData data, SkinnedMeshRenderer skin, Transform[] bones = null)
        {
            // link skin's rootbone and bones
            if (bones != null)
            {
                skin.rootBone = bones[data.rootBoneIndex];

                if (data.bonesIndices != null && data.bonesIndices.Length > 0)
                {
                    Transform[] skinBones = new Transform[data.bonesIndices.Length];
                    for (int i = 0; i < data.bonesIndices.Length; i++)
                    {
                        skinBones[i] = bones[data.bonesIndices[i]];
                    }

                    skin.bones = skinBones;
                }
            }


            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mesh.vertices = data.vertices;
            mesh.triangles = data.triangles;
            mesh.colors = data.colors;
            mesh.uv = data.uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            mesh.bounds = new Bounds(data.boundsCenter, data.boundsSize);
            mesh.RecalculateBounds();

            // submeshes
            if (data.submeshes != null) mesh.subMeshCount = data.submeshes.GetLength(0);
            else mesh.subMeshCount = 0;
            for (int i = 0; i < mesh.subMeshCount; i++) mesh.SetTriangles(data.submeshes[i], i);

            // boneweights
            BoneWeight[] boneweights = new BoneWeight[data.BoneWeightIndices.GetLength(0)];
            for (int i = 0; i < boneweights.Length; i++)
            {
                boneweights[i].boneIndex0 = data.BoneWeightIndices[i, 0];
                boneweights[i].boneIndex1 = data.BoneWeightIndices[i, 1];
                boneweights[i].boneIndex2 = data.BoneWeightIndices[i, 2];
                boneweights[i].boneIndex3 = data.BoneWeightIndices[i, 3];
                boneweights[i].weight0 = data.BoneWeightWeights[i, 0];
                boneweights[i].weight1 = data.BoneWeightWeights[i, 1];
                boneweights[i].weight2 = data.BoneWeightWeights[i, 2];
                boneweights[i].weight3 = data.BoneWeightWeights[i, 3];
            }
            mesh.boneWeights = boneweights;

            mesh.bindposes = data.bindposes;
            mesh.ClearBlendShapes();
            for (int i = 0; i < data.blendshapeCount; i++)
            {
                mesh.AddBlendShapeFrame(data.blendshapeNames[i], 100, data.deltaVertices[i], data.deltaNormals[i], data.deltaTangents[i]);
            }

            skin.sharedMesh = Instantiate(skin.sharedMesh);
            skin.sharedMesh = mesh;

            await Task.Yield();
        }

    }
}
