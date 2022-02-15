using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;
using System.Linq;
using UnityEngine;


public class C {
    public static float PLAYER_HEIGHT = 1.6f;

    public static float PLAYER_SPRINT_DURATION = 5.0f;

    public static float PLAYER_SPEED_MULTIPLIER = 4.0f;
    public static float STAGGER_FACTOR_INCREMENT = 20;
}

public class C_WEAK_ZOMBIE
{
    public static float CHASE_STOPPING_DISTANCE = 0.85f;
    public static float FOLLOW_STOPPING_DISTANCE = 0.6f;
}

public class DEBUG
{
    public static bool SPAWN_ENEMIES = true;
    public static bool ENEMIES_STAND = false;
    public static bool INVINCIBLE = false;
}

public class SpawnItem
{
    public SpawnItem(string name, DropChance chance, SpawnerTag tags)
    {
        prefabName = name;
        dropChance = chance;
        this.tags = tags;
        this.dimensions = Dimensions.Medium;
    }
    
    public SpawnItem(string name, DropChance chance, SpawnerTag tags, Dimensions dimensions)
    {
        prefabName = name;
        dropChance = chance;
        this.tags = tags;
        this.dimensions = dimensions;
    }

    public string prefabName;
    public DropChance dropChance;
    public SpawnerTag tags;
    public Dimensions dimensions;
}

public class EnemySpawnMeta
{

    public EnemySpawnMeta(string meshName, DropChance chance, SpawnerTag tags)
    {
        this.meshName = meshName;
        this.dropChance = chance;
        this.tags = tags;
    }
    
    public string meshName;
    public DropChance dropChance;
    public SpawnerTag tags;
}

public enum DropChance
{
    Common = 2,
    Scarce = 1,
    Rare = 0
}

[Flags]
public enum Dimensions
{
    Tiny = 1,
    Small = 1<<1,
    Medium = 1<<2,
    Big = 1<<3,
    Huge = 1<<4
}

[Flags]
public enum SpawnerTag
{
    Kitchen = 1,
    Office = 1<<1,
    Industrial = 1<<2,
    Bathroom = 1<<3,
    Shop = 1<<4,
    Room = 1<<5,
    Street = 1<<6,
    None = 1<<7
}

public class SpawnSettings {

    private static System.Random ran = new System.Random();

    public static GameItemMetadata spawnRandomItem(SpawnerTag tags, Dimensions? dimensionFilter = null)
    {
        var allItems = GetAllItems();

        if (dimensionFilter.HasValue)
        {
            allItems = allItems.Where(item => dimensionFilter.Value.HasAnyFlag(item.dimensions)).ToArray();
            allItems = allItems.Where(item => tags.HasAnyFlag(item.spawnerTag)).ToArray();
        }

        var r = Random.Range(0.0f, 1.0f);

        var dropChance = DropChance.Common;
        
        if (r < 0.1f)
        {
            dropChance = DropChance.Rare;
        }

        if (r < 0.3f)
        {
            dropChance = DropChance.Scarce;
        }

        ran.Shuffle(allItems);

        allItems = allItems.OrderBy(item => item.dropChance).ToArray();
        
        foreach(GameItemMetadata item in allItems)
        {
            if (item.dropChance >= dropChance)
            {
                return item;
            }
        }
        
        return allItems[0];
    }

    public static EnemySpawnMeta SpawnRandomEnemy()
    {
        var all = GetAllEnemySpawnMetas();
        
        ran.Shuffle(all);

        return all[0];
    }

    private static AllItemsList allItems;

    public static EnemySpawnMeta[] GetAllEnemySpawnMetas()
    {
        List<EnemySpawnMeta> items = new List<EnemySpawnMeta>();
        
        items.Add(new EnemySpawnMeta("Bellboy", DropChance.Common, SpawnerTag.None));
        items.Add(new EnemySpawnMeta("Biker", DropChance.Common, SpawnerTag.None));
        items.Add(new EnemySpawnMeta("Footballer", DropChance.Common, SpawnerTag.None));

        return items.ToArray();
    }

    public static GameItemMetadata[] GetAllItems()
    {
        if (allItems == null)
        {
            allItems = Resources.Load<AllItemsList>("InventoryItemList");
        }

        return allItems.itemList.ToArray();
    }
}