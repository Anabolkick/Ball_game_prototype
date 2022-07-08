using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    public GameObject MissilePrefab;
    public Camera MainCamera;
    public GameObject Road;

    private GameObject _missile;
    private float _touchStartTime;
    private float _touchTime;
    private int _currWaypoint;
    private Vector3? _shotPoint;

    private bool _isJumping;
    private bool _isCheckingWaypoint;
    private bool _isAlive;
    private bool _isWin;
    private bool _isCanShoot;

    void Start()
    {
        _isAlive = true;
        _isCanShoot = true;
        StartCoroutine(FindWaypoint());
    }

    void Update()
    {
        if (_isAlive && _isCanShoot && !_isJumping)
        {
            PrepareShot();
            CheckSizeDeath();
        }
    }

    private IEnumerator FindWaypoint()
    {
        _isCheckingWaypoint = true;
        List<Ray> rays = new List<Ray>();
        List<bool> results = new List<bool>();
        while (_isCheckingWaypoint && _currWaypoint < MapGenerator.WaypointsList.Count && _isAlive)
        {
            rays.Clear();
            results.Clear();
            Ray leftRay = new Ray(transform.position - transform.localScale / 1.9f + Vector3.forward*3, Vector3.forward);
            Ray leftUpRay = new Ray(transform.position - transform.localScale / 1.9f + Vector3.up * 1.5f + Vector3.forward*3, Vector3.forward);
            Ray rightRay = new Ray(transform.position + transform.localScale / 1.9f + Vector3.forward, Vector3.forward);
            Ray rightDownRay = new Ray(transform.position + transform.localScale / 1.9f + Vector3.down * 1.5f + Vector3.forward, Vector3.forward);
            Ray midRay = new Ray(transform.position + Vector3.forward *2,  Vector3.forward);
            Ray midUpRay = new Ray(transform.position + Vector3.up * 1.5f + Vector3.forward, Vector3.forward );

            rays.AddRange(new[] {leftRay, leftUpRay, rightRay, rightDownRay, midRay, midUpRay });

            foreach (var ray in rays)
            {
                //Debug.DrawRay(ray.origin, ray.direction * 70f, Color.blue, 3f);
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.transform.gameObject.tag == "Waypoint")
                        results.Add(true);
                    else
                    {
                        results.Add(false);
                    }
                }
            }

            if (!results.Contains(false))
            {
                Physics.Raycast(midRay, out var hit);
                hit.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
                hit.transform.GetChild(0).gameObject.SetActive(false);

                StartCoroutine(JumpToPoint(MapGenerator.WaypointsList[_currWaypoint].transform, transform.forward, 0.45f, 2.5f));
                _isCheckingWaypoint = false;
                _currWaypoint++;
                if (MapGenerator.WaypointsList.Count == _currWaypoint)
                {
                    _isWin = true;
                }
                yield return 0;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void PrepareShot()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if (transform.localScale.x > 0)
            {
                if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Ray ray = MainCamera.ScreenPointToRay(touch.position);
                    //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 5f);
                    if (Physics.Raycast(ray, out var hit))
                        _shotPoint = hit.point;
                    else
                        return;

                    _touchStartTime = Time.time;
                    _missile = Instantiate(MissilePrefab, transform.position, Quaternion.identity);
                    _missile.transform.SetParent(this.transform);
                    _missile.transform.localScale = Vector3.one * 0.08f;
                    transform.localScale -= Vector3.one * 0.08f;
                    _missile.transform.position = new Vector3(
                         transform.position.x,
                        transform.localScale.y / 2,
                        transform.position.z + transform.localScale.z / 2 + 1f);
                    _missile.GetComponent<Rigidbody>().isKinematic = true;
                }

                if (_shotPoint != null)
                {
                    _touchTime = Time.time - _touchStartTime;
                    var resizeVector = new Vector3(_touchTime, _touchTime, _touchTime) * 0.015f;
                    transform.localScale -= resizeVector;
                    _missile.transform.localScale += resizeVector * 1.05f;
                    _missile.transform.position += (resizeVector * 1.05f) / 2f;
                    Road.transform.localScale -= new Vector3(resizeVector.x * 1.3f, 0, 0);


                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Ray ray = MainCamera.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out var hit))
                            _shotPoint = hit.point;

                        _missile.GetComponent<Rigidbody>().isKinematic = false;
                        _missile.GetComponent<Rigidbody>()
                            .AddForce((Vector3)_shotPoint - transform.position, ForceMode.Impulse);
                        _shotPoint = null;
                        _missile.GetComponent<MissileExplosion>().Trigger.radius = (0.7f + _touchTime * 4) * 2;
                    }
                }
            }
        }
    }
    private void CheckSizeDeath()
    {
        if (transform.localScale.x < 0.85f)
        {
            Lose();
        }
    }
    private IEnumerator JumpToPoint(Transform point, Vector3 direction, float jumpDuration, float jumpDistance)
    {
        _isCanShoot = false;
        while (Vector3.Distance(transform.position, point.position - Vector3.forward * 3) > 2f)
        {
            StartCoroutine(Jump(direction, jumpDuration, jumpDistance));
            yield return new WaitForSeconds(jumpDuration + 0.02f);
        }

        if (!_isWin)
            StartCoroutine(FindWaypoint());
        else
            StartCoroutine(Win());

        _isCanShoot = true;
    }
    private IEnumerator Jump(Vector3 direction, float jumpDuration, float jumpDistance)
    {
        _isJumping = true;
        var jumpDirection = direction * jumpDistance;
        var jumpStartVelocityY = -jumpDuration * Physics.gravity.y / 2;
        Vector3 startPoint = transform.position;
        Vector3 targetPoint = startPoint + jumpDirection;
        float time = 0;
        float velocityY = jumpStartVelocityY;
        float height = startPoint.y;

        while (_isJumping)
        {
            var jumpProgress = time / jumpDuration;

            if (jumpProgress > 1)
            {
                _isJumping = false;
                jumpProgress = 1;
            }

            Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, jumpProgress);
            currentPos.y = height;
            transform.position = currentPos;

            //Wait until next frame.
            yield return null;

            height += velocityY * 4.5f * Time.deltaTime;
            velocityY += Time.deltaTime * Physics.gravity.y;
            time += Time.deltaTime;
        }

        transform.position = targetPoint;
        SoundManager.Instance.PlayBallSound();
        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Barrier")
        { 
            Lose();
        }
    }

    private void Lose()
    {
        _isAlive = false;
        MenuManager.OpenLoseMenu();
        gameObject.transform.localScale = Vector3.zero;
        SoundManager.Instance.PlayLoseSound();
    }

    private IEnumerator Win()
    {
        yield return new WaitForSeconds(2);
        MenuManager.OpenWinMenu();
    }

}
