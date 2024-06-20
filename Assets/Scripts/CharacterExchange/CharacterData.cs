using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/* The center piece of the software: a class which holds all relevant data of a character by value rather than by reference. Instances of this class can
 * then be serialized into byte[] and be sent over a network. CharacterData consists of instances of three other, more low-level classes which hold
 * data of the skeleton (SkeletonUtilities.BonesData), data of a Mesh of a SkinnedMeshRenderer (MeshUtilities.SkinnedMeshData) and data of the materials
 * of a SkinnedMeshRenderer (MaterialUtilities.MaterialsData).
 * This script furthermore stores a function which applies new CharacterData to an existing and referenced character (ApplyCharacterDataToCharacter). 
 * by Marius Rubo, 2023
 * */
namespace CharacterDataProcessing
{
    /// <summary>
    /// A class which holds all relevant data of a character by value rather than by reference.
    /// </summary>
    public class CharacterData
    {
        public int id;
        public SkeletonUtilities.BonesData BonesData;
        public MeshUtilities.SkinnedMeshData Body_MeshData;
        public MaterialUtilities.MaterialsData Body_TextureData;
        public MeshUtilities.SkinnedMeshData Eyes_MeshData;
        public MaterialUtilities.MaterialsData Eyes_TextureData;
        public MeshUtilities.SkinnedMeshData Eyebrows_MeshData;
        public MaterialUtilities.MaterialsData Eyebrows_TextureData;
        public MeshUtilities.SkinnedMeshData Hair_MeshData;
        public MaterialUtilities.MaterialsData Hair_TextureData;
        public MeshUtilities.SkinnedMeshData Teeth_MeshData;
        public MaterialUtilities.MaterialsData Teeth_TextureData;
        public MeshUtilities.SkinnedMeshData Tongue_MeshData;
        public MaterialUtilities.MaterialsData Tongue_TextureData;
        public MeshUtilities.SkinnedMeshData Clothes_MeshData;
        public MaterialUtilities.MaterialsData Clothes_TextureData;
        public MeshUtilities.SkinnedMeshData Shoes_MeshData;
        public MaterialUtilities.MaterialsData Shoes_TextureData;

        /// <summary>
        /// Just an empty constructor, sometimes needed when a CharacterData is created using deserialization in a subsequent step. 
        /// </summary>
        public CharacterData()
        {

        }

        /// <summary>
        /// A constructor which automatically goes through an existing character.
        /// </summary>
        /// <param name="charref">The referenced character from which data should be stored by value.</param>
        public CharacterData(CharacterReference charref)
        {
            id = charref.id; // charref must have an id, a skeleton and a body mesh. The rest is optional
            BonesData = new SkeletonUtilities.BonesData(charref.Bones);
            Body_MeshData = new MeshUtilities.SkinnedMeshData(charref.Body.sharedMesh, charref.Bones, charref.Body.rootBone, charref.Body.bones);
            Body_TextureData = new MaterialUtilities.MaterialsData(charref.Body.sharedMaterials); // don't specify resolution to keep at original resolution

            if (charref.Eyes != null)
            {
                Eyes_MeshData = new MeshUtilities.SkinnedMeshData(charref.Eyes.sharedMesh, charref.Bones, charref.Eyes.rootBone, charref.Eyes.bones);
                Eyes_TextureData = new MaterialUtilities.MaterialsData(charref.Eyes.sharedMaterials, 512); // specify resolution to downscale, save some data
            }

            if (charref.Eyebrows != null)
            {
                Eyebrows_MeshData = new MeshUtilities.SkinnedMeshData(charref.Eyebrows.sharedMesh, charref.Bones, charref.Eyebrows.rootBone, charref.Eyebrows.bones);
                Eyebrows_TextureData = new MaterialUtilities.MaterialsData(charref.Eyebrows.sharedMaterials, 512);
            }

            if (charref.Hair != null)
            {
                Hair_MeshData = new MeshUtilities.SkinnedMeshData(charref.Hair.sharedMesh, charref.Bones, charref.Hair.rootBone, charref.Hair.bones);
                Hair_TextureData = new MaterialUtilities.MaterialsData(charref.Hair.sharedMaterials, 512);
            }

            if (charref.Teeth != null)
            {
                Teeth_MeshData = new MeshUtilities.SkinnedMeshData(charref.Teeth.sharedMesh, charref.Bones, charref.Teeth.rootBone, charref.Teeth.bones);
                Teeth_TextureData = new MaterialUtilities.MaterialsData(charref.Teeth.sharedMaterials, 512);
            }

            if (charref.Tongue != null)
            {
                Tongue_MeshData = new MeshUtilities.SkinnedMeshData(charref.Tongue.sharedMesh, charref.Bones, charref.Tongue.rootBone, charref.Tongue.bones);
                Tongue_TextureData = new MaterialUtilities.MaterialsData(charref.Tongue.sharedMaterials, 512);
            }

            if (charref.Clothes != null)
            {
                Clothes_MeshData = new MeshUtilities.SkinnedMeshData(charref.Clothes.sharedMesh, charref.Bones, charref.Clothes.rootBone, charref.Clothes.bones);
                Clothes_TextureData = new MaterialUtilities.MaterialsData(charref.Clothes.sharedMaterials, 1024);
            }

            if (charref.Shoes != null)
            {
                Shoes_MeshData = new MeshUtilities.SkinnedMeshData(charref.Shoes.sharedMesh, charref.Bones, charref.Shoes.rootBone, charref.Shoes.bones);
                Shoes_TextureData = new MaterialUtilities.MaterialsData(charref.Shoes.sharedMaterials, 512);
            }
        }

        /// <summary>
        /// Apply CharacterData onto an existing and referenced character.
        /// </summary>
        /// <param name="chardat">The character data, i.e., the structure holding all relevant data by value which was stored in a serialized form in a chardat file</param>
        /// <param name="charref">The referenced character, i.e., the structure linking to all the parts of the character will should be transformed according to chardat</param>
        public static async Task ApplyCharacterDataToCharacter(CharacterData chardat, CharacterReference charref)
        {
            // apply new skeleton
            for (int i = 0; i < chardat.BonesData.localPositions.Length; i++)
            {
                charref.Bones[i].localPosition = chardat.BonesData.localPositions[i];
                charref.Bones[i].localRotation = chardat.BonesData.localRotations[i];
            }

            await MeshUtilities.ApplyMeshData(chardat.Body_MeshData, charref.Body, charref.Bones);
            await MaterialUtilities.ApplyMaterialsData(chardat.Body_TextureData, charref.Body);

            if (charref.Eyes != null & chardat.Eyes_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Eyes_MeshData, charref.Eyes, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Eyes_TextureData, charref.Eyes);
            }

            if (charref.Eyebrows != null & chardat.Eyebrows_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Eyebrows_MeshData, charref.Eyebrows, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Eyebrows_TextureData, charref.Eyebrows);
            }

            if (charref.Hair != null & chardat.Hair_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Hair_MeshData, charref.Hair, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Hair_TextureData, charref.Hair);
            }

            if (charref.Teeth != null & chardat.Teeth_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Teeth_MeshData, charref.Teeth, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Teeth_TextureData, charref.Teeth);
            }

            if (charref.Tongue != null & chardat.Tongue_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Tongue_MeshData, charref.Tongue, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Tongue_TextureData, charref.Tongue);
            }


            if (charref.Clothes != null & chardat.Clothes_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Clothes_MeshData, charref.Clothes, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Clothes_TextureData, charref.Clothes);
            }

            if (charref.Shoes != null & chardat.Shoes_MeshData != null)
            {
                await MeshUtilities.ApplyMeshData(chardat.Shoes_MeshData, charref.Shoes, charref.Bones);
                await MaterialUtilities.ApplyMaterialsData(chardat.Shoes_TextureData, charref.Shoes);
            }

        }
    }
}
