using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Character_Component : ScriptableObject
{
    public string CharacterName;
    public Sprite spr_VNCharacter;
    public Sprite spr_DialogBox;
    [Header("Emotions")]
    [Space]
    public Sprite Joie;
    public Sprite Colere;
    public Sprite Ennuie;
    public Sprite Tristesse;
    public Sprite Surprise;

}
