using System;

namespace EasyBuildSystem.Runtimes.Internal.Addons
{
    public enum AddOnTarget
    {
        None,
        PartBehaviour,
        SocketBehaviour,
        BuilderBehaviour,
        BuildManager,
        AreaBehaviour,
        GroupBehaviour,
        StorageBehaviour
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AddOnAttribute : Attribute
    {
        #region Public Fields

        public readonly string Name;

        public readonly string Author;

        public readonly string Description;

        public readonly AddOnTarget Target;

        public Type Behaviour;

        #endregion Public Fields

        #region Public Methods

        public AddOnAttribute(string name, string author, string description, AddOnTarget target)
        {
            Name = name;
            Author = author;
            Description = description;
            Target = target;
        }

        #endregion Public Methods
    }
}