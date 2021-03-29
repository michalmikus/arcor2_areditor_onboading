using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.UI;

public class ControlBoxManager : Singleton<ControlBoxManager> {

    [SerializeField]
    private InputDialog InputDialog;
    [SerializeField]

    public Toggle MoveToggle;
    public Toggle RotateToggle;
    public Toggle TrackablesToggle;
    public Toggle ConnectionsToggle;
    public Toggle VRModeToggle;
    public Toggle CalibrationElementsToggle;

    public AddActionPointUsingRobotDialog AddActionPointUsingRobotDialog;

    private ManualTooltip calibrationElementsTooltip;


    private bool useGizmoMove = false;
    public bool UseGizmoMove {
        get => useGizmoMove;
        set {
            useGizmoMove = value;
            if (useGizmoMove) {
                TransformGizmo.Instance.transformType = TransformType.Move;
                useGizmoRotate = false;
                RotateToggle.isOn = false;
                MoveToggle.isOn = true;
            }
        }
    }

    private bool useGizmoRotate = false;
    public bool UseGizmoRotate {
        get => useGizmoRotate;
        set {
            useGizmoRotate = value;
            if (useGizmoRotate) {
                TransformGizmo.Instance.transformType = TransformType.Rotate;
                useGizmoMove = false;
                RotateToggle.isOn = true;
                MoveToggle.isOn = false;
            }
        }
    }

    private void Start() {
        MoveToggle.isOn = PlayerPrefsHelper.LoadBool("control_box_gizmo_move", false);
        RotateToggle.isOn = PlayerPrefsHelper.LoadBool("control_box_gizmo_rotate", false);
#if UNITY_ANDROID && AR_ON
        TrackablesToggle.isOn = PlayerPrefsHelper.LoadBool("control_box_display_trackables", false);
        CalibrationElementsToggle.interactable = false;
        CalibrationElementsToggle.isOn = true;
        calibrationElementsTooltip = CalibrationElementsToggle.GetComponent<ManualTooltip>();
        calibrationElementsTooltip.DisplayAlternativeDescription = true;
#endif
        ConnectionsToggle.isOn = PlayerPrefsHelper.LoadBool("control_box_display_connections", true);
        GameManager.Instance.OnGameStateChanged += GameStateChanged;
    }


    private void OnEnable() {
        CalibrationManager.Instance.OnARCalibrated += OnARCalibrated;
        CalibrationManager.Instance.OnARRecalibrate += OnARRecalibrate;
    }

    private void OnDisable() {
        CalibrationManager.Instance.OnARCalibrated -= OnARCalibrated;
        CalibrationManager.Instance.OnARRecalibrate -= OnARRecalibrate;
    }

    

    /// <summary>
    /// Triggered when the system calibrates = anchor is created (either when user clicks on calibration cube or when system loads the cloud anchor).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnARCalibrated(object sender, GameObjectEventArgs args) {
#if UNITY_ANDROID && AR_ON
        // Activate toggle to enable hiding/displaying calibration cube
        CalibrationElementsToggle.interactable = true;
        calibrationElementsTooltip.DisplayAlternativeDescription = false;
#endif
    }


    private void OnARRecalibrate(object sender, EventArgs args) {
#if UNITY_ANDROID && AR_ON
        // Disactivate toggle to disable hiding/displaying calibration cube
        CalibrationElementsToggle.interactable = false;
        calibrationElementsTooltip.DisplayAlternativeDescription = true;
#endif
    }


    /// <summary>
    /// Called when the user tries to click on the show/hide toggle before the system is calibrated.
    /// </summary>
    public void OnCalibrationElementsToggleClick() {
        if (!CalibrationManager.Instance.Calibrated) {
            Notifications.Instance.ShowNotification("System is not calibrated", "Please locate the visual marker, wait for the calibration cube to show up and click on it, in order to calibrate the system");
        }
    }

    private void GameStateChanged(object sender, GameStateEventArgs args) {
        MoveToggle.gameObject.SetActive(false);
        RotateToggle.gameObject.SetActive(false);
        ConnectionsToggle.gameObject.SetActive(false);
        switch (args.Data) {

            case GameManager.GameStateEnum.ProjectEditor:
                MoveToggle.gameObject.SetActive(true);
                // use move only if the gizmo was previously active (for tablet version)
                if (UseGizmoMove || UseGizmoRotate)
                    UseGizmoMove = true;
                ConnectionsToggle.gameObject.SetActive(true);
                break;
            case GameManager.GameStateEnum.SceneEditor:
                RotateToggle.gameObject.SetActive(true);
                MoveToggle.gameObject.SetActive(true);
                break;
            case GameManager.GameStateEnum.PackageRunning:
                ConnectionsToggle.gameObject.SetActive(true);
                break;
        }

    }

    public void DisplayTrackables(bool active) {
#if UNITY_ANDROID && AR_ON
        TrackingManager.Instance.DisplayPlanesAndPointClouds(active);
#endif
    }

    public void DisplayCalibrationElements(bool active) {
#if UNITY_ANDROID && AR_ON
        CalibrationManager.Instance.ActivateCalibrationElements(active);
#endif
    }

    public void ToggleVRMode(bool active) {
        if (active) {
            VRModeManager.Instance.EnableVRMode();
        } else {
            VRModeManager.Instance.DisableVRMode();
        }
    }

    public void ShowActionObjectSettingsMenu() {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.ActionObjectSettingsMenu);
    }

    public void DisplayConnections(bool active) {
        ConnectionManagerArcoro.Instance.DisplayConnections(active);
    }

    

    private void OnDestroy() {
        PlayerPrefsHelper.SaveBool("control_box_gizmo_move", MoveToggle.isOn);
        PlayerPrefsHelper.SaveBool("control_box_gizmo_rotate", RotateToggle.isOn);
#if UNITY_ANDROID && AR_ON
        PlayerPrefsHelper.SaveBool("control_box_display_trackables", TrackablesToggle.isOn);
#endif
        PlayerPrefsHelper.SaveBool("control_box_display_connections", ConnectionsToggle.isOn);
    }
}
