using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Objects;
using UI;

namespace InGame
{
    public class GameManager : MonoBehaviour
    {
        public API api;
        public GameObject playerPrefab;
        private Player player;
        public int playerDeathCount;
        public int? playerLeaderboardCount;
        public TextMeshProUGUI deathCountText;
        void Awake()
        {
            PauseMenu.GameIsPaused = false;
            if (playerPrefab != null)
            {
                player = Instantiate(playerPrefab).GetComponent<Player>();
            }
            api = FindObjectOfType<API>();
            api.GetUserLeaderboard(MapManager.Instance.activeMapId, UserManager.Instance.username);

        }

        void Start()
        {
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
            if (spawnPoint != null && player != null)
            {
                player.transform.position = spawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        public void UpdateDeathCountUI(int deathCount)
        {
            deathCountText.text = deathCount.ToString();
        }

        public IEnumerator FinishedLevelRoutine()
        {
            if (playerDeathCount < playerLeaderboardCount || playerLeaderboardCount == null)
            {
                yield return api.UpdateLeaderboardLeaderboardDeathRequest(MapManager.Instance.activeMapId, UserManager.Instance.username, playerDeathCount);
            }

            yield return api.UpdateLeaderboardCurrentDeathRequest(MapManager.Instance.activeMapId, UserManager.Instance.username, null);

            MapManager.Instance.LoadNextMap();
            SceneController.Instance.LoadScene();
        }

        public void SaveDeathCount()
        {
            api.UpdateLeaderboardCurrentDeath(MapManager.Instance.activeMapId, UserManager.Instance.username, playerDeathCount);
        }
    }
}