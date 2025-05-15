using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] public int MinutesAfterMidnight = 0;
    public int CurrDayOfMonth = 9;

    [SerializeField] private List<CustomerTendencyGroup> abnormalCustomers = new();
    [SerializeField] public Dictionary<int, int> MAMShiftStart, MAMShiftEnd;
    private Dictionary<int, int> maxNPCsPerNight;


    private float spawnChance;
    private int currNight = 1;
    private bool triggeredEndShift = false;

    private Dictionary<int, List<(int, CameraStatus)>> night1CamStatusChanges,
        night2CamStatusChanges,
        night3CamStatusChanges,
        night4CamStatusChanges;

    private void Awake() {
        #region AbnormalCustomers

        // Night 1
        abnormalCustomers.Add(new CustomerTendencyGroup(new List<CustomerTendencyEntry> {
            new CustomerTendencyEntry(
                new(){CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 3), Random.Range(0, 59))
            )
        }));

        // Night 2
        abnormalCustomers.Add(new CustomerTendencyGroup(new List<CustomerTendencyEntry> {
            new CustomerTendencyEntry(
                new(){CustomerTendency.OBSESSIVE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 3), Random.Range(0, 59))
            ),
            new CustomerTendencyEntry(
                new(){CustomerTendency.SIDEDOOR},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 3), Random.Range(0, 59))
            )
        }));
        
        // Night 3
        abnormalCustomers.Add(new CustomerTendencyGroup(new List<CustomerTendencyEntry> {
            new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE,CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 3), Random.Range(0, 59))
            ),
            new CustomerTendencyEntry(
                new(){CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(3, 4), Random.Range(0, 30))
            ),
            new CustomerTendencyEntry(
            new(){CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
            TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(3, 4), Random.Range(0, 30))
        )
        }));
        
        // Night 4
        abnormalCustomers.Add(new CustomerTendencyGroup(new List<CustomerTendencyEntry> {
            new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(0, 40))
            ),new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(0, 40))
            ),new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(0, 40))
            ),new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(0, 40))
            ),new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(0, 40))
            ),new CustomerTendencyEntry(
                new(){CustomerTendency.OUTSIDEWALK, CustomerTendency.STARE},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(0, 40))
            ),
            new CustomerTendencyEntry(
                new() { CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(15, 59))
            ),
            new CustomerTendencyEntry(
                new() { CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(15, 59))
            ),
            new CustomerTendencyEntry(
                new() { CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(15, 59))
            ),
            new CustomerTendencyEntry(
                new() { CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(15, 59))
            ),
            new CustomerTendencyEntry(
                new() { CustomerTendency.SIDEDOOR, CustomerTendency.STARE, CustomerTendency.OBSESSIVE, CustomerTendency.VARIEDSPEED},
                TimeUtils.ConvertToMinutesAfterMidnight(TimeUtils.RandomRangeMod24(22, 4), Random.Range(15, 59))
            ),
            
        }));
        
      

        #endregion

        #region CameraStatusChanges

        night1CamStatusChanges = new Dictionary<int, List<(int, CameraStatus)>> {
            {
                TimeUtils.ConvertToMinutesAfterMidnight(22, 1), new() {
                    new(0, CameraStatus.NORMAL),
                    new(1, CameraStatus.NORMAL),
                    new(2, CameraStatus.NORMAL),
                    new(3, CameraStatus.NORMAL),
                    new(4, CameraStatus.NORMAL),
                    new(5, CameraStatus.NORMAL),
                    new(6, CameraStatus.NORMAL),
                    new(7, CameraStatus.NORMAL),
                    new(8, CameraStatus.NORMAL),
                }
            }, {
                TimeUtils.ConvertToMinutesAfterMidnight(0, Random.Range(0, 20)), new() {
                    new(0, CameraStatus.NORMAL),
                    new(1, CameraStatus.NORMAL),
                    new(2, CameraStatus.SLIGHT_GLITCH),
                    new(3, GetRandomCameraStatus()),
                    new(4, CameraStatus.NORMAL),
                    new(5, CameraStatus.MODERATE_GLITCH),
                    new(6, CameraStatus.NORMAL),
                    new(7, CameraStatus.FAINT_GLITCH),
                    new(8, GetRandomCameraStatus()),
                }
            }, {
                TimeUtils.ConvertToMinutesAfterMidnight(Random.Range(1, 2), Random.Range(0, 59)), new() {
                    new(0, CameraStatus.MODERATE_GLITCH),
                    new(1, CameraStatus.FAINT_GLITCH),
                    new(2, CameraStatus.SLIGHT_GLITCH),
                    new(3, GetRandomCameraStatus()),
                    new(4, CameraStatus.SLIGHT_GLITCH),
                    new(5, CameraStatus.MODERATE_GLITCH),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            }, {
                TimeUtils.ConvertToMinutesAfterMidnight(Random.Range(3, 4), Random.Range(0, 59)), new() {
                    new(0, CameraStatus.NORMAL),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, CameraStatus.NORMAL),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, CameraStatus.NORMAL),
                }
            },
        };

        night2CamStatusChanges = new Dictionary<int, List<(int, CameraStatus)>> {
            {
                TimeUtils.ConvertToMinutesAfterMidnight(22, 1), new() {
                    new(0, CameraStatus.NORMAL),
                    new(1, CameraStatus.NORMAL),
                    new(2, CameraStatus.SLIGHT_GLITCH),
                    new(3, CameraStatus.NORMAL),
                    new(4, CameraStatus.NORMAL),
                    new(5, CameraStatus.NORMAL),
                    new(6, CameraStatus.SLIGHT_GLITCH),
                    new(7, CameraStatus.NORMAL),
                    new(8, CameraStatus.SLIGHT_GLITCH),
                }
            }, {
                TimeUtils.ConvertToMinutesAfterMidnight(0, Random.Range(0, 20)), new() {
                    new(0, CameraStatus.NORMAL),
                    new(1, CameraStatus.NORMAL),
                    new(2, CameraStatus.SLIGHT_GLITCH),
                    new(3, GetRandomCameraStatus()),
                    new(4, CameraStatus.NORMAL),
                    new(5, CameraStatus.MODERATE_GLITCH),
                    new(6, CameraStatus.NORMAL),
                    new(7, CameraStatus.FAINT_GLITCH),
                    new(8, GetRandomCameraStatus()),
                }
            }, {
                TimeUtils.ConvertToMinutesAfterMidnight(Random.Range(1, 2), Random.Range(0, 59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            }, {
                TimeUtils.ConvertToMinutesAfterMidnight(Random.Range(3, 4), Random.Range(0, 59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, CameraStatus.SLIGHT_GLITCH),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, CameraStatus.NORMAL),
                    new(8, GetRandomCameraStatus()),
                }
            }
        };

        night3CamStatusChanges = new Dictionary<int, List<(int, CameraStatus)>> {
            {
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(23, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(0, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(0,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },
        };

        night4CamStatusChanges = new Dictionary<int, List<(int, CameraStatus)>> {
            {
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(22, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(23, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(23, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(0, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(0, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(1, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(1, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(2, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(2, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(3, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(3, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(4, Random.Range(0,29)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },{
                TimeUtils.ConvertToMinutesAfterMidnight(4, Random.Range(30,59)), new() {
                    new(0, GetRandomCameraStatus()),
                    new(1, GetRandomCameraStatus()),
                    new(2, GetRandomCameraStatus()),
                    new(3, GetRandomCameraStatus()),
                    new(4, GetRandomCameraStatus()),
                    new(5, GetRandomCameraStatus()),
                    new(6, GetRandomCameraStatus()),
                    new(7, GetRandomCameraStatus()),
                    new(8, GetRandomCameraStatus()),
                }
            },
        };

        #endregion
    }

    private void OnEnable() {
        GameEvents.OnSetDayOfMonth += SetDayOfMonth;
        GameEvents.OnNightUpdate += SetCurrNight;
        GameEvents.OnTriggerEndShift += TriggerEndShift;

        GameQuery.OnGetCurrentDayOfMonth = GetCurrDayOfMonth;
        GameQuery.OnGetCurrentNight = () => currNight;
        GameQuery.OnGetMinutesAfterMidnight = () => MinutesAfterMidnight;
    }

    private void OnDisable() {
        GameEvents.OnSetDayOfMonth -= SetDayOfMonth;
        GameEvents.OnNightUpdate -= SetCurrNight;
        GameEvents.OnTriggerEndShift -= TriggerEndShift;

        GameQuery.OnGetCurrentDayOfMonth = null;
        GameQuery.OnGetCurrentNight = null;
        GameQuery.OnGetMinutesAfterMidnight = null;
    }

    private void Start() {
        MAMShiftStart = new Dictionary<int, int>();
        MAMShiftEnd = new Dictionary<int, int>();
        MAMShiftStart[1] = (60 * 22) + 0;
        MAMShiftStart[2] = (60 * 22) + 0;
        MAMShiftStart[3] = (60 * 22) + 0;
        MAMShiftStart[4] = (60 * 22) + 0;
        MAMShiftEnd[1] = (60 * 5) + 0;
        MAMShiftEnd[2] = (60 * 5) + 0;
        MAMShiftEnd[3] = (60 * 5) + 0;
        MAMShiftEnd[4] = (60 * 5) + 0;
        maxNPCsPerNight = new Dictionary<int, int>();
        maxNPCsPerNight[1] = 5;
        maxNPCsPerNight[2] = 9;
        maxNPCsPerNight[3] = 11;
        maxNPCsPerNight[4] = 14;

        StartCoroutine(DayNightCycle());
    }


    private IEnumerator DayNightCycle() {
        GameEvents.RaiseOnNightUpdate(currNight);
        GameEvents.RaiseOnGenerateRandomizedStalkerTimeSlots();
        GameEvents.RaiseOnEndShiftTriggerSetColliderEnabled(false);

        Dictionary<int, List<(int, CameraStatus)>> currNightCamChangesDict;

        switch (currNight) {
            case 1:
                currNightCamChangesDict = night1CamStatusChanges;
            break;
            case 2:
                currNightCamChangesDict = night2CamStatusChanges;
            break;
            case 3:
                currNightCamChangesDict = night3CamStatusChanges;
            break;
            case 4:
                currNightCamChangesDict = night4CamStatusChanges;
            break;
            default:
                currNightCamChangesDict = new();
                Debug.LogError("Invalid night for switching camera setting!");
            break;
        }


        MinutesAfterMidnight = MAMShiftStart[currNight];
        int remainingNPCs = maxNPCsPerNight[currNight];
        while (MinutesAfterMidnight != MAMShiftEnd[currNight]) {
            yield return new WaitForSeconds(.25f);

            if (remainingNPCs > 0) {
                int totalMinutesInDay = 1440;
                int remainingTime = (MAMShiftEnd[currNight] - MinutesAfterMidnight + totalMinutesInDay) %
                                    totalMinutesInDay;
                spawnChance = (float)remainingNPCs / (float)remainingTime;
                if (Random.value < spawnChance) {
                    GameEvents.RaiseOnSpawnRandomNPC();
                    remainingNPCs--;
                }
            }

            MinutesAfterMidnight++;
            if (MinutesAfterMidnight == 60 * 24) {
                MinutesAfterMidnight = 0;
                CurrDayOfMonth++;
                GameEvents.RaiseOnSetDayOfMonth(CurrDayOfMonth);
            }

            if (currNight == 2 && (MinutesAfterMidnight == TimeUtils.ConvertToMinutesAfterMidnight(2, 0)) || MinutesAfterMidnight == TimeUtils.ConvertToMinutesAfterMidnight(3,19)) {
                GameEvents.RaiseOnKonbiniDoorOpened();
            }

            if (currNight == 3 && MinutesAfterMidnight == TimeUtils.ConvertToMinutesAfterMidnight(1, 44)) {
                GameEvents.RaiseOnBreakerBoxTogglePower(false);
            }

            if (currNightCamChangesDict.ContainsKey(MinutesAfterMidnight)) {
                foreach (var tuple in currNightCamChangesDict[MinutesAfterMidnight]) {
                    (int indx, CameraStatus camStatus) = tuple;
                    GameEvents.RaiseOnSecurityCamerasSetCameraStatus(indx, camStatus);
                }
            }

            foreach (CustomerTendencyEntry entry in abnormalCustomers[currNight-1].Tendencies) {
                if (MinutesAfterMidnight == entry.Time) {
                    GameEvents.RaiseOnSpawnRandomNPC(entry.Types);
                }
            }

            
            

            GameEvents.RaiseOnTimeUpdate(MinutesAfterMidnight);
        }

        // Shift has ended at this point
        string endShiftMessage;
        switch (currNight) {
            case 1:
                endShiftMessage =
                    "My shift is over. People tend to say that the first day on the job is always the hardest, right?";
            break;
            case 2:
                endShiftMessage = "These people are a little... unsettling. Anyways, time to get off!";
            break;
            case 3:
                endShiftMessage = "This place is giving the creeps... I should quit soon.";
            break;
            case 4:
                endShiftMessage = "That's it. I'm never coming back here ever again!";
            break;
            default:
                endShiftMessage = "...";
            break;
        }

        GameEvents.RaiseOnDialogTypewriterStartTypewriter(endShiftMessage, 10f);
        GameEvents.RaiseOnEndShiftTriggerSetColliderEnabled(true);
        yield return new WaitUntil(() => triggeredEndShift);
        triggeredEndShift = false;

        if (currNight == 4) yield break;

        currNight++;
        GameEvents.RaiseOnDialogTypewriterToggleVisibility(false);
        GameEvents.RaiseOnNightUpdate(currNight);
        GameEvents.RaiseOnClearSpawnedNPCs();
        GameEvents.RaiseOnCounterItemDisplayClearItems();
        GameEvents.RaiseOnLineupManagerClearItems();
        GameEvents.RaiseOnResetCustomerItem();
        StartCoroutine(DayNightCycle());
    }

    private int GetCurrDayOfMonth() { return CurrDayOfMonth; }

    private void SetDayOfMonth(int newDate) { CurrDayOfMonth = newDate; }

    private void SetCurrNight(int newNight) { currNight = newNight; }

    private void TriggerEndShift() { triggeredEndShift = true; }

    public void ResetGame() {
        currNight = 1;
        CurrDayOfMonth = 9;
        MinutesAfterMidnight = 0;
    }

    private CameraStatus GetRandomCameraStatus() {
        Array values = Enum.GetValues(typeof(CameraStatus));
        // return (CameraStatus)values.GetValue(Random.Range(0, values.Length - 1)); // exclude NONE enum
        return (CameraStatus)values.GetValue(UnityEngine.Random.Range(0, 4));
    }
}