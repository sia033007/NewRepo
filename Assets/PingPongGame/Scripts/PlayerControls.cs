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
using EdgeMultiplay;

namespace MobiledgeXPingPongGame
{
    public class PlayerControls : NetworkedPlayer
    {
        #region public variables

        public GameManager gameManager;
        public KeyCode moveUp = KeyCode.W;
        public KeyCode moveDown = KeyCode.S;
        public float speed = 15f;
        public float boundX = 4.0f;
        public float boundY = 2.25f;
        public Rigidbody2D rb2d;

        #endregion

        #region Monobehaviour Callbacks

        void Start()
        {
            ListenToMessages();
            gameManager = FindObjectOfType<GameManager>();
            rb2d = GetComponent<Rigidbody2D>();
        }
        void OnDestroy()
        {
            StopListening();
        }
        void Update()
        {

            if (isLocalPlayer && ActivePlayer)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector3 p = Camera.main.ScreenToWorldPoint(touch.position);
                        p.x = transform.position.x;
                        p.z = transform.position.z;
                        rb2d.MovePosition(p);
                    }
                }

                var vel = rb2d.velocity;
                if (Input.GetKey(moveUp))
                {
                    vel.y = speed;
                }
                else if (Input.GetKey(moveDown))
                {
                    vel.y = -speed;
                }
                else
                {
                    vel.y = 0;
                }

                rb2d.velocity = vel;
                var pos = transform.position;
                if (pos.y > boundY)
                {
                    pos.y = boundY;
                }
                else if (pos.y < -boundY)
                {
                    pos.y = -boundY;
                }
                transform.position = pos;
            }
        }

        #endregion

        #region NetworkedPlayer Callbacks

        public override void OnMessageReceived(GamePlayEvent gameEvent)
        {
            switch (gameEvent.eventName)
            {
                case "restart":
                    StartCoroutine(gameManager.RestartGame());
                    break;
                case "score":
                    gameManager.UpdateScore(gameEvent.integerData[0], gameEvent.integerData[1]);
                    break;
            }
        }

        #endregion
    }
}