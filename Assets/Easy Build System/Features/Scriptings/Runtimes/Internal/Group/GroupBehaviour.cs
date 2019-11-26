using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Storage.Data;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Group
{
    public class GroupBehaviour : MonoBehaviour
    {
        #region Private Methods

        private void Update()
        {
            if (transform.childCount == 0)
                Destroy(gameObject);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to get a model who contains all the base part data.
        /// </summary>
        public PartModel GetModel()
        {
            PartModel Model = new PartModel();

            foreach (PartBehaviour Part in GetComponentsInChildren<PartBehaviour>())
            {
                Model.Prefabs.Add(new PartModel.SerializedPart()
                {
                    Id = Part.Id,
                    AppearanceIndex = Part.AppearanceIndex,
                    Position = PartModel.ParseToSerializedVector3(Part.transform.position),
                    Rotation = PartModel.ParseToSerializedVector3(Part.transform.eulerAngles),
                    Scale = PartModel.ParseToSerializedVector3(Part.transform.localScale)
                });
            }

            return Model;
        }

        #endregion Public Methods
    }
}