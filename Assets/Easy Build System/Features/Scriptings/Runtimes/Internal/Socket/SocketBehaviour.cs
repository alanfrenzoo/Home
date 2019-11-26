using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Part.Data;
using EasyBuildSystem.Runtimes.Internal.Socket.Data;
using EasyBuildSystem.Runtimes.Internal.Terrain;
using EasyBuildSystem.Runtimes.Internal.Terrain.Compatibilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Socket
{
    public enum OccupancyType
    {
        Free,
        Busy
    }

    public enum SocketType
    {
        Socket,
        Attachment
    }

    [AddComponentMenu("Easy Build System/Features/Buildings Behaviour/Socket Behaviour")]
    public class SocketBehaviour : MonoBehaviour
    {
        #region Public Fields

        public SocketType Type;

        public float Radius = 0.5f;

        public bool DisableOnGroundContact;

        public Bounds AttachmentBounds;

        public List<PartOffset> PartOffsets = new List<PartOffset>();

        public List<Occupancy> BusySpaces = new List<Occupancy>();

        public PartBehaviour AttachedPart;

        public bool IsDisabled;

        #endregion Public Fields

        #region Private Methods

        private void Awake()
        {
            AttachedPart = GetComponentInParent<PartBehaviour>();

            gameObject.layer = LayerMask.NameToLayer(Constants.LAYER_SOCKET);

            if (Type == SocketType.Socket)
                gameObject.AddSphereCollider(Radius);
            else
                gameObject.AddBoxCollider(AttachmentBounds.extents, AttachmentBounds.center);
        }

        private void Start()
        {
            BuildManager.Instance.AddSocket(this);

            ChangeAreaState(OccupancyType.Busy,
                LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower()),
                LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()), AttachedPart);

            CheckContacts();
        }

        private void OnDestroy()
        {
            BuildManager.Instance.RemoveSocket(this);
        }

        private void OnDrawGizmos()
        {
            if (IsDisabled)
                return;

            if (BusySpaces.Count != 0)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawCube(transform.position, Vector3.one / 6);
            }
            else
            {
                Gizmos.color = Color.cyan;

                Gizmos.DrawCube(transform.position, Vector3.one / 6);
            }

#if UNITY_EDITOR

            if (UnityEditor.Selection.activeGameObject != gameObject)
                return;

#endif

            if (Type == SocketType.Socket)
            {
                Gizmos.DrawWireCube(transform.position, Vector3.one / 6);

                Gizmos.DrawWireSphere(transform.position, Radius);
            }
            else
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

                Gizmos.DrawWireCube(AttachmentBounds.center, Vector3.one / 6);

                Gizmos.DrawWireCube(AttachmentBounds.center, AttachmentBounds.extents);
            }
        }

        private void CheckContacts()
        {
            Collider[] Colliders = PhysicExtension.GetNeighborsTypesBySphere<Collider>(transform.position, Radius, LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()));

            for (int i = 0; i < Colliders.Length; i++)
            {
                SocketBehaviour Socket = Colliders[i].GetComponent<SocketBehaviour>();

                if (Socket != null)
                {
                    if (Socket != this)
                    {
                        if (Socket.AllowPart(Socket.AttachedPart))
                            for (int x = 0; x < BusySpaces.Count; x++)
                                Socket.AddOccupancy(BusySpaces[x].Part);
                    }
                }
            }

            if (DisableOnGroundContact)
            {
                Colliders = PhysicExtension.GetNeighborsTypesBySphere<Collider>(transform.position, Radius,
                    LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower()));

                for (int i = 0; i < Colliders.Length; i++)
                {
                    if (Colliders[i] as TerrainCollider || Colliders[i].GetComponentInParent<VoxelandCollider>() ||
                        Colliders[i].GetComponentInParent<VoxelandCollider>())
                        DisableCollider();
                }
            }
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to disable the collider of the socket.
        /// </summary>
        public void DisableCollider()
        {
            IsDisabled = true;

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().gameObject.layer = Physics.IgnoreRaycastLayer;
        }

        /// <summary>
        /// This method allows to enable the collider of the socket.
        /// </summary>
        public void EnableCollider()
        {
            IsDisabled = false;

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().gameObject.layer = LayerMask.NameToLayer(Constants.LAYER_SOCKET);
        }

        /// <summary>
        /// This method allows to enable the collider of the socket only with if a offsets part has the type.
        /// </summary>
        public void EnableColliderByPartType(PartType type)
        {
            if (PartOffsets.Count > 0)
            {
                for (int i = 0; i < PartOffsets.Count; i++)
                {
                    if (PartOffsets[i].Part != null && !CheckOccupancy(PartOffsets[i].Part))
                    {
                        if (PartOffsets[i].Part.Type == type)
                            EnableCollider();
                    }
                    else
                    {
                        EnableCollider();
                    }
                }
            }
            else
                DisableCollider();
        }

        /// <summary>
        /// This method allows to check if the part is allowed by this socket.
        /// </summary>
        public bool AllowPart(PartBehaviour part)
        {
            for (int i = 0; i < PartOffsets.Count; i++)
                if (PartOffsets[i] != null)
                    if (PartOffsets[i].Part != null)
                        if (PartOffsets[i].Part.Id == part.Id && !CheckOccupancy(part))
                            return true;

            return false;
        }

        /// <summary>
        /// This method allows to check if this socket is busy or not.
        /// </summary>
        public OccupancyType CheckState(LayerMask freeLayers, PartBehaviour placedPart)
        {
            Collider[] Colliders = Physics.OverlapSphere(transform.position, Radius, freeLayers, QueryTriggerInteraction.Ignore);

            foreach (Collider Collider in Colliders)
            {
                if (AttachedPart != placedPart)
                {
                    if (Collider as TerrainCollider == null)
                        return OccupancyType.Busy;
                }
                else
                {
                    if (!AttachedPart.HasCollider(Collider) && Collider as TerrainCollider == null)
                        if (Collider.GetComponent<SocketBehaviour>() != null || Collider.GetComponentInParent<PartBehaviour>() != null)
                            return OccupancyType.Busy;
                }
            }

            return OccupancyType.Free;
        }

        /// <summary>
        /// This method allows to check if the part is placed on this socket.
        /// </summary>
        public bool CheckOccupancy(PartBehaviour part)
        {
            return BusySpaces.Any(entry => entry.Part.Type == part.Type);
        }

        /// <summary>
        /// This method allows to add a occupancy on this socket.
        /// </summary>
        public void AddOccupancy(PartBehaviour part)
        {
            if (!CheckOccupancy(part))
            {
                if (part != AttachedPart)
                {
                    BusySpaces.Add(new Occupancy(part));

                    for (int i = 0; i < part.OccupancyParts.Length; i++)
                        BusySpaces.Add(new Occupancy(part.OccupancyParts[i]));
                }
            }
        }

        /// <summary>
        /// This method allows to remove a occupancy from this socket.
        /// </summary>
        public void RemoveOccupancy(PartBehaviour part)
        {
            if (CheckOccupancy(part))
            {
                BusySpaces.Remove(BusySpaces.Find(entry => entry.Part == part));

                for (int i = 0; i < part.OccupancyParts.Length; i++)
                    BusySpaces.Remove(BusySpaces.Find(entry => entry.Part == part.OccupancyParts[i]));
            }
        }

        /// <summary>
        /// This method allows to change the current socket state.
        /// </summary>
        public void ChangeState(OccupancyType state, PartBehaviour part)
        {
            if (state == OccupancyType.Busy)
            {
                if (!CheckOccupancy(part))
                    AddOccupancy(part);
            }
            else if (state == OccupancyType.Free)
            {
                if (CheckOccupancy(part))
                    RemoveOccupancy(part);
            }
        }

        /// <summary>
        /// This method allows to change all around connected socket(s) state.
        /// </summary>
        public void ChangeAreaState(OccupancyType state, LayerMask freeLayer, LayerMask socketLayer, PartBehaviour placedPart)
        {
            if (placedPart == null)
                return;

            if (placedPart.UseConditionalPhysics)
                if (!placedPart.CheckStability())
                    return;

            Collider[] Colliders = Physics.OverlapBox(placedPart.GetWorldPartMeshBounds().center,
                placedPart.GetWorldPartMeshBounds().extents,
                placedPart.transform.rotation, socketLayer, QueryTriggerInteraction.Collide);

            for (int i = 0; i < Colliders.Length; i++)
            {
                SocketBehaviour Socket = Colliders[i].GetComponent<SocketBehaviour>();

                if (Socket != null)
                {
                    if (state == OccupancyType.Busy)
                    {
                        if (Socket.AllowPart(placedPart) && Socket.CheckState(freeLayer, placedPart) == OccupancyType.Busy)
                        {
                            if (Vector3.Distance(placedPart.transform.position, Socket.transform.position) < placedPart.MeshBounds.extents.magnitude)
                            {
                                placedPart.LinkPart(Socket.AttachedPart);

                                Socket.ChangeState(OccupancyType.Busy, placedPart);
                            }
                        }
                    }
                    else
                    {
                        Socket.ChangeState(OccupancyType.Free, placedPart);
                    }
                }
            }

            Colliders = Physics.OverlapSphere(transform.position, Radius, freeLayer);

            for (int i = 0; i < Colliders.Length; i++)
            {
                if (Colliders[i].GetComponent<SocketBehaviour>() == null)
                {
                    PartBehaviour Part = Colliders[i].GetComponentInParent<PartBehaviour>();

                    if (Part != null)
                    {
                        if (Part != AttachedPart)
                        {
                            if (AllowPart(Part))
                            {
                                ChangeState(OccupancyType.Busy, Part);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method allows to get the part offset wich allowed on this socket.
        /// </summary>
        public PartOffset GetOffsetPart(int id)
        {
            for (int i = 0; i < PartOffsets.Count; i++)
                if (PartOffsets[i].Part != null && PartOffsets[i].Part.Id == id)
                    return PartOffsets[i];

            return null;
        }

        #endregion Public Methods
    }
}