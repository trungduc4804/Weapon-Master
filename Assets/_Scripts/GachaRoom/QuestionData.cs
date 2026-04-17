using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestionData", menuName = "WeaponMaster/Puzzle Question Data")]
public class QuestionData : ScriptableObject
{
    [TextArea(3, 5)]
    public string questionText;
    
    [Header("Answers (Must be 4)")]
    public string[] answers = new string[4];
    
    [Header("Correct Answer Index (0 to 3)")]
    [Range(0, 3)]
    public int correctIndex;
}
