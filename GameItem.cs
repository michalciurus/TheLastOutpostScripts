using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public enum ItemComposite
{
    Metal, Carbon, DELETE, Mineral, Glass
}

[Serializable]
public struct CompositeAmount
{
    public ItemComposite component;
    public int requiredAmount;

    public CompositeAmount(ItemComposite composite, int amount)
    {
        this.component = composite;
        requiredAmount = amount;
    }

    public static CompositeAmount[] GetAllPossibleRequirements()
    {
        var valuesAsArray = Enum.GetValues(typeof(ItemComposite));
        var countValuesArray = new CompositeAmount[valuesAsArray.Length];

        var i = 0;
        foreach (ItemComposite possibleComposite in valuesAsArray)
        {
            countValuesArray[i] = new CompositeAmount(possibleComposite, 0);

            i++;
        }

        return countValuesArray;
    }
}

public class GameItem : MonoBehaviour
{
    public bool isComposite;
    public GameItemMetadata metadata;

    private GameObject labelInterface;

    private void Start()
    {
        labelInterface = UnitySugar.instantiatePrefab("ItemLabelInterface");
        labelInterface.transform.parent = transform;
        
        var textInterface = labelInterface.transform.FindDeepChildComponent<Text>("Text");

        var textToShow = gameObject.name + "\n";

        foreach (CompositeAmount req in metadata.itemCompositeRequirements)
        {

            if (req.requiredAmount > 0)
            {
                textToShow += req.component.ToString() + "        " + req.requiredAmount + "\n";
            }
        }

        textInterface.text = textToShow;

        gameObject.AddComponent<Scaleable>();
    }

    private void Update()
    {
        labelInterface.transform.LookAt(GameObject.Find("Camera").transform.position );
        labelInterface.transform.Rotate(0,180,0);

        labelInterface.transform.position = transform.position - new Vector3(0, 0, 0.5f);
    }
}