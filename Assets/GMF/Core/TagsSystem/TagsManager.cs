using System.Collections.Generic;
using System.Linq;

namespace GMF.Tags
{

    public interface ITagRepository
    {
        ITag GetTagById(uint id);
        ITag GetTagByName(string name);
        IEnumerable<ITag> GetAllTags();
        void RemoveTag(ITag ITag);
        void RemoveTagFromGroup(ITag parentITag, ITag ITagToRemove);
        void RemoveTagFromGroups(ITag ITag);
        ITag CreateTag(string name);
    }

    public static class TagManager
    {
        public static ITagRepository repository;


        public static void Initialize(ITagRepository repository)
        {
            TagManager.repository = repository;
        }

        public static void RemoveTag(string name)
        {
            var existingITag = repository.GetTagByName(name);
            if (existingITag != null)
                RemoveTag(existingITag);
        }

        public static void RemoveTag(ITag ITag)
        {
            if (ITag != null)
                repository.RemoveTag(ITag);
        }

        public static void RemoveTagFromGroup(ITag parentITag, ITag ITagToRemove)
        {
            repository.RemoveTagFromGroup(parentITag, ITagToRemove);
        }

        public static ITag CreateTag(string name)
        {
            return repository.CreateTag(name);
        }

        public static ITag GetTagById(uint id)
        {
            return repository.GetTagById(id);
        }

        public static ITag GetITagByName(string name)
        {
            return repository.GetTagByName(name);
        }

        public static IEnumerable<ITag> GetAllTags()
        {
            return repository.GetAllTags();
        }
        public static void RemoveITagFromGroups(ITag ITag)
        {
            repository.RemoveTagFromGroups(ITag);
        }

        public static bool IsSubsetOf(ITagsIdCollection thisCollection, ITagsIdCollection otherCollection)
        {
            var tags = thisCollection.GetAsTags();
            var tagsOther = otherCollection.GetAsTags();
            bool issubset = tags.Count() != 0 && tagsOther.Count() != 0 && tagsOther.All(tagOther => tags.Any(thisTag => tagOther.ContainsTag(thisTag)));

            // UnityEngine.Debug.Log($"issubset : {issubset}, this: {string.Join(' ', tags.Select(arg => arg.ToString()))}, other: {string.Join(' ', tagsOther.Select(arg => arg.ToString()))}");
            return issubset;

        }

    }
}