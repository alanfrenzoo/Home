using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Addons
{
    public class AddOnHelper : MonoBehaviour
    {
        #region Public Methods

        public static List<AddOnAttribute> GetAddons()
        {
            List<AddOnAttribute> ResultAddons = new List<AddOnAttribute>();

            Type[] ActiveBehaviours = GetAllSubTypes(typeof(MonoBehaviour));

            foreach (Type Type in ActiveBehaviours)
            {
                object[] Attributes = Type.GetCustomAttributes(typeof(AddOnAttribute), false);

                if (Attributes != null)
                {
                    for (int i = 0; i < Attributes.Length; i++)
                    {
                        if ((AddOnAttribute)Attributes[i] != null)
                        {
                            ((AddOnAttribute)Attributes[i]).Behaviour = Type;

                            ResultAddons.Add((AddOnAttribute)Attributes[i]);
                        }
                    }
                }
            }

            return ResultAddons;
        }

        public static List<AddOnAttribute> GetAddonsByTarget(AddOnTarget target)
        {
            List<AddOnAttribute> ResultAddons = new List<AddOnAttribute>();

            Type[] ActiveBehaviours = GetAllSubTypes(typeof(MonoBehaviour));

            foreach (Type Type in ActiveBehaviours)
            {
                object[] Attributes = Type.GetCustomAttributes(typeof(AddOnAttribute), false);

                if (Attributes != null)
                {
                    for (int i = 0; i < Attributes.Length; i++)
                    {
                        if ((AddOnAttribute)Attributes[i] != null)
                        {
                            if (((AddOnAttribute)Attributes[i]).Target == target)
                            {
                                ((AddOnAttribute)Attributes[i]).Behaviour = Type;

                                ResultAddons.Add((AddOnAttribute)Attributes[i]);
                            }
                        }
                    }
                }
            }

            return ResultAddons;
        }

        public static Type[] GetAllSubTypes(Type aBaseClass)
        {
            List<Type> Result = new List<Type>();

            Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly Assembly in Assemblies)
            {
                Type[] Types = Assembly.GetTypes();

                foreach (Type T in Types)
                {
                    if (T.IsSubclassOf(aBaseClass))
                        Result.Add(T);
                }
            }

            return Result.ToArray();
        }

        #endregion Public Methods
    }
}