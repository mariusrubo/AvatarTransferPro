using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A class which stores all relevant parts of a character by reference, i.e., links to those parts. This centralized reference helps to
 * (a) obtain the relevant data by value which are then stored in CharacterData and (b) apply CharacterData onto a character. Note that the character's
 * parts are searched here by name, so the script only works if the names meet the expectations. Common workflows are to either rename all the GameObjects 
 * manually to fit the description or to implement different strings in this script to directly match the names in characters as they are exported from whatever
 * character creation tool you are using. Note that some character creation tools are not consistent in the naming of parts, e.g., "Shoes" can be called differently 
 * depending on what sort of shoes they are. Make sure your approach covers all use cases.
 * by Marius Rubo, 2023
 * */
namespace CharacterDataProcessing
{
    /// <summary>
    /// A class which holds references to all relevant data of a character.
    /// </summary>
    public class CharacterReference
    {
        public GameObject playerGO; // the actual player as it has been instantiated
        public int id;
        public GameObject Root;
        public Transform[] Bones;
        public SkinnedMeshRenderer Body;
        public SkinnedMeshRenderer Eyes;
        public SkinnedMeshRenderer Eyebrows;
        public SkinnedMeshRenderer Hair;
        public SkinnedMeshRenderer Teeth;
        public SkinnedMeshRenderer Tongue;
        public SkinnedMeshRenderer Clothes;
        public SkinnedMeshRenderer Shoes;

        /// <summary>
        /// The only constructor.
        /// </summary>
        /// <param name="go">The character itself, i.e., the gameObject which is the parent of all the parts such as Root, Body etc.</param>
        /// <param name="id">An id which is passed to CharacterData and used to name the data file when stored.</param>
        public CharacterReference(GameObject go, int id = 0)
        {
            playerGO = go;
            this.id = id;
            InitializeCharacterComponents(go);
        }

        /// <summary>
        /// Initiates the looping through all the character's children to find fitting GameObjects by name.
        /// </summary>
        /// <param name="go">the character, simply passed on from the constructor.</param>
        private void InitializeCharacterComponents(GameObject go)
        {
            Root = FindComponentByName(go, "Root");
            Bones = Root.GetComponentsInChildren<Transform>();
            Body = FindSkinnedMeshRenderer(go, "Body");
            Eyes = FindSkinnedMeshRenderer(go, "Eyes");
            Eyebrows = FindSkinnedMeshRenderer(go, "Eyebrows");
            Hair = FindSkinnedMeshRenderer(go, "Hair");
            Teeth = FindSkinnedMeshRenderer(go, "Teeth");
            Tongue = FindSkinnedMeshRenderer(go, "Tongue");
            Clothes = FindSkinnedMeshRenderer(go, "Clothes");
            Hair = FindSkinnedMeshRenderer(go, "Hair");
            Shoes = FindSkinnedMeshRenderer(go, "Shoes");
        }

        /// <summary>
        /// Finds a GameObject with a specific string (to be precise: may only contain that string) among all direct children of a parent.
        /// If more than one GameObject should fit the description, the first in the loop will be used.
        /// </summary>
        /// <param name="parent">The GameObject in which to look for fitting GameObjects by name (only among direct children)</param>
        /// <param name="name">The string to look for, i.e., can be "Body" or just "ody" to find a GameObject named "Body"</param>
        /// <returns>The first GameObject which contains the specified string in its name.</returns>
        private GameObject FindComponentByName(GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.gameObject.name.Contains(name))
                    return child.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Finds the SkinnedMeshRenderer of a GameObject with a specific string (to be precise: may only contain that string) among all direct children of a parent.
        /// If more than one GameObject should fit the description, the first in the loop will be used.
        /// </summary>
        /// <param name="parent">The GameObject in which to look for fitting GameObjects by name (only among direct children)</param>
        /// <param name="name">The string to look for, i.e., can be "Body" or just "ody" to find a GameObject named "Body"</param>
        /// <returns>The SkinnedMeshRenderer of the first GameObject which contains the specified string in its name.</returns>
        private SkinnedMeshRenderer FindSkinnedMeshRenderer(GameObject parent, string name)
        {
            SkinnedMeshRenderer[] renderers = parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.name.Contains(name))
                    return renderer;
            }
            return null;
        }
    }
}
