using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseEntity {
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Vector2 moveInput;

    // Start is called before the first frame update
    protected override void Start() {
        rb = GetComponent<Rigidbody2D>();

        base.Start();
    }

    // Update is called once per frame
    void Update() {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
    }

    void FixedUpdate() {
        rb.velocity = moveInput * moveSpeed;
    }
}
