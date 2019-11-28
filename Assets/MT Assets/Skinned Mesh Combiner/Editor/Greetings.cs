using UnityEngine;
using UnityEditor;
using System.IO;

namespace SkinnedMeshCombiner_ForEditor
{
    /*
     * This class is responsible for displaying the welcome message when installing this asset.
     */

    [InitializeOnLoad]
    class Greetings
    {
        static Greetings()
        {
            if (File.Exists("Assets/MT Assets/_AssetsData/GreetingsData.smc") == false)
            {
                CreateDirectoriesOfFeedbackIfNotExists();
                File.Create("Assets/MT Assets/_AssetsData/GreetingsData.smc");
                
                //Show greetings and save
                EditorUtility.DisplayDialog("Skinned Mesh Combiner was imported!",
                    "The \"Skinned Mesh Combiner\" was imported for your project. You should be able to locate it in the directory\n" +
                    "(MT Assets/Skinned Mesh Combiner).\n\n" +
                    "Remember to read the documentation to learn how to use this asset. To read the documentation, extract the contents of \"Documentation.zip\" inside the\n" +
                    "\"MT Assets/Skinned Mesh Combiner\" folder. Then just open the \"Documentation.html\" in your favorite browser.\n\n" +
                    "If you need help, contact me via email (mtassets@windsoft.xyz).",
                    "Cool!");
            }
        }

        public static void CreateDirectoriesOfFeedbackIfNotExists()
        {
            //Create the directory to feedbacks folder, of this asset
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets"))
            {
                AssetDatabase.CreateFolder("Assets", "MT Assets");
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData"))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets", "_AssetsData");
            }
        }
    }
}