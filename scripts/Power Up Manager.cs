using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject[] powerUpPrefabs; // Drag your power-up prefabs here
    public SnakeMovement snakeMovement;

    private void Start() {
        InvokeRepeating(nameof(SpawnPowerUp), 10f, 10f);
    }

    public void SpawnPowerUp() {
        if (powerUpPrefabs.Length == 0 || (snakeMovement != null && snakeMovement.IsGameOver())) return;

        int type = Random.Range(0, powerUpPrefabs.Length);
        GameObject prefab = powerUpPrefabs[type];

        float x = Random.Range(-11f, 11f);
        float y = Random.Range(-2f, 4f);
        Vector2 pos = new Vector2(Mathf.Round(x), Mathf.Round(y));

        Instantiate(prefab, pos, Quaternion.identity);
    }
}