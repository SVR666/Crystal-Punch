using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class signup_handler : MonoBehaviour
{
    public GameObject loadingscreen, username, password, passwordconfirm, msg;
    public Slider slider;
    private string uid, pass1, pass2, mode = "reg";

    public void signup()
    {
        uid = username.GetComponent<InputField>().text.ToLower();
        pass1 = password.GetComponent<InputField>().text.ToLower();
        pass2 = passwordconfirm.GetComponent<InputField>().text.ToLower();

        if (uid == "" || pass1 == "" || pass2 == "")
            message("Please fill all the fields");
        else
        {
            if (pass1 == pass2)
            {
                WWWForm form = new WWWForm();
                form.AddField("uid", uid);
                form.AddField("pass", pass1);
                form.AddField("mode",mode);
                UnityWebRequest link = UnityWebRequest.Post("https://crystalpunch-server.herokuapp.com/signuphandler.php", form);
                StartCoroutine(signup_routine(link));
            }
            else
                message("Password not matching");
        }
    }

    IEnumerator signup_routine(UnityWebRequest connect)
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
            if (connect.downloadHandler.text == "signup_success")
            {
                SceneManager.LoadScene("login");
            }
            else
            {
                loadingscreen.SetActive(false);
                message(connect.downloadHandler.text); 
            }
        }   
    }

    public void message(string msgout)
    {
        msg.GetComponent<Text>().text = msgout;
    }
}
