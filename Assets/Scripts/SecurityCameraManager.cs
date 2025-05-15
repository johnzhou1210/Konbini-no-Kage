using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public enum CameraStatus {
    NORMAL,
    FAINT_GLITCH,
    SLIGHT_GLITCH,
    MODERATE_GLITCH,
    SEVERE_GLITCH,
    NO_SIGNAL,
    NONE,
}

[Serializable]
public class CameraInfo {
    public CinemachineCamera CineCamObj;
    public bool IsDark;
    public CameraStatus CamStatus;

    public CameraInfo(CinemachineCamera cineCamObj, bool isDark, CameraStatus camStatus) {
        CineCamObj = cineCamObj;
        IsDark = isDark;
        CamStatus = camStatus;
    }

    public void SetCameraStatus(CameraStatus newStatus) { CamStatus = newStatus; }
}


public class SecurityCameraManager : MonoBehaviour {
    [SerializeField] private SecurityCamera interactable;

    [SerializeField] private Volume volume;

    [SerializeField] private Canvas glowCanvas;

    [SerializeField] private GameObject securityCameraUIOverlay;

    [Header("CMCameras")]
    [field: SerializeField]
    public CinemachineCamera PlayerCineCam { get; private set; }

    [field: SerializeField] public CinemachineCamera OutsideCineCam1 { get; private set; }
    [field: SerializeField] public CinemachineCamera OutsideCineCam2 { get; private set; }
    [field: SerializeField] public CinemachineCamera OutsideCineCam3 { get; private set; }
    [field: SerializeField] public CinemachineCamera OutsideCineCam4 { get; private set; }
    [field: SerializeField] public CinemachineCamera OutsideCineCam5 { get; private set; }
    [field: SerializeField] public CinemachineCamera InsideCineCam1 { get; private set; }
    [field: SerializeField] public CinemachineCamera InsideCineCam2 { get; private set; }
    [field: SerializeField] public CinemachineCamera InsideCineCam3 { get; private set; }
    [field: SerializeField] public CinemachineCamera InsideCineCam4 { get; private set; }

    [Header("Volumes")]
    [field: SerializeField]
    public VolumeProfile SCVNormal { get; private set; }

    [field: SerializeField] public VolumeProfile SCVNormalDark { get; private set; }

    [field: SerializeField] public VolumeProfile SCVFaintGlitch { get; private set; }
    [field: SerializeField] public VolumeProfile SCVFaintGlitchDark { get; private set; }
    [field: SerializeField] public VolumeProfile SCVSlightGlitch { get; private set; }
    [field: SerializeField] public VolumeProfile SCVSlightGlitchDark { get; private set; }
    [field: SerializeField] public VolumeProfile SCVModerateGlitch { get; private set; }
    [field: SerializeField] public VolumeProfile SCVModerateGlitchDark { get; private set; }
    [field: SerializeField] public VolumeProfile SCVSevereGlitch { get; private set; }
    [field: SerializeField] public VolumeProfile SCVSevereGlitchDark { get; private set; }
    [field: SerializeField] public VolumeProfile SCVNoSignal { get; private set; }
    [field: SerializeField] public VolumeProfile SCVNone { get; private set; }

    [SerializeField] public List<IntTransformPair> StalkerLocations;


    public List<CameraInfo> CameraInfos { get; private set; }
    public CameraInfo PlayerCamInfo { get; private set; }

    public bool CheckingCameras { get; private set; } = false;

    public int SecurityCameraIndx { get; private set; } = 0;

    private List<GameObject> stalkerCache;
    private List<TimeRange> stalkerNight1TimeSlots = new();
    private List<TimeRange> stalkerNight2TimeSlots = new();
    private List<TimeRange> stalkerNight3TimeSlots = new();
    private List<TimeRange> stalkerNight4TimeSlots = new();
    private List<(int start, int end)> randomizedSlots;

    private void Awake() {
        stalkerNight1TimeSlots = new List<TimeRange> {
            new(TimeUtils.ConvertToMinutesAfterMidnight(22, 30), TimeUtils.ConvertToMinutesAfterMidnight(0, 0), 10),
            new(TimeUtils.ConvertToMinutesAfterMidnight(23, 0), TimeUtils.ConvertToMinutesAfterMidnight(1, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(0, 0), TimeUtils.ConvertToMinutesAfterMidnight(1, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(1, 0), TimeUtils.ConvertToMinutesAfterMidnight(2, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(2, 0), TimeUtils.ConvertToMinutesAfterMidnight(3, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(3, 0), TimeUtils.ConvertToMinutesAfterMidnight(4, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(4, 0), TimeUtils.ConvertToMinutesAfterMidnight(4, 55), 5),
        };
        stalkerNight2TimeSlots = new List<TimeRange> {
            new(TimeUtils.ConvertToMinutesAfterMidnight(22, 30), TimeUtils.ConvertToMinutesAfterMidnight(0, 0), 10),
            new(TimeUtils.ConvertToMinutesAfterMidnight(23, 0), TimeUtils.ConvertToMinutesAfterMidnight(1, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(0, 0), TimeUtils.ConvertToMinutesAfterMidnight(1, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(2, 0), TimeUtils.ConvertToMinutesAfterMidnight(2, 0), 0),
            new(TimeUtils.ConvertToMinutesAfterMidnight(2, 0), TimeUtils.ConvertToMinutesAfterMidnight(3, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(3, 0), TimeUtils.ConvertToMinutesAfterMidnight(4, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(4, 0), TimeUtils.ConvertToMinutesAfterMidnight(4, 55), 5),
        };
        stalkerNight3TimeSlots = new List<TimeRange> {
            new(TimeUtils.ConvertToMinutesAfterMidnight(22, 30), TimeUtils.ConvertToMinutesAfterMidnight(23, 0), 10),
            new(TimeUtils.ConvertToMinutesAfterMidnight(23, 0), TimeUtils.ConvertToMinutesAfterMidnight(0, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(0, 0), TimeUtils.ConvertToMinutesAfterMidnight(0, 45), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(1, 0), TimeUtils.ConvertToMinutesAfterMidnight(1, 5), 5),
        };
        stalkerNight4TimeSlots = new List<TimeRange> {
            new(TimeUtils.ConvertToMinutesAfterMidnight(22, 30), TimeUtils.ConvertToMinutesAfterMidnight(23, 0), 10),
            new(TimeUtils.ConvertToMinutesAfterMidnight(23, 0), TimeUtils.ConvertToMinutesAfterMidnight(0, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(0, 0), TimeUtils.ConvertToMinutesAfterMidnight(1, 0), 15),
            new(TimeUtils.ConvertToMinutesAfterMidnight(1, 0), TimeUtils.ConvertToMinutesAfterMidnight(2, 0), 15),
        };
    }

    private void OnEnable() {
        GameEvents.OnSetActiveCamera += SetActiveCamera;
        GameEvents.OnExitSecurityCamera += ExitSecurityCamera;
        GameEvents.OnGenerateRandomizedStalkerTimeSlots += GenerateRandomizedStalkerTimeSlots;
        GameEvents.OnSecurityCamerasSetCameraStatus += SetCamStatus;

        GameQuery.OnGetCurrentSecurityCameraIndex = () => SecurityCameraIndx;
        GameQuery.OnGetIsCheckingCameras = () => CheckingCameras;
    }

    private void OnDisable() {
        GameEvents.OnSetActiveCamera -= SetActiveCamera;
        GameEvents.OnExitSecurityCamera -= ExitSecurityCamera;
        GameEvents.OnGenerateRandomizedStalkerTimeSlots -= GenerateRandomizedStalkerTimeSlots;
        GameEvents.OnSecurityCamerasSetCameraStatus -= SetCamStatus;

        GameQuery.OnGetCurrentSecurityCameraIndex = null;
        GameQuery.OnGetIsCheckingCameras = null;
    }

    private void InitializeCamInfos() {
        PlayerCamInfo = new CameraInfo(PlayerCineCam, false, CameraStatus.NONE);

        print("before finish init");
        CameraInfos = new List<CameraInfo>();

        CameraInfos.Add(new CameraInfo(InsideCineCam1, false, CameraStatus.NORMAL)); // inside cam 1
        CameraInfos.Add(new CameraInfo(InsideCineCam2, false, CameraStatus.NORMAL)); // inside cam 2
        CameraInfos.Add(new CameraInfo(InsideCineCam3, false, CameraStatus.NORMAL)); // inside cam 3
        CameraInfos.Add(new CameraInfo(InsideCineCam4, false, CameraStatus.NORMAL)); // inside cam 4

        CameraInfos.Add(new CameraInfo(OutsideCineCam1, true, CameraStatus.NORMAL)); // outside cam 1
        CameraInfos.Add(new CameraInfo(OutsideCineCam5, true, CameraStatus.NORMAL)); // outside cam 5
        CameraInfos.Add(new CameraInfo(OutsideCineCam2, true, CameraStatus.NORMAL)); // outside cam 2
        CameraInfos.Add(new CameraInfo(OutsideCineCam3, true, CameraStatus.NORMAL)); // outside cam 3
        CameraInfos.Add(new CameraInfo(OutsideCineCam4, true, CameraStatus.NORMAL)); // outside cam 4


        print("finish init");
    }

    private void Start() { InitializeCamInfos(); }

    public void SetActiveCamera(CameraInfo camInfo) {
        GameObject.FindWithTag("MainCamera").GetComponent<CinemachineInputAxisController>().enabled = false;
        CheckingCameras = true;
        print("Setting active camera to " + camInfo.CineCamObj.name);
        ZeroAllCamPriorities();
        ResetVolumeProfile();
        ReassessStalkerLocations();
        volume.profile = GetVolumeProfileFromCamInfo(camInfo);
        camInfo.CineCamObj.Priority = 10;
        camInfo.CineCamObj.GetComponent<Camera>().depth = 10;
        glowCanvas.worldCamera = camInfo.CineCamObj.GetComponent<Camera>();
        securityCameraUIOverlay.SetActive(true);
        securityCameraUIOverlay.transform.Find("CamNum").GetComponent<TextMeshProUGUI>().text =
            $"[CAM {SecurityCameraIndx + 1:00}]";
        var camComponent = camInfo.CineCamObj.GetComponent<Camera>();
        if (camComponent) camComponent.enabled = true;
    }

    public void SetActiveCamera(int index) {
        if (index < 0) {
            index = CameraInfos.Count - 1;
        } else if (index > CameraInfos.Count - 1) {
            index = 0;
        }

        SecurityCameraIndx = index;
        GameObject.FindWithTag("MainCamera").GetComponent<CinemachineInputAxisController>().enabled = false;
        CheckingCameras = true;
        print("Setting active camera to " + CameraInfos[index].CineCamObj.name);
        ZeroAllCamPriorities();
        ResetVolumeProfile();
        ReassessStalkerLocations();
        volume.profile = GetVolumeProfileFromCamInfo(CameraInfos[index]);
        CameraInfos[index].CineCamObj.Priority = 10;
        CameraInfos[index].CineCamObj.GetComponent<Camera>().depth = 10;
        glowCanvas.worldCamera = CameraInfos[index].CineCamObj.GetComponent<Camera>();
        securityCameraUIOverlay.SetActive(true);
        securityCameraUIOverlay.transform.Find("CamNum").GetComponent<TextMeshProUGUI>().text =
            $"[CAM {SecurityCameraIndx + 1:00}]";
        var camComponent = CameraInfos[index].CineCamObj.GetComponent<Camera>();
        if (camComponent) camComponent.enabled = true;
    }

    private void ZeroAllCamPriorities() {
        PlayerCamInfo.CineCamObj.Priority = 0;
        var playerCam = PlayerCamInfo.CineCamObj.GetComponent<Camera>();
        if (playerCam) playerCam.enabled = false;

        foreach (CameraInfo camInfo in CameraInfos) {
            camInfo.CineCamObj.Priority = 0;
            var unityCam = camInfo.CineCamObj.GetComponent<Camera>();
            if (unityCam) unityCam.enabled = false;
        }
    }

    public void ExitSecurityCamera() {
        SetActiveCamera(PlayerCamInfo);
        securityCameraUIOverlay.SetActive(false);
        CheckingCameras = false;
        GameObject.FindWithTag("MainCamera").GetComponent<CinemachineInputAxisController>().enabled = true;
        interactable.UnlitScreen();
        var playerCam = PlayerCamInfo.CineCamObj.GetComponent<Camera>();
        if (playerCam) playerCam.enabled = true;
    }

    private void ResetVolumeProfile() { volume.profile = SCVNone; }

    private void ReassessStalkerLocations() {
        
        
        int currNight = GameQuery.OnGetCurrentNight?.Invoke() ?? 1;
        stalkerCache = new List<GameObject>();
        Transform chosenTransform = StalkerLocations[currNight - 1].value;

        foreach (IntTransformPair item in StalkerLocations)
        {
            foreach (Transform child in item.value) {
                child.gameObject.SetActive(false);
            }
        }
        
        foreach (Transform child in chosenTransform) {
            stalkerCache.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        int minutesAfterMidnight = GameQuery.OnGetMinutesAfterMidnight?.Invoke() ?? 0;

        // if (currNight == 1) {
        for (int i = 0; i < stalkerCache.Count; i++) {
            var (start, end) = randomizedSlots[i];
            if (TimeUtils.IsTimeInRange(minutesAfterMidnight, start, end)) {
                stalkerCache[i].SetActive(true);
                break; // Only one active at a time
            }
        }

        // }
    }

    private VolumeProfile GetVolumeProfileFromCamInfo(CameraInfo camInfo) {
        switch (camInfo.CamStatus) {
            case CameraStatus.NORMAL:
                if (camInfo.IsDark) return SCVNormalDark;
                return SCVNormal;
            case CameraStatus.FAINT_GLITCH:
                if (camInfo.IsDark) return SCVFaintGlitchDark;
                return SCVFaintGlitch;
            case CameraStatus.SLIGHT_GLITCH:
                if (camInfo.IsDark) return SCVSlightGlitchDark;
                return SCVSlightGlitch;
            case CameraStatus.MODERATE_GLITCH:
                if (camInfo.IsDark) return SCVModerateGlitchDark;
                return SCVModerateGlitch;
            case CameraStatus.SEVERE_GLITCH:
                if (camInfo.IsDark) return SCVSevereGlitchDark;
                return SCVSevereGlitch;
            case CameraStatus.NO_SIGNAL:
                return SCVNoSignal;
            case CameraStatus.NONE:
                return SCVNone;
            default:
                Debug.LogError("Unknown CameraStatus");
                return SCVNone;
        }
    }

    private void GenerateRandomizedStalkerTimeSlots() {
        randomizedSlots = new();
        foreach (var slot in stalkerNight1TimeSlots) {
            randomizedSlots.Add(slot.GetRandomizedRange());
        }

        foreach (var slot in stalkerNight2TimeSlots) {
            randomizedSlots.Add(slot.GetRandomizedRange());
        }

        foreach (var slot in stalkerNight3TimeSlots) {
            randomizedSlots.Add(slot.GetRandomizedRange());
        }

        foreach (var slot in stalkerNight4TimeSlots) {
            randomizedSlots.Add(slot.GetRandomizedRange());
        }
    }

    private void SetCamStatus(int index, CameraStatus status) {
        CameraInfos[index].SetCameraStatus(status);
        // Debug.LogWarning("Set cam index " + index + " to " + status + ", " + CameraInfos[index].CineCamObj.transform.parent.name);
    }

    public void Reset() {
        ExitSecurityCamera();
        ResetVolumeProfile();
        SecurityCameraIndx = 0;
    }
}