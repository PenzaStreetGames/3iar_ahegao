using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Level.TileEntity {
    public class MovingTile : MonoBehaviour {
        public FieldController FieldController;
        public Vector3 speed = Vector3.zero;
        public float speedScale = 2f;
        public float lifetime = 0.2f;
        public SpriteRenderer spriteRenderer;

        // Start is called before the first frame update
        void Start() {
            Destroy(gameObject, lifetime);
        }

        public void SetFields(Tile tileFrom, Tile tileTo) {
            FieldController = tileFrom.fieldController;
            speed = tileTo.transform.position - tileFrom.transform.position;
            spriteRenderer.sprite = tileFrom.sprites[(int)tileFrom.tileColor];;
        }

        // Update is called once per frame
        void Update() {
            transform.Translate(speed * (speedScale * Time.deltaTime));
        }

        void OnDestroy() {

        }
    }
}
