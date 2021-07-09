/**
 * Copyright 2018-2021 MobiledgeX, Inc. All rights and licenses reserved.
 * MobiledgeX, Inc. 156 2nd Street #408, San Francisco, CA 94105
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using EdgeMultiplay;

namespace MobiledgeXPingPongGame
{
    public class GameManager : EdgeMultiplayCallbacks
    {
        #region GameManager variables
        public Text uiConsole;
        private GameObject theBall;
        public int[] score;
        public GUISkin layout;
        public Material activePlayerMaterial;
        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            ConnectToEdge();
        }
        private void OnGUI()
        {
            if (EdgeManager.gameStarted)
            {
                GUI.skin = layout;
                GUI.Label(new Rect(Screen.width / 2 - 150 - 12, 20, 100, 100), "" + score[0]);
                GUI.Label(new Rect(Screen.width / 2 + 150 + 12, 20, 100, 100), "" + score[1]);
                if (score.Max() == 10)
                {
                    theBall.SetActive(false);
                    GUI.Label(new Rect(Screen.width / 2 - 150, 200, 2000, 1000), "PLAYER" + (score.ToList().IndexOf(10) + 1) + " WINS");
                    if (GUI.Button(new Rect(Screen.width / 2 - 60, 35, 120, 53), "RESTART"))
                    {
                        EdgeManager.MessageSender.BroadcastMessage(new GamePlayEvent
                        {
                            eventName = "restart"
                        });
                        StartCoroutine(RestartGame());
                    }
                }
            }
        }
        private void Awake()
        {
            score = new int[2] { 0, 0 };
        }
                
        private void Start()
        {
            uiConsole = GameObject.FindGameObjectWithTag("UIConsole").GetComponent<Text>();
            clog("Connecting to Edge");
        }

        #endregion

        #region EdgeMultiplay Callbacks

        public override void OnConnectionToEdge()
        {
            clog("Connected to EdgeMultiPlay Server");
        }
        public override void OnFaliureToConnect(string reason)
        {
            clog("Failed to connect to Edge!" + "\n" + reason);
        }
        public override void OnRegisterEvent()
        {
            clog("Game session saved ...");
            EdgeManager.JoinOrCreateRoom(playerName: "", playerAvatar: 0, maxPlayersPerRoom: 2);
        }
        public override void OnRoomCreated(Room room)
        {
            clog("Didn't find vacancy in a room, Room created!");
            clog("Waiting for players to join!");
        }
        public override void OnRoomJoin(Room room)
        {
            clog("Joined Room!");
            clog("Waiting for Game to Start!");
        }
        public override void OnGameStart()
        {
            clog("Game started", true);
            foreach (NetworkedPlayer player in EdgeManager.currentRoomPlayers)
            {
                player.GetComponentInChildren<TextMesh>().text = player.playerName;
            }
            StartBallMovement();
        }
        public override void OnPlayerLeft(RoomMemberLeft playerLeft)
        {
            clog(EdgeManager.GetPlayer(playerLeft.idOfPlayerLeft).playerName + " left");
            theBall.transform.position = Vector3.zero;
        }

        public override void OnNewObservableCreated(Observable observable)
        {
            base.OnNewObservableCreated(observable);
            if (observable.owner != EdgeManager.localPlayer)
            {
                theBall = observable.observeredTransform.gameObject;
            }
        }

        #endregion

        #region GameManager Functions

        void StartBallMovement()
        {

            EdgeManager.localPlayer.gameObject.GetComponent<SpriteRenderer>().material = activePlayerMaterial;
            if (EdgeManager.localPlayer.playerIndex == 0) //first player in the room owns the ball
            {
                if(theBall == null)
                {
                    Observable ballObservable = EdgeManager.localPlayer.CreateObservableObject("Ball", Vector3.zero, Quaternion.identity, SyncOptions.SyncPosition);
                    theBall = ballObservable.observeredTransform.gameObject;
                }
                BallControl bc = theBall.GetComponent<BallControl>();
                bc.rb2d = theBall.GetComponent<Rigidbody2D>();
                bc.transform.position = Vector3.zero;
                bc.rb2d.velocity = Vector3.zero;
                Vector2 startingForce = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
                bc.rb2d.AddForce(startingForce);
                bc.rb2d.velocity = new Vector2(Random.Range(1, 10), Random.Range(1, 10));
            }
        }


        public void UpdateScore(int playerScore, int playerIndex)
        {
            score[playerIndex] = playerScore;
            if (score.Max() < 10)
            {
                theBall.GetComponent<BallControl>().ResetBall();
                StartCoroutine(theBall.GetComponent<BallControl>().GoBall());
            }
            else
            {
                theBall.GetComponent<BallControl>().ResetBall();
            }
        }

        public IEnumerator RestartGame()
        {
            yield return new WaitForSeconds(1);
            score = new int[2] { 0, 0 };
            theBall.SetActive(true);
            theBall.GetComponent<BallControl>().ResetBall();
            yield return new WaitForSeconds(1);
            StartBallMovement();
        }
        public void clog(string msg, bool emptyLog = false)
        {
            if (!emptyLog)
                uiConsole.text += "\n" + msg;
            else
                uiConsole.text = msg;
            Debug.Log(msg);
        }

        #endregion
    }
}