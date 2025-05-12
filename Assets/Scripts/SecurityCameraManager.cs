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
public struct CameraInfo {
    public CinemachineCamera CineCamObj;
    public bool IsDark;
    public CameraStatus CamStatus;

    public CameraInfo(CinemachineCamera cineCamObj, bool isDark, CameraStatus camStatus) {
        CineCamObj = cineCamObj;
        IsDark = isDark;
        CamStatus = camStatus;
    }
}


public class SecurityCameraManager : MonoBehaviour {
    public static SecurityCameraManager Instance;

    [SerializeField] private SecurityCamera interactable;
    
    [SerializeField] private Volume volume;

    [SerializeField] private Canvas glowCanvas;

    [SerializeField] private GameObject securityCameraUIOverlay;
    
    [Header("CMCameras")]
    [field: SerializeField] public CinemachineCamera PlayerCineCam { get; private set; }
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
    
    public List<CameraInfo> CameraInfos { get; private set; }
    public CameraInfo PlayerCamInfo { get; private set; }

    public bool CheckingCameras { get; private set; } = false;

    public int SecurityCameraIndx {get; private set;} = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            InitializeCamInfos();
        } else {
            Destroy(gameObject);
            return;
        }

       
    }

    private void InitializeCamInfos() {
        PlayerCamInfo = new CameraInfo(PlayerCineCam, false, CameraStatus.NONE);
        
        print("before finish init");
        CameraInfos = new List<CameraInfo>();
        
        CameraInfos.Add(new CameraInfo(InsideCineCam1, false, CameraStatus.NORMAL)); // inside cam 1
        CameraInfos.Add(new CameraInfo(InsideCineCam2, false, CameraStatus.SLIGHT_GLITCH)); // inside cam 2
        CameraInfos.Add(new CameraInfo(InsideCineCam3, false, CameraStatus.NORMAL)); // inside cam 3
        CameraInfos.Add(new CameraInfo(InsideCineCam4, false, CameraStatus.NORMAL)); // inside cam 4
        
        CameraInfos.Add(new CameraInfo(OutsideCineCam1, true, CameraStatus.FAINT_GLITCH)); // outside cam 1
        CameraInfos.Add(new CameraInfo(OutsideCineCam5, true, CameraStatus.NORMAL)); // outside cam 5
        CameraInfos.Add(new CameraInfo(OutsideCineCam2, true, CameraStatus.SEVERE_GLITCH)); // outside cam 2
        CameraInfos.Add(new CameraInfo(OutsideCineCam3, true, CameraStatus.MODERATE_GLITCH)); // outside cam 3
        CameraInfos.Add(new CameraInfo(OutsideCineCam4, true, CameraStatus.NO_SIGNAL)); // outside cam 4
        
       
        
        print("finish init");
    }

    public void SetActiveCamera(CameraInfo camInfo) {
        GameObject.FindWithTag("MainCamera").GetComponent<CinemachineInputAxisController>().enabled = false;
        CheckingCameras = true;
        print("Setting active camera to " + camInfo.CineCamObj.name);
        ZeroAllCamPriorities();
        ResetVolumeProfile();
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

    public void Reset() {
        ExitSecurityCamera();
        ResetVolumeProfile();
        SecurityCameraIndx = 0;
    }
    
}