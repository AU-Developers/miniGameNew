using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace conditioning
{

    public class gameplayController : MonoBehaviour
    {
        int _barMovement;
        int _color;
        public int _speed;

      
        private void Update()
        {
            if (transform.position.x < -8.9f)
                _barMovement = 1;
            else if (transform.position.x > 8.9f)
                _barMovement = 2;
            switch (_barMovement)
            {
                case 1:
                    transform.Translate(Vector2.right * _speed * Time.deltaTime);
                    break;
                case 2:
                    transform.Translate(Vector2.left * _speed * Time.deltaTime);
                    break;
                default:
                    transform.Translate(Vector2.right * _speed * Time.deltaTime);
                    break;
            }
            switch (_color)
            {
                case 1:
                    _speed = 0;
                    break;
                case 2:
                    _speed = 0;
                    break;
                default:
                    _speed = 0;
                    break;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (collision.gameObject.CompareTag("red"))
                    _color = 1;
                else if (collision.gameObject.CompareTag("black"))
                    _color = 2;
            }
        }
    }
  
}
