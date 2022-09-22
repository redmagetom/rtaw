using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
[CreateAssetMenu(fileName = "New Processed Food", menuName ="Items/Food/Processed Food")]
public class ProcessedFood : Food
{
    public enum ProcessQuality{Awful, Akward, Decent, Good, Professional}
    public ProcessQuality processQuality;
    public List<string> processList;
    public Food coreItem;

}
