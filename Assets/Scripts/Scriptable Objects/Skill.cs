using UnityEngine;

public enum SkillType
{
    PASSIVE = 0,
    ONCE_PER_TURN = 1,
    ONCE_PER_DUNGEON = 2
}

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    public string skillName;

    [Space]
    public SkillType type;

    [TextArea] public string displayDescription;

    [Space]
    public string effectCode;
}