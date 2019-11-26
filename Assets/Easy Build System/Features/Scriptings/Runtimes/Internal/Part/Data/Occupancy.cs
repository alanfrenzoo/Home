namespace EasyBuildSystem.Runtimes.Internal.Part.Data
{
    [System.Serializable]
    public class Occupancy
    {
        #region Public Fields

        public PartBehaviour Part;

        #endregion Public Fields

        #region Public Methods

        public Occupancy(PartBehaviour part)
        {
            Part = part;
        }

        #endregion Public Methods
    }
}