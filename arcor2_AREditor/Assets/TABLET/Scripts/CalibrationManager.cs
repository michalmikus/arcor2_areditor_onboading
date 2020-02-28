using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CalibrationManager : Singleton<CalibrationManager> {

    public ARAnchorManager ARAnchorManager;
    public ARPlaneManager ARPlaneManager;
    public ARRaycastManager ARRaycastManager;
    public ARTrackedImageManager ARTrackedImageManager;

    private ARAnchor WorldAnchor;
    
    private void OnEnable() {
        GameManager.Instance.OnConnectedToServer += ConnectedToServer;
    }

    private void OnDisable() {
        GameManager.Instance.OnConnectedToServer -= ConnectedToServer;
    }

    public void CreateAnchor(Transform tf) {
#if !UNITY_EDITOR
        RemoveWorldAnchor();

        List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        if (ARRaycastManager.Raycast(new Ray(tf.position, Vector3.down), raycastHits, TrackableType.PlaneWithinPolygon)) {
            Pose hitPose = raycastHits[0].pose;
            TrackableId hitPlaneId = raycastHits[0].trackableId;
            ARPlane plane = ARPlaneManager.GetPlane(hitPlaneId);

            WorldAnchor = ARAnchorManager.AttachAnchor(plane,
                new Pose(hitPose.position, Quaternion.FromToRotation(tf.up, plane.normal) * tf.rotation));

            AttachScene();
        }

        foreach (ARTrackedImage trackedImg in ARTrackedImageManager.trackables) {
            trackedImg.gameObject.SetActive(false);
        }
#endif
    }

    public void RemoveWorldAnchor() {
#if !UNITY_EDITOR
        if (WorldAnchor != null) {
            ARAnchorManager.RemoveAnchor(WorldAnchor);
        }
#endif
    }

    private void AttachScene() {
#if !UNITY_EDITOR
        if (WorldAnchor == null) {
            WorldAnchor = ARAnchorManager.AddAnchor(new Pose(Camera.main.transform.position, Camera.main.transform.rotation));
        }

        Scene.Instance.transform.parent = WorldAnchor.transform;

        Scene.Instance.transform.localPosition = Vector3.zero;
        Scene.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
        Scene.Instance.transform.localEulerAngles = Vector3.zero;
#endif
    }

    private void ConnectedToServer(object sender, Base.StringEventArgs e) {
        AttachScene();
    }
}
