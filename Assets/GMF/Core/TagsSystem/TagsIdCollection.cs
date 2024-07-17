using System;
using System.Collections.Generic;
using System.Linq;
using BaseCore.Collections;
using UnityEngine;


namespace GMF.Tags
{

    [Serializable]
    public class TagsIdCollection : ITagsIdCollection
    {
        [SerializeField]
        TagId[] tagIds;
        HashSet<ITag> tags;

        public bool IsEmpty => tagIds.Length == 0;


        public TagsIdCollection()
        {
            tagIds = new TagId[0];
        }

        public TagsIdCollection(IEnumerable<TagId> tagIds)
        {
            this.tagIds = tagIds.ToArray();
        }

        public void Add(TagId tagId)
        {
            tagIds.Add(tagId, out tagIds);

            if (tags == null)
            {
                tags = new HashSet<ITag>();
            }
            var tag = TagManager.GetTagById(tagId.Id);
            if (tag != null)
            {
                tags.Add(tag);
            }
        }

        public void Remove(TagId tagId)
        {
            tagIds.Remove(tagId, out tagIds);
            if (tags == null)
            {
                tags = new HashSet<ITag>();
            }

            var tag = TagManager.GetTagById(tagId.Id);
            if (tag != null)
            {
                tags.Remove(tag);
            }


        }

        public bool Contains(TagId tagId)
        {
            return tagIds.Contains(tagId);
        }

        public bool Contains(ITag tag)
        {
            return GetAsTags().Contains(tag);
        }

        public IEnumerable<ITag> GetAsTags()
        {
            if (tags == null)
            {
                tags = new HashSet<ITag>();
                for (int i = 0; i < tagIds.Length; i++)
                {
                    var tag = TagManager.GetTagById(tagIds[i].Id);
                    if (tag != null)
                    {
                        tags.Add(tag);
                    }
                }
            }

            return tags;
        }

        public override string ToString()
        {
            return string.Join(", ", tagIds);
        }

    }
}