using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

    abstract class UISetting 
{
    
    public abstract void GetAccuratedCharacter(string _characterName);
    public abstract void SpeakerPosition(int _pos);
    public abstract void CharacterSetting(List<Character_Component> character_Component);
    public abstract Character_Component SetCharacterSetting();
}

class SetCharactersToUI : UISetting
{
    Character_Component character;
    string _TalkingCharacter;
    public override void CharacterSetting(List<Character_Component> character_Component)
    {
        foreach (Character_Component _chara in character_Component)
        {
            if(_TalkingCharacter == _chara.CharacterName)
            {
                Debug.Log(_chara.CharacterName);
                character = _chara;
            }
        }    
    }

    public override void GetAccuratedCharacter(string _characterName)
    {
        _TalkingCharacter = _characterName;
    }

    public override Character_Component SetCharacterSetting()
    {
        return character;
    }

    public override void SpeakerPosition(int _pos)
    {
        throw new System.NotImplementedException();
    }
}


