using EasyBuildSystem.Runtimes.Internal.Group;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Storage.Data;
using EasyBuildSystem.Runtimes.Internal.Storage.Serialization;
using EasyBuildSystem.Runtimes.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Storage
{
    public enum StorageType
    {
        Desktop,
        Android
    }

    public enum StorageSerializerType
    {
        Binary,
        Json
    }

    [AddComponentMenu("Easy Build System/Features/Utilities/Build Storage")]
    public class BuildStorage : MonoBehaviour
    {
        #region Public Fields

        public static BuildStorage Instance;

        public StorageType StorageType;

        public StorageSerializerType StorageSerializer = StorageSerializerType.Json;

        public bool AutoSave = false;

        public float AutoSaveInterval = 60f;

        public bool SavePrefabs = true;

        public bool LoadPrefabs = true;

        public string StorageOutputFile;

        [HideInInspector]
        public bool LoadedFile = false;

        #endregion Public Fields

        #region Private Fields

        private float TimerAutoSave;

        private List<PartBehaviour> PrefabsLoaded = new List<PartBehaviour>();

        private bool FileIsCorrupted;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// (Editor) This method allows to load a storage file in Editor scene.
        /// </summary>
        public void LoadInEditor(string path)
        {
            int PrefabLoaded = 0;

            PrefabsLoaded = new List<PartBehaviour>();

            BuildManager Manager = FindObjectOfType<BuildManager>();

            if (Manager == null)
            {
                Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : The BuildManager is not in the scene, please add it to load a file.");

                return;
            }

            BinaryFormatter Formatter = new BinaryFormatter();

            Formatter.Binder = new BinderFormatter();

            FileStream Stream = File.Open(path, FileMode.Open);

            PartModel Serializer = null;

            try
            {
                if (StorageSerializer == StorageSerializerType.Binary)
                {
                    Serializer = (PartModel)Formatter.Deserialize(Stream);
                }
                else if (StorageSerializer == StorageSerializerType.Binary)
                {
                    using (StreamReader Reader = new StreamReader(Stream))
                    {
                        Serializer = JsonUtility.FromJson<PartModel>(Reader.ReadToEnd());
                    }
                }
            }
            catch
            {
                Stream.Close();

                Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Please check that the file extension to load is correct.");

                return;
            }

            if (Serializer == null || Serializer.Prefabs == null)
            {
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The file is empty or the data are corrupted.");

                return;
            }

            GameObject Parent = new GameObject("(Editor) " + path, typeof(GroupBehaviour));

            foreach (PartModel.SerializedPart Data in Serializer.Prefabs)
            {
                if (Data != null)
                {
                    PartBehaviour Prefab = Manager.GetPart(Data.Id);

                    if (Prefab != null)
                    {
                        PartBehaviour PlacedPrefab = Manager.PlacePrefab(Prefab,
                            PartModel.ParseToVector3(Data.Position),
                            PartModel.ParseToVector3(Data.Rotation),
                            PartModel.ParseToVector3(Data.Scale),
                            Parent.transform, null);

                        PlacedPrefab.name = Prefab.Name;
                        PlacedPrefab.transform.position = PartModel.ParseToVector3(Data.Position);
                        PlacedPrefab.transform.eulerAngles = PartModel.ParseToVector3(Data.Rotation);
                        PlacedPrefab.transform.localScale = PartModel.ParseToVector3(Data.Scale);

                        PrefabsLoaded.Add(PlacedPrefab);

                        PrefabLoaded++;
                    }
                    else
                        Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The Prefab (" + Data.Id + ") does not exists in the Build Manager.");
                }
            }

            Stream.Close();

#if UNITY_EDITOR
            Selection.activeGameObject = Parent;

            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.FrameSelected();
#endif

            Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Data file loaded " + PrefabLoaded + " Prefab(s) loaded in " + Time.realtimeSinceStartup.ToString("#.##") + " ms in the Editor scene.");

            PrefabsLoaded.Clear();
        }

        /// <summary>
        /// This method allows to load the storage file.
        /// </summary>
        public void LoadStorageFile()
        {
            StartCoroutine(LoadDataFile());
        }

        /// <summary>
        /// This method allows to save the storage file.
        /// </summary>
        public void SaveStorageFile()
        {
            StartCoroutine(SaveDataFile());
        }

        /// <summary>
        /// This method allows to delete the storage file.
        /// </summary>
        public void DeleteStorageFile()
        {
            StartCoroutine(DeleteDataFile());
        }

        /// <summary>
        /// This method allows to check if the storage file.
        /// </summary>
        public bool ExistsStorageFile()
        {
            return File.Exists(StorageOutputFile);
        }

        #endregion Public Methods

        #region Private Methods

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (LoadPrefabs)
                StartCoroutine(LoadDataFile());

            if (AutoSave)
                TimerAutoSave = AutoSaveInterval;
        }

        private void Update()
        {
            if (AutoSave)
            {
                if (TimerAutoSave <= 0)
                {
                    Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Saving of " + FindObjectsOfType<PartBehaviour>().Length + " Part(s) ...");

                    SaveStorageFile();

                    Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Saved with successfuly !");

                    TimerAutoSave = AutoSaveInterval;
                }
                else
                    TimerAutoSave -= Time.deltaTime;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (StorageType == StorageType.Android)
            {
                if (!SavePrefabs)
                    return;

                SaveStorageFile();
            }
        }

        private void OnApplicationQuit()
        {
            if (!SavePrefabs)
                return;

            SaveStorageFile();
        }

        private IEnumerator LoadDataFile()
        {
            if (StorageType == StorageType.Desktop)
            {
                if (StorageOutputFile == string.Empty || Directory.Exists(StorageOutputFile))
                {
                    Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Please define output path.");

                    yield break;
                }
            }

            int PrefabLoaded = 0;

            PrefabsLoaded = new List<PartBehaviour>();

            if (ExistsStorageFile() || StorageType == StorageType.Android)
            {
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Loading data file (" + StorageSerializer.ToString() + ") ...");

                BinaryFormatter Formatter = new BinaryFormatter();

                Formatter.Binder = new BinderFormatter();

                FileStream Stream = null;

                if (StorageType == StorageType.Desktop)
                {
                    Stream = File.Open(StorageOutputFile, FileMode.Open);
                }

                PartModel Serializer = null;

                try
                {
                    if (StorageType == StorageType.Desktop)
                    {
                        if (StorageSerializer == StorageSerializerType.Binary)
                        {
                            Serializer = (PartModel)Formatter.Deserialize(Stream);
                        }
                        else if (StorageSerializer == StorageSerializerType.Json)
                        {
                            using (StreamReader Reader = new StreamReader(Stream))
                            {
                                Serializer = JsonUtility.FromJson<PartModel>(Reader.ReadToEnd());
                            }
                        }
                    }
                    else
                    {
                        Serializer = JsonUtility.FromJson<PartModel>(PlayerPrefs.GetString("EBS_Storage"));
                    }
                }
                catch (Exception ex)
                {
                    Stream.Close();

                    FileIsCorrupted = true;

                    Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : " + ex);

                    EventHandlers.StorageFailed(ex.Message);

                    yield break;
                }

                if (Serializer == null)
                {
                    EventHandlers.StorageFailed("The storage file is corrupted.");
                    yield break;
                }

                GameObject Parent = new GameObject("(Runtime) " + StorageOutputFile, typeof(GroupBehaviour));

                foreach (PartModel.SerializedPart Data in Serializer.Prefabs)
                {
                    if (Data != null)
                    {
                        PartBehaviour Prefab = BuildManager.Instance.GetPart(Data.Id);

                        if (Prefab != null)
                        {
                            PartBehaviour PlacedPrefab = BuildManager.Instance.PlacePrefab(Prefab,
                                PartModel.ParseToVector3(Data.Position),
                                PartModel.ParseToVector3(Data.Rotation),
                                PartModel.ParseToVector3(Data.Scale),
                                Parent.transform, null);

                            PlacedPrefab.name = Data.Name;
                            PlacedPrefab.transform.position = PartModel.ParseToVector3(Data.Position);
                            PlacedPrefab.transform.eulerAngles = PartModel.ParseToVector3(Data.Rotation);
                            PlacedPrefab.transform.localScale = PartModel.ParseToVector3(Data.Scale);
                            PlacedPrefab.ChangeAppearance(Data.AppearanceIndex);
                            PlacedPrefab.ExtraProperties = Data.Properties;

                            PrefabsLoaded.Add(PlacedPrefab);

                            PrefabLoaded++;
                        }
                        else
                            Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The prefab (" + Data.Id + ") does not exists in the list of Build Manager.");
                    }
                }

                if (Stream != null)
                    Stream.Close();

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Data file loaded " + PrefabLoaded + " prefab(s) loaded in " + Time.realtimeSinceStartup.ToString("#.##") + " ms.");

                LoadedFile = true;

                EventHandlers.StorageLoadingDone(PrefabsLoaded.ToArray());

                yield break;
            }
            else
            {
                EventHandlers.StorageFailed("The file does not exists or the path is not found.");
            }

            yield break;
        }

        private IEnumerator SaveDataFile()
        {
            if (FileIsCorrupted)
            {
                Debug.LogWarning("<b><color=cyan>[Easy Build System]</color></b> : The file is corrupted, the Prefabs could not be saved.");

                yield break;
            }

            if (StorageOutputFile == string.Empty || Directory.Exists(StorageOutputFile))
            {
                Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Please define out file path.");

                yield break;
            }

            int SavedCount = 0;

            if (ExistsStorageFile())
            {
                File.Delete(StorageOutputFile);
            }
            else
            {
                EventHandlers.StorageFailed("The file does not exists or the path is not found.");
            }

            if (FindObjectsOfType<PartBehaviour>().Length > 0)
            {
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Saving data file ...");

                FileStream Stream = null;

                if (StorageType == StorageType.Desktop)
                {
                    Stream = File.Create(StorageOutputFile);
                }

                PartModel Data = new PartModel();

                PartBehaviour[] PartsAtSave = FindObjectsOfType<PartBehaviour>();

                foreach (PartBehaviour Prefab in PartsAtSave)
                {
                    if (Prefab != null)
                    {
                        if (Prefab.CurrentState == StateType.Placed || Prefab.CurrentState == StateType.Remove)
                        {
                            PartModel.SerializedPart DataTemp = new PartModel.SerializedPart();

                            DataTemp.Id = Prefab.Id;
                            DataTemp.Name = Prefab.name;
                            DataTemp.Position = PartModel.ParseToSerializedVector3(Prefab.transform.position);
                            DataTemp.Rotation = PartModel.ParseToSerializedVector3(Prefab.transform.eulerAngles);
                            DataTemp.Scale = PartModel.ParseToSerializedVector3(Prefab.transform.localScale);
                            DataTemp.AppearanceIndex = Prefab.AppearanceIndex;
                            DataTemp.Properties = Prefab.ExtraProperties;
                            Data.Prefabs.Add(DataTemp);

                            SavedCount++;
                        }
                    }
                }

                if (StorageType == StorageType.Desktop)
                {
                    if (StorageSerializer == StorageSerializerType.Binary)
                    {
                        BinaryFormatter Formatter = new BinaryFormatter();

                        Formatter.Binder = new BinderFormatter();

                        Formatter.Serialize(Stream, Data);
                    }
                    else if (StorageSerializer == StorageSerializerType.Json)
                    {
                        using (StreamWriter Writer = new StreamWriter(Stream))
                        {
                            Writer.Write(JsonUtility.ToJson(Data));
                        }
                    }

                    Stream.Close();
                }
                else
                {
                    PlayerPrefs.SetString("EBS_Storage", JsonUtility.ToJson(Data));

                    PlayerPrefs.Save();
                }

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Data file saved " + SavedCount + " Prefab(s).");

                EventHandlers.StorageSavingDone(PartsAtSave);

                yield break;
            }
        }

        private IEnumerator DeleteDataFile()
        {
            if (StorageOutputFile == string.Empty || Directory.Exists(StorageOutputFile))
            {
                Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Please define out file path.");

                yield break;
            }

            if (File.Exists(StorageOutputFile) == true)
            {
                foreach (PartBehaviour Prefab in PrefabsLoaded)
                {
                    Destroy(Prefab.gameObject);
                }

                File.Delete(StorageOutputFile);

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The storage file has been removed.");
            }
            else
            {
                EventHandlers.StorageFailed("The file does not exists or the path is not found.");
            }

            EventHandlers.StorageDeleted();
        }

        #endregion Private Methods
    }
}