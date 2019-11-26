using EasyBuildSystem.Runtimes.Internal.Storage.Structs;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Storage.Data
{
    [System.Serializable]
    public class PartModel
    {
        #region Public Fields

        public List<SerializedPart> Prefabs = new List<SerializedPart>();

        [System.Serializable]
        public class SerializedPart
        {
            #region Public Fields

            public int Id;

            public string Name;

            public int AppearanceIndex;

            public SerializeVector3 Position;

            public SerializeVector3 Rotation;

            public SerializeVector3 Scale;

            public List<string> Properties = new List<string>();

            #endregion Public Fields
        }

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// This method return the prefabs encode to custom string.
        /// </summary>
        public string EncodeToStr()
        {
            string Result = string.Empty;

            for (int i = 0; i < Prefabs.Count; i++)
                Result += string.Format("{0}:{1}:{2}:{3}:{4}:{5}", Prefabs[i].Id, Prefabs[i].Name, Prefabs[i].AppearanceIndex,
                    ParseToVector3(Prefabs[i].Position).ToString("F4"),
                    ParseToVector3(Prefabs[i].Rotation).ToString("F4"),
                    ParseToVector3(Prefabs[i].Scale).ToString("F4")) + "|";

            return Result;
        }

        /// <summary>
        /// This method return the prefabs decode from custom string.
        /// </summary>
        public List<SerializedPart> DecodeToStr(string customStr)
        {
            List<SerializedPart> Result = new List<SerializedPart>();

            string[] Data = customStr.Split('|');

            for (int i = 0; i < Data.Length - 1; i++)
            {
                string[] Args = Data[i].Split(':');

                Result.Add(new SerializedPart()
                {
                    Id = int.Parse(Args[0]),
                    Name = Args[1],
                    AppearanceIndex = int.Parse(Args[2]),
                    Position = ParseToSerializedVector3(ToVector3(Args[3])),
                    Rotation = ParseToSerializedVector3(ToVector3(Args[4])),
                    Scale = ParseToSerializedVector3(ToVector3(Args[5]))
                });
            }

            return Result;
        }

        /// <summary>
        /// This method return a Vector3 from a string Vector3.
        /// </summary>
        public static Vector3 ToVector3(string strVector)
        {
            if (strVector.StartsWith("(") && strVector.EndsWith(")"))
            {
                strVector = strVector.Substring(1, strVector.Length - 2);
            }

            string[] Data = strVector.Split(',');

            Vector3 result = new Vector3(
                float.Parse(Data[0]),
                float.Parse(Data[1]),
                float.Parse(Data[2]));

            return result;
        }

        /// <summary>
        /// This method return a serialized Vector3 in a Vector3.
        /// </summary>
        public static Vector3 ParseToVector3(SerializeVector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// This method return a Vector3 in a serialized Vector3.
        /// </summary>
        public static SerializeVector3 ParseToSerializedVector3(Vector3 vector)
        {
            SerializeVector3 SerializedVector3 = new SerializeVector3();
            SerializedVector3.X = vector.x;
            SerializedVector3.Y = vector.y;
            SerializedVector3.Z = vector.z;

            return SerializedVector3;
        }

        #endregion Public Methods
    }
}