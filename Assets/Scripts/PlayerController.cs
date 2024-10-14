using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Vector2 moveValue;
    public float speed;
    private int count;

    private void Start() {
        count = 0;
    }

    void OnMove(InputValue value) {
        moveValue = value.Get<Vector2>();
    }

    void FixedUpdate() {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other) { 
        if (other.gameObject.tag == "PickUp") {
            count++;
            other.gameObject.SetActive(false);
        }
    }
}