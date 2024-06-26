﻿using GameNetcodeStuff;
using UnityEngine;

namespace PiggyVarietyMod.Patches
{
    public class CustomTouchInteractTrigger : MonoBehaviour
    {
        public bool isIdleTrigger;
        public bool isKillTrigger;
        public TeslaGate teslaGate;

        void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.parent.GetComponent<PlayerControllerB>() != null)
            {
                PlayerControllerB playerScript = collider.transform.parent.GetComponent<PlayerControllerB>();
                if (!playerScript.isPlayerDead)
                {
                    if (isIdleTrigger)
                    {
                        teslaGate.activatePlayerList.Add(playerScript);
                        teslaGate.activateList.Add(collider.gameObject);
                    }
                    else if (!isIdleTrigger && !isKillTrigger)
                    {
                        teslaGate.engagingPlayerList.Add(playerScript);
                        teslaGate.engagingList.Add(collider.gameObject);
                    }
                }
            }
            
            if (collider.transform.parent.GetComponent<EnemyAICollisionDetect>() != null)
            {
                if (collider.gameObject.GetComponent<EnemyAICollisionDetect>())
                {
                    EnemyAICollisionDetect enemyDetection = collider.gameObject.GetComponent<EnemyAICollisionDetect>();
                    IHittable hittable;
                    if (enemyDetection.transform.TryGetComponent<IHittable>(out hittable))
                    {
                        Plugin.mls.LogInfo("Tesla gate detected enemy: " + enemyDetection.mainScript.enemyType.enemyName + ", Idle: " + isIdleTrigger + ", Kill: " + isKillTrigger);
                        if (isIdleTrigger)
                        {
                            /*
                            teslaGate.activateList.Add(collider.gameObject);
                            */
                        }
                        if (isKillTrigger)
                        {
                            if (enemyDetection != null && enemyDetection.mainScript != null && enemyDetection.mainScript.IsOwner && enemyDetection.mainScript.enemyType.canDie && !enemyDetection.mainScript.isEnemyDead)
                            {
                                hittable.Hit(5, Vector3.zero, null, true, -1);
                            }
                        }
                        else
                        {
                            /*
                            teslaGate.engagingList.Add(collider.gameObject);
                            */
                        }
                    }
                }
            }
        }

        void OnTriggerStay(Collider collider)
        {
            if (isKillTrigger)
            {
                PlayerControllerB component = collider.gameObject.GetComponent<PlayerControllerB>();
                if (component != null && component == GameNetworkManager.Instance.localPlayerController && !component.isPlayerDead)
                {
                    GameNetworkManager.Instance.localPlayerController.KillPlayer(Vector3.down * 17f, true, CauseOfDeath.Electrocution, 0);
                    return;
                }
                EnemyAICollisionDetect component3 = collider.gameObject.GetComponent<EnemyAICollisionDetect>();
                if (component3 != null && component3.mainScript != null && component3.mainScript.IsOwner && component3.mainScript.enemyType.canDie && !component3.mainScript.isEnemyDead)
                {
                    component3.mainScript.KillEnemyOnOwnerClient(false);
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.transform.parent.GetComponent<PlayerControllerB>() != null)
            {
                PlayerControllerB playerScript = collider.transform.parent.GetComponent<PlayerControllerB>();
                teslaGate.engagingPlayerList.Remove(playerScript);
                teslaGate.engagingList.Remove(collider.gameObject);
                if (isIdleTrigger)
                {
                    teslaGate.activatePlayerList.Remove(playerScript);
                    teslaGate.activateList.Remove(collider.gameObject);
                }
            }

            /*
            if (collider.transform.parent.GetComponent<EnemyAICollisionDetect>() != null)
            {
                EnemyAICollisionDetect enemyDetection = collider.gameObject.GetComponent<EnemyAICollisionDetect>();
                teslaGate.engagingList.Remove(collider.gameObject);
                if (isIdleTrigger)
                {
                    teslaGate.activateList.Remove(collider.gameObject);
                }
            }
            */
        }
    }
}