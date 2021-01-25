using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveMultiplayer : MonoBehaviour
{
    private int currentMove = 0;
    private bool isClockwise = false;
    private bool LocalRotation = false;
    private bool LocalPlayer = false;
    public float rayLength;
    public LayerMask layermask;
    Material originalMaterial, tempMaterial;
    Renderer rend;
    private Color highlightColor;
    public GameObject X;
    public GameObject O;
    public GameObject rotationSprites;
    private int[] movesX;
    public Text movesXText;
    private int[] movesY;
    public Text movesYText;
    private int movesMadeX;
    private int movesMadeY;
    private bool player1Turn;
    private int counter = 0;
    private int rotateCounter = 0;
    private int cubeUsed = 105;
    Renderer[] outlineRends;
    string[] cubeface = new string[9];
    private bool otherRotate = false;
    string[] childHelper;
    private GameObject rotationSprite;
    private bool startDisappearing = false;
    private bool wonGame = false;
    private bool didXWin = false;
    private bool draw = false;
    public Text winMessage;
    public PlayerObject Player1;
    public PlayerObject Player2;
    public Text winningMovesText;
    public Text whosTurnText;
    private string winCondition = "";
    private List<int> winningMoves;
    private int[] drawingMoves;
    public bool player1;
    public AudioSource MoveSource;
    public AudioClip XSound;
    public AudioClip YSound;
    public AudioSource BackGroundSource;
    public AudioClip BackGroundMusic;
    public Image BackGroundSprite;
    public Button MainMenuButton;
    private int rotationSide;
    public int[] lastMoves;
    public List<int> lastMovesTemp;
    private Renderer lastMoveRend;
    private string[] semiPermCubeFace;
    private GameObject network;
    private void Awake()
    {
        GameObject backgroundAndMusic = GameObject.Find("Background + music");
        if (backgroundAndMusic != null)
        {
            BackGroundSource.clip = backgroundAndMusic.GetComponent<ScriptHolder>().backgroundMusic;
            BackGroundSprite.sprite = backgroundAndMusic.GetComponent<ScriptHolder>().backgroundPicture;
            BackGroundSource.Play();
        }
    }

    void Start()
    {
        semiPermCubeFace = new string[9];
        lastMoves = new int[3];
        lastMovesTemp = new List<int>();
        winningMoves = new List<int>();
        whosTurnText.text = "X's Turn";
        rotationSide = 0;
        player1Turn = true;
        highlightColor = Color.green;
        movesX = new int[54];
        movesMadeX = 0;
        movesY = new int[54];
        movesMadeY = 0;
        drawingMoves = new int[54];
        outlineRends = new Renderer[3];
        outlineRends[0] = GameObject.Find("Alpha Outline").GetComponent<Renderer>();
        outlineRends[1] = GameObject.Find("Beta Outline").GetComponent<Renderer>();
        outlineRends[2] = GameObject.Find("Gamma Outline").GetComponent<Renderer>();
        lastMoveRend = new Renderer();
        childHelper = new string[9];
        
        if (NetworkServer.connections.Count == 1)
        {
            player1 = true;
        }
        else 
        {
            player1 = false;
        }
    }


    private void Update()
    {
        if (Player1 == null || Player2 == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 2)
            {
                if (players[0].GetComponent<NetworkIdentity>().netId.Value < players[1].GetComponent<NetworkIdentity>().netId.Value)
                {
                    Player1 = players[0].GetComponent<PlayerObject>();
                    Player2 = players[1].GetComponent<PlayerObject>();
                    
                }
                else
                {
                    Player1 = players[1].GetComponent<PlayerObject>();
                    Player2 = players[0].GetComponent<PlayerObject>();
                }
                Player1.SetIsX(true);
                Player2.SetIsX(false);
                if (Player1.isLocalPlayer)
                {
                    player1 = true;
                }
            }
        }
        if (!wonGame && !draw)
        {
            if (player1Turn)
            {
                whosTurnText.text = "X's Turn";
            }
            else
            {
                whosTurnText.text = "O's Turn";
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Renderer currRend;


            if (Physics.Raycast(ray, out hit, 20) && player1 == player1Turn)
            {
                if (rotateCounter == 12)
                {
                    if (hit.collider.gameObject.tag != "Clockwise" && hit.collider.gameObject.tag != "Counter Clockwise" && !rotationSprite)
                    {
                        string cubefacetemp = cubeface[0];
                        for (int i = 0; i < 9; i++)
                        {
                            cubeface[i] = hit.collider.gameObject.name[0].ToString() + hit.collider.gameObject.name[1].ToString();
                            cubeface[i] += (i + 1).ToString();
                            if (cubefacetemp != cubeface[0] && cubefacetemp != null && i == 0)
                            {
                                FaceHighlightHelper();

                                rend = GameObject.Find(cubefacetemp).GetComponent<Renderer>();

                                rend.material.SetColor("_Color", Color.white);
                            }
                        }

                        for (int i = 0; i < 9; i++)
                        {
                            currRend = GameObject.Find(cubeface[i]).GetComponent<Renderer>();

                            currRend.material.SetColor("_Color", highlightColor);
                        }
                    }
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && rotateCounter == 12 &&
                        (hit.collider.gameObject.tag == "Front" || hit.collider.gameObject.tag == "Left" || hit.collider.gameObject.tag == "Right" ||
                         hit.collider.gameObject.tag == "Hind" || hit.collider.gameObject.tag == "Top" || hit.collider.gameObject.tag == "Bottom")
                       )
                    {
                        if (!rotationSprite)
                        {
                            rotationSprite = Instantiate(rotationSprites, Vector3.zero, Quaternion.identity) as GameObject;
                            Int32.TryParse(hit.collider.gameObject.name, out rotationSide);
                            rotationSide /= 10;
                        }
                        else
                        {
                            Destroy(rotationSprite);
                        }
                    }
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && rotateCounter == 12 &&
                        (hit.collider.gameObject.tag == "Clockwise" || hit.collider.gameObject.tag == "Counter Clockwise"))
                    {
                        //RotateEverything(hit);
                        LocalRotation = true;
                        if (hit.collider.gameObject.tag == "Clockwise")
                        {
                            isClockwise = true;
                        }
                        else
                        {
                            isClockwise = false;
                        }
                        //SwitchPlayerTurnHighlights();
                        //Destroy(rotationSprite);
                        //for (int i = 0; i < 9; i++)
                        //{
                        //    rend = GameObject.Find(cubeface[i]).GetComponent<Renderer>();

                        //    rend.material.SetColor("_Color", Color.white);
                        //}
                        //wonGame = CheckForWin(movesX, movesMadeX);
                        //if (wonGame)
                        //{
                        //    if (wonGame)
                        //    {
                        //        didXWin = true;
                        //    }
                        //    if (CheckForWin(movesY, movesMadeY))
                        //    {
                        //        draw = true;
                        //        didXWin = false;
                        //    }
                        //}
                        //else
                        //{
                        //    wonGame = CheckForWin(movesY, movesMadeY);
                        //    didXWin = false;
                        //}
                        //rotationSide = 0;

                    }



                }
                else if (hit.transform.childCount == 0)
                {
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        //add mouse offset here maybe and then make it work on mousebuttonup if it hasn't moved (or much)
                        if (hit.collider.gameObject.transform.parent.name == "Alpha")
                        {
                            if (cubeUsed % 3 == 0)
                            {
                                outlineRends[0].enabled = false;
                                CreateMove(hit);
                                cubeUsed /= 3;

                            }
                        }
                        else if (hit.collider.gameObject.transform.parent.name == "Beta")
                        {
                            if (cubeUsed % 5 == 0)
                            {
                                outlineRends[1].enabled = false;
                                CreateMove(hit);
                                cubeUsed /= 5;

                            }
                        }
                        else if (hit.collider.gameObject.transform.parent.name == "Gamma")
                        {
                            if (cubeUsed % 7 == 0)
                            {
                                outlineRends[2].enabled = false;
                                CreateMove(hit);
                                cubeUsed /= 7;

                            }
                        }
                        if (cubeUsed == 1)
                        {
                            SwitchTurns();
                        }
                    }
                    if (rotateCounter != 12)
                    {
                        currRend = hit.collider.gameObject.GetComponent<Renderer>();
                        if (currRend == rend)
                            return;
                        if (currRend && currRend != rend)
                        {
                            if (rend)
                            {
                                rend.sharedMaterial = originalMaterial;
                            }
                        }

                        if (currRend)
                            rend = currRend;
                        else
                            return;

                        originalMaterial = rend.sharedMaterial;

                        tempMaterial = new Material(originalMaterial);
                        rend.material = tempMaterial;
                        rend.material.color = highlightColor;
                    }
                }
            }
            else
            {
                if (rotateCounter != 12)
                {
                    if (rend)
                    {
                        rend.sharedMaterial = originalMaterial;
                        rend = null;
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && rotateCounter == 12)
                    {
                        if (rotationSprite)
                        {
                            Destroy(rotationSprite);
                            for (int i = 0; i < 9; i++)
                            {
                                rend = GameObject.Find(cubeface[i]).GetComponent<Renderer>();

                                rend.material.SetColor("_Color", Color.white);
                            }
                        }
                    }
                    FaceHighlightHelper();
                }
            }
            HighlightLastMoves();
        }
        else
        {
            HighlightWinningMoves();
            MainMenuButton.gameObject.SetActive(true);
            if (didXWin && !draw)
            {
                winMessage.text = "X WON THE GAME! \nBY ";
                winMessage.text += winCondition;
                winningMovesText.text = "";
                foreach (int move in winningMoves)
                {
                    if (move != 0)
                    {
                        if (move / 100 == 1)
                        {
                            winningMovesText.text += "A-";
                        }
                        else if (move / 100 == 2)
                        {
                            winningMovesText.text += "B-";
                        }
                        else if (move / 100 == 3)
                        {
                            winningMovesText.text += "G-";
                        }
                        if (move % 100 - move % 10 == 10)
                        {
                            winningMovesText.text += "F-";
                        }
                        else if (move % 100 - move % 10 == 20)
                        {
                            winningMovesText.text += "L-";
                        }
                        else if (move % 100 - move % 10 == 30)
                        {
                            winningMovesText.text += "T-";
                        }
                        else if (move % 100 - move % 10 == 40)
                        {
                            winningMovesText.text += "H-";
                        }
                        else if (move % 100 - move % 10 == 50)
                        {
                            winningMovesText.text += "R-";
                        }
                        else if (move % 100 - move % 10 == 60)
                        {
                            winningMovesText.text += "B-";
                        }
                        winningMovesText.text += move % 10;
                        winningMovesText.text += " ";
                    }
                }
            }
            else if (!didXWin && !draw)
            {
                winMessage.text = "O WON THE GAME! \nBY ";
                winMessage.text += winCondition;
                winningMovesText.text = "";
                foreach (int move in winningMoves)
                {
                    if (move != 0)
                    {
                        if (move / 100 == 1)
                        {
                            winningMovesText.text += "A-";
                        }
                        else if (move / 100 == 2)
                        {
                            winningMovesText.text += "B-";
                        }
                        else if (move / 100 == 3)
                        {
                            winningMovesText.text += "G-";
                        }
                        if (move % 100 - move % 10 == 10)
                        {
                            winningMovesText.text += "F-";
                        }
                        else if (move % 100 - move % 10 == 20)
                        {
                            winningMovesText.text += "L-";
                        }
                        else if (move % 100 - move % 10 == 30)
                        {
                            winningMovesText.text += "T-";
                        }
                        else if (move % 100 - move % 10 == 40)
                        {
                            winningMovesText.text += "H-";
                        }
                        else if (move % 100 - move % 10 == 50)
                        {
                            winningMovesText.text += "R-";
                        }
                        else if (move % 100 - move % 10 == 60)
                        {
                            winningMovesText.text += "B-";
                        }
                        winningMovesText.text += move % 10;
                        winningMovesText.text += " ";
                    }
                }
            }
            else
            {

                winMessage.text = "TIE GAME!";

                winningMovesText.text = "";
                foreach (int move in winningMoves)
                {
                    if (winningMoves.Count > 9)
                    {
                        if (move == winningMoves[9])
                        {
                            winningMovesText.text += "\n";
                        }
                    }
                    if (move != 0)
                    {
                        if (move / 100 == 1)
                        {
                            winningMovesText.text += "A-";
                        }
                        else if (move / 100 == 2)
                        {
                            winningMovesText.text += "B-";
                        }
                        else if (move / 100 == 3)
                        {
                            winningMovesText.text += "G-";
                        }
                        if (move % 100 - move % 10 == 10)
                        {
                            winningMovesText.text += "F-";
                        }
                        else if (move % 100 - move % 10 == 20)
                        {
                            winningMovesText.text += "L-";
                        }
                        else if (move % 100 - move % 10 == 30)
                        {
                            winningMovesText.text += "T-";
                        }
                        else if (move % 100 - move % 10 == 40)
                        {
                            winningMovesText.text += "H-";
                        }
                        else if (move % 100 - move % 10 == 50)
                        {
                            winningMovesText.text += "R-";
                        }
                        else if (move % 100 - move % 10 == 60)
                        {
                            winningMovesText.text += "B-";
                        }
                        winningMovesText.text += move % 10;
                        winningMovesText.text += " ";
                    }
                }
            }
        }
    }

    public void SwitchTurns()
    {
        cubeUsed = 105;
        rotateCounter++;
        if (player1Turn && startDisappearing)
        {
            foreach (Transform child in GameObject.Find(movesX[movesMadeX].ToString()).transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameObject.Find(movesX[movesMadeX + 1].ToString()).transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameObject.Find(movesX[movesMadeX + 2].ToString()).transform)
            {
                Destroy(child.gameObject);
            }

            movesX[movesMadeX] = 0;
            movesX[movesMadeX + 1] = 0;
            movesX[movesMadeX + 2] = 0;


        }
        else if (!player1Turn && startDisappearing)
        {
            foreach (Transform child in GameObject.Find(movesY[movesMadeY].ToString()).transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameObject.Find(movesY[movesMadeY + 1].ToString()).transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameObject.Find(movesY[movesMadeY + 2].ToString()).transform)
            {
                Destroy(child.gameObject);
            }

            movesY[movesMadeY] = 0;
            movesY[movesMadeY + 1] = 0;
            movesY[movesMadeY + 2] = 0;


        }
        if (rotateCounter == 12)
        {
            otherRotate = true;
        }
        SwitchPlayerTurnHighlights();
    }

    public void CreateOnlineMove(bool isX)
    {
        GameObject obj;
        
        if (isX)
        {
            obj = Instantiate(X, Vector3.zero, Quaternion.identity) as GameObject;
        }
        else
        {
            obj = Instantiate(O, Vector3.zero, Quaternion.identity) as GameObject;
        }
        if (isX)
        {
            obj.transform.parent = GameObject.Find(movesX[movesMadeX-1].ToString()).transform;
            RotateMoveToFitCube(GameObject.Find(movesX[movesMadeX-1].ToString()).tag, obj);
        }
        else
        {
            obj.transform.parent = GameObject.Find(movesY[movesMadeY-1].ToString()).transform;
            RotateMoveToFitCube(GameObject.Find(movesY[movesMadeY-1].ToString()).tag, obj);
        }
        AddOnlineMoveToMoveArray(obj, isX);

    }

    private void AddOnlineMoveToMoveArray(GameObject obj, bool isX)
    {
        int sum;
        Int32.TryParse(obj.transform.parent.name, out sum);

        //can add sum to list or array and make it orange
        if (sum != 0)
        {
            lastMovesTemp.Add(sum);
            switch (sum / 100)
            {
                case 1:
                    outlineRends[0].enabled = false;
                    cubeUsed /= 3;
                    break;
                case 2:
                    outlineRends[1].enabled = false;
                    cubeUsed /= 5;
                    break;
                case 3:
                    outlineRends[2].enabled = false;
                    cubeUsed /= 7;
                    break;
            }
        }
        if (cubeUsed == 1)
        {
            SwitchTurns();
        }

        if (isX && sum != 0)
        {
            MoveSource.clip = XSound;
            MoveSource.Play();
            if (movesMadeX - 1 != -1)
            {
                wonGame = CheckForWin(movesX, movesMadeX - 1);
            }
            if (wonGame)
            {
                Debug.Log("you win!");
                didXWin = true;
            }
            if (movesMadeX == 54)
            {
                movesMadeX = 0;
                draw = CheckForDraw(movesX);
                for (int i = 0; i < movesX.Length; i++)
                {
                    drawingMoves[i] = movesX[i];
                }
            }
        }
        else if (!isX && sum != 0)
        {
            MoveSource.clip = YSound;
            MoveSource.Play();
            if (movesMadeY - 1 != -1)
            {
                wonGame = CheckForWin(movesY, movesMadeY - 1);
            }
            if (wonGame)
            {
                Debug.Log("you win!");
                didXWin = false;
            }
            if (movesMadeY == 54)
            {
                movesMadeY = 0;
                if (startDisappearing)
                {
                    draw = CheckForDraw(movesY);
                    for (int i = 0; i < movesY.Length; i++)
                    {
                        drawingMoves[i] = movesY[i];
                    }
                }
                startDisappearing = true;
            }
        }
        DisplayMoveList();
    }

    private void FaceHighlightHelper()
    {
        if (cubeface[0] == null)
        {
            return;
        }
        if (rotationSprite)
        {
            return;
        }
        for (int i = 0; i < 9; i++)
        {
            rend = GameObject.Find(cubeface[i]).GetComponent<Renderer>();

            rend.material.SetColor("_Color", Color.white);
        }
    }

    private void CreateMove(RaycastHit hit)
    {
        GameObject obj;
        if (player1Turn)
        {
            obj = Instantiate(X, Vector3.zero, Quaternion.identity) as GameObject;
            counter++;
        }
        else
        {
            obj = Instantiate(O, Vector3.zero, Quaternion.identity) as GameObject;
            counter++;

        }
        obj.transform.parent = hit.transform;

        RotateMoveToFitCube(hit.collider.gameObject.tag, obj);
        AddMoveToMoveArray(obj);

    }

    private void RotateMove(int place, bool x)
    {
        //maybe add an animation and then snap it back instantly to preserve the places
        GameObject obj;
        if (x)
        {
            obj = Instantiate(X, Vector3.zero, Quaternion.identity) as GameObject;
        }
        else
        {
            obj = Instantiate(O, Vector3.zero, Quaternion.identity) as GameObject;
        }
        obj.transform.parent = GameObject.Find(place.ToString()).transform;

        RotateMoveToFitCube(GameObject.Find(place.ToString()).tag, obj);
        //AddMoveToMoveArray(obj);
    }

    private bool CheckForDraw(int[] movesZ)
    {
        bool draw = true;
        for (int i = 0; i < drawingMoves.Length; i += 3)
        {
            if ((drawingMoves[i] == movesZ[i] || drawingMoves[i] == movesZ[i + 1] || drawingMoves[i] == movesZ[i + 2])
                && (drawingMoves[i + 1] == movesZ[i] || drawingMoves[i + 1] == movesZ[i + 1] || drawingMoves[i + 1] == movesZ[i + 2]) &&
                (drawingMoves[i + 2] == movesZ[i] || drawingMoves[i + 2] == movesZ[i + 1] || drawingMoves[i + 2] == movesZ[i + 2]))
            {
                draw = true;
            }
            else
            {
                draw = false;
            }
        }

        return draw;
    }

    public bool LocalPlayerObjectSoUpdate()
    {
        if (LocalPlayer)
        {
            LocalPlayer = false;
            return true;
        }
        return false;
    }

    public int CurrentMove()
    {
        return currentMove;
    }

    private void AddMoveToMoveArray(GameObject obj)
    {
        int sum;
        Int32.TryParse(obj.transform.parent.name, out sum);

        if (sum != 0)
        {
            lastMovesTemp.Add(sum);
        }
        if (player1Turn && sum != 0)
        {
            movesX[movesMadeX] = sum;
            MoveSource.clip = XSound;
            MoveSource.Play();
            wonGame = CheckForWin(movesX, movesMadeX);
            if (wonGame)
            {
                Debug.Log("you win!");
                didXWin = true;
            }
            movesMadeX++;
            currentMove = sum;
            Debug.Log(CurrentMove());
            LocalPlayer = true;
            if (movesMadeX == 54)
            {
                movesMadeX = 0;
                draw = CheckForDraw(movesX);
                for (int i = 0; i < movesX.Length; i++)
                {
                    drawingMoves[i] = movesX[i];
                }
            }
        }
        else if (!player1Turn && sum != 0)
        {
            movesY[movesMadeY] = sum;
            MoveSource.clip = YSound;
            MoveSource.Play();
            wonGame = CheckForWin(movesY, movesMadeY);
            if (wonGame)
            {
                Debug.Log("you win!");
                didXWin = false;
            }
            movesMadeY++;
            currentMove = sum;
            Debug.Log(CurrentMove());
            LocalPlayer = true;
            if (movesMadeY == 54)
            {
                movesMadeY = 0;
                if (startDisappearing)
                {
                    draw = CheckForDraw(movesY);
                    for (int i = 0; i < movesY.Length; i++)
                    {
                        drawingMoves[i] = movesY[i];
                    }
                }
                startDisappearing = true;
            }
        }
        DisplayMoveList();
    }

    public bool LocalPlayerObjectSoRotate()
    {
        if (LocalRotation)
        {
            LocalRotation = false;
            return true;
        }
        return false;
    }

    public string[] GetCubeFace()
    {
        return cubeface;
    }

    public bool GetRotationDir()
    {
        if (isClockwise)
        {
            isClockwise = false;
            return true;
        }
        return false;
    }

    private void RotateMoveToFitCube(string tag, GameObject obj)
    {
        switch (tag)
        {
            case "Left":
                obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x, obj.transform.parent.rotation.y + 90f, obj.transform.parent.rotation.z - 46f);
                obj.transform.localPosition = new Vector3(-0.46f, 0, 0);
                obj.transform.localScale = new Vector3(1f, 0.2f, 0.1f);
                break;
            case "Front":
                obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x, obj.transform.parent.rotation.y, obj.transform.parent.rotation.z - 46f);
                obj.transform.localPosition = new Vector3(0, 0, -0.46f);
                obj.transform.localScale = new Vector3(1f, 0.2f, 0.1f);
                break;
            case "Right":
                obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x, obj.transform.parent.rotation.y + 90f, obj.transform.parent.rotation.z - 46f);
                obj.transform.localPosition = new Vector3(0.46f, 0, 0);
                if (obj.tag == "X")
                    obj.transform.localScale = new Vector3(1f, 0.2f, 0.1f);
                else
                    obj.transform.localScale = new Vector3(1f, 0.2f, -0.1f);
                break;
            case "Hind":
                obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x, obj.transform.parent.rotation.y, obj.transform.parent.rotation.z - 46f);
                obj.transform.localPosition = new Vector3(0, 0, 0.46f);
                if (obj.tag == "X")
                    obj.transform.localScale = new Vector3(1f, 0.2f, 0.1f);
                else
                    obj.transform.localScale = new Vector3(1f, 0.2f, -0.1f);
                break;
            case "Top":
                obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x + 90f, obj.transform.parent.rotation.y, obj.transform.parent.rotation.z - 46f);
                obj.transform.localPosition = new Vector3(0, 0.46f, 0);
                obj.transform.localScale = new Vector3(1f, 0.2f, 0.1f);
                break;
            case "Bottom":
                obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x + 90f, obj.transform.parent.rotation.y, obj.transform.parent.rotation.z - 46f);
                obj.transform.localPosition = new Vector3(0, -0.46f, 0);
                if (obj.tag == "X")
                    obj.transform.localScale = new Vector3(1f, 0.2f, 0.1f);
                else
                    obj.transform.localScale = new Vector3(1f, 0.2f, -0.1f);
                break;
        }
    }

    public void RotateEverythingOnline(bool isClockwise, string[] faceOfCube)
    {
        if (semiPermCubeFace[0] != null)
        {
            for (int i = 0; i < 9; i++)
            {
                lastMoveRend = GameObject.Find(semiPermCubeFace[i]).GetComponent<Renderer>();
                lastMoveRend.material.SetColor("_Color", Color.white);
            }
        }
        for (int i = 0; i < faceOfCube.Length; i++)
        {
            semiPermCubeFace[i] = faceOfCube[i];
        }
        cubeface = faceOfCube;
        if (!otherRotate)
        {
            rotateCounter = 0;
        }
        otherRotate = false;
        FaceHighlightHelper();

        for (int i = 0; i < 9; i++)
        {
            if (i == 0)
            {
                childHelper = new string[9];
                for (int z = 0; z < 9; z++)
                {
                    if (GameObject.Find(cubeface[z]).transform.childCount != 0)
                    {
                        childHelper[z] = cubeface[z];
                        if (GameObject.Find(cubeface[z]).transform.Find("X(Clone)"))
                        {
                            childHelper[z] += "1";
                        }
                        else
                        {
                            childHelper[z] += "2";
                        }
                        foreach (Transform child in GameObject.Find(cubeface[z]).transform)
                        {

                            Destroy(child.gameObject);
                        }
                    }
                }
            }

            if (childHelper[i] != null && childHelper[i] != "")
            {

                GameObject obj;
                if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                {
                    obj = Instantiate(X, Vector3.zero, Quaternion.identity) as GameObject;
                }
                else
                {
                    obj = Instantiate(O, Vector3.zero, Quaternion.identity) as GameObject;

                }

                int partOfFace = cubeface[i][2] - 48;


                if (isClockwise)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        rend = GameObject.Find(cubeface[j]).GetComponent<Renderer>();

                        rend.material.SetColor("_Color", Color.white);
                    }
                    ClockwiseRotation(partOfFace, i, obj);
                }
                else 
                {
                    CounterClockwiseRotation(partOfFace, i, obj);
                    for (int j = 0; j < 9; j++)
                    {
                        rend = GameObject.Find(cubeface[j]).GetComponent<Renderer>();

                        rend.material.SetColor("_Color", Color.white);
                    }
                }
            }
        }
        SwitchPlayerTurnHighlights();
        Destroy(rotationSprite);
        for (int i = 0; i < 9; i++)
        {
            rend = GameObject.Find(cubeface[i]).GetComponent<Renderer>();

            rend.material.SetColor("_Color", Color.white);
        }
        wonGame = CheckForWin(movesX, movesMadeX-1);
        if (wonGame)
        {
            if (wonGame)
            {
                didXWin = true;
            }
            if (CheckForWin(movesY, movesMadeY-1))
            {
                draw = true;
                didXWin = false;
            }
        }
        else
        {
            wonGame = CheckForWin(movesY, movesMadeY-1);
            didXWin = false;
        }
        rotationSide = 0;
    }

    private void HighlightLastMoves()
    {

        if (semiPermCubeFace[0] != null && !otherRotate)
        {
            for (int i = 0; i < 3; i++)
            {
                lastMoveRend = GameObject.Find(lastMoves[i].ToString()).GetComponent<Renderer>();
                lastMoveRend.material.SetColor("_Color", Color.white);
            }
            for (int i = 0; i < 9; i++)
            {
                lastMoveRend = GameObject.Find(semiPermCubeFace[i]).GetComponent<Renderer>();
                lastMoveRend.material.SetColor("_Color", new Color(255, 128, 0));
            }

        }
        else if (lastMovesTemp.Count == 3)
        {
            if (lastMoves[2] != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    lastMoveRend = GameObject.Find(lastMoves[i].ToString()).GetComponent<Renderer>();

                    lastMoveRend.material.SetColor("_Color", Color.white);
                }
            }
            lastMovesTemp.CopyTo(lastMoves);

            lastMovesTemp.Clear();
        }
        else if (lastMoves[2] != 0)
        {
            for (int i = 0; i < 3; i++)
            {
                lastMoveRend = GameObject.Find(lastMoves[i].ToString()).GetComponent<Renderer>();

                lastMoveRend.material.SetColor("_Color", new Color(255, 128, 0));
            }
        }
        if (rotateCounter == 1 && semiPermCubeFace[0] != null)
        {
            for (int i = 0; i < 9; i++)
            {
                lastMoveRend = GameObject.Find(semiPermCubeFace[i]).GetComponent<Renderer>();
                lastMoveRend.material.SetColor("_Color", Color.white);
            }
            semiPermCubeFace[0] = null;

        }
    }

    private void RotateEverything(RaycastHit hit)
    {
        if (!otherRotate)
        {
            rotateCounter = 0;
        }
        otherRotate = false;
        FaceHighlightHelper();

        for (int i = 0; i < 9; i++)
        {
            if (i == 0)
            {
                childHelper = new string[9];
                for (int z = 0; z < 9; z++)
                {
                    if (GameObject.Find(cubeface[z]).transform.childCount != 0)
                    {
                        childHelper[z] = cubeface[z];
                        if (GameObject.Find(cubeface[z]).transform.Find("X(Clone)"))
                        {
                            childHelper[z] += "1";
                        }
                        else
                        {
                            childHelper[z] += "2";
                        }
                        foreach (Transform child in GameObject.Find(cubeface[z]).transform)
                        {

                            Destroy(child.gameObject);
                        }
                    }
                }
            }

            if (childHelper[i] != null && childHelper[i] != "")
            {

                GameObject obj;
                if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                {
                    obj = Instantiate(X, Vector3.zero, Quaternion.identity) as GameObject;
                }
                else
                {
                    obj = Instantiate(O, Vector3.zero, Quaternion.identity) as GameObject;

                }

                int partOfFace = cubeface[i][2] - 48;


                if (hit.collider.gameObject.tag == "Clockwise")
                {
                    for (int j = 0; j < 9; j++)
                    {
                        rend = GameObject.Find(cubeface[j]).GetComponent<Renderer>();

                        rend.material.SetColor("_Color", Color.white);
                    }
                    ClockwiseRotation(partOfFace, i, obj);
                }
                else if (hit.collider.gameObject.tag == "Counter Clockwise")
                {
                    CounterClockwiseRotation(partOfFace, i, obj);
                    for (int j = 0; j < 9; j++)
                    {
                        rend = GameObject.Find(cubeface[j]).GetComponent<Renderer>();

                        rend.material.SetColor("_Color", Color.white);
                    }
                }
            }
        }
    }

    private void CounterClockwiseRotation(int partOfFace, int i, GameObject obj)
    {
        switch (partOfFace)
        {
            case 1:
                partOfFace = 7;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 2:
                partOfFace = 4;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 3:
                partOfFace = 1;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 4:
                partOfFace = 8;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 5:
                partOfFace = 5;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 6:
                partOfFace = 2;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 7:
                partOfFace = 9;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 8:
                partOfFace = 6;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 9:
                partOfFace = 3;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
        }
    }

    private void ClockwiseRotation(int partOfFace, int i, GameObject obj)
    {
        switch (partOfFace)
        {
            case 1:
                partOfFace = 3;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 2:
                partOfFace = 6;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 3:
                partOfFace = 9;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 4:
                partOfFace = 2;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 5:
                partOfFace = 5;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 6:
                partOfFace = 8;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 7:
                partOfFace = 1;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 8:
                partOfFace = 4;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;
            case 9:
                partOfFace = 7;
                for (int j = 0; j < 54; j++)
                {
                    if ((movesX[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "1") || (movesY[j].ToString() == cubeface[i] && childHelper[i][childHelper[i].Length - 1].ToString() == "2"))
                    {
                        string test = cubeface[i].Substring(0, 2) + partOfFace.ToString();

                        obj.transform.parent = GameObject.Find(test).transform;
                        int sum = 0;
                        Int32.TryParse(test, out sum);
                        if (sum != 0)
                        {
                            if (childHelper[i][childHelper[i].Length - 1].ToString() == "1")
                                movesX[j] = sum;
                            else
                                movesY[j] = sum;
                        }
                        break;
                    }
                }
                RotateMoveToFitCube(obj.transform.parent.tag, obj);
                break;

        }
    }

    private void SwitchPlayerTurnHighlights()
    {
        player1Turn = !player1Turn;
        if (player1Turn)
        {
            outlineRends[0].material.SetColor("_OutlineColor", Color.magenta);
            outlineRends[1].material.SetColor("_OutlineColor", Color.magenta);
            outlineRends[2].material.SetColor("_OutlineColor", Color.magenta);
        }
        else
        {
            outlineRends[0].material.SetColor("_OutlineColor", Color.red);
            outlineRends[1].material.SetColor("_OutlineColor", Color.red);
            outlineRends[2].material.SetColor("_OutlineColor", Color.red);
        }
        outlineRends[0].enabled = true;
        outlineRends[1].enabled = true;
        outlineRends[2].enabled = true;
    }

    private void DisplayMoveList()
    {

        int count = 0;
        string movesXTempText = "";
        foreach (int move in movesX)
        {
            if (move != 0)
            {

                count %= 3;
                if (count == 0)
                {
                    movesXTempText = movesXTempText + "\n";
                }
                count++;
                if (move / 100 == 1)
                {
                    movesXTempText += "A-";
                }
                else if (move / 100 == 2)
                {
                    movesXTempText += "B-";
                }
                else if (move / 100 == 3)
                {
                    movesXTempText += "G-";
                }
                if (move % 100 - move % 10 == 10)
                {
                    movesXTempText += "F-";
                }
                else if (move % 100 - move % 10 == 20)
                {
                    movesXTempText += "L-";
                }
                else if (move % 100 - move % 10 == 30)
                {
                    movesXTempText += "T-";
                }
                else if (move % 100 - move % 10 == 40)
                {
                    movesXTempText += "H-";
                }
                else if (move % 100 - move % 10 == 50)
                {
                    movesXTempText += "R-";
                }
                else if (move % 100 - move % 10 == 60)
                {
                    movesXTempText += "B-";
                }
                movesXTempText += move % 10;
                movesXTempText += " ";
            }
        }
        movesXText.text = movesXTempText;

        int count2 = 0;
        string movesYTempText = "";
        foreach (int move in movesY)
        {
            if (move != 0)
            {

                count2 %= 3;
                if (count2 == 0)
                {
                    movesYTempText = movesYTempText + "\n";
                }
                count2++;
                if (move / 100 == 1)
                {
                    movesYTempText += "A-";
                }
                else if (move / 100 == 2)
                {
                    movesYTempText += "B-";
                }
                else if (move / 100 == 3)
                {
                    movesYTempText += "G-";
                }
                if (move % 100 - move % 10 == 10)
                {
                    movesYTempText += "F-";
                }
                else if (move % 100 - move % 10 == 20)
                {
                    movesYTempText += "L-";
                }
                else if (move % 100 - move % 10 == 30)
                {
                    movesYTempText += "T-";
                }
                else if (move % 100 - move % 10 == 40)
                {
                    movesYTempText += "H-";
                }
                else if (move % 100 - move % 10 == 50)
                {
                    movesYTempText += "R-";
                }
                else if (move % 100 - move % 10 == 60)
                {
                    movesYTempText += "B-";
                }
                movesYTempText += move % 10;
                movesYTempText += " ";
            }
        }
        movesYText.text = movesYTempText;

    }

    public void LoadMainMenu()
    {
        network = GameObject.Find("NetworkManager");
        network.transform.SetParent(Camera.main.transform);
        network.GetComponent<LobbyManager>().StopHost();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private bool CheckForWin(int[] movesList, int movesIndex)
    {
        int[] movesZ = movesList;
        List<int>[] pinPointHelp = new List<int>[9];
        for (int i = 0; i < 9; i++)
        {
            pinPointHelp[i] = new List<int>();
        }
        List<int> cornersHelp = new List<int>();
        List<int> lineHelp = new List<int>();
        List<int> diagonalHelp = new List<int>();
        List<int> triangleHelp = new List<int>();
        List<int> warpHelp = new List<int>();
        List<int>[] chaosHelp = new List<int>[9];
        for (int i = 0; i < 9; i++)
        {
            chaosHelp[i] = new List<int>();
        }



        foreach (int move in movesZ)
        {
            if (move % 10 != 0)
            {
                pinPointHelp[move % 10 - 1].Add(move);
                for (int i = 0; i < pinPointHelp.Length; i++)
                {
                    List<int> pinPointHelper2 = new List<int>();
                    if (pinPointHelp[i].Count == 9)
                    {
                        for (int j = 0; j < pinPointHelp[i].Count; j++)
                        {
                            int faceNumCounter = 0;
                            for (int k = 0; k < pinPointHelp[i].Count; k++)
                            {
                                if (pinPointHelp[i][j] / 100 % 10 == pinPointHelp[i][k] / 100 % 10)
                                {
                                    faceNumCounter++;
                                }
                                if (faceNumCounter == 3)
                                {
                                    pinPointHelper2.Add(pinPointHelp[i][j]);
                                    break;
                                }
                            }
                        }
                        if (pinPointHelper2.Count >= 9)
                        {
                            winCondition = "PINPOINT";
                            for (int j = 0; j < pinPointHelper2.Count; j++)
                            {
                                winningMoves.Add(pinPointHelper2[j]);
                            }
                            return true;
                        }
                    }
                }
            }
            if (move % 100 != 0)
            {
                int count = 0;
                for (int i = 0; i < movesZ.Length; i++)
                {
                    if (movesZ[i] % 100 == move % 100)
                    {
                        count++;
                    }
                }
                if (count == 3)
                {
                    if (move % 10 == 1 || move % 10 == 3 || move % 10 == 7 || move % 10 == 9)
                    {
                        cornersHelp.Add(move % 100 + 100);
                        cornersHelp.Add(move % 100 + 200);
                        cornersHelp.Add(move % 100 + 300);
                    }

                    if ((cornersHelp.Contains(111) && cornersHelp.Contains(137) && cornersHelp.Contains(123)))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(111 + 100 * i);
                            winningMoves.Add(137 + 100 * i);
                            winningMoves.Add(123 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }

                    else if (cornersHelp.Contains(113) && cornersHelp.Contains(139) && cornersHelp.Contains(151))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(113 + 100 * i);
                            winningMoves.Add(139 + 100 * i);
                            winningMoves.Add(151 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }
                    else if (cornersHelp.Contains(117) && cornersHelp.Contains(129) && cornersHelp.Contains(161))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(117 + 100 * i);
                            winningMoves.Add(129 + 100 * i);
                            winningMoves.Add(161 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }
                    else if (cornersHelp.Contains(119) && cornersHelp.Contains(157) && cornersHelp.Contains(163))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(119 + 100 * i);
                            winningMoves.Add(157 + 100 * i);
                            winningMoves.Add(163 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }
                    else if (cornersHelp.Contains(141) && cornersHelp.Contains(133) && cornersHelp.Contains(153))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(141 + 100 * i);
                            winningMoves.Add(133 + 100 * i);
                            winningMoves.Add(153 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }
                    else if (cornersHelp.Contains(143) && cornersHelp.Contains(131) && cornersHelp.Contains(121))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(143 + 100 * i);
                            winningMoves.Add(131 + 100 * i);
                            winningMoves.Add(121 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }
                    else if (cornersHelp.Contains(147) && cornersHelp.Contains(159) && cornersHelp.Contains(169))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(147 + 100 * i);
                            winningMoves.Add(159 + 100 * i);
                            winningMoves.Add(169 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }
                    else if (cornersHelp.Contains(149) && cornersHelp.Contains(127) && cornersHelp.Contains(167))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            winningMoves.Add(149 + 100 * i);
                            winningMoves.Add(127 + 100 * i);
                            winningMoves.Add(167 + 100 * i);
                        }
                        winCondition = "CORNER";
                        return true;
                    }

                    lineHelp.Add(move % 100 + 100);
                    lineHelp.Add(move % 100 + 200);
                    lineHelp.Add(move % 100 + 300);

                    for (int i = 0; i < lineHelp.Count; i++)
                    {
                        if (lineHelp[i] % 10 + 2 < 10 && (lineHelp[i] % 10) % 3 == 1)
                        {
                            if (lineHelp.Contains(lineHelp[i] + 1) && lineHelp.Contains(lineHelp[i] + 2))
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    winningMoves.Add(lineHelp[i] + 100 * j);
                                    winningMoves.Add(lineHelp[i] + 1 + 100 * j);
                                    winningMoves.Add(lineHelp[i] + 2 + 100 * j);
                                }
                                winCondition = "LINE";
                                return true;
                            }
                        }
                        if (lineHelp[i] % 10 + 6 < 10)
                        {
                            if (lineHelp.Contains(lineHelp[i] + 3) && lineHelp.Contains(lineHelp[i] + 6))
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    winningMoves.Add(lineHelp[i] + 100 * j);
                                    winningMoves.Add(lineHelp[i] + 3 + 100 * j);
                                    winningMoves.Add(lineHelp[i] + 6 + 100 * j);
                                }
                                winCondition = "LINE";
                                return true;
                            }
                        }
                    }

                    if (move % 10 == 1 || move % 10 == 3 || move % 10 == 7 || move % 10 == 9 || move % 10 == 5)
                    {
                        diagonalHelp.Add(move % 100 + 100);
                        diagonalHelp.Add(move % 100 + 200);
                        diagonalHelp.Add(move % 100 + 300);
                    }

                    for (int i = 0; i < diagonalHelp.Count; i++)
                    {
                        if (diagonalHelp[i] % 10 == 5)
                        {
                            if (diagonalHelp.Contains(diagonalHelp[i] + 2) && diagonalHelp.Contains(diagonalHelp[i] - 2))
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    winningMoves.Add(diagonalHelp[i] + 100 * j);
                                    winningMoves.Add(diagonalHelp[i] + 2 + 100 * j);
                                    winningMoves.Add(diagonalHelp[i] - 2 + 100 * j);
                                }
                                winCondition = "DIAGONAL";
                                return true;
                            }
                            if (diagonalHelp.Contains(diagonalHelp[i] + 4) && diagonalHelp.Contains(diagonalHelp[i] - 4))
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    winningMoves.Add(diagonalHelp[i] + 100 * j);
                                    winningMoves.Add(diagonalHelp[i] + 4 + 100 * j);
                                    winningMoves.Add(diagonalHelp[i] - 4 + 100 * j);
                                }
                                winCondition = "DIAGONAL";
                                return true;
                            }
                        }

                    }

                }
            }
            if (move != 0)
            {
                int[] puzzleHelp = new int[54];
                if (move % 10 == 1)
                {

                    for (int i = 0; i < movesZ.Length; i++)
                    {
                        if (movesZ[i] % 100 == move % 100)
                        {
                            if (movesZ[i] / 100 == move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 1)
                        {
                            if (movesZ[i] / 100 != move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 2)
                        {
                            if (movesZ[i] / 100 == move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 3)
                        {
                            if (movesZ[i] / 100 != move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 4)
                        {
                            if (movesZ[i] / 100 != move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 5)
                        {
                            if (movesZ[i] / 100 != move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 6)
                        {
                            if (movesZ[i] / 100 == move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 7)
                        {
                            if (movesZ[i] / 100 != move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }
                        else if (movesZ[i] % 100 == move % 100 + 8)
                        {
                            if (movesZ[i] / 100 == move / 100)
                            {
                                puzzleHelp[i] = movesZ[i];
                            }
                        }


                    }
                }

                int fiveCube = 0;
                int evenCube = 0;
                if (puzzleHelp.Length >= 9)
                {
                    for (int i = 0; i < puzzleHelp.Length; i++)
                    {
                        if (puzzleHelp[i] % 10 == 5)
                        {
                            List<int> puzzleHelperWinMoves = new List<int>();
                            int countHelper = 0;
                            fiveCube = puzzleHelp[i] / 100;
                            evenCube = (fiveCube + move / 100) % 4;
                            if (evenCube == 0)
                            {
                                evenCube = 2;
                            }
                            for (int j = 0; j < puzzleHelp.Length; j++)
                            {
                                if (puzzleHelp[j] == move || puzzleHelp[j] == move + 2 || puzzleHelp[j] == move + 6 || puzzleHelp[j] == move + 8 ||
                                    puzzleHelp[j] == move % 100 + 1 + evenCube * 100 ||
                                    puzzleHelp[j] == move % 100 + 3 + evenCube * 100 ||
                                    puzzleHelp[j] == move % 100 + 5 + evenCube * 100 ||
                                    puzzleHelp[j] == move % 100 + 7 + evenCube * 100 ||
                                    puzzleHelp[j] == move % 100 + 4 + fiveCube * 100)
                                {
                                    puzzleHelperWinMoves.Add(puzzleHelp[j]);
                                    countHelper++;
                                }
                            }
                            if (countHelper == 9)
                            {
                                winningMoves = puzzleHelperWinMoves;
                                winCondition = "PUZZLE";
                                return true;
                            }
                        }
                    }
                }
            }
            if (move != 0)
            {
                int[] blockHelp = new int[54];
                if (move % 10 == 1)
                {
                    for (int i = 0; i < movesZ.Length; i++)
                    {
                        if (movesZ[i] == move)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 1)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 2)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 3)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 4)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 5)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 6)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 7)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                        else if (movesZ[i] == move + 8)
                        {
                            blockHelp[i] = movesZ[i];
                        }
                    }
                    List<int> trimmedBlockHelp = new List<int>();

                    for (int i = 0; i < blockHelp.Length; i++)
                    {
                        if (blockHelp[i] != 0)
                        {
                            trimmedBlockHelp.Add(blockHelp[i]);
                        }
                    }

                    if (trimmedBlockHelp.Count == 9)
                    {
                        winningMoves = trimmedBlockHelp;
                        winCondition = "BLOCK";
                        return true;
                    }
                }
            }
            if (move != 0)
            {
                if (move % 10 == 1 || move % 10 == 3 || move % 10 == 7 || move % 10 == 9 || move % 10 == 5)
                {
                    triangleHelp.Add(move);
                }

                //must contain front/left/top or a combination of their negatives
                //if (front/left/top)
                //then you need these 9
                //if (front/right/top)
                //then you need these 9
                //etc

                List<int> alphaCenters = new List<int>();
                List<int> betaCenters = new List<int>();
                List<int> gammaCenters = new List<int>();
                for (int i = 0; i < triangleHelp.Count; i++)
                {
                    if (triangleHelp[i] % 10 == 5)
                    {
                        switch (triangleHelp[i] / 100)
                        {
                            case 1:
                                alphaCenters.Add(triangleHelp[i]);
                                break;
                            case 2:
                                betaCenters.Add(triangleHelp[i]);
                                break;
                            case 3:
                                gammaCenters.Add(triangleHelp[i]);
                                break;

                        }
                    }

                }
                if (alphaCenters.Count >= 3)
                {
                    if (alphaCenters.Contains(115))
                    {
                        if (alphaCenters.Contains(125))
                        {
                            if (alphaCenters.Contains(135))
                            {
                                if (triangleHelp.Contains(117) && triangleHelp.Contains(113) && triangleHelp.Contains(129) && triangleHelp.Contains(121) &&
                                    triangleHelp.Contains(131) && triangleHelp.Contains(139))
                                {
                                    winningMoves.Add(115);
                                    winningMoves.Add(125);
                                    winningMoves.Add(135);
                                    winningMoves.Add(117);
                                    winningMoves.Add(113);
                                    winningMoves.Add(129);
                                    winningMoves.Add(121);
                                    winningMoves.Add(131);
                                    winningMoves.Add(139);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (alphaCenters.Contains(165))
                            {
                                if (triangleHelp.Contains(111) && triangleHelp.Contains(119) && triangleHelp.Contains(163) && triangleHelp.Contains(167) &&
                                    triangleHelp.Contains(127) && triangleHelp.Contains(123))
                                {
                                    winningMoves.Add(115);
                                    winningMoves.Add(125);
                                    winningMoves.Add(165);
                                    winningMoves.Add(111);
                                    winningMoves.Add(119);
                                    winningMoves.Add(163);
                                    winningMoves.Add(167);
                                    winningMoves.Add(127);
                                    winningMoves.Add(123);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                        if (alphaCenters.Contains(155))
                        {
                            if (alphaCenters.Contains(135))
                            {
                                if (triangleHelp.Contains(111) && triangleHelp.Contains(119) && triangleHelp.Contains(153) && triangleHelp.Contains(157) &&
                                    triangleHelp.Contains(133) && triangleHelp.Contains(137))
                                {
                                    winningMoves.Add(155);
                                    winningMoves.Add(135);
                                    winningMoves.Add(111);
                                    winningMoves.Add(119);
                                    winningMoves.Add(153);
                                    winningMoves.Add(157);
                                    winningMoves.Add(133);
                                    winningMoves.Add(137);
                                    winningMoves.Add(115);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (alphaCenters.Contains(165))
                            {
                                if (triangleHelp.Contains(113) && triangleHelp.Contains(117) && triangleHelp.Contains(161) && triangleHelp.Contains(169) &&
                                    triangleHelp.Contains(151) && triangleHelp.Contains(159))
                                {
                                    winningMoves.Add(115);
                                    winningMoves.Add(155);
                                    winningMoves.Add(165);
                                    winningMoves.Add(113);
                                    winningMoves.Add(117);
                                    winningMoves.Add(161);
                                    winningMoves.Add(169);
                                    winningMoves.Add(151);
                                    winningMoves.Add(159);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                    }
                    if (alphaCenters.Contains(145))
                    {
                        if (alphaCenters.Contains(125))
                        {
                            if (alphaCenters.Contains(135))
                            {
                                if (triangleHelp.Contains(141) && triangleHelp.Contains(149) && triangleHelp.Contains(123) && triangleHelp.Contains(127) &&
                                    triangleHelp.Contains(133) && triangleHelp.Contains(137))
                                {
                                    winningMoves.Add(145);
                                    winningMoves.Add(125);
                                    winningMoves.Add(135);
                                    winningMoves.Add(141);
                                    winningMoves.Add(149);
                                    winningMoves.Add(123);
                                    winningMoves.Add(127);
                                    winningMoves.Add(137);
                                    winningMoves.Add(133);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (alphaCenters.Contains(165))
                            {
                                if (triangleHelp.Contains(143) && triangleHelp.Contains(147) && triangleHelp.Contains(169) && triangleHelp.Contains(161) &&
                                    triangleHelp.Contains(129) && triangleHelp.Contains(121))
                                {
                                    winningMoves.Add(145);
                                    winningMoves.Add(125);
                                    winningMoves.Add(165);
                                    winningMoves.Add(143);
                                    winningMoves.Add(147);
                                    winningMoves.Add(169);
                                    winningMoves.Add(161);
                                    winningMoves.Add(129);
                                    winningMoves.Add(121);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                        if (alphaCenters.Contains(155))
                        {
                            if (alphaCenters.Contains(135))
                            {
                                if (triangleHelp.Contains(143) && triangleHelp.Contains(147) && triangleHelp.Contains(151) && triangleHelp.Contains(159) &&
                                    triangleHelp.Contains(131) && triangleHelp.Contains(139))
                                {
                                    winningMoves.Add(145);
                                    winningMoves.Add(155);
                                    winningMoves.Add(135);
                                    winningMoves.Add(143);
                                    winningMoves.Add(147);
                                    winningMoves.Add(151);
                                    winningMoves.Add(159);
                                    winningMoves.Add(131);
                                    winningMoves.Add(139);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (alphaCenters.Contains(165))
                            {
                                if (triangleHelp.Contains(141) && triangleHelp.Contains(149) && triangleHelp.Contains(167) && triangleHelp.Contains(163) &&
                                    triangleHelp.Contains(157) && triangleHelp.Contains(153))
                                {
                                    winningMoves.Add(145);
                                    winningMoves.Add(155);
                                    winningMoves.Add(165);
                                    winningMoves.Add(141);
                                    winningMoves.Add(149);
                                    winningMoves.Add(167);
                                    winningMoves.Add(163);
                                    winningMoves.Add(157);
                                    winningMoves.Add(153);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                    }
                }

                if (betaCenters.Count >= 3)
                {
                    if (betaCenters.Contains(215))
                    {
                        if (betaCenters.Contains(225))
                        {
                            if (betaCenters.Contains(235))
                            {
                                if (triangleHelp.Contains(217) && triangleHelp.Contains(213) && triangleHelp.Contains(229) && triangleHelp.Contains(221) &&
                                    triangleHelp.Contains(231) && triangleHelp.Contains(239))
                                {
                                    winningMoves.Add(215);
                                    winningMoves.Add(225);
                                    winningMoves.Add(235);
                                    winningMoves.Add(217);
                                    winningMoves.Add(213);
                                    winningMoves.Add(229);
                                    winningMoves.Add(221);
                                    winningMoves.Add(231);
                                    winningMoves.Add(239);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (betaCenters.Contains(265))
                            {
                                if (triangleHelp.Contains(211) && triangleHelp.Contains(219) && triangleHelp.Contains(263) && triangleHelp.Contains(267) &&
                                    triangleHelp.Contains(227) && triangleHelp.Contains(223))
                                {
                                    winningMoves.Add(215);
                                    winningMoves.Add(225);
                                    winningMoves.Add(265);
                                    winningMoves.Add(211);
                                    winningMoves.Add(219);
                                    winningMoves.Add(263);
                                    winningMoves.Add(267);
                                    winningMoves.Add(227);
                                    winningMoves.Add(223);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                        if (betaCenters.Contains(255))
                        {
                            if (betaCenters.Contains(235))
                            {
                                if (triangleHelp.Contains(211) && triangleHelp.Contains(219) && triangleHelp.Contains(253) && triangleHelp.Contains(257) &&
                                    triangleHelp.Contains(233) && triangleHelp.Contains(237))
                                {
                                    winningMoves.Add(215);
                                    winningMoves.Add(255);
                                    winningMoves.Add(235);
                                    winningMoves.Add(211);
                                    winningMoves.Add(219);
                                    winningMoves.Add(253);
                                    winningMoves.Add(257);
                                    winningMoves.Add(233);
                                    winningMoves.Add(237);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (betaCenters.Contains(265))
                            {
                                if (triangleHelp.Contains(213) && triangleHelp.Contains(217) && triangleHelp.Contains(261) && triangleHelp.Contains(269) &&
                                    triangleHelp.Contains(251) && triangleHelp.Contains(259))
                                {
                                    winningMoves.Add(215);
                                    winningMoves.Add(255);
                                    winningMoves.Add(265);
                                    winningMoves.Add(213);
                                    winningMoves.Add(217);
                                    winningMoves.Add(261);
                                    winningMoves.Add(269);
                                    winningMoves.Add(251);
                                    winningMoves.Add(259);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                    }
                    if (betaCenters.Contains(245))
                    {
                        if (betaCenters.Contains(225))
                        {
                            if (betaCenters.Contains(235))
                            {
                                if (triangleHelp.Contains(241) && triangleHelp.Contains(249) && triangleHelp.Contains(223) && triangleHelp.Contains(227) &&
                                    triangleHelp.Contains(233) && triangleHelp.Contains(237))
                                {
                                    winningMoves.Add(245);
                                    winningMoves.Add(225);
                                    winningMoves.Add(235);
                                    winningMoves.Add(241);
                                    winningMoves.Add(249);
                                    winningMoves.Add(223);
                                    winningMoves.Add(227);
                                    winningMoves.Add(233);
                                    winningMoves.Add(237);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (betaCenters.Contains(265))
                            {
                                if (triangleHelp.Contains(243) && triangleHelp.Contains(247) && triangleHelp.Contains(269) && triangleHelp.Contains(261) &&
                                    triangleHelp.Contains(229) && triangleHelp.Contains(221))
                                {
                                    winningMoves.Add(245);
                                    winningMoves.Add(225);
                                    winningMoves.Add(265);
                                    winningMoves.Add(243);
                                    winningMoves.Add(247);
                                    winningMoves.Add(269);
                                    winningMoves.Add(261);
                                    winningMoves.Add(229);
                                    winningMoves.Add(221);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                        if (betaCenters.Contains(255))
                        {
                            if (betaCenters.Contains(235))
                            {
                                if (triangleHelp.Contains(243) && triangleHelp.Contains(247) && triangleHelp.Contains(251) && triangleHelp.Contains(259) &&
                                    triangleHelp.Contains(231) && triangleHelp.Contains(239))
                                {
                                    winningMoves.Add(245);
                                    winningMoves.Add(255);
                                    winningMoves.Add(235);
                                    winningMoves.Add(243);
                                    winningMoves.Add(247);
                                    winningMoves.Add(251);
                                    winningMoves.Add(259);
                                    winningMoves.Add(231);
                                    winningMoves.Add(239);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (betaCenters.Contains(265))
                            {
                                if (triangleHelp.Contains(241) && triangleHelp.Contains(249) && triangleHelp.Contains(267) && triangleHelp.Contains(263) &&
                                    triangleHelp.Contains(257) && triangleHelp.Contains(253))
                                {
                                    winningMoves.Add(245);
                                    winningMoves.Add(255);
                                    winningMoves.Add(265);
                                    winningMoves.Add(241);
                                    winningMoves.Add(249);
                                    winningMoves.Add(267);
                                    winningMoves.Add(263);
                                    winningMoves.Add(257);
                                    winningMoves.Add(253);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                    }
                }

                if (gammaCenters.Count >= 3)
                {
                    if (gammaCenters.Contains(315))
                    {
                        if (gammaCenters.Contains(325))
                        {
                            if (gammaCenters.Contains(335))
                            {
                                if (triangleHelp.Contains(317) && triangleHelp.Contains(313) && triangleHelp.Contains(329) && triangleHelp.Contains(321) &&
                                    triangleHelp.Contains(331) && triangleHelp.Contains(339))
                                {
                                    winningMoves.Add(315);
                                    winningMoves.Add(325);
                                    winningMoves.Add(335);
                                    winningMoves.Add(317);
                                    winningMoves.Add(313);
                                    winningMoves.Add(329);
                                    winningMoves.Add(321);
                                    winningMoves.Add(331);
                                    winningMoves.Add(339);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (gammaCenters.Contains(365))
                            {
                                if (triangleHelp.Contains(311) && triangleHelp.Contains(319) && triangleHelp.Contains(363) && triangleHelp.Contains(367) &&
                                    triangleHelp.Contains(327) && triangleHelp.Contains(323))
                                {
                                    winningMoves.Add(315);
                                    winningMoves.Add(325);
                                    winningMoves.Add(365);
                                    winningMoves.Add(311);
                                    winningMoves.Add(319);
                                    winningMoves.Add(363);
                                    winningMoves.Add(367);
                                    winningMoves.Add(327);
                                    winningMoves.Add(323);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                        if (gammaCenters.Contains(355))
                        {
                            if (gammaCenters.Contains(335))
                            {
                                if (triangleHelp.Contains(311) && triangleHelp.Contains(319) && triangleHelp.Contains(353) && triangleHelp.Contains(357) &&
                                    triangleHelp.Contains(333) && triangleHelp.Contains(337))
                                {
                                    winningMoves.Add(315);
                                    winningMoves.Add(355);
                                    winningMoves.Add(335);
                                    winningMoves.Add(311);
                                    winningMoves.Add(319);
                                    winningMoves.Add(353);
                                    winningMoves.Add(357);
                                    winningMoves.Add(333);
                                    winningMoves.Add(337);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (gammaCenters.Contains(365))
                            {
                                if (triangleHelp.Contains(313) && triangleHelp.Contains(317) && triangleHelp.Contains(361) && triangleHelp.Contains(369) &&
                                    triangleHelp.Contains(351) && triangleHelp.Contains(359))
                                {
                                    winningMoves.Add(315);
                                    winningMoves.Add(355);
                                    winningMoves.Add(365);
                                    winningMoves.Add(313);
                                    winningMoves.Add(317);
                                    winningMoves.Add(361);
                                    winningMoves.Add(369);
                                    winningMoves.Add(351);
                                    winningMoves.Add(359);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                    }
                    if (gammaCenters.Contains(345))
                    {
                        if (gammaCenters.Contains(325))
                        {
                            if (gammaCenters.Contains(335))
                            {
                                if (triangleHelp.Contains(341) && triangleHelp.Contains(349) && triangleHelp.Contains(323) && triangleHelp.Contains(327) &&
                                    triangleHelp.Contains(333) && triangleHelp.Contains(337))
                                {
                                    winningMoves.Add(345);
                                    winningMoves.Add(325);
                                    winningMoves.Add(335);
                                    winningMoves.Add(341);
                                    winningMoves.Add(349);
                                    winningMoves.Add(323);
                                    winningMoves.Add(327);
                                    winningMoves.Add(333);
                                    winningMoves.Add(337);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (gammaCenters.Contains(365))
                            {
                                if (triangleHelp.Contains(343) && triangleHelp.Contains(347) && triangleHelp.Contains(369) && triangleHelp.Contains(361) &&
                                    triangleHelp.Contains(329) && triangleHelp.Contains(321))
                                {
                                    winningMoves.Add(345);
                                    winningMoves.Add(325);
                                    winningMoves.Add(365);
                                    winningMoves.Add(343);
                                    winningMoves.Add(347);
                                    winningMoves.Add(369);
                                    winningMoves.Add(361);
                                    winningMoves.Add(329);
                                    winningMoves.Add(321);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                        if (gammaCenters.Contains(355))
                        {
                            if (gammaCenters.Contains(335))
                            {
                                if (triangleHelp.Contains(343) && triangleHelp.Contains(347) && triangleHelp.Contains(351) && triangleHelp.Contains(359) &&
                                    triangleHelp.Contains(331) && triangleHelp.Contains(339))
                                {
                                    winningMoves.Add(345);
                                    winningMoves.Add(355);
                                    winningMoves.Add(335);
                                    winningMoves.Add(343);
                                    winningMoves.Add(347);
                                    winningMoves.Add(351);
                                    winningMoves.Add(359);
                                    winningMoves.Add(331);
                                    winningMoves.Add(339);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                            if (gammaCenters.Contains(365))
                            {
                                if (triangleHelp.Contains(341) && triangleHelp.Contains(349) && triangleHelp.Contains(367) && triangleHelp.Contains(363) &&
                                    triangleHelp.Contains(357) && triangleHelp.Contains(353))
                                {
                                    winningMoves.Add(345);
                                    winningMoves.Add(355);
                                    winningMoves.Add(365);
                                    winningMoves.Add(341);
                                    winningMoves.Add(349);
                                    winningMoves.Add(367);
                                    winningMoves.Add(363);
                                    winningMoves.Add(357);
                                    winningMoves.Add(353);
                                    winCondition = "TRIANGLE";
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            if (move != 0)
            {
                if (move % 10 == 1 || move % 10 == 3 || move % 10 == 7 || move % 10 == 9)
                {
                    warpHelp.Add(move);
                }

                List<int> threeCornersOneFace = new List<int>();
                int threeCornersCounter = 0;

                if (warpHelp.Count >= 9)
                {
                    for (int i = 0; i < warpHelp.Count; i++)
                    {
                        for (int j = 0; j < warpHelp.Count; j++)
                        {
                            if (warpHelp[i] / 10 == warpHelp[j] / 10)
                            {
                                threeCornersCounter++;
                            }
                            if (threeCornersCounter == 3)
                            {
                                threeCornersOneFace.Add(warpHelp[i]);
                                break;
                            }
                        }
                        threeCornersCounter = 0;
                    }
                }

                if (threeCornersOneFace.Count >= 9)
                {
                    for (int i = 0; i < threeCornersOneFace.Count; i++)
                    {
                        int temp;
                        int secondTemp;
                        int thirdTemp;
                        if (threeCornersOneFace[i] / 100 == 1)
                        {
                            temp = 100;
                            secondTemp = 200;
                            thirdTemp = 300;
                        }
                        else if (threeCornersOneFace[i] / 100 == 2)
                        {
                            temp = 200;
                            secondTemp = 300;
                            thirdTemp = 100;
                        }
                        else
                        {
                            temp = 300;
                            secondTemp = 100;
                            thirdTemp = 200;
                        }
                        if (((threeCornersOneFace[i] % 100) - (threeCornersOneFace[i] % 10)) == 10)
                        {

                            if (threeCornersOneFace[i] % 10 == 1)
                            {



                                if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 2 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 6 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 20 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 20 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 10 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 10 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + thirdTemp)))
                                {
                                    winningMoves.Add(threeCornersOneFace[i]);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 2 + temp);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 6 + temp);
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 20 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 20 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 20 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 10 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 10 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 10 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + thirdTemp);
                                    }
                                    winCondition = "WARP";
                                    return true;
                                }
                                else if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 2 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 8 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 22 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 22 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 40 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 40 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 42 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 42 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 46 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 46 + thirdTemp)))
                                {
                                    winningMoves.Add(threeCornersOneFace[i]);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 2 + temp);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 8 + temp);
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 22 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 22 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 22 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 40 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 40 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 40 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 42 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 42 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 42 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 46 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 46 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 46 + thirdTemp);
                                    }
                                    winCondition = "WARP";
                                    return true;
                                }
                                else if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 6 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 8 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 38 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 38 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 44 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 44 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 46 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 46 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 48 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 48 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 50 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 50 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 56 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 56 + thirdTemp)))

                                {
                                    winningMoves.Add(threeCornersOneFace[i]);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 6 + temp);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 8 + temp);
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 38 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 38 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 38 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 44 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 44 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 44 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 46 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 46 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 46 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 48 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 48 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 48 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 50 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 50 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 50 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 56 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 56 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 56 + thirdTemp);
                                    }
                                    winCondition = "WARP";
                                    return true;
                                }
                                else if (threeCornersOneFace[i] % 10 == 3)
                                {
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 4 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 6 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 16 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 16 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 50 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 50 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 52 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 52 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 56 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 56 + thirdTemp)))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i]);
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 4 + temp);
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 6 + temp);
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 16 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 16 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 16 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 50 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 50 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 50 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 52 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 52 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 52 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 56 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 56 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 56 + thirdTemp);
                                        }
                                        winCondition = "WARP";
                                        return true;
                                    }
                                }
                            }
                        }
                        else if (((threeCornersOneFace[i] % 100) - (threeCornersOneFace[i] % 10)) == 40)
                        {
                            if (threeCornersOneFace[i] % 10 == 1)
                            {
                                if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 2 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 6 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 10 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 10 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 8 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 8 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 2 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 2 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 10 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 10 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + thirdTemp)))
                                {
                                    winningMoves.Add(threeCornersOneFace[i]);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 2 + temp);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 6 + temp);
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 10 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 10 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 10 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 8 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 8 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 8 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 2 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 2 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 2 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 10 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 10 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 10 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + thirdTemp);
                                    }
                                    winCondition = "WARP";
                                    return true;
                                }
                                else if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 2 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 8 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 8 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 8 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 10 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 10 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 4 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 4 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 20 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 20 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 14 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 14 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 18 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 18 + thirdTemp)))
                                {
                                    winningMoves.Add(threeCornersOneFace[i]);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 2 + temp);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 8 + temp);
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 8 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 8 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 8 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 10 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 10 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 10 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 4 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 4 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 4 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 20 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 20 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 20 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 14 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 14 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 14 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 18 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 18 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 - 18 + thirdTemp);
                                    }
                                    winCondition = "WARP";
                                    return true;
                                }
                                else if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 6 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 8 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 16 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 16 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 22 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 22 + thirdTemp)))
                                {
                                    winningMoves.Add(threeCornersOneFace[i]);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 6 + temp);
                                    winningMoves.Add(threeCornersOneFace[i] % 100 + 8 + temp);
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 12 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 12 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 18 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 18 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 16 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 16 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 16 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + thirdTemp);
                                    }
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 22 + secondTemp))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 22 + secondTemp);
                                    }
                                    else
                                    {
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 22 + thirdTemp);
                                    }
                                    winCondition = "WARP";
                                    return true;
                                }
                                else if (threeCornersOneFace[i] % 10 == 3)
                                {
                                    if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 4 + temp) &&
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 6 + temp) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 20 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 20 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 14 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 14 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 12 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 12 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + thirdTemp)) &&
                                        (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 20 + secondTemp) ||
                                        threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 20 + thirdTemp)))
                                    {
                                        winningMoves.Add(threeCornersOneFace[i]);
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 4 + temp);
                                        winningMoves.Add(threeCornersOneFace[i] % 100 + 6 + temp);
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 20 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 - 20 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 - 20 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 14 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 - 14 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 - 14 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 - 12 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 - 12 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 - 12 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 28 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 28 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 26 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 26 + thirdTemp);
                                        }
                                        if (threeCornersOneFace.Contains(threeCornersOneFace[i] % 100 + 20 + secondTemp))
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 20 + secondTemp);
                                        }
                                        else
                                        {
                                            winningMoves.Add(threeCornersOneFace[i] % 100 + 20 + thirdTemp);
                                        }
                                        winCondition = "WARP";
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (move != 0)
            {
                chaosHelp[move % 10 - 1].Add(move);
            }
        }

        //bool chaosWin = false;

        //if (rotationSide == 0)
        //{
        //    chaosWin = ChaosHelper(chaosHelp, movesZ, movesIndex);
        //}
        //else
        //{
        //    int tempMoveIndex = 0;
        //    for (int z = 1; z < 10; z++)
        //    {
        //        tempMoveIndex = rotationSide * 10 + z;
        //        for (int i = 0; i < movesZ.Length; i++)
        //        {
        //            if (tempMoveIndex == movesZ[i])
        //            {
        //                tempMoveIndex = i;
        //                chaosWin = ChaosHelper(chaosHelp, movesZ, tempMoveIndex);
        //                break;
        //            }
        //        }

        //        if (chaosWin)
        //        {
        //            break;
        //        }
        //    }
        //}

        //return chaosWin; 
        return false;
    }

    private bool ChaosHelper(List<int>[] chaosHelp, int[] movesZ, int movesIndex)
    {
        bool chaosHasNoChance = false;

        for (int i = 0; i < 9; i++)
        {
            if (chaosHelp[i].Count == 0)
            {
                chaosHasNoChance = true;
            }
            
        }
        if (chaosHasNoChance)
        {
            return false;
        }
        List<int> tempChaos = new List<int>(chaosHelp[movesZ[movesIndex] % 10 - 1]);
        if (!chaosHasNoChance)
        {
            chaosHelp[movesZ[movesIndex] % 10 - 1].Clear();
            chaosHelp[movesZ[movesIndex] % 10 - 1].Add(movesZ[movesIndex]);
            //worst case this does 6^8 loops... optimize this... very slow...
            //added an if to skip if two are on the same face... still slow...
            //
            for (int i = 0; i < chaosHelp[0].Count; i++)
            {
                if (movesZ[movesIndex] / 10 != chaosHelp[0][i] / 10 || movesZ[movesIndex] == chaosHelp[0][i])
                {
                    for (int j = 0; j < chaosHelp[1].Count; j++)
                    {
                        if (movesZ[movesIndex] / 10 != chaosHelp[1][j] / 10 || movesZ[movesIndex] == chaosHelp[1][j])
                        {
                            for (int k = 0; k < chaosHelp[2].Count; k++)
                            {
                                if (movesZ[movesIndex] / 10 != chaosHelp[2][k] / 10 || movesZ[movesIndex] == chaosHelp[2][k])
                                {
                                    for (int l = 0; l < chaosHelp[3].Count; l++)
                                    {
                                        if (movesZ[movesIndex] / 10 != chaosHelp[3][l] / 10 || movesZ[movesIndex] == chaosHelp[3][l])
                                        {
                                            for (int m = 0; m < chaosHelp[4].Count; m++)
                                            {
                                                if (movesZ[movesIndex] / 10 != chaosHelp[4][m] / 10 || movesZ[movesIndex] == chaosHelp[4][m])
                                                {
                                                    for (int n = 0; n < chaosHelp[5].Count; n++)
                                                    {
                                                        if (movesZ[movesIndex] / 10 != chaosHelp[5][n] / 10 || movesZ[movesIndex] == chaosHelp[5][n])
                                                        {
                                                            for (int o = 0; o < chaosHelp[6].Count; o++)
                                                            {
                                                                if (movesZ[movesIndex] / 10 != chaosHelp[6][o] / 10 || movesZ[movesIndex] == chaosHelp[6][o])
                                                                {
                                                                    for (int p = 0; p < chaosHelp[7].Count; p++)
                                                                    {
                                                                        if (movesZ[movesIndex] / 10 != chaosHelp[7][p] / 10 || movesZ[movesIndex] == chaosHelp[7][p])
                                                                        {
                                                                            for (int q = 0; q < chaosHelp[8].Count; q++)
                                                                            {
                                                                                if (movesZ[movesIndex] / 10 != chaosHelp[8][q] / 10 || movesZ[movesIndex] == chaosHelp[8][q])
                                                                                {
                                                                                    List<int> alphaCount = new List<int>();
                                                                                    List<int> betaCount = new List<int>();
                                                                                    List<int> gammaCount = new List<int>();

                                                                                    switch (chaosHelp[0][i] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[0][i]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[0][i]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[0][i]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[1][j] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[1][j]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[1][j]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[1][j]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[2][k] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[2][k]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[2][k]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[2][k]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[3][l] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[3][l]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[3][l]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[3][l]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[4][m] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[4][m]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[4][m]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[4][m]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[5][n] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[5][n]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[5][n]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[5][n]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[6][o] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[6][o]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[6][o]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[6][o]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[7][p] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[7][p]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[7][p]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[7][p]);
                                                                                            break;
                                                                                    }
                                                                                    switch (chaosHelp[8][q] / 100)
                                                                                    {
                                                                                        case 1:
                                                                                            alphaCount.Add(chaosHelp[8][q]);
                                                                                            break;
                                                                                        case 2:
                                                                                            betaCount.Add(chaosHelp[8][q]);
                                                                                            break;
                                                                                        case 3:
                                                                                            gammaCount.Add(chaosHelp[8][q]);
                                                                                            break;
                                                                                    }

                                                                                    if (alphaCount.Count == 3 && betaCount.Count == 3 && gammaCount.Count == 3)
                                                                                    {
                                                                                        if (alphaCount[0] / 10 != alphaCount[1] / 10 && alphaCount[0] / 10 != alphaCount[2] / 10 &&
                                                                                            alphaCount[2] / 10 != alphaCount[1] / 10)
                                                                                        {
                                                                                            if (betaCount[0] / 10 != betaCount[1] / 10 && betaCount[0] / 10 != betaCount[2] / 10 &&
                                                                                                betaCount[2] / 10 != betaCount[1] / 10)
                                                                                            {
                                                                                                if (gammaCount[0] / 10 != gammaCount[1] / 10 && gammaCount[0] / 10 != gammaCount[2] / 10 &&
                                                                                                    gammaCount[2] / 10 != gammaCount[1] / 10)
                                                                                                {
                                                                                                    winningMoves.Add(alphaCount[0]);
                                                                                                    winningMoves.Add(alphaCount[1]);
                                                                                                    winningMoves.Add(alphaCount[2]);
                                                                                                    winningMoves.Add(betaCount[0]);
                                                                                                    winningMoves.Add(betaCount[1]);
                                                                                                    winningMoves.Add(betaCount[2]);
                                                                                                    winningMoves.Add(gammaCount[0]);
                                                                                                    winningMoves.Add(gammaCount[1]);
                                                                                                    winningMoves.Add(gammaCount[2]);
                                                                                                    winCondition = "CHAOS";
                                                                                                    return true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            chaosHelp[movesZ[movesIndex] % 10 - 1] = tempChaos;
        }
        return false;
    }

    private void HighlightWinningMoves()
    {
        for (int i = 0; i < winningMoves.Count; i++)
        {
            rend = GameObject.Find(winningMoves[i].ToString()).GetComponent<Renderer>();

            rend.material.SetColor("_Color", highlightColor);
        }
    }

    public int[] GetMovesX()
    {
        return movesX;
    }

    public void SetMovesX(int x, int index)
    {
        movesX[index] = x;
        movesMadeX++;
    }

    public int GetMovesMadeX()
    {
        return movesMadeX;
    }

    public int[] GetMovesY()
    {
        return movesY;
    }

    public void SetMovesY(int y, int index)
    {
        movesY[index] = y;
        movesMadeY++;
    }

    public int GetMovesMadeY()
    {
        return movesMadeY;
    }
}