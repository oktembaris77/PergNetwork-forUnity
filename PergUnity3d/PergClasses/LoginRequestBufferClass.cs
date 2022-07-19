using System;
using System.Collections.Generic;
using System.Text;

public class LoginRequestBufferClass
{
    public LoginResult loginResult;
    public int clientId = -1;

    public LoginRequestBufferClass(LoginResult loginResult, int clientId)
    {
        this.loginResult = loginResult;
        this.clientId = clientId;
    }
}