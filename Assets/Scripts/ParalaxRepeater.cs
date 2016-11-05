using UnityEngine;
using System;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class ParalaxRepeater : MonoBehaviour {

    public Camera Camera;
    public float Margin = 0;

    bool Initialised = false;

    // Object dependent values
    Vector3 InitialPosition;
    Bounds bounds;
    int lowerIndex;
    List<GameObject> replicas;

    // Camera dependent values
    float cameraWidth;
    int objectCount;

    private float CalculatePosition(int index) {
        return bounds.center.x + (index + lowerIndex) * (bounds.size.x + Margin);
    }

    void Start() {
        Initialise();
    }

    void OnDrawGizmosSelected() {
        if (!Application.isPlaying)
            return;
        
        for (int i = 0; i < objectCount; i++) {
            Gizmos.DrawWireCube(new Vector3(CalculatePosition(i), bounds.center.y, bounds.center.z), bounds.size);
        }
        
    }

    private void Initialise() {
        if (Initialised)
            return;

        if (replicas == null) {
            replicas = new List<GameObject>();
            replicas.Add(gameObject);
        }

        objectCount = 0; // Force signal camera moved

        UpdateObjectDependentValues();
        InitialPosition = bounds.center;

        UpdateCameraDependentValues();

        Initialised = true;
    }

    public void RefreshAll() {
        objectCount = 0; // Force signal camera moved
        UpdateObjectDependentValues();
        UpdateCameraDependentValues();
    }

    private void UpdateObjectDependentValues() {

        // Update Bounds
        bounds = GetBoundingBox(gameObject);

    }

    private void UpdateCameraDependentValues() {
        // Assumes bounds has already been calculated

        // Update camera width value
        cameraWidth = Camera.orthographicSize * 2.0f * Camera.aspect;

        // Derive count value
        float width = bounds.size.x + Margin;
        int newCount = (int) Mathf.Ceil(cameraWidth / width) + 2;

        // Set lower limit
        if (newCount < 3)
            newCount = 3;

        // Signal camera that the count has changed
        if (Application.isPlaying && newCount != objectCount) {

            // Update count value
            objectCount = newCount;

            SignalCameraMoved();

        }

    }

    public void SignalCameraMoved() {

        float width = bounds.size.x + Margin;
        float cameraCenterX = Camera.transform.position.x - cameraWidth / 2;
        int newLowerIndex = Mathf.FloorToInt((cameraCenterX - InitialPosition.x - bounds.extents.x) / width);

        if (Application.isPlaying && lowerIndex != newLowerIndex) {
            lowerIndex = newLowerIndex;
            DistributeReplicas();
        }

    }

    private void DistributeReplicas() {

        GameObject replica;

        // make sure there are at least enough replicas
        while (replicas.Count < objectCount) {
            replica = Instantiate(gameObject);
            replica.transform.SetParent(transform.parent);
            replica.name = gameObject.name + " (Replica " + replicas.Count + ")";
            replicas.Add(replica);

            ParalaxRepeater pr = replica.gameObject.GetComponent<ParalaxRepeater>();
            if (pr != null)
                Destroy(pr);
        }

        for (int i = 0; i < objectCount; i++) {
            replica = replicas[i];
            if (i < objectCount) {
                replica.SetActive(true);
                replica.transform.position = new Vector3(CalculatePosition(i), transform.position.y, transform.position.z);
            } else {
                replica.SetActive(false);
            }
        }

    }

    private Bounds GetBoundingBox(GameObject go) {
        Bounds b;
        SpriteRenderer[] rs = go.GetComponentsInChildren<SpriteRenderer>();

        // If no sprites are found
        if (rs.Length == 0) {
            b = new Bounds(Vector3.zero, Vector3.zero);
            return default(Bounds);
        }

        // Use the first bounds
        b = rs[0].bounds;

        // Grow the bounds to include one after that
        for (int i = 1; i < rs.Length; i++) {
            b.Encapsulate(rs[i].bounds);
        }

        return b;
    }

}
