using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerHealthBar : NetworkBehaviour
{
    // GameObjects
    GameRoundsManager roundsManager;
    RobotList robotList;
    ParticleSystem smoke;
    ChickenDinner chickenDinner;
    //[SerializeField] RectTransform healthAmount;
    public Slider healthSlider;
    public Slider abovePlayerHealth;

    // Network variables
    NetworkVariable<int> healthPoints = new NetworkVariable<int>(100);
    NetworkVariable<bool> changeColor = new NetworkVariable<bool>();

    // Local variables
    public int localHealth = 100;
    public int heal = 50;
    public bool changeColorLocal = false;


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            healthSlider = GameObject.Find("Hud").transform.Find("HealthBar").GetComponent<Slider>();
        }

        abovePlayerHealth = this.GetComponentInChildren<Slider>();

        roundsManager = GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>();
        chickenDinner = GameObject.Find("ChickenDinner").GetComponent<ChickenDinner>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
            GetHit(25);

        //if (IsOwner)
        //{
        //    healthSlider.value -= (healthSlider.value - (float)localHealth) * Time.deltaTime * 2;
        //}
        if (!IsOwner)
        {
            localHealth = healthPoints.Value;
            abovePlayerHealth.value = (float)localHealth;
        }

        //Die on fall
        if (gameObject.transform.position.y < -20) GetHit(100);
    }

    public void GetHit(int damageAmount)
    {
        if (IsOwner)
        {
            if ((localHealth - damageAmount) > 0)
            {
                localHealth = localHealth - damageAmount;
                healthSlider.value = (float)localHealth;
                abovePlayerHealth.value = (float)localHealth;
            }
            else
            {
                localHealth = 0;
                abovePlayerHealth.value = (float)localHealth;
                killed();
            }
            UpdateHealthInfoServerRpc(localHealth);
        }
    }

    [ServerRpc]
    public void UpdateHealthInfoServerRpc(int health)
    {
        healthPoints.Value = health;
    }

    public void healPowerUp()
    {
        if (IsOwner)
        {
            if ((localHealth + heal) < 100)
            {
                localHealth = localHealth + heal;
            }
            else
            {
                localHealth = 100;
            }
            UpdateHealthInfoServerRpc(localHealth);
        }
    }
    public void killed()
    {
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            if (c.GetType() != typeof(PlayerHealthBar))
            {
                c.enabled = false;
            }
        }

        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if (!IsHost)
        {
            if (!NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<Dead>(out var dead))
                return;
            dead.SetDeadServerRpc(true);
        }
        else
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
                return;
            if (!networkClient.PlayerObject.TryGetComponent<Dead>(out var dead))
                return;
            dead.SetDeadServerRpc(true);
        }
    }
}