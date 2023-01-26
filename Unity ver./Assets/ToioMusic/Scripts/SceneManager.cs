using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using Random = UnityEngine.Random;

namespace ToioMusic
{
    public class SceneManager : MonoBehaviour
    {
        // private Cube cube1;
        // private Cube cube2;
        private CubeManager cm;
        public ConnectType connectType;

        private bool forwardButtonDownFlag = false;
        private bool backButtonDownFlag = false;
        private bool leftButtonDownFlag = false;
        private bool rightButtonDownFlag = false;

        public GameObject playButton;
        public GameObject stopButton;
        public GameObject controlButtons;

        public double cube1MoveSpeed;
        public int cube1MoveTime;    // ms
        public double cube1RotateSpeed;
        public int cube1RotateTime; // ms

        public float waitSec;
        private WaitForSeconds wait;
        private Coroutine playSound;
        private Coroutine moveCube2;
        public byte volume = 15;
        // public int addToVolume;
        // public int volumeDivided;
        // public int duration = 300;
        public int addToDuration;
        public float durationMultiplied;
        public int addToPitch;
        public float pitchMultiplied;

        public int cube2Range;
        public double cube2Speed = 50;
        public int cube2MoveTime = 250;
        public double cube2Tolerance = 8;

        // Start is called before the first frame update
        async void Start()
        {
            playButton.SetActive(true);
            stopButton.SetActive(false);
            controlButtons.SetActive(false);

            cm = new CubeManager(connectType);
            await cm.MultiConnect(2);
            // var peripheral = await new NearestScanner().Scan();
            // cube1 = await new CubeConnecter().Connect(peripheral);

            wait = new WaitForSeconds(waitSec);
        }

        // Update is called once per frame
        void Update()
        {
            if (forwardButtonDownFlag)
            {
                cm.handles[0].MoveRaw(cube1MoveSpeed, cube1MoveSpeed, cube1MoveTime);
                // cube1.Move(cube1MoveSpeed, cube1MoveSpeed, cube1MoveTime);
            }
            else if (backButtonDownFlag)
            {
                cm.handles[0].MoveRaw(-1 * cube1MoveSpeed, -1 * cube1MoveSpeed, cube1MoveTime);
                // cube1.Move(-1 * cube1MoveSpeed, -1 * cube1MoveSpeed, cube1MoveTime);                
            }
            else if (leftButtonDownFlag)
            {
                cm.handles[0].Move(0, -1 * cube1RotateSpeed, cube1RotateTime);
            }
            else if (rightButtonDownFlag)
            {
                cm.handles[0].Move(0, cube1RotateSpeed, cube1RotateTime);
            }
        }

        public void OnPlayButtonClicked()
        {
            playSound = StartCoroutine("PlaySound");
            moveCube2 = StartCoroutine("MoveCube2");
            stopButton.SetActive(true);
            controlButtons.SetActive(true);
            playButton.SetActive(false);
        }

        public void OnStopButtonClicked()
        {
            StopCoroutine(playSound);
            StopCoroutine(moveCube2);
            playButton.SetActive(true);
            controlButtons.SetActive(false);
            stopButton.SetActive(false);
        }

        public void OnForwardButtonDown()
        {
            Debug.Log("ForwardButton down.");
            forwardButtonDownFlag = true;
        }
        public void OnForwardButtonUp()
        {
            Debug.Log("ForwardButton up.");
            forwardButtonDownFlag = false;
        }

        public void OnBackButtonDown()
        {
            Debug.Log("BackButton down.");
            backButtonDownFlag = true;
        }
        public void OnBackButtonUp()
        {
            Debug.Log("BackButton up.");
            backButtonDownFlag = false;
        }

        public void OnLeftButtonDown()
        {
            Debug.Log("LeftButton down.");
            leftButtonDownFlag = true;
        }
        public void OnLeftButtonUp()
        {
            Debug.Log("LeftButton up.");
            leftButtonDownFlag = false;
        }

        public void OnRightButtonDown()
        {
            Debug.Log("RightButton down.");
            rightButtonDownFlag = true;
        }
        public void OnRightButtonUp()
        {
            Debug.Log("RightButton up.");
            rightButtonDownFlag = false;
        }

        private IEnumerator PlaySound()
        {
            while (true)
            {
                foreach (var cube in cm.syncCubes)
                {
                    // byte volume = Convert.ToByte((cube.x + addToVolume) / volumeDivided);
                    int duration = Convert.ToInt32((cube.x + addToDuration) * durationMultiplied);
                    Debug.Log("cube.x " + cube.x);
                    Debug.Log("Duration " + duration);
                    byte pitch = Convert.ToByte((cube.y + addToPitch) * pitchMultiplied);
                    List<Cube.SoundOperation> sound = new List<Cube.SoundOperation>();
                    sound.Add(new Cube.SoundOperation(duration, volume, pitch));
                    cube.PlaySound(1, sound.ToArray());
                    Debug.Log("Play sound.");
                }
                yield return wait;
            }
        }

        private IEnumerator MoveCube2()
        {
            while (true)
            {
                double cube2MoveSpeed = Random.Range(-1 * cube2Range, cube2Range);
                double cube2RotateSpeed = Random.Range(-1 * cube2Range, cube2Range);
                Debug.Log(cube2MoveSpeed + ", " + cube2RotateSpeed);
                cm.handles[1].Move(cube2MoveSpeed, cube2RotateSpeed, cube2MoveTime);
                // cm.handles[1].Move2Target(x, y, cube2Speed, cube2RotateTime, cube2Tolerance);
                Debug.Log("Cube2 moved.");
                yield return wait;
            }
        }
    }
}