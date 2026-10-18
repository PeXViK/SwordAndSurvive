using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Windows;
public abstract class BaseSword : Weapon
{
    public GameObject WaveProj { get; protected set; }
    public string Name { get; protected set; }
    public float WaveSpeed {  get; protected set; }
    public float WaveLifetime {  get; protected set; }

}
