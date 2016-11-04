using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    public float speed;
    public Paralax Paralax;

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

        if (moved)
            Paralax.UpdateParalaxObjects();

    }

}
