using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject[] fruitPrefabs;
    public SnakeMovement snakeMovement;

    public void SpawnFood() {
        // 🔥 Safety check 1: Array exists?
        if (fruitPrefabs == null || fruitPrefabs.Length == 0) {
            Debug.LogError("FoodManager: No fruit prefabs assigned!");
            return;
        }

        // 🔥 Safety check 2: Find a valid fruit prefab (skips null ones)
        GameObject fruitPrefab = null;
        int attempts = 0;
        while (fruitPrefab == null && attempts < fruitPrefabs.Length) {
            int index = Random.Range(0, fruitPrefabs.Length);
            fruitPrefab = fruitPrefabs[index];
            attempts++;
            if (fruitPrefab == null) {
                Debug.LogWarning($"FoodManager: Fruit prefab at index {index} is null. Trying another one.");
            }
        }

        // 🔥 Safety check 3: If all prefabs are null, stop.
        if (fruitPrefab == null) {
            Debug.LogError("FoodManager: No valid fruit prefabs found! Check your array.");
            return;
        }

        // Find a valid position
        Vector2 pos = GetValidPosition();
        Instantiate(fruitPrefab, pos, Quaternion.identity);
    }

    // 🆕 NEW METHOD: Clears all food from the screen
    public void ClearAllFood() {
        GameObject[] existingFoods = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject food in existingFoods) {
            Destroy(food);
        }
    }

    private Vector2 GetValidPosition() {
        Vector2 pos;
        bool isOnSnake;
        int maxAttempts = 100;
        int attempts = 0;
        
        do {
            float x = Random.Range(-11f, 11f);
            float y = Random.Range(-2f, 4f);
            pos = new Vector2(Mathf.Round(x), Mathf.Round(y));
            isOnSnake = false;
            attempts++;

            if (snakeMovement != null) {
                var segments = snakeMovement.GetSegments();
                if (segments != null) {
                    foreach (Transform seg in segments) {
                        if (seg != null && (Vector2)seg.position == pos) {
                            isOnSnake = true;
                            break;
                        }
                    }
                }
            } else {
                // If snake reference is missing, just spawn anywhere
                return pos;
            }

            if (attempts >= maxAttempts) break;
        } while (isOnSnake);

        return pos;
    }
}