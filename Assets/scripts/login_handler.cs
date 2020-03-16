using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class login_handler : MonoBehaviour
{
    public GameObject loadingscreen, username, password, msg;
    public Slider slider;
    public string uid, pass;
    public string[] res_arr;
    private string mode = "login";

    void Start()
    {
        serializer data = savesystem.LoadCred();
        if (data != null)
        {
            uid = data.uid_ser;
            pass = data.pid_ser;

            if (uid != "" && pass != "")
            {
                WWWForm form = new WWWForm();
                form.AddField("uid", uid);
                form.AddField("pass", pass);
                form.AddField("mode", mode);
                UnityWebRequest link = UnityWebRequest.Post("https://crystalpunch-server.herokuapp.com/signuphandler.php", form);
                StartCoroutine(login_routine(link));
            }
        }
    }

    public void login()
    {
        uid = username.GetComponent<InputField>().text.ToLower();
        pass = password.GetComponent<InputField>().text.ToLower();

        if (uid == "" || pass == "")
            message("Please fill all the fields");
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("uid", uid);
            form.AddField("pass", pass);
            form.AddField("mode",mode);
            UnityWebRequest link = UnityWebRequest.Post("https://crystalpunch-server.herokuapp.com/signuphandler.php", form);
            StartCoroutine(login_routine(link));
        }
    }

    IEnumerator login_routine(UnityWebRequest connect)
    {
        loadingscreen.SetActive(true);

        AsyncOperation oper = connect.SendWebRequest();

        while (!oper.isDone)
        {
            float progress = Mathf.Clamp01(oper.progress / .5f);
            slider.value = progress;
            yield return null;
        }
        if (connect.isNetworkError || connect.isHttpError)
        {
            loadingscreen.SetActive(false);
            message(connect.error);
        }
        else
        {
            string response = connect.downloadHandler.text;
            res_arr = response.Split('|');
            if (res_arr[0] == "login_success")
            {
                savesystem.SaveCred(this);
                local_sav.init_local(uid, res_arr[1], res_arr[2], res_arr[3]);
                SceneManager.LoadScene("Opener");
            }
            else
            {
                loadingscreen.SetActive(false);
                message(response);
            }
        }
    }

    public void message(string msgout)
    {
        msg.GetComponent<Text>().text = msgout;
    }

}
