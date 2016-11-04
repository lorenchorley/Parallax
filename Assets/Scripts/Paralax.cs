using UnityEngine;
using System;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class Paralax : MonoBehaviour {
    
    private abstract class ParalaxObjectProfile {

        public Transform obj;
        public Vector3 originalLocalPosition;

        protected Transform onePoint;
        protected float zTransformed;

        public abstract float GetNormalisedZ();

        public ParalaxObjectProfile(Transform obj, Transform zeroPoint, Transform onePoint) {
            this.onePoint = onePoint;
            this.obj = obj;
            originalLocalPosition = obj.position;

            // Normalised object z value (0 at zeroPoint, 1 at onePoint)
            //zTransformed = (obj.position.z - zeroPoint.position.z) / (onePoint.position.z - zeroPoint.position.z); 
            zTransformed = (obj.position.z - onePoint.position.z); 

        }

    }

    private class UpdatingParalaxObjectProfile : ParalaxObjectProfile {

        public UpdatingParalaxObjectProfile(Transform obj, Transform zeroPoint, Transform onePoint) : base (obj, zeroPoint, onePoint) {}

        public override float GetNormalisedZ() {
            return obj.position.z - onePoint.position.z;
        }

    }

    private class CachingParalaxObjectProfile : ParalaxObjectProfile {

        public CachingParalaxObjectProfile(Transform obj, Transform zeroPoint, Transform onePoint) : base (obj, zeroPoint, onePoint) { }

        public override float GetNormalisedZ() {
            return zTransformed;
        }

    }

    public float Speed = 1;
    public bool UpdateZValues = false;

    [Space]

    public Transform Camera; // The transform of the object that views the scene
    public Transform ParalaxFocus; // The transform of the object that should not move when the camera moves (Appears to move relative to the camera, but at the speed of the camera)

    [Space]

    public Transform[] InitialParalaxObjects;

    List<ParalaxObjectProfile> ParalaxObjects; // All the other objects that move when the camera moves
    Vector3 CameraInitialPosition;

    void Start() {
        Assert.IsNotNull(Camera);
        Assert.IsNotNull(ParalaxFocus);

        CameraInitialPosition = Camera.position;

        for (int i = 0; i < InitialParalaxObjects.Length; i++) {
            AddParalaxObject(InitialParalaxObjects[i]);
        }
        InitialParalaxObjects = null;

    }

    public void AddParalaxObject(Transform t) {
        if (ParalaxObjects == null)
            ParalaxObjects = new List<ParalaxObjectProfile>();

        if (UpdateZValues)
            ParalaxObjects.Add(new UpdatingParalaxObjectProfile(t, Camera, ParalaxFocus));
        else
            ParalaxObjects.Add(new CachingParalaxObjectProfile(t, Camera, ParalaxFocus));

    }

    public void UpdateParalaxObjects() {
        for (int i = 0; i < ParalaxObjects.Count; i++) {
            ParalaxObjectProfile profile = ParalaxObjects[i];
            float modifiedNormalisedZ = Speed * profile.GetNormalisedZ();

            Vector3 cameraDisplacement = Camera.position - CameraInitialPosition;
            //Vector3 paralaxedObjectDisplacement = new Vector3(ParalaxFn(profile.zTransformed, cameraDisplacement.x), ParalaxFn(profile.zTransformed, cameraDisplacement.y));
            Vector3 paralaxedObjectDisplacement = new Vector3(modifiedNormalisedZ * cameraDisplacement.x, modifiedNormalisedZ * cameraDisplacement.y);

            profile.obj.position = profile.originalLocalPosition + paralaxedObjectDisplacement;
        }
    }

    public void ClearAllObjects() {
        ParalaxObjects.Clear();
    }

    private float ParalaxFn(float z, float d) {
        return Speed * z * d;
    }

}
