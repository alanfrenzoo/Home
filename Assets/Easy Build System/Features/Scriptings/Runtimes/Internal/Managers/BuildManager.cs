using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Area;
using EasyBuildSystem.Runtimes.Internal.Group;
using EasyBuildSystem.Runtimes.Internal.Managers.Data;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using EasyBuildSystem.Runtimes.Internal.Terrain;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Managers
{
    public enum SupportType
    {
        All,
        Terrain,
        Voxeland,
        Surface
    }

    [AddComponentMenu("Easy Build System/Features/Managers/Build Manager")]
    public class BuildManager : MonoBehaviour
    {
        #region Public Fields

        public static BuildManager Instance;

        public SupportType BuildingSupport;

        public bool UsePhysics = true;

        public PartsCollection PartsCollection;

        public bool UseDefaultPreviewMaterial = false;

        public Material CustomPreviewMaterial;

        public Color PreviewAllowedColor = new Color(0, 1.0f, 0, 0.5f);

        public Color PreviewDeniedColor = new Color(1.0f, 0, 0, 0.5f);

        [HideInInspector]
        public List<PartBehaviour> Parts = new List<PartBehaviour>();

        [HideInInspector]
        public List<SocketBehaviour> Sockets = new List<SocketBehaviour>();

        [HideInInspector]
        public List<AreaBehaviour> Areas = new List<AreaBehaviour>();

        [HideInInspector]
        public StateType DefaultState = StateType.Placed;

        #endregion Public Fields

        #region Private Methods

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (BuildingSupport == SupportType.Terrain)
            {
                if (FindObjectOfType<UnityEngine.Terrain>() != null)
                    TerrainManager.Initialize();
            }
            else if (FindObjectOfType<UnityEngine.Terrain>() != null)
            {
                Debug.LogWarning("<b><color=yellow>[Easy Build System]</color></b> : A terrain was detected but the Build Manager did not consider the Placement Support Type.");
            }
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to add a part from the manager cache.
        /// </summary>
        public void AddPart(PartBehaviour part)
        {
            if (part == null)
                return;

            Parts.Add(part);
        }

        /// <summary>
        /// This method allows to remove a part from the manager cache.
        /// </summary>
        public void RemovePart(PartBehaviour part)
        {
            if (part == null)
                return;

            Parts.Remove(part);
        }

        /// <summary>
        /// This method allows to add a socket from the manager cache.
        /// </summary>
        public void AddSocket(SocketBehaviour socket)
        {
            if (socket == null)
                return;

            Sockets.Add(socket);
        }

        /// <summary>
        /// This method allows to remove a socket from the manager cache.
        /// </summary>
        public void RemoveSocket(SocketBehaviour socket)
        {
            if (socket == null)
                return;

            Sockets.Remove(socket);
        }

        /// <summary>
        /// This method allows to add a area from the manager cache.
        /// </summary>
        public void AddArea(AreaBehaviour area)
        {
            if (area == null)
                return;

            Areas.Add(area);
        }

        /// <summary>
        /// This method allows to remove a area from the manager cache.
        /// </summary>
        public void RemoveArea(AreaBehaviour area)
        {
            if (area == null)
                return;

            Areas.Remove(area);
        }

        /// <summary>
        /// This method allows to get a prefab by id.
        /// </summary>
        public PartBehaviour GetPart(int id)
        {
            return PartsCollection.Parts.Find(entry => entry.Id == id);
        }

        /// <summary>
        /// This method allows to get a prefab by name.
        /// </summary>
        public PartBehaviour GetPart(string name)
        {
            return PartsCollection.Parts.Find(entry => entry.Name == name);
        }

        /// <summary>
        /// This method allows to place a prefab.
        /// </summary>
        public PartBehaviour PlacePrefab(PartBehaviour part, Vector3 position, Vector3 rotation, Vector3 scale, Transform parent = null, SocketBehaviour socket = null)
        {
            GameObject PlacedTemp = Instantiate(part.gameObject, position, Quaternion.Euler(rotation));

            PlacedTemp.transform.localScale = scale;

            PartBehaviour PlacedPart = PlacedTemp.GetComponent<PartBehaviour>();

            if (parent == null)
            {
                if (socket != null)
                {
                    if (socket.AttachedPart.HasGroup)
                    {
                        PlacedTemp.transform.SetParent(socket.AttachedPart.transform.parent, true);
                    }
                }
                else
                {
                    GameObject Group = new GameObject("Group (" + PlacedPart.GetInstanceID() + ")", typeof(GroupBehaviour));

                    PlacedTemp.transform.SetParent(Group.transform, true);
                }
            }
            else
                PlacedTemp.transform.SetParent(parent, true);

            PlacedPart.EntityInstanceId = PlacedPart.GetInstanceID();

            EventHandlers.PlacedPart(PlacedPart, socket);

            PlacedPart.ChangeState(DefaultState);

            return PlacedPart;
        }

        /// <summary>
        /// This method allows to get the nearest area.
        /// </summary>
        public AreaBehaviour GetNearestArea(Vector3 position)
        {
            foreach (AreaBehaviour Area in Areas)
            {
                if (Area != null)
                    if (Area.gameObject.activeSelf == true)
                        if (Vector3.Distance(position, Area.transform.position) <= Area.Radius)
                            return Area;
            }

            return null;
        }

        #endregion Public Methods
    }
}