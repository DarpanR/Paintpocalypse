using System;
using UnityEngine;

public class PlayerController : BaseEntity {
    //public static PlayerController Instance { get; private set; }
    
    Rigidbody2D rb;

    // Remember last non-zero aim so you keep facing when no arrow is pressed
    Vector2 lastAim = Vector2.up;
    CountdownTimer timer = new CountdownTimer(10);
    Animator animator;

    protected override void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = transform.Find("Sprite").GetComponent<Animator>();

        //if (Instance != null && this != Instance) Destroy(gameObject);
        //Instance = this;
        base.Awake();
    }

    public override void Init(EntityData entityData, string guid) {
        base.Init(entityData, guid);
        timer.Start();

        GameEvents.RaiseHealthBarUpdate(
            (int)CurrentStats[StatType.CurrentHealth].value,
            (int)CurrentStats[StatType.MaxHealth].value);
    }

    // Update is called once per frame
    void Update() {
        // 1) Movement with WASD
        Vector2 move = Vector2.zero;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.W)) move.y += 1;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.S)) move.y -= 1;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.D)) move.x += 1;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.A)) move.x -= 1;
        move = move.normalized * CurrentStats[StatType.Speed].value;
        rb.velocity = move;

        rend.flipX = move.x < 0;
        animator.SetFloat("Velocity", move.magnitude);

        // 2) Facing/Aiming with arrows
        Vector2 aim = Vector2.zero;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.UpArrow)) aim.y += 1;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.DownArrow)) aim.y -= 1;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.RightArrow)) aim.x += 1;
        if (GameInputManager.Instance.IsKeyPressed(KeyCode.LeftArrow)) aim.x -= 1;

        if (aim.sqrMagnitude > 0.01f) {
            lastAim = aim.normalized;
            // rotate so local "up" points toward lastAim
            float angle = Mathf.Atan2(lastAim.y, lastAim.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public override void TakeDamage(IoperationStrategy operation) {
        base.TakeDamage(operation);

        GameEvents.RaiseHealthBarUpdate(
            (int)CurrentStats[StatType.CurrentHealth].value,
            (int)CurrentStats[StatType.MaxHealth].value
        );
    }

    protected override void Die() {
        Debug.Log(gameObject.name + " Died");
        GameEvents.RaiseVictory(new VictoryData {
            Type = VictoryType.Mouse,
            Message = "Mouse Wins!",
        });
        //int score = 0;                      //   PLACEHOLDER, IF WE WANT SCORE NEED CHANGE
    }
}
