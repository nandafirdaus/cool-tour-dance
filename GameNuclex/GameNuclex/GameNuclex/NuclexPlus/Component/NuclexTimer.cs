using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameNuclex.NuclexPlus.Component
{
    public class NuclexTimer
    {
        long lastTime = -1;    // -1 berarti belum dimulai timernya
        long timeInterval;
        long deltaTime;
        public long totalTime = 0; // totalTime (in miliseconds) 
        bool isActive;
        bool isOnceTicked;
        /**
         * Class untuk menghitung interval waktu (timeInterval)
         * fungsi isTicked akan return true jika timeInteval sudah dilewati
         * @param timeInterval 
         */
        public NuclexTimer(long timeInterval)
        {
            this.timeInterval = timeInterval;
            isActive = true;
            isOnceTicked = false;
        }

        /**
         * fungsi isTicked akan return true jika timeInteval sudah dilewati
         * @param currentTime
         * @return 
         */
        public bool isTicked(GameTime gameTime) {
            return isTicked((long) (gameTime.TotalGameTime.TotalMilliseconds));
        }
        public bool alreadyTicked(GameTime gameTime) {
            return alreadyTicked((long)(gameTime.TotalGameTime.TotalMilliseconds));
        }
        public bool alreadyTicked(long currentTime) {
            if (isOnceTicked)
            {
                isTicked(currentTime);
                return false;
            }
            return false;
        }

        public bool isTicked(long currentTime)
        {
            if (isActive)
            {
                if (lastTime == -1)
                {
                    lastTime = currentTime;
                }
                else
                {
                    deltaTime += currentTime - lastTime;
                    totalTime += currentTime - lastTime;
                    lastTime = currentTime;
                    
                    if (deltaTime >= timeInterval)
                    {
                        deltaTime -= timeInterval;
                        isOnceTicked = true;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        /**
         * Mereset timer
         */
        public void reset()
        {
            totalTime = 0;
            deltaTime = 0;
            lastTime = -1;
            isOnceTicked = false;
        }

        /**
         * Provide time interval for this timer. If the time interval is 4000, so
         * isTicked method will return true every 4 seconds.
         * @param i 
         */
        public void setTimeInterval(int i)
        {
            timeInterval = i;
            reset();
        }

        /**
         * Provide time interval for this timer. If the time interval is 4000, so
         * isTicked method will return true every 4 seconds.
         * @param i 
         */
        public void setTimeInterval(long i)
        {
            timeInterval = i;
            reset();
        }

        /**
         * Berbeda dengan fungsi reset, fungsi sleep ini hanya dipanggil
         * jika screen berada pada kondisi sleep. Yang fungsi sleep lakukan
         * hanya mengubah nilai lastTime menjadi -1, supaya perhitungannya
         * dimulai ulang pada saat dibangunkan kembali nanti (tidak sleep lagi)
         */
        public void sleep()
        {
            lastTime = -1;
        }

        /**
         * Start the timer immediately from the start.
         * Yes, it will reset the timer all over.
         */
        public void start()
        {
            reset();
            isActive = true;
        }

        /**
         * Stop the timer immediately.
         */
        public void stop()
        {
            reset();
            isActive = false;
        }

        /**
         * Checking whether this timer has been started or not.
         * @return 
         */
        public bool isStarted()
        {
            return isActive;
        }

        public long getTimeInterval()
        {
            return timeInterval;
        }
    }
}
