using System.Collections.Generic;

public class UserManager
{
    private static UserManager instance;

    public static UserManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UserManager();
            }
            return instance;
        }
    }

    public string username;
}
