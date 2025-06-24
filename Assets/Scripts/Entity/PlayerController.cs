using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseEntity {
    public static PlayerController Instance { get; private set; }

    Rigidbody2D rb;

    // Remember last non-zero aim so you keep facing when no arrow is pressed
    Vector2 lastAim = Vector2.up;

    // Start is called before the first frame update

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    protected override void Start() {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // 1) Movement with WASD
        Vector2 move = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) move.y += 1;
        if (Input.GetKey(KeyCode.S)) move.y -= 1;
        if (Input.GetKey(KeyCode.D)) move.x += 1;
        if (Input.GetKey(KeyCode.A)) move.x -= 1;
        move = move.normalized * moveSpeed;
        rb.velocity = move;

        // 2) Facing/Aiming with arrows
        Vector2 aim = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) aim.y += 1;
        if (Input.GetKey(KeyCode.DownArrow)) aim.y -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) aim.x += 1;
        if (Input.GetKey(KeyCode.LeftArrow)) aim.x -= 1;

        if (aim.sqrMagnitude > 0.01f) {
            lastAim = aim.normalized;
            // rotate so local "up" points toward lastAim
            float angle = Mathf.Atan2(lastAim.y, lastAim.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void Die() {
        Debug.Log(gameObject.name + " Died");
        GameController.Instance.ChangeState(GameController.GameState.End);
        //int score = 0;                      //   PLACEHOLDER, IF WE WANT SCORE NEED CHANGE
    }
}
