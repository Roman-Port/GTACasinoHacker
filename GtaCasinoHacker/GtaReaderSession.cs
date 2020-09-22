using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaCasinoHacker
{
    public class GtaReaderSession
    {
        public GtaScreenshot screenshot;
        public GtaFingerprintTarget screenTarget;
        public List<GtaFingerprintChallenge> screenChallenges;
        public GtaFingerPackFile file;
        public List<GtaFingerprintChallenge> matches;
        public bool isWindowed;

        public List<GtaFingerPackFile> registeredFingers;

        public GtaReaderSession(List<GtaFingerPackFile> registeredFingers, Bitmap screenshotBitmap, bool isWindowed)
        {
            //Set
            this.registeredFingers = registeredFingers;
            this.isWindowed = isWindowed;
            
            //Open GTA screenshot
            screenshot = new GtaScreenshot(screenshotBitmap, isWindowed);

            //Get screen challenges
            screenChallenges = new List<GtaFingerprintChallenge>();
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    screenChallenges.Add(screenshot.GetComponent(x, y));
                }
            }

            //Determine which file this is
            screenTarget = screenshot.GetCloneTarget();
            file = FindMatchingFinger(screenTarget);

            //Run check on each registered finger
            matches = new List<GtaFingerprintChallenge>();
            List<GtaFingerprintChallenge> workingScreenChallenges = new List<GtaFingerprintChallenge>();
            workingScreenChallenges.AddRange(screenChallenges);
            foreach (var f in file.challenges)
            {
                //Find
                var match = FindMatchingTarget(f, workingScreenChallenges.ToArray());

                //Remove from list so we don't find it again
                workingScreenChallenges.Remove(match);

                //Add
                matches.Add(match);
                Console.WriteLine("X: " + match.fingerX + ", Y: " + match.fingerY);
            }
        }

        /// <summary>
        /// Finds which file has the closest matching finger to this one
        /// </summary>
        /// <param name="target"></param>
        private GtaFingerPackFile FindMatchingFinger(GtaFingerprintTarget target)
        {
            //Find finger with the highest match
            GtaFingerPackFile highestMatch = null;
            int highestMatchValue = int.MinValue;
            foreach (var f in registeredFingers)
            {
                //Get challenege
                var c = f.finger;

                //Resize the mask of the finger
                bool[,] targetMask = target.ResizeDataArray(c.width, c.height);

                //Loop through and generate an number to tell us how closely this matches
                int value = 0;
                for (int x = 0; x < c.width; x++)
                {
                    for (int y = 0; y < c.height; y++)
                    {
                        //Check
                        bool isMatch = targetMask[x, y] == c.data[x, y];
                        if (isMatch)
                            value++;
                        else
                            value--;
                    }
                }

                //Check if this is the best match
                if (value > highestMatchValue)
                {
                    highestMatchValue = value;
                    highestMatch = f;
                }
            }

            //Validate
            if (highestMatch == null)
                throw new Exception("Could not find any fingers!");

            return highestMatch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">FROM THE PACKAGE, the finger to check</param>
        /// <param name="challenges">FROM THE SCREEN, the fingers to check</param>
        /// <returns></returns>
        private GtaFingerprintChallenge FindMatchingTarget(GtaFingerprintChallenge target, GtaFingerprintChallenge[] challenges)
        {
            //Find finger with the highest match
            GtaFingerprintChallenge highestMatch = null;
            int highestMatchValue = int.MinValue;
            foreach (var c in challenges)
            {
                //Resize the mask
                bool[,] targetMask = c.ResizeDataArray(target.width, target.height);

                //Loop through and generate an number to tell us how closely this matches
                int value = 0;
                for (int x = 0; x < c.width; x++)
                {
                    for (int y = 0; y < c.height; y++)
                    {
                        //Check
                        bool isMatch = targetMask[x, y] == target.data[x, y];
                        if (isMatch)
                            value++;
                        else
                            value--;
                    }
                }

                //Check if this is the best match
                if (value > highestMatchValue)
                {
                    highestMatchValue = value;
                    highestMatch = c;
                }
            }

            //Validate
            if (highestMatch == null)
                throw new Exception("Could not find any fingers!");

            return highestMatch;
        }
    }
}
