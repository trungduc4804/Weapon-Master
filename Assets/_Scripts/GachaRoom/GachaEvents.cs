using System;

public static class GachaEvents
{
    // Puzzle Events
    public static Action<bool> OnQuestionAnswered; // true if correct, false if wrong
    public static Action OnPuzzleSolved; // Triggered when player answers correctly to give rewards
    
    // Gacha Events
    public static Action<ShopItemData> OnGachaRolled; // Triggered when rolling successfully, passing the item
    public static Action OnRollFailed_NoRolls; // Triggered when rolling but no rolls left
    
    // Player Stat Update Events (Optional for UI)
    public static Action<int> OnGachaRollsCountChanged;
    public static Action<int> OnRemainingQuestionsChanged;
}
