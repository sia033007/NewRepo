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

using System.Collections;
using UnityEngine;

using EdgeMultiplay;

namespace MobiledgeXPingPongGame
{
  public class BallControl : MonoBehaviour
  {
        ObservableView view; //ObservableView component added to all observables at runtime
        public Rigidbody2D rb2d;

        private void Start()
        {
            view = GetComponent<ObservableView>();
            if(view == null)
            {
                throw new System.Exception("Can't find ObservableView component on the ball.");
            }
            if (GetComponent<Rigidbody2D>())
            {
                rb2d = GetComponent<Rigidbody2D>();
            }
        }

        public IEnumerator GoBall()
        {
            if (view.OwnerIsLocalPlayer())
            {
                yield return new WaitForSeconds(1);
                float rand = Random.Range(0f, 2f);
                float updown = Random.Range(-1f, 1f);
                if (rand < 1f)
                {
                    rb2d.AddForce(new Vector2(20, 15 * updown));
                }
                else
                {
                    rb2d.AddForce(new Vector2(-20, 15 * updown));
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (view.OwnerIsLocalPlayer() && collision.collider.attachedRigidbody)
            {
                if (collision.collider.CompareTag("Player"))
                {
                    Vector2 vel;
                    rb2d.AddForce(new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)));
                    vel.x = rb2d.velocity.x * 1.1f;
                    vel.y = (rb2d.velocity.y / 2f) + (collision.collider.attachedRigidbody.velocity.y / 1.5f);
                    rb2d.velocity = vel;
                }
            }
        }

        public void ResetBall()
        {
            if (view.OwnerIsLocalPlayer())
            {
                rb2d = GetComponent<Rigidbody2D>();
                transform.position = Vector2.zero;
                rb2d.velocity = Vector2.zero;
            }
        }
    }
}