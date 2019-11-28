using UnityEngine;
using UnityEditor;

namespace SkinnedMeshCombiner_ForEditor{

    /*
     * This class is responsible for creating the menu for this asset. 
     */

    public class Menu : MonoBehaviour
    {
        [MenuItem("Tools/MT Assets/Skinned Mesh Combiner/Read Documentation", false, 10)]
        static void ReadDocumentation()
        {
            EditorUtility.DisplayDialog("Read Documentation", "The documentation is located inside the \n\"MT Assets/Skinned Mesh Combiner\" folder. Just unzip \"Documentation.zip\" on your desktop and open the \"Documentation.html\" file with your preferred browser.", "Cool!");
        }

        [MenuItem("Tools/MT Assets/Skinned Mesh Combiner/Support", false, 10)]
        static void GetSupport()
        {
            EditorUtility.DisplayDialog("Support", "If you have any questions, problems or want to contact me, just contact me by email (mtassets@windsoft.xyz).", "Got it!");
        }
    }
}