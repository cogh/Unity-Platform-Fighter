using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class User
{
    public string username;
    public int score;
}

[Serializable]
public class UserListWrapper
{
    public User[] list;
}



public class WebManagerScript : MonoBehaviour
{
    void Start()
    {
        // Get date
        StartCoroutine(GetRequest("http://localhost/UnityWebGame/GetDate.php"));

        // Get users
        StartCoroutine(GetUsers());
    }

    public void StartUploadScoreCoroutine()
    {
        // Get name
        GameObject nameTextObject = GameObject.Find("NameText");
        string nameText = nameTextObject.GetComponent<Text>().text;

        // Get score
        GameObject gameManager = GameObject.Find("GameManager");
        string score = gameManager.GetComponent<GameManagerScript>().totalScore.ToString();

        // Start coroutine
        StartCoroutine(UploadScore("http://localhost/UnityWebGame/UploadScore.php", nameText, score));
    }

    private void Update()
    {
        // Receive response
        if (receivedResponse && textTarget != null)
        {
            // Fix response text
            response = response.Insert(0, "{\"list\": ");
            response = response.Insert(response.Length, "}");

            // Get user list from response
            UserListWrapper userList = JsonUtility.FromJson<UserListWrapper>(response);

            // Iterate through users
            foreach (User user in userList.list)
            {
                // Add text
                textTarget.text += user.username + ": " + user.score + "\n\n";
            }

            // Set received to false
            receivedResponse = false;
            response = "";
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page. 
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + response);
                    break;

                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
            }
        }
    }

    IEnumerator GetUsers()
    {
        string uri = "http://localhost/UnityWebGame/GetUsers.php";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page. 
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                response = webRequest.downloadHandler.text;
                receivedResponse = true;
            }
        }
    }

    IEnumerator UploadScore(string uri, string username, string score)
    {
        WWWForm form = new WWWForm();
        form.AddField("postUsername", username);
        form.AddField("postScore", score);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form))
        {
            // Request and wait for the desired page. 
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    // Variables
    public bool receivedResponse;
    public string response;
    public Text textTarget;
}
