using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerCharacterDatabaseSO : ScriptableObject
{
    [SerializeField]
    private List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

    public int PlayerCharacterCount
    {
        get
        {
            return playerCharacters.Count;
        }
    }

    public PlayerCharacter GetPlayerCharacter(int index)
    {
        if (index < 0 || index >= playerCharacters.Count)
        {
            Debug.LogError("PlayerCharacterDatabase: Requested index is out of bounds");
            return null;
        }
        return playerCharacters[index];
    }
}
