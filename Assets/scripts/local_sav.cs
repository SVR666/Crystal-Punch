using UnityEngine;

public class local_sav : MonoBehaviour
{
    public static int win_sav, loss_sav, coin_sav, am_kicked;
    public static string uid_sav;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public static void init_local(string uid, string win, string loss, string coins)
    {
        uid_sav = uid;
        win_sav = int.Parse(win);
        loss_sav = int.Parse(loss);
        coin_sav = int.Parse(coins);
    }

    public static void kickme()
    {
        am_kicked = 1;
    }

    public static void kick_reset()
    {
        am_kicked = 0;
    }

    public static void Update_win(int coin)
    {
        win_sav = win_sav + 1;
        coin_sav = coin_sav + coin;
    }

    public static void Update_loss(int coin)
    {
        loss_sav = loss_sav + 1;
        coin_sav = coin_sav + coin;
    }
}
