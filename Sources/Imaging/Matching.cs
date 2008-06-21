// AForge Image Processing Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Matching class keeps information about found template matching. The class is
    /// used with template matching algorithms implementing <see cref="ITemplateMatching"/>
    /// interface.
    /// </summary>
    public class Matching
    {
        private Rectangle rect;
        private float similarity;

        /// <summary>
        /// Rectangle of the matching area.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rect; }
        }

        /// <summary>
        /// Similarity between template and found matching, [0..1].
        /// </summary>
        public float Similarity
        {
            get { return similarity; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matching"/> class.
        /// </summary>
        /// 
        /// <param name="rect">Rectangle of the matching area.</param>
        /// <param name="similarity">Similarity between template and found matching, [0..1].</param>
        /// 
        public Matching( Rectangle rect, float similarity )
        {
            this.rect = rect;
            this.similarity = similarity;
        }
    }
}
