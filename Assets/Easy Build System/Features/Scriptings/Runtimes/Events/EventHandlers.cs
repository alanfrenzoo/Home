using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;

namespace EasyBuildSystem.Runtimes.Events
{
    public class EventHandlers
    {
        #region Builder Events

        public delegate void EventHandlerChangeBuildMode(BuildMode mode);

        public static event EventHandlerChangeBuildMode OnBuildModeChanged;

        /// <summary>
        /// This method is called when the build mode is changed.
        /// </summary>
        public static void BuildModeChanged(BuildMode mode)
        {
            if (OnBuildModeChanged != null)
                OnBuildModeChanged.Invoke(mode);
        }

        public delegate void EventHandlerChangePartState(PartBehaviour part, StateType state);

        public static event EventHandlerChangePartState OnChangePartState;

        /// <summary>
        /// This method is called when the part change state.
        /// </summary>
        public static void PartStateChanged(PartBehaviour part, StateType state)
        {
            if (OnChangePartState != null)
                OnChangePartState.Invoke(part, state);
        }

        public delegate void EventHandlerPreviewCreated(PartBehaviour part);

        public static event EventHandlerPreviewCreated OnPreviewCreated;

        /// <summary>
        /// This method is called when the a preview is created.
        /// </summary>
        public static void PreviewCreated(PartBehaviour part)
        {
            if (OnPreviewCreated != null)
                OnPreviewCreated.Invoke(part);
        }

        public delegate void EventHandlerPreviewCanceled(PartBehaviour part);

        public static event EventHandlerPreviewCanceled OnPreviewCanceled;

        /// <summary>
        /// This method is called when the a preview is canceled.
        /// </summary>
        public static void PreviewCanceled(PartBehaviour part)
        {
            if (OnPreviewCanceled != null)
                OnPreviewCanceled.Invoke(part);
        }

        public delegate void EventHandlerPlacedPart(PartBehaviour part, SocketBehaviour socket);

        public static event EventHandlerPlacedPart OnPlacedPart;

        /// <summary>
        /// This method is called when a part is placed.
        /// </summary>
        public static void PlacedPart(PartBehaviour part, SocketBehaviour socket)
        {
            if (OnPlacedPart != null)
                OnPlacedPart.Invoke(part, socket);
        }

        public delegate void EventHandlerChangedAppearancePart(PartBehaviour part, int appearanceIndex);

        public static event EventHandlerChangedAppearancePart OnChangedAppearance;

        /// <summary>
        /// This method is called when the part appearance change.
        /// </summary>
        public static void ChangedAppearance(PartBehaviour part, int appearanceIndex)
        {
            if (OnChangedAppearance != null)
                OnChangedAppearance.Invoke(part, appearanceIndex);
        }

        public delegate void EventHandlerDestroyedPart(PartBehaviour part);

        public static event EventHandlerDestroyedPart OnDestroyedPart;

        /// <summary>
        /// This method is called when a part is removed.
        /// </summary>
        public static void DestroyedPart(PartBehaviour part)
        {
            if (OnDestroyedPart != null)
                OnDestroyedPart.Invoke(part);
        }

        public delegate void EventHandlerEditedPart(PartBehaviour part, SocketBehaviour socket);

        public static event EventHandlerEditedPart OnEditedPart;

        /// <summary>
        /// This method is called when a part is edited.
        /// </summary>
        public static void EditedPart(PartBehaviour part, SocketBehaviour socket)
        {
            if (OnEditedPart != null)
                OnEditedPart.Invoke(part, socket);
        }

        #endregion Builder Events

        #region Storage Events

        public delegate void EventHandlerStorageSaving();

        public static event EventHandlerStorageSaving OnStorageSaving;

        /// <summary>
        /// This method is called during the saving process of the current scene.
        /// </summary>
        public static void StorageSaving()
        {
            if (OnStorageSaving != null)
                OnStorageSaving.Invoke();
        }

        public delegate void EventHandlerStorageLoading();

        public static event EventHandlerStorageLoading OnStorageLoading;

        /// <summary>
        /// This method is called during the loading process of the current scene.
        /// </summary>
        public static void StorageLoading()
        {
            if (OnStorageLoading != null)
                OnStorageLoading.Invoke();
        }

        public delegate void EventHandlerStorageLoadingDone(PartBehaviour[] Parts);

        public static event EventHandlerStorageLoadingDone OnStorageLoadingDone;

        /// <summary>
        /// This method is called when the loading process is done.
        /// </summary>
        public static void StorageLoadingDone(PartBehaviour[] Parts)
        {
            if (OnStorageLoadingDone != null)
                OnStorageLoadingDone.Invoke(Parts);
        }

        public delegate void EventHandlerStorageSavingDone(PartBehaviour[] Parts);

        public static event EventHandlerStorageSavingDone OnStorageSavingDone;

        /// <summary>
        /// This method is called when the saving process is done.
        /// </summary>
        public static void StorageSavingDone(PartBehaviour[] Parts)
        {
            if (OnStorageSavingDone != null)
                OnStorageSavingDone.Invoke(Parts);
        }

        public delegate void EventHandlerStorageFailed(string exception);

        public static event EventHandlerStorageFailed OnStorageFailed;

        /// <summary>
        /// This method is called when the loading or saving process has failed.
        /// </summary>
        public static void StorageFailed(string exception)
        {
            if (OnStorageFailed != null)
                OnStorageFailed.Invoke(exception);
        }

        public delegate void EventHandlerStorageDeleted();

        public static event EventHandlerStorageDeleted OnStorageDeleted;

        /// <summary>
        /// This method is called when the file is deleted.
        /// </summary>
        public static void StorageDeleted()
        {
            if (OnStorageDeleted != null)
                OnStorageDeleted.Invoke();
        }

        #endregion Storage Events
    }
}