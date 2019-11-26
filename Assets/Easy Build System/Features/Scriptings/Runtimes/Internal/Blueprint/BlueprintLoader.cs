using EasyBuildSystem.Runtimes.Internal.Blueprint.Data;
using EasyBuildSystem.Runtimes.Internal.Group;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Storage.Data;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Blueprint
{
    [AddComponentMenu("Easy Build System/Features/Utilities/Blueprint Loader")]
    public class BlueprintLoader : MonoBehaviour
    {
        #region Public Fields

        public BlueprintData Blueprint;

        #endregion Public Fields

        #region Private Methods

        private void Start()
        {
            if (Blueprint != null)
            {
                LoadInRuntime(Blueprint);
            }
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to load a blueprint data in edit times.
        /// </summary>
        public void LoadInEditor(BlueprintData blueprint)
        {
            BuildManager Manager = FindObjectOfType<BuildManager>();

            if (Manager == null)
            {
                Debug.Log("<b><color=red>[Easy Build System]</color></b> : The build manager does not exists.");

                return;
            }

            List<PartModel.SerializedPart> Parts = blueprint.Model.DecodeToStr(blueprint.Data);

            GameObject Parent = new GameObject("(Editor) Blueprint", typeof(GroupBehaviour));

            for (int i = 0; i < Parts.Count; i++)
            {
                PartBehaviour Part = Manager.GetPart(Parts[i].Id);

                PartBehaviour PlacedPart = Manager.PlacePrefab(Part, PartModel.ParseToVector3(Parts[i].Position),
                    PartModel.ParseToVector3(Parts[i].Rotation), PartModel.ParseToVector3(Parts[i].Scale), Parent.transform);

                PlacedPart.ChangeAppearance(Parts[i].AppearanceIndex);
            }
        }

        /// <summary>
        /// This method allows to load a blueprint data in runtimes.
        /// </summary>
        public void LoadInRuntime(BlueprintData blueprint)
        {
            BuildManager Manager = FindObjectOfType<BuildManager>();

            if (Manager == null)
            {
                Debug.Log("<b><color=red>[Easy Build System]</color></b> : The build manager does not exists.");

                return;
            }

            List<PartModel.SerializedPart> Parts = blueprint.Model.DecodeToStr(blueprint.Data);

            GameObject Parent = new GameObject("(Runtime) Blueprint", typeof(GroupBehaviour));

            for (int i = 0; i < Parts.Count; i++)
            {
                PartBehaviour Part = Manager.GetPart(Parts[i].Id);

                PartBehaviour PlacedPart = Manager.PlacePrefab(Part, PartModel.ParseToVector3(Parts[i].Position),
                    PartModel.ParseToVector3(Parts[i].Rotation), PartModel.ParseToVector3(Parts[i].Scale), Parent.transform);

                PlacedPart.ChangeAppearance(Parts[i].AppearanceIndex);
            }
        }

        #endregion Public Methods
    }
}