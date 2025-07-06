using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;

    [Serializable]
    public struct EntityEntry {
        public EntityData Data;
        public GameObject Prefab;
    }

    public List<EntityEntry> entries;

    Dictionary<string, EntityEntry> prefabMap;

    private void Awake() {
        if(Instance == null && Instance != this) Destroy(this);
        Instance = this;
        prefabMap = entries.ToDictionary(e => e.Data.GUID, e => e);
    }

    public GameObject Spawn(string guid, Vector3 position) {
        if (!prefabMap.TryGetValue(guid, out var def)) {
            Debug.LogError($"No entity prefab registered with ID {guid}");
            return null;
        }
        GameObject go = Instantiate(def.Prefab, position, Quaternion.identity);

        var baseEntity = go.GetComponent<BaseEntity>();
        baseEntity?.Init(def.Data, guid);

        return go;
    }

    public EntityData GetEntityData(string guid) {
        if (prefabMap.TryGetValue(guid, out var def)) return def.Data;
        return null;
    }

    //public void Unregister(string guid) {
    //    entities.Remove(guid);
    //}
    //public void Register (string guid, EntityData data) {
    //    prefabMap[guid] = data;
    //}
}
