using System;
using UnityEngine;

namespace GMF.Tags
{
    [Serializable]
    public struct TagId : ITagID
    {
        [SerializeField]
        [HideInInspector]
        uint id;

        public uint Id => id;


        public TagId(uint id)
        {
            this.id = id;
        }

        public bool Equals(TagId other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return (int) id;
        }

        public override Boolean Equals(System.Object obj)
        {
            return Equals((TagId) obj);
        }

        public static bool operator ==(TagId left, TagId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TagId left, TagId right)
        {
            return !(left == right);
        }

        public Boolean Equals(ITagID other)
        {
            return Equals((TagId) other);
        }


    }
}