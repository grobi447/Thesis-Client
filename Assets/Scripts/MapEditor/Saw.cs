using UnityEngine;

public class Saw : Trap
{

    void Awake()
    {
        this.trapType = TrapType.Saw;
    }
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + 6f);
        border.transform.rotation = Quaternion.Euler(0, 0, 0);

    }
}