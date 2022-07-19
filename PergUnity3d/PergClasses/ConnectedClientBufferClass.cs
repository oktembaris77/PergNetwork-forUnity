using System;
using System.Collections.Generic;
using System.Text;

public class ConnectedClientBufferClass
{
    public int accountId;
    public int clientId;
    public string uniqueKey;
    public ConnectedClientBufferClass(int accountId, int clientId, string uniqueKey)
    {
        this.accountId = accountId;
        this.clientId = clientId;
        this.uniqueKey = uniqueKey;
     }
}