using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;

public class healthbar : MonoBehaviourPunCallbacks
{
    public GameObject gameover_screen, play_screen;
    public TextMeshProUGUI myuid, oppuid, myhealthnum, opphealthnum, win_loss, timer;
    public Slider Myhealthslider, Opphealthslider;
    public Gradient gradient;
    public Image MYfill, OPPfill;
    private int coin, timer_sec;
    private string uid1;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        coin = 0; timer_sec = 90;
        uid1 = local_sav.uid_sav;
        myuid.GetComponent<TextMeshProUGUI>().text = uid1;
        Myhealthslider.maxValue = 30;
        Myhealthslider.value = 30;
        MYfill.color = gradient.Evaluate(1f);
        myhealthnum.GetComponent<TextMeshProUGUI>().text = Myhealthslider.value.ToString();
        myhealthnum.GetComponent<TextMeshProUGUI>().color = gradient.Evaluate(1f);
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("OPP_MaxHealth", RpcTarget.Others, 30, uid1);
        timer.GetComponent<TextMeshProUGUI>().color = Color.green;
        timer_counter();
    }

    void timer_counter()
    {
        if (timer_sec > 0 && local_sav.am_kicked == 0)
        {
            TimeSpan counter = TimeSpan.FromSeconds(timer_sec);
            if (counter.Minutes < 1)
            {
                timer.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            timer.GetComponent<TextMeshProUGUI>().text = counter.Minutes.ToString("00") + ":" + counter.Seconds.ToString("00");
            timer_sec--;
            Invoke("timer_counter", 1.0f);
        }
        else if (timer_sec == 0 && local_sav.am_kicked == 0)
        {
            local_sav.kickme();
            if (Myhealthslider.value > Opphealthslider.value)
            {              
                victory();
            }
            else
            {
                defeated();
            }
        }
    }

    [PunRPC]
    public void OPP_MaxHealth(int max_health, string uid2)
    {
        oppuid.GetComponent<TextMeshProUGUI>().text = uid2;
        Opphealthslider.maxValue = max_health;
        Opphealthslider.value = max_health;
        OPPfill.color = gradient.Evaluate(1f);
        opphealthnum.GetComponent<TextMeshProUGUI>().text = Opphealthslider.value.ToString();
        opphealthnum.GetComponent<TextMeshProUGUI>().color = gradient.Evaluate(1f);
    }

    public void attackzz(int val)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdateHealth", RpcTarget.Others, -val);
    }

    public void healzz(int val)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdateHealth", RpcTarget.Others, val);
        Myhealthslider.value = Myhealthslider.value + val;
        MYfill.color = gradient.Evaluate(Myhealthslider.normalizedValue);
        myhealthnum.GetComponent<TextMeshProUGUI>().text = Myhealthslider.value.ToString();
        myhealthnum.GetComponent<TextMeshProUGUI>().color = gradient.Evaluate(Myhealthslider.normalizedValue);
    }

    public void UpdateCoin(int coins)
    {
        coin = coin + coins;
    }

    [PunRPC]
    public void UpdateHealth(int health_change)
    {
        if(health_change < 0) //attack
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("UpdateOppHealth", RpcTarget.Others, health_change);
            Myhealthslider.value = Myhealthslider.value + health_change;
            MYfill.color = gradient.Evaluate(Myhealthslider.normalizedValue);           
            myhealthnum.GetComponent<TextMeshProUGUI>().text = Myhealthslider.value.ToString();
            myhealthnum.GetComponent<TextMeshProUGUI>().color = gradient.Evaluate(Myhealthslider.normalizedValue);
        }
        else if(health_change > 0) //heal
        {
            Opphealthslider.value = Opphealthslider.value + health_change;
            OPPfill.color = gradient.Evaluate(Opphealthslider.normalizedValue);
            opphealthnum.GetComponent<TextMeshProUGUI>().text = Opphealthslider.value.ToString();
            opphealthnum.GetComponent<TextMeshProUGUI>().color = gradient.Evaluate(Opphealthslider.normalizedValue);
        }
        
    }

    [PunRPC]
    public void UpdateOppHealth(int health_change)
    {
        Opphealthslider.value = Opphealthslider.value + health_change;
        OPPfill.color = gradient.Evaluate(Opphealthslider.normalizedValue);
        opphealthnum.GetComponent<TextMeshProUGUI>().text = Opphealthslider.value.ToString();
        opphealthnum.GetComponent<TextMeshProUGUI>().color = gradient.Evaluate(Opphealthslider.normalizedValue);
    }
   
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        victory(); 
    }


    [PunRPC]
    public void victory()
    {
        play_screen.SetActive(false);
        gameover_screen.SetActive(true);
        coin = coin + (coin / 2);
        update_winloss("win", coin, local_sav.uid_sav);
        win_loss.GetComponent<TextMeshProUGUI>().text = "Victory";
        local_sav.Update_win(coin);
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    private void defeated()
    {
        play_screen.SetActive(false);
        gameover_screen.SetActive(true);
        coin = coin - (coin / 2);
        update_winloss("loss", coin, local_sav.uid_sav);
        win_loss.GetComponent<TextMeshProUGUI>().text = "Defeated";
        local_sav.Update_loss(coin);
        PhotonNetwork.LeaveRoom(); 
    }

    void update_winloss(string win_loss, int coin, string uid)
    {
        WWWForm form = new WWWForm();
        form.AddField("win_loss", win_loss);
        form.AddField("coin_count", coin);
        form.AddField("uid", uid);
        UnityWebRequest link = UnityWebRequest.Post("https://crystalpunch-server.herokuapp.com/winloss.php", form);
        link.SendWebRequest();
    }

    void Update()
    {
        if (Myhealthslider.value == 0 && local_sav.am_kicked == 0)
        {
            local_sav.kickme();
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("victory", RpcTarget.Others);
            defeated();
        }
        else if (local_sav.am_kicked == 0 && !(PhotonNetwork.InRoom))
        {
            local_sav.kickme();
            defeated();              
        }
    }
}
