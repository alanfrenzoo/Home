using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Managers.Data;
using EasyBuildSystem.Runtimes.Internal.Part;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimplePrefab : MonoBehaviour
{
    [MenuItem("My Project/Create Simple Prefab")]
    static void DoCreateSimplePrefab()
    {
        Transform[] transforms = Resources.LoadAll<Transform>("FurnitureImport");
        var pc = (PartsCollection)AssetDatabase.LoadAssetAtPath("Assets/Resources/Collection/PartsCollection.asset", typeof(PartsCollection));

        foreach (Transform t in transforms)
        {
            // Get Assets
            var part = new GameObject();
            part.AddComponent<PartBehaviour>();
            var furni = Instantiate(t.gameObject);
            furni.transform.SetParent(part.transform);

            //layer for child (for item interaction)
            furni.layer = LayerMask.NameToLayer("PartBehaviour");
            for (int i = 0; i < furni.transform.childCount; i++)
                furni.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("PartBehaviour");

            // Modify
            // To do: update by balancing in the future?
            var pb = part.GetComponent<PartBehaviour>();
            pb.AdvancedFeatures = true;
            pb.Id = Resources.LoadAll<Transform>("FurnitureOutput").Length;
            pb.Name = t.name;
            pb.Type = PartType.None;
            pb.AvoidClipping = true;
            pb.PhysicsOnlyStablePlacement = true;
            pb.UseConditionalPhysics = true;
            pb.PhysicsLayers = LayerMask.GetMask("Default", "Furniture");

            // Default Custom Mesh Bounds (for collision conflict dectection)
            pb.MeshBounds = part.gameObject.GetChildsBounds();

            // Stable Support
            pb.CustomDetections = new EasyBuildSystem.Runtimes.Internal.Part.Data.Detection[1];
            var dectection = new EasyBuildSystem.Runtimes.Internal.Part.Data.Detection
            {
                Size = new Vector3(0.1f, 0.5f, 0.1f),
                RequiredSupports = new SurfaceType[]
                {
                     SurfaceType.SurfaceAndTerrain
                }
            };
            pb.CustomDetections[0] = dectection;

            // Add to Collections
            //pc.Parts.Add(pb);

            // Save Prefab
            var name = t.gameObject.name;
            var path = string.Format("Assets/Resources/FurnitureOutput/{0}.prefab", name);
            PrefabUtility.SaveAsPrefabAsset(part, path);
            DestroyImmediate(part);
            DestroyImmediate(furni);

        }

    }
}