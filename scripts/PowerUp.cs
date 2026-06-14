using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int type; // 0-9
    private SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        // 🆕 Auto-destroy after 5 seconds if not collected
        Destroy(gameObject, 5f);
    }

    public void Setup(int t) {
        type = t;
        if (sr == null) return;

        switch (t) {
            case 0: sr.color = Color.gold; break;
            case 1: sr.color = Color.cyan; break;
            case 2: sr.color = Color.magenta; break;
            case 3: sr.color = Color.green; break;
            case 4: sr.color = Color.red; break;
            case 5: sr.color = Color.white; break;
            case 6: sr.color = Color.magenta; break;
            case 7: sr.color = new Color(1f, 0.5f, 0f); break;
            case 8: sr.color = Color.blue; break;
            case 9: sr.color = Color.black; break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            SnakeMovement snake = other.GetComponent<SnakeMovement>();
            if (snake != null) {
                snake.ActivatePowerUp(type);
                Destroy(gameObject);
            }
        }
    }
}