﻿namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing.Imaging;
    
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    public class TwoFrameDifferenceDetector : IMotionDetector
    {
        // frame's dimension
        private int width;
        private int height;
        private int frameSize;

        // previous frame of video stream
        private UnmanagedImage previousFrame;
        // current frame of video sream
        private UnmanagedImage motionFrame;
        // number of pixels changed in the new frame of video stream
        private int pixelsChanged;

        // threshold values
        private int differenceThreshold    =  15;
        private int differenceThresholdNeg = -15;

        // grayscale filter
        private GrayscaleBT709 grayFilter = new GrayscaleBT709( );

        /// <summary>
        /// Difference threshold value, [1, 255].
        /// </summary>
        /// 
        /// <remarks><para>The value specifies the amount off difference between pixels, which is treated
        /// as motion pixel.</para>
        /// 
        /// <para>Default value is set to <b>15</b>.</para>
        /// </remarks>
        /// 
        public int DifferenceThreshold
        {
            get { return differenceThreshold; }
            set
            {
                differenceThreshold = Math.Max( 1, Math.Min( 255, value ) );
                differenceThresholdNeg = -differenceThreshold;
            }
        }

        /// <summary>
        /// Motion level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks>Amount of changes in the last processed frame.</remarks>
        /// 
        public double MotionLevel
        {
            get { return (double) pixelsChanged / ( width * height ); }
        }

        public UnmanagedImage MotionFrame
        {
            get { return motionFrame; }
        }


        public unsafe void ProcessFrame( UnmanagedImage videoFrame )
        {
            // check previous frame
            if ( previousFrame == null )
            {
                // save image dimension
                width  = videoFrame.Width;
                height = videoFrame.Height;

                // alocate memory for previous and current frames
                previousFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                motionFrame   = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );

                frameSize = motionFrame.Stride * height;

                // temporary buffer
/*                if ( suppressNoise )
                {
                    tempFrame = Marshal.AllocHGlobal( frameSize );
                }*/

                // convert source frame to grayscale
                grayFilter.Apply( videoFrame, previousFrame );

                return;
            }

            // check image dimension
            if ( ( videoFrame.Width != width ) || ( videoFrame.Height != height ) )
                return;

            // convert current image to grayscale
            grayFilter.Apply( videoFrame, motionFrame );

            // pointers to previous and current frames
            byte* prevFrame = (byte*) previousFrame.ImageData.ToPointer( );
            byte* currFrame = (byte*) motionFrame.ImageData.ToPointer( );
            // difference value
            int diff;

            // 1 - get difference between frames
            // 2 - threshold the difference
            // 3 - copy current frame to previous frame
            for ( int i = 0; i < frameSize; i++, prevFrame++, currFrame++ )
            {
                // difference
                diff = (int) *currFrame - (int) *prevFrame;
                // copy current frame to previous
                *prevFrame = *currFrame;
                // treshold
                *currFrame = ( ( diff >= differenceThreshold ) || ( diff <= differenceThresholdNeg ) ) ? (byte) 255 : (byte) 0;
            }

            // calculate motion without suppressing noise
            byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

            for ( int i = 0; i < frameSize; i++, motion++ )
            {
                pixelsChanged += ( *motion & 1 );
            }
        }

        public void Reset( )
        {
            if ( previousFrame != null )
            {
                previousFrame.Dispose( );
                previousFrame = null;
            }

            if ( motionFrame != null )
            {
                motionFrame.Dispose( );
                motionFrame = null;
            }
        }

    }
}