[System.Serializable]
public class serializer
{
    public string uid_ser, pid_ser;

    public serializer (login_handler logindata)
    {
        uid_ser = logindata.uid;
        pid_ser = logindata.pass;
    }
}
