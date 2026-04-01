using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private float _nextUpdate;
    public float speed = 0.1f; // Adjust this to change speed!

    void Update()
    {
        // Capture input in Update so it's snappy
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down) _direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up) _direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right) _direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left) _direction = Vector2.right;
    }

    void FixedUpdate()
    {
        // Only move when the clock hits the next update time
        if (Time.time >= _nextUpdate)
        {
            _nextUpdate = Time.time + speed;

            this.transform.position = new Vector3(
                Mathf.Round(this.transform.position.x) + _direction.x,
                Mathf.Round(this.transform.position.y) + _direction.y,
                0.0f
            );
        }
    }
}