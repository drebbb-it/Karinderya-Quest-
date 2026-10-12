using System;  

public static class GameEvents
{   
    // When player successfully collects all ingredients in the inventory
    public static event Action<IngredientData> OnIngredientCollected;
    public static void RaiseIngredientsCollected(IngredientData data) => OnIngredientCollected?.Invoke(data);
    // Fired when NPC steals an ingredient from the player
    public static event Action<string>OnIngredientStolen;
    public static void RaiseIngredientStolen(string ingredientId) => OnIngredientStolen?.Invoke(ingredientId);
    //
    public static event Action OnInventoryComplete;
    public static void RaiseInventoryComplete() => OnInventoryComplete?.Invoke();
    // Fired when the player is hit by a vehicle
    public static event Action OnPlayerHitByVehicle;
    public static void RaisePlayerHitByVehicle() => OnPlayerHitByVehicle?.Invoke();

    // Fired when lives reaches zero
    public static event Action OnPlayerOutOfLives;  
    public static void RaisePlayerOutOfLives() => OnPlayerOutOfLives?.Invoke();
    // Fired when the stage timer hits zero before inventory is completed
    public static event Action OnTimerExpired;
    public static void RaiseTimerExpired() => OnTimerExpired?.Invoke();
    //
    public static event Action<float> OnTimerTick;
    public static void RaiseTimerTick(float secondsRemaining) => OnTimerTick?.Invoke(secondsRemaining);

    // Fired when a required category's count changes (category, collected, required)
    public static event Action<IngredientCategory, int, int> OnInventoryProgress;
    public static void RaiseInventoryProgress(IngredientCategory category, int collected, int required)
    => OnInventoryProgress?.Invoke(category, collected, required);

}
