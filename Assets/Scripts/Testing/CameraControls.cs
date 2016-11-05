using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    public float speed;
    public Paralax Paralax;
    public ParalaxRepeater[] Repeaters;

    void Update () {
        bool moved = false;

        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.position += Vector3.up * speed * Time.deltaTime;
            moved = true;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position -= Vector3.up * speed * Time.deltaTime;
            moved = true;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.position -= Vector3.right * speed * Time.deltaTime;
            moved = true;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.position += Vector3.right * speed * Time.deltaTime;
            moved = true;
        }

        if (moved && Paralax != null)
            Paralax.UpdateParalaxObjects();

        if (moved && Repeaters != null)
            foreach (ParalaxRepeater r in Repeaters)
                r.SignalCameraMoved();

    }

}
