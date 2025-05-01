using System;
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
    public CinemachineCamera CamObj;
    public bool IsDark;
    public CameraStatus CamStatus;

    public CameraInfo(CinemachineCamera camObj, bool isDark, CameraStatus camStatus) {
        CamObj = camObj;
        IsDark = isDark;
        CamStatus = camStatus;
    }
}


public class SecurityCameraManager : MonoBehaviour {
    public static SecurityCameraManager Instance;

    [SerializeField] private Volume volume;

    [Header("Cameras")]
    [field: SerializeField]
    public CinemachineCamera PlayerCineCam { get; private set; }

    [field: SerializeField] public CinemachineCamera OutsideCineCam { get; private set; }

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

    public CameraInfo PlayerCam { get; private set; }
    public CameraInfo OutsideCam { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        InitializeCamInfos();
    }

    private void InitializeCamInfos() {
        PlayerCam = new CameraInfo(PlayerCineCam, false, CameraStatus.NONE);
        OutsideCam = new CameraInfo(OutsideCineCam, true, CameraStatus.FAINT_GLITCH);
    }

    public void SetActiveCamera(CameraInfo camInfo) {
        ZeroAllCamPriorities();
        ResetVolumeProfile();
        volume.profile = GetVolumeProfileFromCamInfo(camInfo);
        camInfo.CamObj.Priority = 10;
    }

    private void ZeroAllCamPriorities() {
        PlayerCam.CamObj.Priority = 0;
        OutsideCam.CamObj.Priority = 0;
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
}