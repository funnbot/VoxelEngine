using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

namespace VoxelEngine.Entities {

    public class RibitEntity : MonoBehaviour {
        public Player.Player player;

        void Start() {
            player = (Player.Player)GameObject.FindObjectOfType(typeof(Player.Player));
            StartCoroutine(Launch());
        }

        IEnumerator Launch() {
            yield return new WaitForSeconds(1);
            transform.LookAt(player.transform.position + Vector3.up);
            yield return new WaitForSeconds(1);
            float lerp = 0f;
            while (lerp < 1f) {
                lerp += Time.deltaTime * 10;
                transform.position = Vector3.Lerp(transform.position, player.transform.position - transform.forward, lerp);
                yield return null;
            }
        }
    }

}