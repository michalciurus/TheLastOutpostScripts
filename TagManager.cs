using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager
{

    public static TagManager instance = new TagManager();

    Dictionary<GameObject, HashSet<string>> tagDict = new Dictionary<GameObject, HashSet<string>>();

    public void addTag(GameObject obj, string tag) {

        HashSet<string> existingHashSet;

        tagDict.TryGetValue(obj, out existingHashSet);

        if(existingHashSet == null) {
            existingHashSet = new HashSet<string>();
        }

        existingHashSet.Add(tag);

        tagDict[obj] = existingHashSet;
    }

    public void removeTag(GameObject obj, string tag) {

        HashSet<string> existingHashSet;

        tagDict.TryGetValue(obj, out existingHashSet);
        if (existingHashSet != null) {
            existingHashSet.Remove(tag);
        }
    }

    public bool hasTag(GameObject obj, string tag) {

        HashSet<string> existingHashSet;

        tagDict.TryGetValue(obj, out existingHashSet);

        if (existingHashSet != null && existingHashSet.Contains(tag)) {
            return true;
        }

        return false;
    }

}

public static class GameObjectMethods {

    public static void addTag(this MonoBehaviour obj, string tag) {
        TagManager.instance.addTag(obj.gameObject, tag);
    }

    public static void removeTag(this MonoBehaviour obj, string tag) {
        TagManager.instance.removeTag(obj.gameObject, tag);
    }

    public static bool hasTag(this MonoBehaviour obj, string tag) {
        return TagManager.instance.hasTag(obj.gameObject, tag);
    }


    public static void addTag(this GameObject obj, string tag) {
        TagManager.instance.addTag(obj.gameObject, tag);
    }

    public static void removeTag(this GameObject obj, string tag) {
        TagManager.instance.removeTag(obj.gameObject, tag);
    }

    public static bool hasTag(this GameObject obj, string tag) {
        return TagManager.instance.hasTag(obj.gameObject, tag);
    }

}