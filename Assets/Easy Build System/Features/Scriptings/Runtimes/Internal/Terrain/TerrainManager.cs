using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Terrain
{
    public class TerrainManager : MonoBehaviour
    {
        #region Public Fields

        public static TerrainManager Instance;

        public UnityEngine.Terrain ActiveTerrain;

        public UnityEngine.TerrainData Data;

        public Dictionary<int, int[,]> TerrainDetails = new Dictionary<int, int[,]>();

        public float[,] TerrainHeights;

        #endregion Public Fields

        #region Private Fields

        private bool IsInitialized;

        #endregion Private Fields

        #region Private Methods

        private void Awake()
        {
            Instance = FindObjectOfType<TerrainManager>();

            ActiveTerrain = FindObjectOfType<UnityEngine.Terrain>();

            Data = ActiveTerrain.terrainData;

            IsInitialized = true;

            SaveTerrainData();
        }

        private void OnApplicationQuit()
        {
            LoadTerrainData();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to initialize the terrain manager component.
        /// </summary>
        public static void Initialize()
        {
            if (Instance != null)
                return;

            GameObject Manager = new GameObject("Terrain Manager");

            Manager.AddComponent<TerrainManager>();

            DontDestroyOnLoad(Manager);
        }

        /// <summary>
        /// This method allows to save the terrain data (details, heightmaps).
        /// </summary>
        public void SaveTerrainData()
        {
            if (!IsInitialized)
                return;

            if (ActiveTerrain == null)
                return;

            TerrainDetails = new Dictionary<int, int[,]>();

            if (Data == null)
                return;

            for (int Layer = 0; Layer < Data.detailPrototypes.Length; Layer++)
                TerrainDetails.Add(Layer, Data.GetDetailLayer(0, 0, Data.detailWidth, Data.detailHeight, Layer));

            TerrainHeights = Data.GetHeights(0, 0, Data.heightmapResolution, Data.heightmapResolution);
        }

        /// <summary>
        /// This method allows to load the terrain data (details, heightmaps).
        /// </summary>
        public void LoadTerrainData()
        {
            if (!IsInitialized)
                return;

            if (ActiveTerrain == null)
                return;

            for (int Layer = 0; Layer < Data.detailPrototypes.Length; Layer++)
                Data.SetDetailLayer(0, 0, Layer, TerrainDetails[Layer]);

            Data.SetHeights(0, 0, TerrainHeights);
        }

        public bool CheckDetailtAt(Vector3 position, float radius)
        {
            bool Result = false;

            for (int Layer = 0; Layer < Data.detailPrototypes.Length; Layer++)
            {
                int TerrainDetailMapSize = Data.detailResolution;

                float DetailSize = TerrainDetailMapSize / Data.size.x;

                Vector3 WorldPoint = position - ActiveTerrain.transform.position;

                WorldPoint = WorldPoint * DetailSize;

                float[] Matrix = new float[4];
                Matrix[0] = WorldPoint.z + radius;
                Matrix[1] = WorldPoint.z - radius;
                Matrix[2] = WorldPoint.x + radius;
                Matrix[3] = WorldPoint.x - radius;

                for (int y = 0; y < Data.detailHeight; y++)
                {
                    for (int x = 0; x < Data.detailWidth; x++)
                    {
                        if (Matrix[0] > x && Matrix[1] < x && Matrix[2] > y && Matrix[3] < y)
                        {
                            if (TerrainDetails[Layer][x, y] != 0)
                                Result = true;
                        }
                    }
                }
            }

            return Result;
        }

        #endregion Public Methods
    }
}