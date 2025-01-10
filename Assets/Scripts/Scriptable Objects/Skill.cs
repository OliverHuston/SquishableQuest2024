using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    PASSIVE = 0,
    ABILITY = 1,
}

public class Skill : ScriptableObject
{
    public string skillName;
    public SkillType skillType;
    public string effectCode;
}
