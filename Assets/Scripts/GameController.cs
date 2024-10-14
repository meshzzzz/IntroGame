using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameController : MonoBehaviour {

    public enum DebugMode { NORMAL, DISTANCE, VISION };
    public DebugMode currentMode;

    public GameObject player;
    public GameObject[] pickup;
    public TextMeshProUGUI posText;
    public TextMeshProUGUI velText;
    public TextMeshProUGUI distText;

    private LineRenderer lineRenderer;
    private Vector3 oldPos;

    // Start is called before the first frame update
    void Start() {
        oldPos = player.transform.position;
        currentMode = DebugMode.NORMAL;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;

    }

    //FIXEDUpdate -> change of velocity when rigidbody physics are processed
    void FixedUpdate() {
        switch (currentMode) {
            case DebugMode.NORMAL:
                posText.text = "";
                velText.text = "";
                distText.text = "";
                lineRenderer.enabled = false;
                break;
            case DebugMode.DISTANCE:
                resetPickups();
                lineRenderer.enabled = true;
                SetDistanceText();
                SetPositionText();
                SetVelocityText();
                break;
            case DebugMode.VISION:
                resetPickups();
                lineRenderer.enabled = true;
                posText.text = "";
                velText.text = "";
                SetVisionMode();
                break;
        }
        Debug.Log("currentmode: " + currentMode);
    }

    void OnChangeMode() {
        currentMode = (DebugMode)(((int)currentMode + 1) % System.Enum.GetValues(typeof(DebugMode)).Length);
    }

    private void SetPositionText() {
        posText.text = "Position: " + player.transform.position.x.ToString("0.00") + ", "                              
                                    + player.transform.position.z.ToString("0.00");
    }

    private void SetVelocityText() {
        Vector3 currentPos = player.transform.position;
        Vector3 velocity = (currentPos - oldPos) / Time.deltaTime;
        float speed = velocity.magnitude;
        velText.text = "Velocity: " + speed.ToString("0.00") + "units/s";

        oldPos = currentPos;
    }

    private void SetDistanceText() {
        GameObject closestPickup = null;
        float closestDist = float.MaxValue;

        for (int i = 0; i < pickup.GetLength(0); i ++) {
            if (pickup[i].activeSelf) {
                float dist = Vector3.Distance(player.transform.position, pickup[i].transform.position);

                if (dist < closestDist) {
                    if (closestPickup != null) {
                        closestPickup.GetComponent<Renderer>().material.color = Color.white;
                    }

                    closestDist = dist;
                    closestPickup = pickup[i];
                    closestPickup.GetComponent<Renderer>().material.color = Color.blue;
                }
            }
        }

        if (closestPickup != null) {
            distText.text = "Closest Pickup: " + closestDist.ToString("0.00") + "m";

            lineRenderer.SetPosition(0, player.transform.position);
            lineRenderer.SetPosition(1, closestPickup.transform.position);
        }
        else {
            distText.text = "No pickups near";
            lineRenderer.enabled = false;
        }
    }

    void resetPickups() {
        for (int i = 0; i < pickup.GetLength(0); i++) {
            pickup[i].GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void SetVisionMode() {
        Vector3 currentPos = player.transform.position;
        Vector3 velocity = (currentPos - oldPos) / Time.deltaTime;
        lineRenderer.SetPosition(0, player.transform.position);
        lineRenderer.SetPosition(1, player.transform.position + velocity);

        GameObject closestPickup = null;
        float closestDot = float.MinValue;

        for (int i = 0; i < pickup.GetLength(0); i++) {
            if (pickup[i].activeSelf) {
                Vector3 direction = pickup[i].transform.position - player.transform.position;
                float dot = Vector3.Dot(direction.normalized, velocity.normalized);

                if (dot > closestDot) {
                    if (closestPickup != null) {
                        closestPickup.GetComponent<Renderer>().material.color = Color.white;     
                    }
                    closestDot = dot;
                    closestPickup = pickup[i];
                    closestPickup.GetComponent<Renderer>().material.color = Color.green;
                    closestPickup.transform.LookAt(player.transform.position);
                }
            }
        }
        distText.text = "Approaching pickup: " + closestDot.ToString("0.00");
    }
}
