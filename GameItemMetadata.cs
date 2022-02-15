using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameItemMetadata
{
    public string prefabName = "Filled Dynamically";
    public SpawnerTag spawnerTag = SpawnerTag.None;
    public DropChance dropChance = DropChance.Common;
    public Dimensions dimensions = Dimensions.Small;
    public CompositeAmount[] itemCompositeRequirements;
}
