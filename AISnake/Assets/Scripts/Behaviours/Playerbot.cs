using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NistaGames
{
    
    [CreateAssetMenu(menuName = "AIBehaviours/Playerbot")]
    public class Playerbot : AIBehaviour
    {

        public enum State
        {
            Collect,
            Attack,
            Danger
        }

        private State _currentState;
        private Transform _dangerPlayer;
        private float _runStartTime;
        private TextMeshProUGUI _textMeshPro;

        public override void Init(GameObject own, SnakeMovement ownMove)
        {
            base.Init(own, ownMove);
            _currentState = State.Collect;

            ownMove.StartCoroutine(UpdateNameCoroutine());
        }

        private IEnumerator UpdateNameCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _textMeshPro = owner.GetComponentInChildren<TextMeshProUGUI>();
            _textMeshPro.text = $"<color=#D84315>https://itch.io/arturnista</color>\n{_currentState.ToString().ToUpper()}";
        }

        private void ChangeState(State state)
        {
            _currentState = state;
            _textMeshPro.text = $"<color=#D84315>https://itch.io/arturnista</color>\n{_currentState.ToString().ToUpper()}";
        }

        public override void Execute()
        {
            LookForDanger();
            switch (_currentState)
            {
                case State.Collect:
                    CollectOrbs();
                    break;
                case State.Attack:
                    Attack();
                    break;
                case State.Danger:
                    Protect();
                    break;
            }

            MoveForward();
        }

        private void LookForDanger()
        {
            Debug.DrawRay(owner.transform.position, (owner.transform.up * 5f), Color.green, Time.fixedDeltaTime);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position + (owner.transform.up * 3f), 2f);
            if (colliders.Length == 0)
            {
                return;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].gameObject.CompareTag("Body")) continue;
                if (colliders[i].transform.parent == owner.transform.parent) continue;

                foreach (Transform item in colliders[i].transform.parent)
                {
                    if (item.CompareTag("Bot"))
                    {
                        _dangerPlayer = item;
                        ChangeState(State.Danger);
                        _runStartTime = Time.time;
                        return;
                    }
                }
            }
        }

        private void CollectOrbs()
        {
            var allOrbs = GameObject.FindGameObjectsWithTag("Orb");
            if (allOrbs.Length > 0)
            {
                GameObject closestOrb = allOrbs[0];
                for (int i = 1; i < allOrbs.Length; i++)
                {
                    if (Vector3.Distance(allOrbs[i].transform.position, ownerMovement.transform.position) < Vector3.Distance(closestOrb.transform.position, ownerMovement.transform.position))
                    {
                        closestOrb = allOrbs[i];
                    }
                }

                Debug.DrawLine(owner.transform.position, closestOrb.transform.position, Color.green, Time.fixedDeltaTime);
                direction = (closestOrb.transform.position - ownerMovement.transform.position).normalized;
            }

            if (ownerMovement.bodyParts.Count > 10)
            {
                ChangeState(State.Attack);
            }
        }

        private void Attack()
        {
            var allBots = GameObject.FindGameObjectsWithTag("Bot");
            if (allBots.Length > 1)
            {
                GameObject closestBot = null;
                for (int i = 0; i < allBots.Length; i++)
                {
                    if (allBots[i] == owner.gameObject) continue;
                    else if (closestBot == null || Vector3.Distance(allBots[i].transform.position, ownerMovement.transform.position) < Vector3.Distance(closestBot.transform.position, ownerMovement.transform.position))
                    {
                        closestBot = allBots[i];
                    }
                }

                Debug.DrawLine(owner.transform.position, closestBot.transform.position, Color.magenta, Time.fixedDeltaTime);
                direction = ((closestBot.transform.position + closestBot.transform.up * 10f) - ownerMovement.transform.position).normalized;
                

                if (Vector3.Distance(closestBot.transform.position, owner.transform.position) > 15f)
                {
                    LookForCloseOrbs();
                }
            }
        }

        private void LookForCloseOrbs()
        {
            Debug.DrawRay(owner.transform.position, (owner.transform.up * 1f), Color.yellow, Time.fixedDeltaTime);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position + (owner.transform.up * 3f), 3f);
            if (colliders.Length == 0) return;

            GameObject closestOrb = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].gameObject.CompareTag("Orb")) continue;
                
                Debug.DrawLine(owner.transform.position, colliders[i].transform.position, Color.green, Time.fixedDeltaTime);
                if (closestOrb == null)
                {
                    closestOrb = colliders[i].gameObject;
                }
                else if (Vector3.Distance(colliders[i].transform.position, ownerMovement.transform.position) < Vector3.Distance(closestOrb.transform.position, ownerMovement.transform.position))
                {
                    closestOrb = colliders[i].gameObject;
                }
            }

            if (closestOrb == null) return;
            direction = (closestOrb.transform.position - ownerMovement.transform.position).normalized;
        }

        private void Protect()
        {
            if (_dangerPlayer == null)
            {
                ChangeState(State.Collect);
                return;
            }

            Vector3 medPosition = Vector3.zero;
            foreach (Transform item in _dangerPlayer.parent)
            {
                medPosition += item.position;
            }
            medPosition = medPosition * (1f / _dangerPlayer.parent.childCount);
            Vector3 dir = (medPosition - owner.transform.position).normalized;
            Debug.DrawLine(owner.transform.position, medPosition, Color.red);

            direction = -dir;
            if (Time.time - _runStartTime > 2f)
            {
                ChangeState(State.Collect);
            }
        }

        private void MoveForward()
        {
            MouseRotationSnake();
            owner.transform.Translate(direction * ownerMovement.speed * Time.deltaTime, Space.World);
        }

        private void MouseRotationSnake()
        {
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, ownerMovement.speed * Time.deltaTime);
        }

    }

}

