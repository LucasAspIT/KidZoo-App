using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User : MonoBehaviour
{
    private string userId;
    private string name;
    private bool isAdmin;
    private string address;
    private string phone;
    private DateTime birthDay;
    private string email;
    private DateTime creationDate;

    public void VerifyLogin()
    {
        //Connect to FB and check login
    }
    public void ViewAnimal()
    {
        //View locally saved Animals
    }
}
