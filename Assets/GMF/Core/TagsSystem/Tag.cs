using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GMF.Tags
{
    [Serializable]
    public class Tag : ITag
    {
        [field: SerializeField]
        [field: HideInInspector]
        public uint Id { get; private set; }
        [field: SerializeField]
        public string Name { get; set; }
        //[SerializeField]
        //Tag[] subTags;
        [field: SerializeField]
        public TagsIdCollection SubTagIds { get; private set; } = new TagsIdCollection();

        public Tag(uint id, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Tag name cannot be null or empty.", nameof(name));

            Id = id;
            Name = name;
            //subTags = new Tag[0];
        }

        public void AddSubTag(ITag subTag)
        {
            if (subTag == null)
                throw new ArgumentNullException(nameof(subTag));
            //subTags.Add(subTag);
            SubTagIds.Add(new TagId(subTag.Id));

        }

        public bool ContainsTag(ITag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (Id == tag.Id)
                return true;

            return SubTagIds.Contains(tag);
            // return subTags.Any(subTag => subTag.ContainsTag(tag));
        }


        public IEnumerable<ITag> GetSubTags()
        {
            return SubTagIds.GetAsTags();

            //return subTags;
        }

        public IEnumerable<ITag> GetAllTags()
        {
            var allTags = new List<ITag> { this };
            allTags.AddRange(SubTagIds.GetAsTags());

            /*            foreach (var subTag in subTags)
						{
							allTags.AddRange(subTag.GetAllTags());
						}*/
            return allTags;
        }

        public void RemoveSubTag(ITag subTag)
        {
            SubTagIds.Remove(new TagId(subTag.Id));
            // subTags.Remove((Tag) subTag);
        }

        public bool Equals(ITag other)
        {
            return Id == other.Id;
        }


        public override int GetHashCode()
        {
            return (int) Id;
        }

        public override string ToString()
        {
            return $"{Name} (ID: {Id})";
        }

        public string ToDropdownString()
        {
            if (!SubTagIds.IsEmpty)
            {
                var subTagNames = string.Join(", ", SubTagIds.GetAsTags().Select(t => t.Name));
                return $"{Name} ({subTagNames})";
            }
            else
            {
                return Name;
            }

            /*			if (subTags != null && subTags.Length > 0)
                        {
                            var subTagNames = string.Join(", ", subTags.Select(t => t.Name));
                            return $"{Name} ({subTagNames})";
                        }
                        else
                        {
                            return Name;
                        }*/
        }

    }


}
