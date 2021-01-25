using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour
{
    private MoveMultiplayer game;

    public GameObject X;
    public GameObject O;
    public bool isX;
    private bool spectator = false;

    public static int movesMadeXOnServer;

    public static int movesMadeYOnServer;

    public static string[] cubeface;
    public static bool isClockwise;

    public static int[] movesX = new int[54];

    public static int[] movesY = new int[54]; 

    private static int currentMove;

    private void Awake()
    {
        if (isServer)
        {
            isX = true;
            game = Camera.main.GetComponent<MoveMultiplayer>();
        }
        else
        {
            isX = false;
            game = Camera.main.GetComponent<MoveMultiplayer>();
        }
        
    }

    private void OnDestroy()
    {
        movesMadeXOnServer = 0;

        movesMadeYOnServer = 0;

        cubeface = new string[9];
        isClockwise = false;

        movesX = new int[54];

        movesY = new int[54];
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        for (int i = 0; i < 54; i+=3)
        {
            if (game.GetMovesX()[i] != movesX[i])
            {
                game.SetMovesX(movesX[i], i);
                currentMove = 0;
                game.CreateOnlineMove(true);
            }
            if (game.GetMovesX()[i+1] != movesX[i+1])
            {
                game.SetMovesX(movesX[i+1], i+1);
                currentMove = 0;
                game.CreateOnlineMove(true);
            }
            if (game.GetMovesX()[i+2] != movesX[i+2])
            {
                game.SetMovesX(movesX[i+2], i+2);
                currentMove = 0;
                game.CreateOnlineMove(true);
            }
            if (game.GetMovesY()[i] != movesY[i])
            {
                game.SetMovesY(movesY[i], i);
                currentMove = 0;
                game.CreateOnlineMove(false);
            }
            if (game.GetMovesY()[i+1] != movesY[i+1])
            {
                game.SetMovesY(movesY[i+1], i+1);
                currentMove = 0;
                game.CreateOnlineMove(false);
            }
            if (game.GetMovesY()[i+2] != movesY[i+2])
            {
                game.SetMovesY(movesY[i+2], i+2);
                currentMove = 0;
                game.CreateOnlineMove(false);
            }
        }

    }

    [Command]
    private void CmdUpdateMovesMadeXOnServer()
    {
        movesMadeXOnServer++;
        RpcUpdateMovesMadeXOnClient(movesMadeXOnServer);
    }

    [ClientRpc]
    private void RpcUpdateMovesMadeXOnClient(int movesMadeX)
    {
        movesMadeXOnServer = movesMadeX;
    }

    [Command]
    private void CmdUpdateMovesMadeYOnServer()
    {
        movesMadeYOnServer++;
        RpcUpdateMovesMadeYOnClient(movesMadeYOnServer);
    }

    [ClientRpc]
    private void RpcUpdateMovesMadeYOnClient(int movesMadeY)
    {
        movesMadeYOnServer = movesMadeY;
    }

    [Command]
    private void CmdLastMadeMove(int move, bool X)
    {
        currentMove = move;

        if (X)
        {
            movesX[movesMadeXOnServer-1] = currentMove;
        }
        else
        {
            movesY[movesMadeYOnServer-1] = currentMove;
        }

        RpcLastMadeMove(currentMove, X);
    }

    [ClientRpc]
    private void RpcLastMadeMove(int num,  bool X)
    {

        currentMove = num;

        if (X)
        {
            movesX[movesMadeXOnServer-1] = currentMove;
        }
        else
        {
            movesY[movesMadeYOnServer-1] = currentMove;
        }

        for (int i = 0; i < 54; i++)
        {
            if (game.GetMovesX()[i] != movesX[i] && X == isX && movesX[i] != 0)
            {
                game.SetMovesX(movesX[i], i);
                currentMove = 0;
                game.CreateOnlineMove(true);
            }
        }
        for (int i = 0; i < 54; i++)
        {
            if (game.GetMovesY()[i] != movesY[i] && X == isX && movesY[i] != 0)
            {
                game.SetMovesY(movesY[i], i);
                currentMove = 0;
                game.CreateOnlineMove(false);
            }
        }


        currentMove = 0;
    }

    [Command]
    private void CmdRotationDirection(bool isCw)
    {
        isClockwise = isCw;
        RpcRotationDirection(isCw);
    }

    [ClientRpc]
    private void RpcRotationDirection(bool isCw)
    {
        isClockwise = isCw;
    }

    [Command]
    private void CmdCubeface(string[] cube)
    {
        cubeface = cube;
        RpcCubeface(cube);
    }

    [ClientRpc]
    private void RpcCubeface(string[] cube)
    {
        cubeface = cube;
        game.RotateEverythingOnline(isClockwise, cubeface);
        for (int i = 0; i < 54; i++)
        {
            movesX[i] = game.GetMovesX()[i];
            movesY[i] = game.GetMovesY()[i];
        }
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !spectator)
        {
            if (game.LocalPlayerObjectSoUpdate())
            {
                if (isX)
                {
                    CmdUpdateMovesMadeXOnServer();
                    CmdLastMadeMove(game.CurrentMove(), true);
                }
                else
                {
                    CmdUpdateMovesMadeYOnServer();
                    CmdLastMadeMove(game.CurrentMove(), false);
                }
            }


            if (game.LocalPlayerObjectSoRotate())
            {
                CmdRotationDirection(game.GetRotationDir());
                CmdCubeface(game.GetCubeFace());
            }
        }
        
    }

    public void SetIsX(bool setX)
    {
        isX = setX;
    }

    public GameObject myPlayerUnit;

    //public string PlayerName = "Anonymous";

    [Command]
    private void CmdSpawnToServer(string name)
    {
        Debug.Log("got inside cmdSpawnToServer");
        GameObject go;
        Debug.Log("raycast works");
        if (isX)
        {
            go = Instantiate(X);
        }
        else
        {
            go = Instantiate(O);
        }
        NetworkServer.Spawn(go);
        RpcSpawnToClient(name, go);
    }
    [ClientRpc]
    private void RpcSpawnToClient(string name, GameObject go)
    {
        go.transform.parent = GameObject.Find(name).transform;
        go.transform.localPosition = new Vector3(0, 0, 0);
        RotateMoveToFitCube(GameObject.Find(name).tag, go);
    }
    //[Command]
    //void CmdChangePlayerName(string n)
    //{
    //    PlayerName = n;
    //    Debug.Log(PlayerName);

    //    //RpcChangePlayerName(n);
    //}
    //[ClientRpc]
    //void RpcChangePlayerName(string n)
    //{
    //    Debug.Log(PlayerName);
    //    PlayerName = n;
    //}
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
}
